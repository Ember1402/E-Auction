using E_Auction.Context;
using E_Auction.Enums;
using E_Auction.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace E_Auction.Jobs
{
    public class LotCloser : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            using (var db = new AuctionContext())
            {
                var lots = db._lots.Where(l=>l.Status==LotStatus.Active).ToList();
                var finishedLots = new List<Lot>();
                foreach (var lot in lots)
                {
                    if (lot.EndDate <= DateTime.Now)
                    {
                        lot.Status = LotStatus.Finished;
                        //db.Entry(lot).State = EntityState.Modified;
                        finishedLots.Add(lot);
                    }
                }
                await db.SaveChangesAsync();
                if (0!=finishedLots.Count)
                {
                    await SendMessagesToLots(finishedLots);
                }
            }
        }

        public Task Send(Message message)
        {
            // настройка логина, пароля отправителя
            var from = "testemailconfirmation11@gmail.com";
            var pass = "KeepItSimple11";

            // адрес и порт smtp-сервера, с которого мы и будем отправлять письмо
            SmtpClient client = new SmtpClient("smtp.gmail.com",
                587);

            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials =
                new System.Net.NetworkCredential(from, pass);
            client.EnableSsl = true;

            // создаем письмо: message.Destination - адрес получателя
            var mail = new MailMessage(from, message.MailTo);
            mail.Subject = message.Subject;
            mail.Body = message.MailContent;
            mail.IsBodyHtml = false;

            return client.SendMailAsync(mail);
            throw new NotImplementedException();
        }

        public async Task SendMessagesToLots(List<Lot> finishedLots)
        {
            var message = new Message();
            ApplicationDbContext context = new ApplicationDbContext();
            using (var db = new AuctionContext())
            {
                foreach (var lot in finishedLots)
                {
                    var user = context.Users.FirstOrDefault(u => u.Id == lot.UserId);
                    Bid maxBid = db._bids.FirstOrDefault(b => b.LotId == lot.LotId && b.BidRate==lot.CurrentCost);
                    if (maxBid==null)
                    {
                        continue;
                    }
                    var bidOwner= context.Users.FirstOrDefault(o => o.Id == maxBid.UserId);
                    message.MailContent = $"Вы выйграли лот {lot.Name}, свяжитесь с {user.UserName} чтобы забрать выйгрышь. Не забудьте опталитить ставку)))";
                    message.Subject = "E-Auction";
                    message.MailTo = bidOwner.UserName;
                    await Send(message);
                }
            } 
            
            throw new NotImplementedException();
        }

    }
}