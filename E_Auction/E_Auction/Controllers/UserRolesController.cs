using E_Auction.Models;
using E_Auction.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Auction.Controllers
{
    public class UserRolesController : Controller
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ActionResult Users()
        {
            using (var context = new ApplicationDbContext())
            {
                var userRoles = new List<UserRolesViewModel>();

                var roles = context.Roles.ToList();
                var users = context.Users.ToList();
                foreach (var user in users)
                {
                    var roleNames = new List<string>();
                    foreach (var userRole in user.Roles)
                    {
                        var role = roles.FirstOrDefault(r => r.Id == userRole.RoleId);
                        roleNames.Add(role.Name);
                    }
                    userRoles.Add(new UserRolesViewModel
                    {
                        UserID = user.Id,
                        UserName = user.UserName,
                        RoleName = string.Join(", ", roleNames)
                    });
                }

                return View(userRoles);
            }
        }

        [HttpPost]
        public ActionResult AddRoleToUser(string UserName,
             string roleID)
        {
            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.FirstOrDefault(u =>
                    u.UserName == UserName);

                var roleName = context.Roles.FirstOrDefault(r => r.Id == roleID).Name;
                UserManager.AddToRole(user.Id, roleName);

                return RedirectToAction("Users");
            }
        }


        public ActionResult EditUser(string userID)
        {
            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.FirstOrDefault(u =>
                    u.Id == userID);

                var roles = context.Roles.ToList();
                var roleNames = new List<string>();
                foreach (var userRole in user.Roles)
                {
                    var role = roles.First(r =>
                        r.Id == userRole.RoleId);
                    roleNames.Add(role.Name);
                }

                ViewBag.Roles = roles
                    .Select(r => new SelectListItem
                    {
                        Value = r.Id,
                        Text = r.Name
                    }).ToList();

                return View(new UserRolesViewModel
                {
                    UserID = user.Id,
                    UserName = user.UserName,
                    RoleName = string.Join(", ", roleNames)
                });
            }
        }

        // GET: UserRoles
        public ActionResult Index()
        {
            using (var context = new ApplicationDbContext())
            {
                var roles = context.Roles.ToList();
                return View(roles);
            }
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(IdentityRole role)
        {
            using (var context = new ApplicationDbContext())
            {
                if (!ModelState.IsValid)
                    return View();

                context.Roles.Add(role);
                context.SaveChanges();

                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Remove(string roleId)
        {
            using (var context = new ApplicationDbContext())
            {
                var roleToDelete = context.Roles.Find(roleId);
                if (roleToDelete == null)
                {
                    return Json(new { statusCode = 404, message = "Role is not Found" });
                }
                if (roleToDelete.Name == "Admin")
                {
                    return Json(new { statusCode = 403, message = "Cannot remove this role" });
                }

                context.Roles.Remove(roleToDelete);
                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { statusCode = 500, message = "Shit happens, dude" });
                }
                return Json(new { statusCode = 204, message = "Role has been delete" });
            }

        }
    }
}