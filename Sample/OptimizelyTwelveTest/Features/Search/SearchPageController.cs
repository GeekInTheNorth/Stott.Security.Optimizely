namespace OptimizelyTwelveTest.Features.Search
{
    using MediatR;

    using Microsoft.AspNetCore.Mvc;

    using Newtonsoft.Json;

    using OptimizelyTwelveTest.Features.Common;

    using System.Threading.Tasks;

    public class SearchPageController : PageControllerBase<SearchPage>
    {
        private readonly IMediator _mediator;

        public SearchPageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(SearchPage currentContent, string searchText, int page = 1)
        {
            var query = new SearchQuery
            {
                InitialPageSize = currentContent.InitialPageSize,
                LoadMorePageSize = currentContent.LoadMorePageSize,
                Page = page,
                SearchText = searchText
            };
            var response = await _mediator.Send(query);
            var model = new SearchPageViewModel
            {
                CurrentPage = currentContent,
                SearchResults = response
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Search(SearchPage currentContent, string searchText, int page)
        {
            var query = new SearchQuery
            {
                InitialPageSize = currentContent.InitialPageSize,
                LoadMorePageSize = currentContent.LoadMorePageSize,
                Page = page,
                SearchText = searchText
            };
            var response = await _mediator.Send(query);

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(response),
                ContentType = "application/json",
                StatusCode = 200
            };
        }
    }
}
