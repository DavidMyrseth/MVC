using MVC_projekt_Peole_kutsumine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace MVC_projekt_Peole_kutsumine.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //динамическое свойство ViewBag.Greeting таким образом, чтобы программа
            //отределяла все четыре времени суток и соответственно здоровалась.
            int hour = DateTime.Now.Hour;
            if (hour >= 5 && hour < 12) //утро - от восхода (условно 5-6 часов) до 12 часов
            {
                ViewBag.Greeting = "Tere hommikust"; 
            }
            else if (hour >= 12 && hour < 19) //день - с 12.00 до 18.00-19.00
            {
                ViewBag.Greeting = "Tere päevast";
            }
            else if (hour >= 19 && hour < 23) //вечер - с 18.00-19.00 до 23.00.
            {
                ViewBag.Greeting = "Tere õhtust"; 
            }
            else
            {
                ViewBag.Greeting = "Head ööd"; 
            }

            //добавьте ViewBag.Message динамуку, например, в зависимости от месяца название
            //праздника будет меняться(Может меняться и картинка тоже)

            int month = DateTime.Now.Month;
            if (month == 12)
            {
                ViewBag.Message = "Häid jõule ja head uut aastat!";
                ViewBag.ImagePath = "~/Images/head.jpg";
            }
            else if (month == 1)
            {
                ViewBag.Message = "Happy New Year!";
                ViewBag.ImagePath = "~/Images/new_year!.jpg";
            }
            else if (month == 2)
            {
                ViewBag.Message = "Head sõbrapäeva";
                ViewBag.ImagePath = "~/Images/soberpaev.jpg";
            }
            else if (month == 4)
            {
                ViewBag.Message = "Lihavõttepühade 1. päev";
                ViewBag.ImagePath = "~/Images/haid_puhi.jpg";
            }
            else if (month == 5)
            {
                ViewBag.Message = "Kevadpäev";
                ViewBag.ImagePath = "~/Images/kevadpaev.jpg";
            }
            else if (month == 6)
            {
                ViewBag.Message = "Võidupüha";
                ViewBag.ImagePath = "~/Images/voidupuha.jpg";
            }
            else
            {
                ViewBag.Message = "Ootan sind minu peole! Palun tule!!!";
                ViewBag.ImagePath = "~/Images/kutse.jpg";
            }

            // Проверка, авторизован ли пользователь
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.LoginStatus = "Вы авторизованы. Ваш логин: " + User.Identity.Name;
            }
            else
            {
                ViewBag.LoginStatus = "Вы не авторизованы";
            }

            return View();
        }

        [HttpGet]
        public ViewResult Ankeet()
        {
            return View();
        }

        [HttpPost]
        public ViewResult Ankeet(Guest guest)
        {
            if (ModelState.IsValid)
            {
                E_mail(guest);
                if (ModelState.IsValid)
                {
                    db.Guests.Add(guest);
                    db.SaveChanges();
                    return View("Thanks", guest);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View(guest);
            }
        }

        public void E_mail(Guest guest)
        {
            try
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.EnableSsl = true;
                WebMail.UserName = "daragalcenko3@gmail.com";
                WebMail.Password = "iqer zkvm czuv lgqn";
                WebMail.From = "daragalcenko3@gmail.com";
                WebMail.Send(guest.Email, " Vastus kutsele ", guest.Name + " vastas " 
                    + ((guest.WillAttend ?? false ? " tuleb peole" : " ei tule saatnud"))); ViewBag.Message = "Kiri on saatnud!";
                ViewBag.Message = "Kiri on saatnud!";
            }
            catch (Exception)
            {
                ViewBag.Message = "Mul on kahju!Ei saa kirja saada!!!";
            }
        }
        [HttpPost]
        //Добавьте кнопку "Отправить напоминание/Meeldetuletus", по нажатию на которую должно отправиться
        //письмо с приложением(файлом) и отобразиться соответствующее
        //представление с сообщением об отправке письма. 
        public ActionResult Meeldetuletus(Guest guest, string Meeldetuletus)
        {
            if (!string.IsNullOrEmpty(Meeldetuletus))
            {
                try
                {
                    WebMail.SmtpServer = "smtp.gmail.com";
                    WebMail.SmtpPort = 587;
                    WebMail.EnableSsl = true;
                    WebMail.UserName = "daragalcenko3@gmail.com";
                    WebMail.Password = "iqer zkvm czuv lgqn";
                    WebMail.From = "daragalcenko3@gmail.com";

                    WebMail.Send(guest.Email, "Meeldetuletus", guest.Name + ", ara unusta. Pidu toimub 25.01.25! Sind ootavad väga!",
                    null, guest.Email,
                    filesToAttach: new String[] { Path.Combine(Server.MapPath("~/Images/"), Path.GetFileName("kutse.jpg ")) }
                   );

                    ViewBag.Message = "Kutse saadetud!";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Tekkis viga kutse saatmisel: " + ex.Message;
                }
            }

            return View("Thanks", guest);
        }
        GuestContext db = new GuestContext();
        [Authorize] //- Данное представление Guests сможет увидить только авторизованный пользователь.
        public ActionResult Guests()
        {
            IEnumerable<Guest> guests = db.Guests;
            return View(guests);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Guest guest)
        {
            db.Guests.Add(guest);
            db.SaveChanges();
            return RedirectToAction("Guests");
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            Guest g = db.Guests.Find(id);
            if (g == null)
            {
                return HttpNotFound();
            }
            return View(g);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Guest g = db.Guests.Find(id);
            if (g == null)
            {
                return HttpNotFound();
            }
            db.Guests.Remove(g);
            db.SaveChanges();
            return RedirectToAction("Guests");
        }
    }
}
