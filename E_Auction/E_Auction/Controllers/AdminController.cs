using E_Auction.Context;
using E_Auction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Auction.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Settings()
        {
            using (var db=new AuctionContext())
            {
                var config = db.Configs.ToList();

                return View(config);
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            using (var db = new AuctionContext())
            {
                var config = db.Configs.Find(id);

                if (config != null)
                {
                    return View(config);
                }
                return View();

            }

        }

        [HttpPost]
        public ActionResult Edit(Config conf)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            using (var db=new AuctionContext())
            {
                var con = db.Configs.FirstOrDefault(c => c.Id == conf.Id);
                if (con!=null)
                {
                    con.Value = conf.Value;
                    db.Entry(con).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Settings");
                }
                ModelState.AddModelError("", "Конфигурация не найденна");
                return View();

            }
        }
    }
}