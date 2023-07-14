using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.DataAnalysis.Business.Contract.Factories;
using PWP.InvoiceCapture.OCR.DataAnalysis.Business.Contract.Models;
using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.DataAnalysis.Business.Factories
{
    internal class FormRecognizerTrainingDocumentFactory : IFormRecognizerTrainingDocumentFactory
    {
        public FormReconizerTrainingDocument Create(string fileName, DataAnnotation annotationsDocument)
        {
            var trainingDocument = new FormReconizerTrainingDocument();
            var layoutItems = annotationsDocument.DocumentLayoutItems;
            trainingDocument.Document = fileName;
            trainingDocument.Labels = new List<TrainingAnnotation>();
            foreach(var annotation in annotationsDocument.InvoiceAnnotations)
            {
                /* Bypass annotations with no ocr word references as this is required for form recognizer training */ 
                if(annotation.DocumentLayoutItemIds.Count > 0)
                {
                    var trainingAnnotation = GetTrainingAnnotationFromDataAnnotation(annotation, layoutItems);
                    trainingDocument.Labels.Add(trainingAnnotation);
                }
            }

            foreach (var annotation in annotationsDocument.InvoiceLineAnnotations)
            {
                var trainingAnnotations = GetTrainingAnnotationsFromLineAnnotation(annotation, layoutItems);
                trainingDocument.Labels.AddRange(trainingAnnotations);
            }

            return trainingDocument;
        }

        private AnnotationValue GetAnnotationValueFromLayoutItem(DocumentLayoutItem item)
        {
            var annotationValue = new AnnotationValue();
            annotationValue.BoundingBoxes = new List<List<float>>();
            annotationValue.BoundingBoxes.Add(new List<float>());
            annotationValue.Page = item.PageNumber;
            annotationValue.Text = item.Text;
            /* top left*/
            annotationValue.BoundingBoxes[0].Add(item.TopLeft.X);
            annotationValue.BoundingBoxes[0].Add(item.TopLeft.Y);
            /* top right */
            annotationValue.BoundingBoxes[0].Add(item.BottomRight.X);
            annotationValue.BoundingBoxes[0].Add(item.TopLeft.Y);
            /* bottom right */
            annotationValue.BoundingBoxes[0].Add(item.BottomRight.X);
            annotationValue.BoundingBoxes[0].Add(item.BottomRight.Y);
            /* bottom left */
            annotationValue.BoundingBoxes[0].Add(item.TopLeft.X);
            annotationValue.BoundingBoxes[0].Add(item.BottomRight.Y);

            return annotationValue;
        }

        private TrainingAnnotation GetTrainingAnnotationFromDataAnnotation(Annotation annotation, List<DocumentLayoutItem> layoutItems)
        {
            var trainingAnnotation = new TrainingAnnotation();
            trainingAnnotation.Label = annotation.FieldType;
            trainingAnnotation.Value = new List<AnnotationValue>();
            foreach (var layoutId in annotation.DocumentLayoutItemIds)
            {
                var item = layoutItems.Where(layoutItem => layoutItem.Id == layoutId).FirstOrDefault();
                var annotationValue = GetAnnotationValueFromLayoutItem(item);
                trainingAnnotation.Value.Add(annotationValue);
            }

            return trainingAnnotation;
        }

        private List<TrainingAnnotation> GetTrainingAnnotationsFromLineAnnotation(LineAnnotation lineAnnotation, List<DocumentLayoutItem> layoutItems)
        {   
            var trainingAnnotations = new List<TrainingAnnotation>();
            foreach (var annotation in lineAnnotation.LineItemAnnotations)
            {
                var trainingAnnotation = GetTrainingAnnotationFromDataAnnotation(annotation, layoutItems);
                trainingAnnotation.Label = $"{ trainingAnnotation.Label}-{lineAnnotation.OrderNumber}";
                trainingAnnotations.Add(trainingAnnotation);
            }

            return trainingAnnotations;
        }
    }
}
