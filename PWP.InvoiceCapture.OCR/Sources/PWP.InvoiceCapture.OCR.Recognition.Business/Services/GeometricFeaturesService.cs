using OpenCvSharp;
using OpenCvSharp.Extensions;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.OCR.Core.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services;
using PWP.InvoiceCapture.OCR.Recognition.Extensions.ImageProcessing;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Services
{
    internal class GeometricFeaturesService : IGeometricFeaturesService
    {
        public GeometricFeaturesService()
        {
            horizontalLineKernel = CreateStructuringElement(horizontalLineWidth, horizontalLineHeight);
            verticalLineKernel = CreateStructuringElement(verticalLineWidth, verticalLineHeight);
            objectKernel = CreateStructuringElement(objectWidth, objectHeight);
        }

        public OperationResult<GeometricFeatureCollection> ProcessImage(byte[] imageBytes)
        {
            Guard.IsNotNull(imageBytes, nameof(imageBytes));
            var imageStream = new MemoryStream(imageBytes);
            var image = (Bitmap)Image.FromStream(imageStream);
            var origin = BitmapConverter.ToMat(image);
            if (!IsImageValid(origin))
            {
                return new OperationResult<GeometricFeatureCollection> { Status = OperationResultStatus.Failed, Message = "Image Format Not Supported" };
            }
            origin = GetBinaryHalfImage(origin);
            var objectClosed = GetClosed(objectKernel, origin);
            Cv2.FindContours(
                objectClosed,
                out var contours,
                out var hierarchyIndexes,
                mode: RetrievalModes.CComp,
                method: ContourApproximationModes.ApproxSimple);

            var connectedComponents = GetConnectedComponents(origin);
            var features = new GeometricFeatureCollection
            {
                ConnectedComponentCount = connectedComponents.LabelCount,
                ContourCount = contours.Length,
                LineCount = GetLineCount(origin),
                AverageBlobHeight = GetAverageHeight(connectedComponents, origin),
                PixelDensity = GetPixelDensity(origin)
            };

            return new OperationResult<GeometricFeatureCollection> { 
                Status = OperationResultStatus.Success,
                Data = features
            };
        }

        private bool IsImageValid(Mat origin)
        {
            var channelCount = origin.Channels();
            if (channelCount == 3 || channelCount == 4 || channelCount == 1)
            {
                return true;
            }

            return false;
        }

        private float GetPixelDensity(Mat bitmap)
        {
            var totalPixels = 0;
            var objectPixels = 0;
            for (var i = 0; i < bitmap.Width; i++)
            {
                for (var j = 0; j < bitmap.Height; j++)
                {
                    var pixelValue = bitmap.GetValue(j, i);
                    if ((int)pixelValue == 0)
                    {
                        objectPixels++;
                    }
                    totalPixels++;

                }
            }

            return (float)objectPixels / (float)totalPixels;
        }

        private Mat BitwiseOr(Mat bitmap1, Mat bitmap2)
        {
            var copyOfBitmap1 = new Mat(bitmap1.Rows, bitmap1.Cols, bitmap1.Type());
            bitmap1.CopyTo(copyOfBitmap1);
            for (var i = 0; i < bitmap2.Width; i++)
            {
                for (var j = 0; j < bitmap2.Height; j++)
                {
                    var pixelValue = bitmap2.GetValue(j, i);
                    if ((int)pixelValue == 0)
                    {

                        copyOfBitmap1.SetValue(j, i, 0);
                    }
                }
            }

            return copyOfBitmap1;
        }

        private Mat CutImage(Mat image)
        {
            var halfImage = new Rect(0, 0, image.Width, image.Height / 3);

            return new Mat(image, halfImage);
        }

        private Bitmap ConvertToGrayScale(Bitmap original)
        {
            //create a blank bitmap the same size as original
            var newBitmap = new Bitmap(original.Width, original.Height);
            //get a graphics object from the new image
            using (var graphicsObject = Graphics.FromImage(newBitmap))
            {
                //create some image attributes
                using (var attributes = new ImageAttributes())
                {
                    //set the color matrix attribute
                    attributes.SetColorMatrix(colorMatrix);
                    //draw the original image on the new image
                    //using the grayscale color matrix
                    graphicsObject.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                                0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return newBitmap;
        }

        private Mat GetBinaryHalfImage(Mat origin)
        {

            /* 
             * the expected number of channels is 3 or 4
             */
            if (origin.Channels() == 3)
            {
                origin = origin.CvtColor(ColorConversionCodes.BGR2GRAY);
            }
            else if (origin.Channels() == 4)
            {
                origin = origin.CvtColor(ColorConversionCodes.BGRA2GRAY);
            }

            origin = CropImage(origin);
            origin = CutImage(origin);
            var binary = origin.Threshold(otsuThreshold, otsuMaxVal, ThresholdTypes.Otsu);

            return binary;
        }

        private Mat CropImage(Mat input)
        {
            var origin = new Mat();
            Cv2.BitwiseNot(input, origin);
            var output = new Mat();
            Cv2.FindNonZero(origin, output);
            var boundingBox = Cv2.BoundingRect(output);

            return new Mat(input, boundingBox);
        }

        private Mat GetClosed(Mat kernel, Mat source)
        {
            var closed = new Mat();
            Cv2.MorphologyEx(source, closed, MorphTypes.Close, kernel);
            return closed;
        }
        private Mat CreateStructuringElement(int width, int height) => Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(width, height));

        private ConnectedComponents GetConnectedComponents(Mat origin)
        {
            var horizontalClosed = GetClosed(horizontalLineKernel, origin);
            var verticalClosed = GetClosed(verticalLineKernel, origin);
            var onlyLinesImage = BitwiseOr(horizontalClosed, verticalClosed);
            var connectedComponents = onlyLinesImage.ConnectedComponentsEx(PixelConnectivity.Connectivity4);
            connectedComponents.RenderBlobs(onlyLinesImage);
            connectedComponents = origin.ConnectedComponentsEx(PixelConnectivity.Connectivity4);
            return connectedComponents;
        }

        private int GetLineCount(Mat image)
        {
            var edges = image.Canny(50, 150, 3);
            var houghLines = edges.HoughLines(1, Math.PI / 180, 200);
            return houghLines.Length;
        }

        private double GetAverageHeight(ConnectedComponents connectedComponents, Mat origin)
        {
            var blobCount = connectedComponents.Blobs.Count;
            var blobsToSkip = blobCount / blobSkipRatio;
            var averageSet = connectedComponents.Blobs.OrderBy(x => x.Height)
                .Skip(blobsToSkip)
                .Reverse()
                .Skip(blobsToSkip);
            var averageYSize = (double)(averageSet.Average(x => x.Height));
            return averageYSize / origin.Height;
        }

        private readonly int blobSkipRatio = 10;
        private readonly int otsuThreshold = 100;
        private readonly int otsuMaxVal = 255;
        private readonly int horizontalLineHeight = 2;
        private readonly int horizontalLineWidth = 100;
        private readonly int verticalLineHeight = 100;
        private readonly int verticalLineWidth = 2;
        private readonly int objectWidth = 21;
        private readonly int objectHeight = 7;
        private readonly Mat horizontalLineKernel;
        private readonly Mat verticalLineKernel;
        private readonly Mat objectKernel;
        private readonly ColorMatrix colorMatrix = new ColorMatrix(
            new float[][]
            {
                new float[] {.3f, .3f, .3f, 0, 0},
                new float[] {.59f, .59f, .59f, 0, 0},
                new float[] {.11f, .11f, .11f, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            }
        );
    }
}
