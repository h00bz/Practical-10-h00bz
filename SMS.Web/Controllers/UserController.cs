using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

using SMS.Data.Services;
using SMS.Web.Models;
using System.Security.Claims;
using SMS.Data.Entities;

namespace SMS.Web.Controllers;
public class UserController : BaseController
{
    private readonly IUserService svc;

    public UserController()
    { 
        svc = new UserServiceDb();
    }

    // GET /user/login
    public IActionResult Login()
    {
        return View();
    }

    // TBC - add Profile Action - optional question


    // POST /user/login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([Bind("Email,Password")]User m)
    {        
        // call service to Authenticate User
        var user = svc.Authenticate(m.Email, m.Password);

        // if user not authenticated manually add validation errors for email and password
        if (user == null)
        {
            ModelState.AddModelError("Email", "Invalid Login Credentials");
            ModelState.AddModelError("Password", "Invalid Login Credentials");
            return View(m);
        }
        
        // authenticated so sign user in using cookie authentication to store principal
        
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            BuildClaimsPrincipal(user)
        );
        return RedirectToAction("Index","Home");
    }

    // GET /user/register
    public IActionResult Register()
    {
        return View();
    }

    // POST /user/register
    [HttpPost]
    // TBC add validate anti forgery token decorator
    public IActionResult Register(/** TBC add bind **/ UserViewModel m)
    {
        // TBC 

        // if email address is already in use 
            //  add model state error for Email
        // endif

        // if valid modelstate
            //   call service to register user
            //   Add alert indicating success and redirect to login
        // endif

        // redisplay view with validation errors
        return View(m);   
    }


    // POST /user/logout
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }


    // GET /user/errornotauthorised
    public IActionResult ErrorNotAuthorised()
    {   
        Alert("You are not Authorised to Carry out that action");
        return RedirectToAction("Index", "Home");
    }
    
        
    // GET /user/errornotauthenticated
    public IActionResult ErrorNotAuthenticated()
    {
         Alert("You must first Authenticate to carry out that action");
        return RedirectToAction("Login", "User"); 
    }

    // =========================== PRIVATE UTILITY METHODS ==============================

    // return a claims principle using the info from the user parameter
    private ClaimsPrincipal BuildClaimsPrincipal(User user)
    { 
        // define user claims
        var claims = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Sid, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString())                              
        }, CookieAuthenticationDefaults.AuthenticationScheme);

        // build principal using claims
        return  new ClaimsPrincipal(claims);
    }       

}

