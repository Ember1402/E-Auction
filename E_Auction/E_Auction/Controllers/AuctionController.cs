using E_Auction.Context;
using E_Auction.Enums;
using E_Auction.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Net.Mail;
using E_Auction.ViewModels;
using System.Net;

namespace E_Auction.Controllers
{
    [Authorize]
    public class AuctionController : Controller
    {
        private ApplicationDbContext _users = new ApplicationDbContext();
        private AuctionContext db = new AuctionContext();
        // GET: Auction
        public ActionResult Index()
        {
            var lotList = db._lots.Where(l => l.Status == LotStatus.Active).ToList();
            return View(lotList);
        }

        [HttpGet]
        public ActionResult AddLot()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddLot(Lot lot, HttpPostedFileBase uploadImage)
        {
            if (ModelState.IsValid)
            {
                byte[] imageData = null;
                var lotExists = db._lots.FirstOrDefault
                    (l => l.Name == lot.Name && l.StartDate == lot.StartDate && l.BeginCost == lot.BeginCost);
                var user = _users.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                if (lotExists == null)
                {
                    lot.EndDate = DateTime.Parse(lot.DateString.ToString());
                    if (lot.EndDate < DateTime.Now)
                    {
                        ModelState.AddModelError("", "Дата окончания не может быть в прошлом");
                        return View(lot);
                    }
                    if (lot.BeginCost < 0)
                    {
                        ModelState.AddModelError("", "Начальная ставка не может быть отрицательной");
                        return View(lot);
                    }
                    lot.CurrentCost = lot.BeginCost;
                    lot.StartDate = DateTime.Now;
                    lot.UserId = user.Id;
                    lot.Status = LotStatus.Active;
                    if (uploadImage != null)
                    {
                        using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                        {
                            imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                            lot.Image = imageData;
                        }
                    }

                    db._lots.Add(lot);
                    await db.SaveChangesAsync();
                }
                else if (lotExists != null)
                {
                    ModelState.AddModelError("", "Ваш лот уже есть на аукционе");
                }
            }
            return RedirectToAction("index", "Auction");
        }



        [HttpGet]
        public ActionResult MyBids1()
        {
            var user = _users.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            var bids = db._bids.Where(b => b.UserId == user.Id).ToList();
            if (bids != null)
            {
                var viewBids = new List<BidView>();
                foreach (var bid in bids)
                {
                    var lotName = db._lots.FirstOrDefault(l => l.LotId == bid.LotId);
                    if (lotName!=null)
                    {
                        var bidView = new BidView();
                        bidView.BidId = bid.BidId;
                        bidView.LotId = bid.LotId;
                        bidView.BidRate = bid.BidRate;
                        var ownerName = _users.Users.FirstOrDefault(u => u.Id == bid.UserId);
                        bidView.BidOwnerName = ownerName.UserName;
                        bidView.BidDate = bid.BidDate;
                        bidView.LotName = lotName.Name;
                        viewBids.Add(bidView);
                    }
                }
                return View(viewBids);
            }
            ModelState.AddModelError("", "У вас нет ставок");
            return View(bids);
        }

        [HttpGet]
        public ActionResult AddBid(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddBid(int id, Bid bid)
        {
            if (ModelState.IsValid)
            {
                var lot = db._lots.FirstOrDefault(l => l.LotId == id);

                if (lot != null)
                {
                    if (lot.CurrentCost < bid.BidRate)
                    {
                        var user = _users.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                        bid.UserId = user.Id;
                        if (bid.UserId == lot.UserId)
                        {
                            ModelState.AddModelError("", "Вы не можете делать ставки на свой лот");
                            return View(bid);
                        }
                        bid.BidDate = DateTime.Now;
                        bid.LotId = lot.LotId;
                        lot.CurrentCost = bid.BidRate;
                        db._bids.Add(bid);
                        db.Entry(lot).State = EntityState.Modified;
                        db._bids.Add(bid);
                        db.SaveChanges();
                        return RedirectToAction("Index", "Auction");
                    }
                }
            }
            ModelState.AddModelError("", "Неверный формат данных");
            return View(bid);
        }



        [HttpGet]
        public ActionResult Bids1(int id)
        {
            var bids = db._bids.Where(b => b.LotId == id).ToList();
            var viewBids = new List<BidView>();
            foreach (var bid in bids)
            {
                var bidView = new BidView();
                bidView.BidRate = bid.BidRate;
                var ownerName = _users.Users.FirstOrDefault(u => u.Id == bid.UserId);
                bidView.BidOwnerName = ownerName.UserName;
                bidView.BidDate = bid.BidDate;
                var lotName = db._lots.FirstOrDefault(l => l.LotId == bid.LotId);
                bidView.LotName = lotName.Name;
                viewBids.Add(bidView);
            }
            ViewBag.LotId = id;
            return View(viewBids);

        }

        [HttpPost]
        public async Task<ActionResult> CloseLot(int id)
        {
            if (ModelState.IsValid)
            {
                var lot = db._lots.FirstOrDefault(l => l.LotId == id);
                if (lot == null)
                {
                    ModelState.AddModelError("", "Лота с таким id нет!");
                    return View(id);
                }
                lot.Status = LotStatus.Finished;
                db.Entry(lot).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("List");
            }
            ModelState.AddModelError("", "Неверный формат данных");
            return View(id);
        }

        [HttpGet]
        public ActionResult MyAuctions()
        {
            var user = _users.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            var auctions = db._lots.Where(b => b.UserId == user.Id);
            if (auctions != null)
            {
                return View(auctions);
            }
            ModelState.AddModelError("", "У вас нет Аукционнов");
            return View(auctions);
        }

        [HttpGet]
        public ActionResult DeleteLot(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var course = db._lots.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }
        [HttpPost,ActionName("DeleteLot")]
        public ActionResult DeleteLotConfirmed(int id)
        {
            
            var lo = db._lots.Find(id);
            db._lots.Remove(lo);
            db.SaveChanges();
            return RedirectToAction("MyAuctions", "Auction");
            

        }

        [HttpGet]
        public ActionResult DeleteBid(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var course = db._bids.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }
        [HttpPost, ActionName("DeleteBid")]
        public ActionResult DeleteBidConfirmed(int id)
        {
            var bi = db._bids.Find(id);
            db._bids.Remove(bi);
            db.SaveChanges();
            return RedirectToAction("MyBids", "Auction");
           
        }

        [HttpGet]
        public ActionResult EditLot(int id)
        {
            var lot = db._lots.FirstOrDefault(l => l.LotId == id);
            if (lot != null)
            {
                return View(lot);
            }
            else
            {
                ModelState.AddModelError("", "Данный лот не найден");
                return RedirectToAction("MyAuctions", "Auction");
            }
        }
        [HttpPost]
        public ActionResult EditLot(Lot lot)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            lot.EndDate = DateTime.Parse(lot.DateString.ToString());
            if (lot.EndDate < DateTime.Now)
            {
                ModelState.AddModelError("", "Дата окончания не может быть в прошлом");
                return View(lot);
            }
            var lo = db._lots.FirstOrDefault(l => l.LotId == lot.LotId);
            lo.Name=lot.Name;
            lo.Image = lot.Image;
            lo.Status = lot.Status;
            lo.BeginCost = lot.BeginCost;
            lo.EndDate = lot.EndDate;
            lo.DateString = lot.DateString;
            lo.Description = lot.Description;
            db.Entry(lo).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("MyAuctions");
        }

       
        
    }
}