using BusssinessObject;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Data.Models;
using Data.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Data.Utils.HashPass;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Data.Enum.Enums;

namespace Second_Soul.Pages.UserPage.Profile
{
    public class IndexModel : PageModel
    {
        private readonly Cloudinary _cloudinary;
        private readonly IUserBusiness _userBusiness;
        public class ChangeProfileInput
        {
            [Required]
            public string FullName { get; set; } = string.Empty;
            [DataType(DataType.PhoneNumber)]
            public string Phone { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;

        }

        public IndexModel(Cloudinary cloudinary, IUserBusiness userBusiness)
        {
            _cloudinary = cloudinary;
            _userBusiness = userBusiness;
        }
        [BindProperty]
        public ChangeProfileInput Change { get; set; }
        public User? UserProfile { get; set; } = null;
        [BindProperty]
        public string ErrorMessage { get; set; }
        [BindProperty]
        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            
            var user = await _userBusiness.GetFromCookie(Request);
            if (user == null)
            {
                return RedirectToPage("/Login");

            }
            var result = await _userBusiness.GetById(user.UserId);
            UserProfile = (User)result.Data;
            Banks = Enum.GetNames(typeof(Bank))
                    .Select(b => new SelectListItem
                    {
                        Text = b.ToString(),
                        Value = b,
                        Selected = user.Bank == b.ToString()
                    })
                    .ToList();
            return Page();
        } 
        public List<SelectListItem> Banks { get; set; }

        public async Task<IActionResult> OnPostAsync(string action, IFormFile PictureFile)
        {
            var user = await _userBusiness.GetFromCookie(Request);
            if (user == null)
            {
                return RedirectToPage("/Login");

            }
            var var = await _userBusiness.GetById(user.UserId);
            UserProfile = (User)var.Data;

            switch (action)
            {
                case "UpdateEmail":
                    var confirmationLink = $"{Request.Scheme}://{Request.Host}/UserPage/ChangeEmail/{UserProfile.UserId}";
                    var emailSend = await SendMail.SendToChangeEmail(UserProfile.Email, confirmationLink);
                    SuccessMessage = "Change Email Link has been sent to your current email.";
                    return Page();
                case "UpdatePassword":
                    UserProfile.Token = FormatUtilities.GenerateRandomCodeWithExpiration(20);

                    var result = await _userBusiness.Update(UserProfile);

                    if (result != null && result.Status > 0)
                    {
                        var resetLink =
                            $"{Request.Scheme}://{Request.Host}/Verify?token={HashPassWithSHA256.HashWithSHA256(UserProfile.Token)}";
                        bool emailSent = await SendMail.SendResetLinkEmail(UserProfile.Email, resetLink);
                        if (emailSent)
                        {
                            return Page();
                        }
                    }
                    ErrorMessage = "Failed to send email. Please try again.";
                    return Page();
                case "UpdateProfile":
                    UserProfile.FullName = Change.FullName;
                    UserProfile.Address = Change.Address;
                    UserProfile.PhoneNumber = Change.Phone;
                    await _userBusiness.Update(UserProfile);
                    SuccessMessage = "Profile picture successfully changed.";
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
                    return Page();
            }
            return Page();
        }
    }
}
