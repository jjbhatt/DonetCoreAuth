﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Basics.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }
        [Authorize(Policy = "Claim.DOB")]
        public IActionResult SecretPolicy()
        {
            return View("Secret");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult SecretRole()
        {
            return View("Secret");
        }
        public async Task<IActionResult> Authenticate()
        {
            var appClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,"Jimit"),
                new Claim(ClaimTypes.Email,"jimitbhatt92@gmail.com"),
                new Claim(ClaimTypes.DateOfBirth,"11/11/1999"),
                //new Claim(ClaimTypes.Role,"AdminTwo"),
                new Claim(ClaimTypes.Role,"Admin"),
                new Claim("App.Says","Nice work")
            };

            var licenseClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,"Jimit Bhatt"),
                new Claim("DrivingLicense","A+")
            };

            var appIdentity = new ClaimsIdentity(appClaims, "App Identity");
            var licenseIdentity = new ClaimsIdentity(appClaims, "Government");


            //We can have multiple identities since one may auth with Facebook or another using twitter - both may have diff emailid
            var userPrincipal = new ClaimsPrincipal(new[] { appIdentity, licenseIdentity });

            await HttpContext.SignInAsync(userPrincipal);

            return RedirectToAction("Index");
        }
    }
}
