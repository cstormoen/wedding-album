using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WeddingAlbum.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    public IList<string> ImagePaths { get; set; } = new List<string>();

    [BindProperty]
    public List<IFormFile> Photos { get; set; }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;

        var images = Directory.GetFiles(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads"),
            "*.*"
        );
        var imagePaths = images.Select(f => Path.GetRelativePath("wwwroot", f));

        ImagePaths = imagePaths.ToList();
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Photos == null || Photos.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "No photo(s) were added");
            return Page();
        }

        string allowedExtensions = ".jpg,.jpeg,.png,.gif,.bmp";

        foreach (var photo in Photos)
        {
            if (photo.Length > 0)
            {
                string fileExtension = Path.GetExtension(photo.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError(string.Empty, "No photo(s) were added");
                    return Page();
                }

                string newFileName = CreateFileName(photo);
                string filePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    newFileName
                );

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }
            }
        }

        _logger.LogInformation($"Det ble lastet opp {Photos.Count} filer.");

        return RedirectToPage("Index", "", "photos");
    }

    private string CreateFileName(IFormFile photo)
    {
        string randomString = GetGuid();
        string fileName = Path.GetFileName(photo.FileName);
        string extension = Path.GetExtension(fileName).ToLower();
        string newFileName = $"{randomString}{extension}";
        return newFileName;
    }

    private string GetGuid()
    {
        return Guid.NewGuid().ToString("N");
    }
}
