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
        public class BankInput
        {
            [Required]
            public string Bank { get; set; } = string.Empty;
            [Required]
            public string BankInfo { get; set; } = string.Empty;
            [Required]
            public string BankUser { get; set; } = string.Empty;

        }
        public IndexModel(Cloudinary cloudinary, IUserBusiness userBusiness)
        {
            _cloudinary = cloudinary;
            _userBusiness = userBusiness;
        }
        [BindProperty]
        public ChangeProfileInput Change { get; set; } = new();
        [BindProperty]
        public BankInput BankInputs { get; set; } = new();
        public User? UserProfile { get; set; } = null;
        public string PopupMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            UserProfile = await _userBusiness.GetFromCookie(Request);
            if (UserProfile == null)
            {
                return RedirectToPage("/Login");
            }

            Banks = Enum.GetNames(typeof(Bank))
                    .Select(b => new SelectListItem
                    {
                        Text = b.ToString(),
                        Value = b,
                        Selected = UserProfile.Bank == b.ToString()
                    })
                    .ToList();
            return Page();
        }
        public List<SelectListItem> Banks { get; set; } = [];

        public async Task<IActionResult> OnPostAsync(string action, IFormFile PictureFile)
        {
            UserProfile = await _userBusiness.GetFromCookie(Request);
            if (UserProfile == null)
            {
                return RedirectToPage("/Login");
            }

            switch (action)
            {
                case "UpdateBank":
                    if (string.IsNullOrWhiteSpace(BankInputs.BankInfo) || string.IsNullOrWhiteSpace(BankInputs.Bank) || string.IsNullOrWhiteSpace(BankInputs.BankUser))
                    {
                        PopupMessage = "Vui long ghi day du thong tin ngan hang";
                        return await OnGetAsync();
                    }
                    UserProfile.Bank = BankInputs.Bank;
                    UserProfile.Bankinfo = BankInputs.BankInfo;
                    UserProfile.Bankuser = BankInputs.BankUser;
                    await _userBusiness.Update(UserProfile);
                    PopupMessage = "Banking infomation successfully updated.";
                    return await OnGetAsync();
                case "Withdraw":
                    if (UserProfile.Wallet == 0)
                    {
                        PopupMessage = "Your Balance need to be more than 0";
                        return await OnGetAsync();
                    }
                    if (UserProfile.Bank == null || UserProfile.Bankinfo == null || UserProfile.Bankuser == null)
                    {
                        PopupMessage = "Please fill in your banking infomation";
                        return await OnGetAsync();
                    }
                    var domain = Request.Scheme + "://" + Request.Host;
                    bool emailSentbank = await SendMail.SendBankTransferEmail("mphamtran8@gmail.com", UserProfile, domain);
                    if (emailSentbank)
                    {
                        PopupMessage = "Your withdraw request has been send,please wait 1-2hours for proceed.";
                        return await OnGetAsync();
                    }
                    else
                    {
                        PopupMessage = "Failed to send email. Please try again.";
                        return await OnGetAsync();

                    }
                case "UpdateEmail":
                    UserProfile.Token = FormatUtilities.GenerateRandomCodeWithExpiration(20);
                    var emailResult = await _userBusiness.Update(UserProfile);

                    if (emailResult != null && emailResult.Status > 0)
                    {

                        var confirmationLink = $"{Request.Scheme}://{Request.Host}/UserPage/ChangeEmail?token=" + HashPassWithSHA256.HashWithSHA256(UserProfile.Token);
                        var emailSend = await SendMail.SendToChangeEmail(UserProfile.Email, confirmationLink);
                        if (emailSend)
                        {
                            PopupMessage = "Change Email Link has been sent to your current email.";
                            return await OnGetAsync();
                        }
                    }
                    PopupMessage = "Changing the Email has failed.";
                    break;
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
                            return await OnGetAsync();
                        }
                    }
                    PopupMessage = "Failed to send email. Please try again.";
                    return await OnGetAsync();
                case "UpdateProfile":
                    if (string.IsNullOrWhiteSpace(Change.FullName) || string.IsNullOrEmpty(Change.Address) || string.IsNullOrWhiteSpace(Change.Phone))
                    {
                        PopupMessage = "Vui long ghi day du thong tin tai khoan.";
                        return await OnGetAsync();
                    }
                    UserProfile.FullName = Change.FullName;
                    UserProfile.Address = Change.Address;
                    UserProfile.PhoneNumber = Change.Phone;
                    await _userBusiness.Update(UserProfile);
                    PopupMessage = "Profile successfully changed.";
                    return await OnGetAsync();
                case "UpdatePicture":
                    if (PictureFile != null)
                    {
                        var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
                        var fileExtension = Path.GetExtension(PictureFile.FileName).ToLower();
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            PopupMessage = "Only PNG, JPG, and JPEG formats are allowed.";
                            return await OnGetAsync();
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
                        PopupMessage = "Please select a file to upload.";
                        return await OnGetAsync();
                    }

                    PopupMessage = "Profile picture successfully changed.";
                    return await OnGetAsync();
            }
            return await OnGetAsync();
        }
    }
}
