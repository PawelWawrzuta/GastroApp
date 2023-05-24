using GastroApp.Data;
using GastroApp.Models.Samochody;
using GastroApp.Models.Samochody.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GastroApp.Areas.Kierowca.Controllers
{
    [Area("Kierowca")]
    [Authorize(Roles = "Kierowca")]
    public class SamochodController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<IdentityUser> _userManager;
        public SamochodController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        //Get Method
        public IActionResult CreateZuzycie()
        {
            var obj = _db.Samochody.ToList();
            ZuzycieZSamochodem zzs = new ZuzycieZSamochodem();
            zzs.SamochodSelect = new SelectList(obj, "IdSamochodu", "NrRejestracyjny");
            return View(zzs);
        }

        //Post Method
        [HttpPost]
        public async Task<IActionResult> CreateZuzycie(ZuzycieZSamochodem zzs)
        {
            int c = _db.Zuzycia.Where(x => x.IdSamochodu == zzs.IdSamochodu && x.Data>= DateTime.Today).Count();
            if (c >= 1)
            {
                ViewBag.mgs = "Dodano już raport zużycia dla tego samochodu w dniu " + DateTime.Today.ToString("dd.MM.yyyy") + "!";
                var sl = _db.Samochody.ToList();
                zzs.SamochodSelect = new SelectList(sl, "IdSamochodu", "Model");
                return View(zzs);
            }

            var obj = _db.Zuzycia.Where(x => x.IdSamochodu == zzs.IdSamochodu).ToList();
            int przebieg = Int32.Parse(zzs.PrzebiegString);

            foreach (var item in obj)
            {
                if (item.Przebieg >= przebieg)
                {
                    ViewBag.mgs = "Przebieg nie może być mniejszy niż w poprzednich raportach!";
                    var sl = _db.Samochody.ToList();
                    zzs.SamochodSelect = new SelectList(sl, "IdSamochodu", "Model");
                    return View(zzs);
                }
            }

            string idPracownika = _userManager.GetUserId(User);
            if (idPracownika == null)
            {
                TempData["fail"] = "Raport nie został dodany!";
                return View("~/Areas/Identity/Views/Home/Index.cshtml");
            }

            Zuzycie zuzycie = new Zuzycie(idPracownika, zzs.IdSamochodu, przebieg, zzs.Data);

            _db.Zuzycia.Add(zuzycie);
            await _db.SaveChangesAsync();
            TempData["success"] = "Raport zużycia został dodany!";
            return RedirectToAction("CreateTankowanie", "Samochod", new { id = zzs.IdSamochodu });
        }

        //Get Method
        public IActionResult CreateTankowanie(int id)
        {
            int idRodzajuPaliwa = _db.Samochody.FirstOrDefault(x => x.IdSamochodu == id).IdRodzajuPaliwa;

            TankowanieZRodzajemPaliwa tzrp = new TankowanieZRodzajemPaliwa();
            tzrp.IdSamochodu = id;

            int c = _db.Tankowania.Where(x => x.IdSamochodu == id && x.Data == DateTime.Today).Count() + 1;
            ViewBag.count = c;
            if (c >= 3)
            {
                return RedirectToAction("Index", "Home");
            }


            if (idRodzajuPaliwa == 1 || idRodzajuPaliwa == 2)
            {
                var obj = _db.RodzajPaliw.Where(x => x.IdRodzajuPaliwa == idRodzajuPaliwa).ToList();
                tzrp.RodzajPaliwaSelect = new SelectList(obj, "IdRodzajuPaliwa", "Nazwa");

                return View(tzrp);
            }
            else if (idRodzajuPaliwa == 3)
            {
                var obj = _db.RodzajPaliw.Where(x => x.IdRodzajuPaliwa == idRodzajuPaliwa || x.IdRodzajuPaliwa == 1);
                tzrp.RodzajPaliwaSelect = new SelectList(obj, "IdRodzajuPaliwa", "Nazwa");

                return View(tzrp);
            }
            return NotFound();
        }

        //Post Method
        [HttpPost]
        public async Task<IActionResult> CreateTankowanie(TankowanieZRodzajemPaliwa tzrp)
        {
            var obj = _db.RodzajPaliw.ToList();
            if (tzrp.IdRodzajuPaliwa == 1 || tzrp.IdRodzajuPaliwa == 2)
            {
                obj = obj.Where(x => x.IdRodzajuPaliwa == tzrp.IdRodzajuPaliwa).ToList();
            }
            else
            {
                obj = obj.Where(x => x.IdRodzajuPaliwa == tzrp.IdRodzajuPaliwa || x.IdRodzajuPaliwa == 1).ToList();
            }

            int c = _db.Tankowania.Where(x => x.IdSamochodu == tzrp.IdSamochodu && x.Data == DateTime.Today).Count() + 1;
            if (tzrp.Cena<=0)
            {
                ViewBag.mgs = "Pole Cena musi być większa od 0!";
                tzrp.RodzajPaliwaSelect = new SelectList(obj, "IdRodzajuPaliwa", "Nazwa");
                ViewBag.count = c;
                return View(tzrp);
            }
            if (tzrp.Ilosc <= 0)
            {
                ViewBag.mgs = "Pole Ilość musi być większa od 0!";
                tzrp.RodzajPaliwaSelect = new SelectList(obj, "IdRodzajuPaliwa", "Nazwa");
                ViewBag.count = c;
                return View(tzrp);
            }

            Tankowanie tankowanie = new Tankowanie(tzrp.IdSamochodu, tzrp.IdRodzajuPaliwa, tzrp.Cena, tzrp.Ilosc);
            tankowanie.Data = DateTime.Now.Date;

            decimal ilosc = Convert.ToDecimal(tzrp.Ilosc);
            tankowanie.Wartosc = ilosc * tzrp.Cena;


            _db.Tankowania.Add(tankowanie);
            await _db.SaveChangesAsync();
            TempData["success"] = "Raport tankowania został dodany!";
            return RedirectToAction("CreateTankowanie");
        }
    }
}
