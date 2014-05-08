// Copyright (c) 2014, Insya Interaktif.
// Developer @yasinkuyu
// All rights reserved.

using System;
using System.Web.Mvc;
using System.Web.Security;
using Insya.NetDash.Models;
using Insya.NetDash;

namespace Insya.NetDash.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var username = model.Username;
                var password = model.Password;
                var userValid = SqLiteDatabase.Authenticate(username, password);

                if (userValid)
                {
                    FormsAuthentication.SetAuthCookie(username, model.RememberMe);
                    return RedirectToAction("Index", "Main");
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Main");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SqLiteDatabase.Register(model.UserName, model.Password);
                    return RedirectToAction("Login", "Account");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("RegisterError", e.Message);
                }
            }

            return View(model);
        }
    }
}
