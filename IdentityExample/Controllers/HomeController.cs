using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityExample.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IEmailService emailService;
        public HomeController(UserManager<IdentityUser> _userManager, SignInManager<IdentityUser> _signInManager, IEmailService _emailService)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            emailService = _emailService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            //login functionality

            var user = await userManager.FindByNameAsync(username);

            if (user != null)
            {
                //Sign-in user
                var signInResult = await signInManager.PasswordSignInAsync(user, password, false, false);
                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Secret");
                }
            }

            return RedirectToAction("Index");
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            //register functionality

            var user = new IdentityUser
            {
                UserName = username,
                Email = "",
                //PasswordHash=""
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {

                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

                var link = Url.Action(nameof(VerifyEmail), "Home", new { userId = user.Id, code }, Request.Scheme, Request.Host.ToString());

                await emailService.SendAsync("Your Email id", "Email Verify", $"<a href=\"{link}\">Verify Email</a>", true);

                return RedirectToAction("EmailVerification");

            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> VerifyEmail(string userid, string code)
        {
            var user = await userManager.FindByIdAsync(userid);
            if (user == null)
            {
                return BadRequest();
            }
            var result = await userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return View();
            }
            return View();
        }
        public IActionResult EmailVerification() => View();


        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
