using Data.Models;
using Data.Utils.HashPass;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Caching.Memory;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Data.Utils
{
	public class SendMail
	{
		public static async Task<bool> SendResetPass(IMemoryCache cache, string toEmail, string code, bool showExpirationTime)
		{
			var userName = "Second Soul";
			var emailFrom = "chechminh1136@gmail.com";
			var password = "fnwl dkyf sqps wgoq";

			var subjet = "Reset Password Confirmation";

			if (!showExpirationTime)
			{
				code = new string(code.Take(6).ToArray());
			}
			var body = $"Please enter this code to reset your password: {code}";

			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(userName, emailFrom));
			message.To.Add(new MailboxAddress("", toEmail));
			message.Subject = subjet;
			message.Body = new TextPart("html")
			{
				Text = body
			};
			// Lưu mã OTP vào cache
			string key = $"{toEmail}_OTP";
			cache.Set(key, code, TimeSpan.FromMinutes(1));
			// Thêm logic xóa key OTP khi hết hạn
			Task.Delay(TimeSpan.FromMinutes(1)).ContinueWith(_ =>
			{
				cache.Remove(key);
			});

			using (var client = new SmtpClient())
			{
				client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
				//authenticate account email
				client.Authenticate(emailFrom, password);

				try
				{
					await client.SendAsync(message);
					await client.DisconnectAsync(true);
					return true;
				}
				catch (Exception ex)
				{
					System.Console.WriteLine(ex.Message);
					return false;
				}
			}
		}
		public static async Task<bool> SendResetLinkEmail(
	string toEmail,
	string resetLink
)
		{
			var userName = "Second Soul";
			var emailFrom = "chechminh1136@gmail.com";
			var password = "fnwl dkyf sqps wgoq";

			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(userName, emailFrom));
			message.To.Add(new MailboxAddress("", toEmail));
			message.Subject = "Your Password Reset Token";
			message.Body = new TextPart("html")
			{
				Text =
					@"
    <html>
        <head>
            <style>
                body {
                    display: flex;
                    justify-content: center;
                    align-items: center;
                    height: 100vh;
                    margin: 0;
                    font-family: Arial, sans-serif;
                }
                .content {
                    text-align: center;
                }
                .token {
                    display: inline-block;
                    padding: 10px 20px;
                    background-color: #f0f0f0;
                    border: 1px solid #ccc;
                    border-radius: 5px;
                    font-size: 16px;
                    margin-top: 20px;
                }
            </style>
        </head>
        <body>
            <body>
                <div class='content'>
                    <p>Please click the button below to reset your password:</p>                    
                      <a class='button' href='"
					+ resetLink
					+ "'>Reset Password</a>"
					+ @"
                </div>
            </body>
        </body>
    </html>
"
			};

			using var client = new SmtpClient();
			try
			{
				await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
				await client.AuthenticateAsync(emailFrom, password);
				await client.SendAsync(message);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error sending email: {ex.Message}");
				return false;
			}
			finally
			{
				await client.DisconnectAsync(true);
			}
		}

		public static async Task<bool> SendConfirmationEmail(
			string toEmail,
			string confirmationLink
		)
		{
			var userName = "Second Soul";
			var emailFrom = "chechminh1136@gmail.com";
			var password = "fnwl dkyf sqps wgoq";

			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(userName, emailFrom));
			message.To.Add(new MailboxAddress("", toEmail));
			message.Subject = "Confirmation your email to login";
			message.Body = new TextPart("html")
			{
				Text =
					@"
        <html>
            <head>
                <style>
                    body {
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        height: 100vh;
                        margin: 0;
                        font-family: Arial, sans-serif;
                    }
                    .content {
                        text-align: center;
                    }
                    .button {
                        display: inline-block;
                        padding: 10px 20px;
                        background-color: #000;
                        color: #ffffff;
                        text-decoration: none;
                        border-radius: 5px;
                        font-size: 16px;
                    }
                </style>
            </head>
            <body>
                <div class='content'>
                    <p>Please click the button below to confirm your email:</p>                    
                      <a class='button' href='"
					+ confirmationLink 
                    + "'>Confirm Email</a>"
					+ @"
                </div>
            </body>
        </html>
    "
			};
			using (var client = new SmtpClient())
			{
				client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
				//authenticate account email
				client.Authenticate(emailFrom, password);

				try
				{
					await client.SendAsync(message);
					await client.DisconnectAsync(true);
					return true;
				}
				catch (Exception ex)
				{
					System.Console.WriteLine(ex.Message);
					return false;
				}
			}
		}
    public static async Task<bool> SendToChangeEmail(
    string toEmail,
    string confirmationLink
)
        {
            var userName = "Second Soul";
            var emailFrom = "chechminh1136@gmail.com";
            var password = "fnwl dkyf sqps wgoq";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Change Email";
            message.Body = new TextPart("html")
            {
                Text =
                    @"
        <html>
            <head>
                <style>
                    body {
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        height: 100vh;
                        margin: 0;
                        font-family: Arial, sans-serif;
                    }
                    .content {
                        text-align: center;
                    }
                    .button {
                        display: inline-block;
                        padding: 10px 20px;
                        background-color: #000;
                        color: #ffffff;
                        text-decoration: none;
                        border-radius: 5px;
                        font-size: 16px;
                    }
                </style>
            </head>
            <body>
                <div class='content'>
                    <p>Please click the button below to change your email:</p>                    
                      <a class='button' href='"
                    + confirmationLink
                    + "'>Change Email</a>"
                    + @"
                </div>
            </body>
        </html>
    "
            };
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                //authenticate account email
                client.Authenticate(emailFrom, password);

                try
                {
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    return true;
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public static async Task<bool> SendRegistrationSuccessEmail(string toEmail)
		{
			var userName = "Second Soul";
			var emailFrom = "chechminh1136@gmail.com";
			var password = "fnwl dkyf sqps wgoq";

			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(userName, emailFrom));
			message.To.Add(new MailboxAddress("", toEmail));
			message.Subject = "Registration Successful";
			message.Body = new TextPart("html")
			{
				Text =
					@"
    <html>
        <head>
            <style>
                body {
                    display: flex;
                    justify-content: center;
                    align-items: center;
                    height: 100vh;
                    margin: 0;
                    font-family: Arial, sans-serif;
                }
                .content {
                    text-align: center;
                }
            </style>
        </head>
        <body>
            <div class='content'>
                <p>Your registration has been confirmed successfully. Welcome to Second Soul!</p>
            </div>
        </body>
    </html>
"
			};
			using (var client = new SmtpClient())
			{
				client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
				//authenticate account email
				client.Authenticate(emailFrom, password);

				try
				{
					await client.SendAsync(message);
					await client.DisconnectAsync(true);
					return true;
				}
				catch (Exception ex)
				{
					System.Console.WriteLine(ex.Message);
					return false;
				}
			}
		}
        public static async Task<bool> SendBankTransferEmail(string toEmail, User user, string domain)
        {
            var userName = "Second Soul";
            var emailFrom = "chechminh1136@gmail.com";
            var password = "fnwl dkyf sqps wgoq";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Bank Transfer Details";
            message.Body = new TextPart("html")
            {
                Text =
                    $@"
    <html>
        <head>
            <style>
                body {{
                    display: flex;
                    justify-content: center;
                    align-items: center;
                    height: 100vh;
                    margin: 0;
                    font-family: Arial, sans-serif;
                }}
                .content {{
                    text-align: center;
                }}
                .info {{
                    margin-top: 20px;
                    font-size: 16px;
                }}
                .field {{
                    font-weight: bold;
                }}
                .button {{
                    display: inline-block;
                    padding: 10px 20px;
                    margin: 10px;
                    color: white;
                    background-color: #28a745; /* Green for success */
                    text-decoration: none;
                    border-radius: 5px;
                }}
                .button-wrong {{
                    background-color: #dc3545; /* Red for wrong */
                }}
            </style>
        </head>
        <body>
            <div class='content'>
                <h2>Withdraw Request</h2>
                <p>User: {user.Username}, UserId: {user.UserId}, User Email: {user.Email}</p>
                <div class='info'>
                    <p><span class='field'>Amount:</span> {user.Wallet.ToString("N0")} VND</p>
                    <p><span class='field'>Bank:</span> {user.Bank}</p>
                    <p><span class='field'>Account Number:</span> {user.Bankinfo}</p>
                    <p><span class='field'>Receiver Full Name:</span> {user.Bankuser}</p>
                </div>
                <div>
                    <a href='{domain}/success/{user.UserId}' class='button'>Success Withdraw</a>
                    <a href='{domain}/fail/{user.UserId}' class='button button-wrong'>Wrong Information</a>
                </div>
            </div>
        </body>
    </html>"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate(emailFrom, password);

                try
                {
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
        public static async Task<bool> SendSuccessWithdrawEmail(string toEmail, User user, int wallet)
        {
            var userName = "Second Soul";
            var emailFrom = "chechminh1136@gmail.com";
            var password = "fnwl dkyf sqps wgoq";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Withdraw Success";
            message.Body = new TextPart("html")
            {
                Text =
                    $@"
    <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                }}
                .content {{
                    text-align: center;
                }}
                .info {{
                    margin-top: 20px;
                    font-size: 16px;
                }}
            </style>
        </head>
        <body>
            <div class='content'>
                <h2>Withdraw Successful!</h2>
                <p>Dear {user.Username},</p>
                <p>Your withdrawal of {user.Wallet} VND has been processed successfully.</p>
            </div>
        </body>
    </html>"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate(emailFrom, password);

                try
                {
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public static async Task<bool> SendWrongInformationEmail(string toEmail, User user)
        {
            var userName = "Second Soul";
            var emailFrom = "chechminh1136@gmail.com";
            var password = "fnwl dkyf sqps wgoq";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Wrong Information";
            message.Body = new TextPart("html")
            {
                Text =
                    $@"
    <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                }}
                .content {{
                    text-align: center;
                }}
                .info {{
                    margin-top: 20px;
                    font-size: 16px;
                }}
            </style>
        </head>
        <body>
            <div class='content'>
                <h2>Withdraw Request Error</h2>
                <p>Dear {user.Username},</p>
                <p>We encountered an error with your withdrawal request due to incorrect information.</p>
                <p>Please verify your details and try again.</p>
            </div>
        </body>
    </html>"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate(emailFrom, password);

                try
                {
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
        public static async Task<bool> SendOrderPaymentSuccessEmail(Order order,List<OrderDetail> orderDetails, User user)
        {
            var userName = "Second Soul";
            var emailFrom = "chechminh1136@gmail.com";
            var password = "fnwl dkyf sqps wgoq";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));  
            message.To.Add(new MailboxAddress("", user.Email));
            message.Subject = "Order Payment Successful";
            var orderItemsHtml = string.Join("", orderDetails.Select(item => $@"
                    <tr>
                        <td>{item.Product.Name}</td>
                        <td>{item.Product.Size}</td>
                        <td>{item.Product.Condition}</td>
                        <td>{item.Price.ToString("N0")} VND</td>
                    </tr>
                "));

            message.Body = new TextPart("html")
            {
                Text = $@"
            <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            margin: 0;
                            padding: 0;
                            display: flex;
                            justify-content: center;
                            align-items: center;
                            height: 100vh;
                        }}
                        .container {{
                            width: 80%;
                            margin: auto;
                        }}
                        .content {{
                            text-align: center;
                        }}
                        table {{
                            width: 100%;
                            border-collapse: collapse;
                        }}
                        th, td {{
                            padding: 10px;
                            border: 1px solid #ddd;
                            text-align: left;
                        }}
                        th {{
                            background-color: #f2f2f2;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='content'>
                            <h1>Thank you for your purchase, {user.Username}!</h1>
                            <p>Your payment for order ID {order.OrderId} has been confirmed successfully on {order.OrderDate:MMMM dd, yyyy}.</p>
                            <h2>Order Details</h2>
                            <table>
                                <thead>
                                    <tr>
                                        <th>Product Name</th>
                                        <th>Size</th>
                                        <th>Condition</th>
                                        <th>Price</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {orderItemsHtml}
                                </tbody>
                                <tfoot>
                                    <tr>
                                        <td colspan='3' style='text-align:right'><strong>Shipping fee:</strong></td>
                                        <td>30,000 VND</td>
                                    </tr>

                                    <tr>
                                        <td colspan='3' style='text-align:right'><strong>Total Price:</strong></td>
                                        <td>{order.TotalAmount.ToString("N0")} VND</td>
                                    </tr>

                                </tfoot>
                            </table>
                        </div>
                    </div>
                </body>
            </html>"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate(emailFrom, password);

                try
                {
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    return true;
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

    }
}
