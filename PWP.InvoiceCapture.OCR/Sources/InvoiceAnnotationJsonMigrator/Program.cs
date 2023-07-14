using NCalc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Definitions;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

namespace InvoiceAnnotationJsonMigrator
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                Console.WriteLine("Please, enter path to folder with list of folders and press enter.");

                var folderPath = Console.ReadLine();

                var folderPaths = Directory.GetDirectories(folderPath).ToList();

                Console.WriteLine(string.Empty);
                Console.WriteLine($"{folderPaths.Count()} folders found.");
                Console.WriteLine(string.Empty);

                FixFilesInFolders(folderPaths);

                Console.WriteLine(string.Empty);
                Console.WriteLine($"Not changed field type annotations: {notChangedFieldTypes.Count()}.");
                Console.WriteLine($"Fixed annotations: {fixedAnnotationsCount}.");

                ShowNotChangedFieldTypes();

                Console.WriteLine(string.Empty);
                Console.WriteLine("Done.");

                WaitForEnter("Press Enter to exit.");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                WaitForEnter("Press Enter to exit.");
            }
        }

        private static void FixFilesInFolders(List<string> folderPaths)
        {
            Parallel.ForEach(folderPaths, folderPath =>
            {
                var files = Directory.GetFiles(folderPath)
                    .Select(filePath => new FileInfo(filePath))
                    .Where(file => file.Extension.ToLower().Equals(json) && !file.Name.Equals($"{configuration}{json}"))
                    .ToList();

                Parallel.ForEach(files, file =>
                {
                    FixDataAnnotationFile(file);
                });

                Interlocked.Increment(ref doneFoldersCount);

                lock (locker)
                {
                    totalFilesCount += files.Count;
                    ClearLastLine();
                    var doneValue = 100 * Convert.ToDouble(doneFoldersCount) / Convert.ToDouble(folderPaths.Count());
                    Console.Write($"{string.Format("{0:0.00}", doneValue)}% done. Total files count: {totalFilesCount}");
                }
            });
        }

        private static void ShowNotChangedFieldTypes()
        {
            if (notChangedFieldTypes.Count() == 0)
            {
                return;
            }

            Console.WriteLine(string.Empty);
            Console.WriteLine("Press Y to show top 50 not changed field types.");

            var key = Console.ReadLine();
            if (key.ToLower() == "y")
            {
                var notChangedFieldTypesTop50 = notChangedFieldTypes.Take(50).Distinct().ToList();
                Console.WriteLine(String.Join(", ", notChangedFieldTypesTop50));
            }
        }

        private static void ClearLastLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            //Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        private static void WaitForEnter(string message)
        {
            Console.WriteLine(message);
            while (Console.ReadKey().Key != ConsoleKey.Enter)
            {
            }
        }

        static private void FixDataAnnotationFile(FileInfo file)
        {
            var dataAnnotation = DeserializeFileToJson<DataAnnotation>(file.FullName);

            if (TryFixDataAnnotation(dataAnnotation))
            {
                SerializeJsonToFile(file.FullName, dataAnnotation);
            }
        }

        static private bool TryFixDataAnnotation(DataAnnotation dataAnnotation)
        {
            bool wasChanged = false;

            foreach (var invoiceAnnotation in dataAnnotation.InvoiceAnnotations)
            {
                var fieldTypeName = fieldTypeNames.FirstOrDefault(fieldTypeName =>
                    string.Equals(fieldTypeName.ToLower(), invoiceAnnotation.FieldType.ToLower()));
                if (fieldTypeName == null)
                {
                    notChangedFieldTypes.Enqueue(invoiceAnnotation.FieldType);
                    continue;
                }

                int newFieldTypeValue = (int)typeof(FieldTypes).GetField(fieldTypeName).GetValue(null);
                invoiceAnnotation.FieldType = newFieldTypeValue.ToString();
                Interlocked.Increment(ref fixedAnnotationsCount);
                wasChanged = true;
            }

            return wasChanged;
        }

        static private TEntity DeserializeFileToJson<TEntity>(string filePath)
        {
            var json = File.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<TEntity>(json);
        }

        static private void SerializeJsonToFile(string filePath, object model)
        {
            var json = JsonConvert.SerializeObject(model, Formatting.Indented);

            File.WriteAllText(filePath, json);
        }

        private const string json = ".json";
        private const string configuration = "configuration";
        private static int doneFoldersCount = 0;
        private static int fixedAnnotationsCount = 0;
        private static int totalFilesCount = 0;
        private static readonly object locker = new object();
        private static readonly ConcurrentQueue<string> notChangedFieldTypes = new ConcurrentQueue<string>();

        private static readonly string[] fieldTypeNames = typeof(FieldTypes).GetFields()
            .Select(field => field.Name)
            .ToArray();
    }
}
