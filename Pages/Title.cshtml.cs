using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieCatalogApi.Entities;
using MovieCatalogApi.Services;
using System.ComponentModel.DataAnnotations;

namespace MovieCatalog.Web.Pages
{
    public class TitleModel : PageModel
    {
        private readonly IMovieCatalogDataService _service;

        public TitleModel(IMovieCatalogDataService service)
        {
            _service = service;
        }

        [TempData]
        public string? SuccessMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }

        [BindProperty, Required, StringLength(500)]
        [Display(Name = "Primary title")]
        public string PrimaryTitle { get; set; } = string.Empty;

        [BindProperty, Required, StringLength(500)]
        [Display(Name = "Original title")]
        public string OriginalTitle { get; set; } = string.Empty;

        [BindProperty]
        [Display(Name = "Title type")]
        public TitleType TitleType { get; set; } = TitleType.Movie;

        [BindProperty, Range(1900, 2100)]
        [Display(Name = "Start/release year")]
        public int? StartYear { get; set; }

        [BindProperty, Range(1900, 2100)]
        [Display(Name = "Year of last season")]
        public int? EndYear { get; set; }

        [BindProperty, Range(1, 9999)]
        [Display(Name = "General runtime in minutes")]
        public int? RuntimeMinutes { get; set; }

        [BindProperty]
        [Display(Name = "Genres")]
        [MaxLength(3, ErrorMessage = "Maximum 3 genres allowed.")]
        public List<int> SelectedGenres { get; set; } = new();


        public List<SelectListItem> AllGenres { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadGenresAsync();

            if (Id.HasValue)
            {
                var title = await _service.GetTitleByIdAsync(Id.Value);
                if (title == null)
                    return NotFound();

                PrimaryTitle = title.PrimaryTitle;
                OriginalTitle = title.OriginalTitle;
                TitleType = title.TitleType;
                StartYear = title.StartYear;
                EndYear = title.EndYear;
                RuntimeMinutes = title.RuntimeMinutes;
                SelectedGenres = title.TitleGenres.Select(tg => tg.GenreId).ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadGenresAsync();

            if (!ModelState.IsValid)
                return Page();

            var entity = await _service.InsertOrUpdateTitleAsync(
                Id,
                PrimaryTitle,
                OriginalTitle,
                TitleType,
                StartYear,
                EndYear,
                RuntimeMinutes,
                SelectedGenres.ToArray()
            );

            SuccessMessage = Id == null
                ? $"Title '{PrimaryTitle}' successfully added."
                : $"Title '{PrimaryTitle}' successfully updated.";

            return RedirectToPage("/Title", new { id = entity.Id });
        }

        private async Task LoadGenresAsync()
        {
            var genres = await _service.GetGenresAsync();
            AllGenres = genres
                .OrderBy(g => g.Name)
                .Select(g => new SelectListItem(g.Name, g.Id.ToString(),
                    SelectedGenres.Contains(g.Id)))
                .ToList();
        }
    }
}