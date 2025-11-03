using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieCatalog.Web.Utils;
using MovieCatalogApi.Entities;
using MovieCatalogApi.Services;

namespace MovieCatalog.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMovieCatalogDataService _service;

        public Dictionary<Genre, int> GenresWithCounts { get; set; } = new();

        public IndexModel(IMovieCatalogDataService service)
        {
            _service = service;
        }

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 20;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public TitleSort TitleSort { get; set; } = TitleSort.ReleaseYear;

        [BindProperty(SupportsGet = true)]
        public bool SortDescending { get; set; } = true;

        [BindProperty(SupportsGet = true)]
        public TitleFilter Filter { get; set; } = new TitleFilter();

        public PagedResult<Title> Titles { get; set; } = PagedResult<Title>.Empty;
        public int TotalResults => Titles.AllResultsCount;
        public int LastPageNumber => Titles.LastPageNumber + 1;
        public int CurrentPageNumber => Titles.CurrentPageNumber + 1;

        public async Task<IActionResult> OnGetAsync()
        {
            GenresWithCounts = await _service.GetGenresWithTitleCountsAsync();
            /*
            var requiredParams = new[] { nameof(PageSize), nameof(PageNumber), nameof(TitleSort), nameof(SortDescending) };
            bool missing = requiredParams.Any(p => !Request.Query.ContainsKey(p));

            if (missing)
            {
                return RedirectToPage(new
                {
                    PageSize = 20,
                    PageNumber = 1,
                    TitleSort = TitleSort.ReleaseYear,
                    SortDescending = true
                });
            }

            if (PageSize < 1) PageSize = 20;
            if (PageNumber < 1) PageNumber = 1;

            TitleFilter? filter = new TitleFilter
            {
                TitleTypes = new[] { TitleType.Movie },
                StartYearMax = DateTime.Now.Year
            };*/

            Titles = await _service.GetTitlesAsync(
            pageSize: PageSize,
            page: PageNumber,
            filter: Filter,
            titleSort: TitleSort,
            sortDescending: SortDescending
            );

            if (Titles.AllResultsCount > 0 && PageNumber > Titles.LastPageNumber + 1)
            {
                return RedirectToPage(new
                {
                    PageSize,
                    PageNumber = Titles.LastPageNumber + 1,
                    TitleSort,
                    SortDescending
                });
            }

            return Page();
        }

        public IEnumerable<SelectListItem> PageSizeOptions => new[]
        {
            new SelectListItem("10 items/page", "10", PageSize == 10),
            new SelectListItem("20 items/page", "20", PageSize == 20),
            new SelectListItem("30 items/page", "30", PageSize == 30),
            new SelectListItem("60 items/page", "60", PageSize == 60),
            new SelectListItem("120 items/page", "120", PageSize == 120)
        };

        public IEnumerable<SelectListItem> SortFieldOptions => Enum.GetValues(typeof(TitleSort))
            .Cast<TitleSort>()
            .Select(v => new SelectListItem(v.ToString(), v.ToString(), v == TitleSort));

        public IEnumerable<SelectListItem> SortDirectionOptions => new[]
        {
            new SelectListItem("Ascending", "False", !SortDescending),
            new SelectListItem("Descending", "True", SortDescending)
        };

        public IReadOnlyList<int> PageNumberOptions => new[]
        {
            1, 2, 3,
            PageNumber - 1, PageNumber, PageNumber + 1,
            Titles.LastPageNumber - 1, Titles.LastPageNumber, Titles.LastPageNumber + 1
        }
        .Where(i => i > 0 && i <= Titles.LastPageNumber)
        .Distinct()
        .OrderBy(i => i)
        .ToArray();
        }
}