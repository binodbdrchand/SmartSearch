using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Serilog;
using SmartSearch.Modules.UserManager.Service;
using SmartSearch.Modules.UserManager.ViewModel;
using System.Web.Helpers;

namespace SmartSearch.UI.WebMVC.Controllers;

public class AccountController : Controller
{
    private readonly IAppUserService _appUserService;

    public AccountController(IAppUserService appUserService)
    {
        _appUserService = appUserService;
    }

    [HttpGet]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Create(AppUserViewModel user)
    {
        if (ModelState.IsValid)
        {
            try
            {
                user.Password = EncodePassword(user.Password);
                //_appUserService.Insert(user);

                TempData["SuccessMessage"] = "User created Successfully";
            }
            catch (Exception ex)
            {
                Log.Error($"(AccountController::Create) -- ERROR -- {ex}");
                TempData["FailureMessage"] = "EXCEPTION creating User";
            }
        }
        else
        {
            TempData["FailureMessage"] = "Fix the form errors";
            return RedirectToAction("Create");
        }

        return RedirectToAction("List", new
        {
            email = HttpContext.Session.GetString("Email"),
            isAdmin = HttpContext.Session.GetString("IsAdmin")
        });
    }

    [HttpGet]
    public ActionResult Delete(string email)
    {
        try
        {
            if (email != null)
            {
                _appUserService.Delete(email);

                TempData["SuccessMessage"] = "User deleted Successfully";
            }
            else
            {
                TempData["FailureMessage"] = "ERROR deleting User";
            }
        }
        catch (Exception ex)
        {
            Log.Error($"(AccountController::Delete) -- ERROR -- {ex}");
            TempData["FailureMessage"] = "EXCEPTION deleting User";
        }

        return RedirectToAction("List", new
        {
            email = HttpContext.Session.GetString("Email"),
            isAdmin = HttpContext.Session.GetString("IsAdmin")
        });
    }

    [HttpGet]
    public ActionResult Edit(string email)
    {
        var user = _appUserService.GetByEmail(email, Convert.ToBoolean(HttpContext.Session.GetString("IsAdmin")));

        return View(user);
    }

    [HttpPost]
    public ActionResult Edit(AppUserViewModel user)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (!user.Password.EndsWith("="))
                {
                    user.Password = EncodePassword(user.Password);
                }
                _appUserService.Update(user);

                TempData["SuccessMessage"] = "User updated Successfully";
            }
            catch (Exception ex)
            {
                Log.Error($"(AccountController::Edit) -- ERROR -- {ex}");
                TempData["FailureMessage"] = "EXCEPTION updating User";
            }
        }
        else
        {
            TempData["FailureMessage"] = "Fix the form errors";
            return RedirectToAction("Edit", new { email = user.Email });
        }

        return RedirectToAction("List", new
        {
            email = HttpContext.Session.GetString("Email"),
            isAdmin = HttpContext.Session.GetString("IsAdmin")
        });
    }

    [HttpGet]
    public ActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public ActionResult List(string email, bool isAdmin)
    {
        try
        {
            List<AppUserViewModel> users = new List<AppUserViewModel>();

            if (isAdmin)
            {
                var results = _appUserService.GetAll();

                if (results.Count > 0)
                {
                    users.AddRange(results);
                }
                else
                {
                    return RedirectToAction("Logout");
                }
            }
            else
            {
                var user = _appUserService.GetByEmail(email, Convert.ToBoolean(HttpContext.Session.GetString("IsAdmin")));

                if (user != null)
                {
                    users.Add(user);
                }
                else
                {
                    return RedirectToAction("Logout");
                }
            }

            return View(users);
        }
        catch (Exception ex)
        {
            Log.Error($"(AccountController::GetUsers) -- ERROR -- {ex}");
            TempData["FailureMessage"] = "EXCEPTION listing Users";
        }

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    public ActionResult Login(IFormCollection form)
    {
        try
        {
            var email = form["email"].ToString();
            var password = form["password"].ToString();

            var user = _appUserService.GetByEmail(email, Convert.ToBoolean(HttpContext.Session.GetString("IsAdmin")));

            if (user != null)
            {
                if (EncodePassword(password) == user.Password)
                {
                    HttpContext.Session.SetString("Email", user.Email);
                    HttpContext.Session.SetString("IsAuthenticated", "true");
                    HttpContext.Session.SetString("IsAdmin", user.IsAdmin ? "true" : "false");

                    return RedirectToAction("Index", "Dashboard");
                }
            }
            else
            {
                TempData["FailureMessage"] = "ERROR loggin in";
            }
        }
        catch (Exception ex)
        {
            Log.Error($"(AccountController::GetUsers) -- ERROR -- {ex}");
            TempData["FailureMessage"] = "EXCEPTION logging in";
        }

        return RedirectToAction("Index", "Account");
    }

    public ActionResult Logout()
    {
        HttpContext.Session.Clear();
        HttpContext.Session.Remove("Email");
        HttpContext.Session.Remove("IsAuthenticated");
        HttpContext.Session.Remove("IsAdmin");

        return RedirectToAction("Index");
    }

    private string EncodePassword(string plainTextPassword)
    {
        byte[] passwordBytes = new byte[plainTextPassword.Length];
        passwordBytes = System.Text.Encoding.UTF8.GetBytes(plainTextPassword);

        return Convert.ToBase64String(passwordBytes);
    }
}
