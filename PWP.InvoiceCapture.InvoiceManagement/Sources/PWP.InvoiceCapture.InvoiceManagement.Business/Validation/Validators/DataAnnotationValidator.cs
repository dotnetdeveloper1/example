using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Definitions;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators
{
    internal class DataAnnotationValidator : IDataAnnotationValidator
    {
        public DataAnnotationValidator(
            IRequiredValidator requiredValidator,
            IRequiredValueValidator requiredValueValidator,
            IDateValidator dateValidator,
            IDecimalValidator decimalValidator,
            IQuantityValidator quantityValidator,
            ISingleFieldPerTypeValidator singleFieldPerTypeValidator,
            ILineOrderValidator lineOrderValidator,
            ITotalMultiplicationValidator totalMultiplicationValidator,
            IFormulaExecutionResultValidator formulaExecutionResultValidator,
            IMaxLengthValidator maxLengthValidator,
            IMinLengthValidator minLengthValidator,
            IMaxValueValidator maxValueValidator,
            IMinValueValidator minValueValidator)
        {
            Guard.IsNotNull(requiredValidator, nameof(requiredValidator));
            Guard.IsNotNull(requiredValueValidator, nameof(requiredValueValidator));
            Guard.IsNotNull(dateValidator, nameof(dateValidator));
            Guard.IsNotNull(decimalValidator, nameof(decimalValidator));
            Guard.IsNotNull(quantityValidator, nameof(quantityValidator));
            Guard.IsNotNull(singleFieldPerTypeValidator, nameof(singleFieldPerTypeValidator));
            Guard.IsNotNull(lineOrderValidator, nameof(lineOrderValidator));
            Guard.IsNotNull(totalMultiplicationValidator, nameof(totalMultiplicationValidator));
            Guard.IsNotNull(formulaExecutionResultValidator, nameof(formulaExecutionResultValidator));
            Guard.IsNotNull(maxLengthValidator, nameof(maxLengthValidator));
            Guard.IsNotNull(minLengthValidator, nameof(minLengthValidator));
            Guard.IsNotNull(maxValueValidator, nameof(maxValueValidator));
            Guard.IsNotNull(minValueValidator, nameof(minValueValidator));

            this.requiredValidator = requiredValidator;
            this.requiredValueValidator = requiredValueValidator;
            this.dateValidator = dateValidator;
            this.decimalValidator = decimalValidator;
            this.quantityValidator = quantityValidator;
            this.singleFieldPerTypeValidator = singleFieldPerTypeValidator;
            this.lineOrderValidator = lineOrderValidator;
            this.totalMultiplicationValidator = totalMultiplicationValidator;
            this.formulaExecutionResultValidator = formulaExecutionResultValidator;
            this.maxLengthValidator = maxLengthValidator;
            this.minLengthValidator = minLengthValidator;
            this.maxValueValidator = maxValueValidator;
            this.minValueValidator = minValueValidator;
        }

        public ValidationResult Validate(DataAnnotation dataAnnotation, CultureInfo cultureInfo, List<Field> fields)
        {
            if (dataAnnotation.InvoiceAnnotations == null)
            {
                return ValidationResult.Failed("InvoiceAnnotations can not be null.");
            }

            var invoiceLineValidationResult = dataAnnotation.InvoiceLineAnnotations == null
                ? ValidationResult.Ok
                : ValidateInvoiceLines(dataAnnotation.InvoiceLineAnnotations);

            return Validator.ValidateMany(
                ValidateInvoiceAnnotations(dataAnnotation.InvoiceAnnotations, cultureInfo, fields),
                invoiceLineValidationResult);
        }

        private ValidationResult ValidateInvoiceAnnotations(List<Annotation> annotations, CultureInfo cultureInfo, List<Field> fields)
        {
            var fieldsValidationResult = fields
                .Select(field => ValidateField(field, annotations, cultureInfo))
                .Combine();

            var fieldTypeNames = fields.ToDictionary(field => field.Id.ToString(), field => field.DisplayName);

            var singleFieldPerTypeValidationResult = singleFieldPerTypeValidator.Validate(annotations, fieldTypeNames);

            if (!singleFieldPerTypeValidationResult.IsValid || !fieldsValidationResult.IsValid)
            {
                return Validator.ValidateMany(singleFieldPerTypeValidationResult, fieldsValidationResult);
            }

            return ValidateFieldsFormulaExcecutionResults(annotations, fields);
        }

        private ValidationResult ValidateFieldsFormulaExcecutionResults(List<Annotation> annotations, List<Field> fields) 
        {
            var fieldsDictionary = fields
                .Where(field => field.Formula != null)
                .ToDictionary(field => field.Id.ToString());

            var resultAnnotations = annotations
                .Where(annotation => fieldsDictionary.ContainsKey(annotation.FieldType))
                .ToList();

            return resultAnnotations
                .Select(resultAnnotation => ValidateFormulaExecutionResult(annotations, resultAnnotation, fieldsDictionary))
                .Combine();
        }

        private ValidationResult ValidateFormulaExecutionResult(List<Annotation> annotations, Annotation resultAnnotation, Dictionary<string, Field> fieldsDictionary) 
        {
            var field = fieldsDictionary[resultAnnotation.FieldType];

            return formulaExecutionResultValidator.Validate(annotations, resultAnnotation, field.DisplayName, field.Formula);
        }

        private ValidationResult ValidateField(Field field, List<Annotation> annotations, CultureInfo cultureInfo)
        {
            var dataAnnotation = annotations.FirstOrDefault(invoiceAnnotation => field.Id.ToString() == invoiceAnnotation.FieldType);

            return ValidateFieldAnnotation(field, dataAnnotation, cultureInfo);
        }

        private ValidationResult ValidateFieldAnnotation(Field field, Annotation dataAnnotation, CultureInfo cultureInfo)
        {
            var isRequiredValidationResult = ValidateIfRequired(field, dataAnnotation);

            if (!isRequiredValidationResult.IsValid)
            {
                return isRequiredValidationResult;
            }

            return ValidateAnnotationIfExists(dataAnnotation, (annotation) => ValidateFieldByType(field, annotation, cultureInfo));
        }

        private ValidationResult ValidateIfRequired(Field field, Annotation annotation)
        {
            if (!field.IsRequired)
            {
                return ValidationResult.Ok;
            }

            return Validator.ValidateMany(
                requiredValidator.Validate(annotation, field.Id.ToString()),
                ValidateAnnotationIfExists(annotation, invoiceAnnotation => requiredValueValidator.Validate(annotation, field.DisplayName)));
        }

        private ValidationResult ValidateFieldByType(Field field, Annotation annotation, CultureInfo cultureInfo)
        {
            switch (field.Type)
            {
                case FieldType.DateTime:
                    return ValidateDate(annotation, cultureInfo, field.DisplayName);
                case FieldType.Decimal:
                    return ValidateDecimal(field, annotation);
                case FieldType.String:
                    return ValidateString(field, annotation);
                default:
                    return ValidationResult.Failed($"Unknown field type {field.Type}");
            }
        }

        private ValidationResult ValidateDate(Annotation annotation, CultureInfo cultureInfo, string fieldName)
        {
            return dateValidator.Validate(annotation, cultureInfo, fieldName);
        }

        private ValidationResult ValidateDecimal(Field field, Annotation annotation)
        {
            var typeValidationResult = decimalValidator.Validate(annotation, invariantCulture, field.DisplayName);
            if (!typeValidationResult.IsValid)
            {
                return typeValidationResult;
            }

            var validators = new List<ValidationResult>();

            if (field.MaxValue.HasValue)
            {
                validators.Add(maxValueValidator.Validate(annotation, field.MaxValue.Value, field.DisplayName));
            }

            if (field.MinValue.HasValue)
            {
                validators.Add(minValueValidator.Validate(annotation, field.MinValue.Value, field.DisplayName));
            }

            return validators.Combine();
        }

        private ValidationResult ValidateString(Field field, Annotation annotation)
        {
            var validators = new List<ValidationResult>();

            if (field.MaxLength.HasValue)
            {
                validators.Add(maxLengthValidator.Validate(annotation, field.MaxLength.Value, field.DisplayName));
            }

            if (field.MinLength.HasValue)
            {
                validators.Add(minLengthValidator.Validate(annotation, field.MinLength.Value, field.DisplayName));
            }

            return validators.Combine();
        }

        private ValidationResult ValidateInvoiceLines(List<LineAnnotation> invoiceLineAnnotations)
        {
            var linesValidationResults = invoiceLineAnnotations
                .Select(lineAnnotation => ValidateLineItem(lineAnnotation))
                .Combine();

            return Validator.ValidateMany(
                linesValidationResults,
                lineOrderValidator.Validate(invoiceLineAnnotations));
        }

        private ValidationResult ValidateLineItem(LineAnnotation lineAnnotation)
        {
            if (lineAnnotation == null)
            {
                return ValidationResult.Failed("LineAnnotation can not be null.");
            }

            if (lineAnnotation.LineItemAnnotations == null)
            {
                return ValidationResult.Failed("LineItemAnnotations can not be null.");
            }

            var totalAnnotation = lineAnnotation.LineItemAnnotations.FirstOrDefault(item => item.FieldType == InvoiceLineFieldTypes.Total);
            var priceAnnotation = lineAnnotation.LineItemAnnotations.FirstOrDefault(item => item.FieldType == InvoiceLineFieldTypes.Price);
            var quantityAnnotation = lineAnnotation.LineItemAnnotations.FirstOrDefault(item => item.FieldType == InvoiceLineFieldTypes.Quantity);

            var validationResult = Validator.ValidateMany(
                requiredValidator.Validate(totalAnnotation, InvoiceLineFieldTypes.Total),
                ValidateAnnotationIfExists(totalAnnotation, annotation => requiredValueValidator.Validate(totalAnnotation, totalAnnotation.FieldType)),
                ValidateAnnotationIfExists(totalAnnotation, annotation => decimalValidator.Validate(totalAnnotation, invariantCulture, totalAnnotation.FieldType)),

                ValidateAnnotationIfExists(priceAnnotation, annotation => requiredValueValidator.Validate(priceAnnotation, totalAnnotation.FieldType)),
                ValidateAnnotationIfExists(priceAnnotation, annotation => decimalValidator.Validate(priceAnnotation, invariantCulture, totalAnnotation.FieldType)),

                ValidateAnnotationIfExists(quantityAnnotation, annotation => requiredValueValidator.Validate(quantityAnnotation, totalAnnotation.FieldType)),
                ValidateAnnotationIfExists(quantityAnnotation, annotation => quantityValidator.Validate(quantityAnnotation, invariantCulture, totalAnnotation.FieldType)),

                singleFieldPerTypeValidator.Validate(lineAnnotation.LineItemAnnotations, invoiceLineFieldTypeNames));

            if (!validationResult.IsValid)
            {
                return validationResult;
            }

            return totalMultiplicationValidator.Validate(
                new Annotation[] { priceAnnotation, quantityAnnotation }, 
                totalAnnotation, 
                invariantCulture,
                invoiceLineFieldTypeNames);
        }

        private ValidationResult ValidateAnnotationIfExists(Annotation annotation, Func<Annotation, ValidationResult> action)
        {
            if (annotation == null)
            {
                return ValidationResult.Ok;
            }

            if (string.IsNullOrWhiteSpace(annotation.FieldType) || string.IsNullOrWhiteSpace(annotation.FieldValue))
            {
                return ValidationResult.Failed("Annotation cannot contain empty types and values.");
            }

            return action(annotation);
        }

        private readonly IRequiredValidator requiredValidator;
        private readonly IRequiredValueValidator requiredValueValidator;
        private readonly IMaxLengthValidator maxLengthValidator;
        private readonly IMinLengthValidator minLengthValidator;
        private readonly IMinValueValidator minValueValidator;
        private readonly IMaxValueValidator maxValueValidator;
        private readonly IDateValidator dateValidator;
        private readonly IDecimalValidator decimalValidator;
        private readonly IQuantityValidator quantityValidator;
        private readonly ISingleFieldPerTypeValidator singleFieldPerTypeValidator;
        private readonly ILineOrderValidator lineOrderValidator;
        private readonly ITotalMultiplicationValidator totalMultiplicationValidator;
        private readonly IFormulaExecutionResultValidator formulaExecutionResultValidator;

        private readonly CultureInfo invariantCulture = CultureInfo.InvariantCulture;
        private readonly Dictionary<string, string> invoiceLineFieldTypeNames = new Dictionary<string, string>()
        {
            { InvoiceLineFieldTypes.Total, nameof(InvoiceLineFieldTypes.Total) },
            { InvoiceLineFieldTypes.Price, nameof(InvoiceLineFieldTypes.Price) },
            { InvoiceLineFieldTypes.Quantity, nameof(InvoiceLineFieldTypes.Quantity) },
            { InvoiceLineFieldTypes.Description, nameof(InvoiceLineFieldTypes.Description) },
            { InvoiceLineFieldTypes.Number, nameof(InvoiceLineFieldTypes.Number) }
        };
    }
}
