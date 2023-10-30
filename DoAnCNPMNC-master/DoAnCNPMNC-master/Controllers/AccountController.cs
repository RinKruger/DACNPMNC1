using Firebase.Auth;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TestingProject.Models;
using Microsoft.Owin.Security;

namespace TestingProject.Controllers
{
    public class AccountController : Controller
    {
        private static string ApiKey = "AIzaSyBmk6KmAsT_jTtmFbPoYNxcZEUePuX0Ryk";
        private static string Bucket = "syncdatabase-73aa0.appspot.com";
        // GET: Account
        public ActionResult SignUp()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> SignUp(SignUpModel model)
        {
            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                var a = await auth.CreateUserWithEmailAndPasswordAsync(model.Email, model.Password,model.Name, true);
                ModelState.AddModelError(string.Empty, "Please Verify your email then login plz)");
            }
            catch (Exception ex) 
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                if(this.Request.IsAuthenticated)
                {
                    return this.RedirectToLocal(returnUrl);
                }
            }
            catch(Exception ex) 
            {
                Console.Write(ex);
            }
            return this.View(); 
        }
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                    var ab = await auth.SignInWithEmailAndPasswordAsync(model.Email, model.Password);
                    string token = ab.FirebaseToken;
                    var user = ab.User;
                    if(token != "") 
                    {
                        this.SignInUser(user.Email, token, false);
                        return this.RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid username or password");
                    }
                }
            }
            catch(Exception ex)
            {
                Console.Write(ex);
            }
            return this.View(model);
        }
        private void SignInUser(string email,string token, bool isPersistent)
        {
            var claims = new List<Claim>();
            try
            {
                claims.Add(new Claim(ClaimTypes.Email, email));
                claims.Add(new Claim(ClaimTypes.Authentication, token));
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = Request.GetOwinContext();
                var authenticantionManager = ctx.Authentication;
                authenticantionManager.SignIn(new AuthnticationProperties() { IsPersistent = isPersistent }, claimIdenties);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void ClaimIdentites(string username, bool isPersistent)
        {
            var claims = new List<Claim>();
            try
            {
                claims.Add(new Claim(ClaimTypes.Name, username));
                var claimsIdentity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
            }
            catch(Exception ex) 
            {
                throw ex;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            try
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return this.Redirect(returnUrl);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return this.RedirectToAction("LogOff", "Account");
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogOff()
        {
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }
    }
}   