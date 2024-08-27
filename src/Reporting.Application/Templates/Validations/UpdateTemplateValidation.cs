using AHI.Infrastructure.Exception;
using AHI.Infrastructure.Service.Tag.Model;
using FluentValidation;
using Reporting.Application.Command;

namespace Reporting.Application.Validation
{
    public class UpdateTemplateValidation : AbstractValidator<UpdateTemplate>
    {
        public UpdateTemplateValidation()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.Name).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.TemplateFileUrl).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.OutputTypeId).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.StorageId).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.DataSets).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleForEach(x => x.Tags).SetValidator(
                new InlineValidator<UpsertTag> {
                    agValidator => agValidator.RuleFor(x => x.Key)
                                              .NotEmpty()
                                              .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED)
                                              .MaximumLength(216)
                                              .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_MAX_LENGTH)
                                              .Must(ContainsInvalidChar)
                                              .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_INVALID),
                    agValidator => agValidator.RuleFor(x => x.Value)
                                              .NotEmpty()
                                              .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED)
                                              .MaximumLength(216)
                                              .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_MAX_LENGTH)
                                              .Must(ContainsInvalidChar)
                                              .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_INVALID)
                }
            );
        }

        private bool ContainsInvalidChar(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return true;
            return !input.Contains(':') && !input.Contains(';') && !input.Contains(',');
        }
    }
}