using BusssinessObject;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;

namespace Second_Soul.Pages.UserPage
{
    public class ProfileModel : PageModel
    {
        private readonly Cloudinary _cloudinary;
        private readonly IUserBusiness _userBusiness;

        public ProfileModel(Cloudinary cloudinary, IUserBusiness userBusiness)
        {
            _cloudinary = cloudinary;
            _userBusiness = userBusiness;
        }

        public User? UserProfile { get; set; } = null;

        [BindProperty]
        public string ErrorMessage { get; set; } = string.Empty;
        [BindProperty]
        public string SuccessMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            UserProfile = await _userBusiness.GetFromCookie(Request);
            if (UserProfile == null)
            {
                return RedirectToPage("/Login");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile PictureFile)
        {
            UserProfile = await _userBusiness.GetFromCookie(Request);
            if (UserProfile == null )
            {
                return RedirectToPage("/Login");
            }
            
            if (PictureFile != null)
            {
                var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
                var fileExtension = Path.GetExtension(PictureFile.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ErrorMessage = "Only PNG, JPG, and JPEG formats are allowed.";
                    return Page();
                }

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(PictureFile.FileName, PictureFile.OpenReadStream())
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                UserProfile.ImageUrl = uploadResult.SecureUrl.ToString();
                await _userBusiness.Update(UserProfile);
/*                 _userBusiness.UpdateCookie(Request,Response);
*/            }
            else
            {
                ErrorMessage = "Please select a file to upload.";
                return Page();
            }

            SuccessMessage = "Profile picture successfully changed.";
            return RedirectToPage();
        }
    }
}
