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

        public User User { get; set; }

        [BindProperty]
        public string ErrorMessage { get; set; }
        [BindProperty]
        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userJson = Request.Cookies["User"];
            if (!string.IsNullOrEmpty(userJson))
            {
                User = JsonSerializer.Deserialize<User>(userJson);
            }

            if (User == null)
            {
                return RedirectToPage("/Login");
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync(IFormFile PictureFile)
        {
            var userJson = Request.Cookies["User"];
            if (!string.IsNullOrEmpty(userJson))
            {
                User = JsonSerializer.Deserialize<User>(userJson);
            }
            if (User == null)
            {
                return RedirectToPage("/Login");
            }
            else
            {
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
                    User.ImageUrl = uploadResult.SecureUrl.ToString();
                    _userBusiness.Update(User);
                }
                else
                {
                    ErrorMessage = "Please select a file to upload.";
                    return Page();

                }

            }


            SuccessMessage = "Profile picture successfully change";
            return RedirectToPage();
        }
    }
}
