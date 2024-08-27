using System;
using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Model;
using Reporting.Application.Constant;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Service.Abstraction;
using MediatR;
using AHI.Infrastructure.Service.Tag.Extension;
using AHI.Infrastructure.Service.Tag.Service.Abstraction;

namespace Template.Application.Command.Handler
{
    public class SearchTemplateRequestHandler : IRequestHandler<SearchTemplate, BaseSearchResponse<TemplateDto>>
    {
        private readonly ITemplateService _service;
        private readonly ISystemContext _systemContext;
        private readonly ITagService _tagService;

        public SearchTemplateRequestHandler(ISystemContext systemContext,
            ITemplateService service,
            ITagService tagService)
        {
            _service = service;
            _systemContext = systemContext;
            _tagService = tagService;
        }

        public async Task<BaseSearchResponse<TemplateDto>> Handle(SearchTemplate request, CancellationToken cancellationToken)
        {
            if (!request.ClientOverride)
            {
                var pageSize = await _systemContext.GetValueAsync(DefaultSearch.DEFAULT_SEARCH_PAGE_SIZE, DefaultSearch.DEFAULT_VALUE_PAGE_SIZE);
                request.PageSize = Convert.ToInt32(pageSize);
            }

            request.MappingSearchTags();
            BaseSearchResponse<TemplateDto> response = await _service.SearchAsync(request);

            if (response != null)
                return await _tagService.FetchTagsAsync(response);
            else
                return response;
        }
    }
}