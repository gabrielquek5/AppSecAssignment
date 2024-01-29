using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.ViewModels;
using WebApplication3.Model;
using System.Net;
using System.IO;
using System.Text.Json;



namespace WebApplication3.Pages
{
    public class LoginModel : PageModel
    {

        [BindProperty]
		
		public Login LModel {get; set;}

		private readonly SignInManager<ApplicationUser> signInManager;
		public LoginModel(SignInManager<ApplicationUser> signInManager)
		{
			this.signInManager = signInManager;
		}



		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				if (await ValidateCaptchaAsync())
				{
					var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, lockoutOnFailure: true);


					if (identityResult.Succeeded)
					{
						HttpContext.Session.SetString("LoggedIn", LModel.Email.ToString().Trim());
						string guid = Guid.NewGuid().ToString();
						HttpContext.Session.SetString("AuthToken", guid);

						// Create a cookie with the name "AuthToken" and the value of the GUID
						Response.Cookies.Append("AuthToken", guid);

						return RedirectToPage("Index");
					}

					if (identityResult.IsLockedOut)
					{

						ModelState.AddModelError("", "Account locked out. Please try again later.");
					}
					else
					{
						ModelState.AddModelError("", "Username or Password incorrect");
					}
				}
				else
				{
					ModelState.AddModelError("", "Captcha validation failed");
				}
			}

			return Page();
		}


		private async Task<bool> ValidateCaptchaAsync()
		{
			bool result = true;

			//When user submits the recaptcha form, the user gets a response POST parameter. 
			//captchaResponse consist of the user click pattern. Behaviour analytics! AI :) 
			string captchaResponse = Request.Form["g-recaptcha-response"];

			//To send a GET request to Google along with the response and Secret key.
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create
		   (" https://www.google.com/recaptcha/api/siteverify?secret=6LcunE4pAAAAAEPbqG7YKaqKHIDfluXku5EgYT6g &response=" + captchaResponse);

			try
			{

				//Codes to receive the Response in JSON format from Google Server
				using (WebResponse wResponse = req.GetResponse())
				{
					using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
					{
						// The response in JSON format
						string jsonResponse = readStream.ReadToEnd();

						// Deserialize JSON response
						var jsonObject = JsonSerializer.Deserialize<Login>(jsonResponse);

						// Create jsonObject to handle the response e.g success or Error


					}
				}

				return result;
			}
			catch (WebException ex)
			{
				throw ex;
			}
		}

	}
	}
