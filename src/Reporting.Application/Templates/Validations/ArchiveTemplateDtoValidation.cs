using AHI.Infrastructure.Exception;
using FluentValidation;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Template.Validation
{
    public class ArchiveTemplateDtoValidation : AbstractValidator<TemplateDto>
    {
        public ArchiveTemplateDtoValidation()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.Name).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.FilePath).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.OutputTypeId).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.StorageId).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleForEach(a => a.DataSets)
                .ChildRules(detail =>{
                    detail.RuleFor(x => x.Id).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
                    detail.RuleFor(x => x.Name).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
                    detail.RuleFor(x => x.TemplateId).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
                    detail.RuleFor(x => x.DataSourceTypeId).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
                    detail.RuleFor(x => x.DataSourceContent).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
                }).When(a => a.DataSets != null);
        }
 
    }
}