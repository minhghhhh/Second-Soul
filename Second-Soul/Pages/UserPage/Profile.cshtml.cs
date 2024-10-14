using BusssinessObject;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Data.Models;
using Data.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<IActionResult> OnPostAsync(string action, IFormFile PictureFile)
        {
            UserProfile = await _userBusiness.GetFromCookie(Request);
            if (UserProfile == null)
            {
                return RedirectToPage("/Login");
            }

            switch (action)
            {
                case "UpdateEmail":
                    var confirmationLink =
$"https://localhost:7141/UserPage/ChangeEmail/{UserProfile.UserId}";
                    //  $"https://secondsoul2nd.azurewebsites.net/UserPage/ChangeEmail/{UserProfile.UserId}"; //deploy
                    var emailSend = await SendMail.SendToChangeEmail(UserProfile.Email, confirmationLink);
                    SuccessMessage = "Change Email Link has been sent to your current email.";
                    return Page();
                case "UpdatePassword":
                    return Page();
                case "UpdateProfile":

                    return Page();
                case "UpdatePicture":
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
                    }
                    else
                    {
                        ErrorMessage = "Please select a file to upload.";
                        return Page();
                    }

                    SuccessMessage = "Profile picture successfully changed.";
                    return RedirectToPage();
            }
            return Page();
        }
    }
}
