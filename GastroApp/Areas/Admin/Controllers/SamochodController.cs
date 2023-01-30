using GastroApp.Data;
using GastroApp.Models.Samochody;
using GastroApp.Models.Samochody.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace GastroApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class SamochodController : Controller
    {
        private ApplicationDbContext _db;

        public SamochodController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.Samochody.Include(c => c.RodzajPaliwa).ToList());
        }

        //Get Method 
        public IActionResult Create()
        {
            var obj = _db.RodzajPaliw.ToList();
            SamochodZRodzajemPaliwa srp = new SamochodZRodzajemPaliwa();
            srp.RodzajPaliwaSelect = new SelectList(obj, "IdRodzajuPaliwa", "Nazwa");
            return View(srp);
        }

        //Post Method 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SamochodZRodzajemPaliwa srp)
        {
            if (ModelState.IsValid)
            {
                srp.NrRejestracyjny = srp.NrRejestracyjny.ToUpper();
                srp.NrRejestracyjny = String.Concat(srp.NrRejestracyjny.Where(c => !Char.IsWhiteSpace(c)));

                if (_db.Samochody.FirstOrDefault(x=>x.NrRejestracyjny==srp.NrRejestracyjny)!=null)
                {
                    ViewBag.msg = "Istnieje już samochód z takim samym numerem rejestracyjnym!";
                    var obj = _db.RodzajPaliw.ToList();
                    srp.RodzajPaliwaSelect = new SelectList(obj, "IdRodzajuPaliwa", "Nazwa");
                    return View(srp);
                }

                Samochod samochod = new Samochod(srp);

                _db.Samochody.Add(samochod);
                await _db.SaveChangesAsync();
                TempData["success"] = "Samochód został dodany!";
                return RedirectToAction(nameof(Index));
            }
            var obj2 = _db.RodzajPaliw.ToList();
            srp.RodzajPaliwaSelect = new SelectList(obj2, "IdRodzajuPaliwa", "Nazwa");
            return View(srp);
        }

        //Get Method 
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var samochod = _db.Samochody.Include(c => c.RodzajPaliwa).FirstOrDefault(x => x.IdSamochodu == id);
            if (samochod == null)
            {
                return NotFound();
            }

            SamochodZRodzajemPaliwa srp = new SamochodZRodzajemPaliwa(samochod, _db.RodzajPaliw.ToList());
            return View(srp);
        }

        //Post Method 
        [HttpPost]
        public async Task<IActionResult> Delete(SamochodZRodzajemPaliwa srp)
        {
            Samochod samochod = new Samochod(srp);

            var zuzycia = _db.Zuzycia.Where(x => x.IdSamochodu == samochod.IdSamochodu).ToList();
            var tankowania = _db.Tankowania.Where(x => x.IdSamochodu == samochod.IdSamochodu).ToList();

            foreach (var item in zuzycia)
            {
                _db.Zuzycia.Remove(item);
            }
            foreach (var item in tankowania)
            {
                _db.Tankowania.Remove(item);
            }

            _db.Samochody.Remove(samochod);
            await _db.SaveChangesAsync();
            TempData["success"] = "Samochód został usunięty!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var samochod = _db.Samochody.Include(c => c.RodzajPaliwa).FirstOrDefault(x => x.IdSamochodu == id);
            if (samochod == null)
            {
                return NotFound();
            }

            SamochodZRodzajemPaliwa srp = new SamochodZRodzajemPaliwa(samochod, _db.RodzajPaliw.ToList());
            return View(srp);
        }

        //Get Method 
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var samochod = _db.Samochody.Include(c => c.RodzajPaliwa).FirstOrDefault(x => x.IdSamochodu == id);
            if (samochod == null)
            {
                return NotFound();
            }
            SamochodZRodzajemPaliwa srp = new SamochodZRodzajemPaliwa(samochod, _db.RodzajPaliw.ToList());

            var obj = _db.RodzajPaliw.ToList();
            srp.RodzajPaliwaSelect = new SelectList(obj, "IdRodzajuPaliwa", "Nazwa");

            return View(srp);
        }

        //Post Method 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SamochodZRodzajemPaliwa srp)
        {
            if (ModelState.IsValid)
            {
                srp.NrRejestracyjny = srp.NrRejestracyjny.ToUpper();
                srp.NrRejestracyjny = String.Concat(srp.NrRejestracyjny.Where(c => !Char.IsWhiteSpace(c)));

                if (_db.Samochody.FirstOrDefault(x => x.NrRejestracyjny == srp.NrRejestracyjny && x.IdSamochodu!=srp.IdSamochodu) != null)
                {
                    ViewBag.msg = "Istnieje już samochód z takim samym numerem rejestracyjnym!";
                    var obj = _db.RodzajPaliw.ToList();
                    srp.RodzajPaliwaSelect = new SelectList(obj, "IdRodzajuPaliwa", "Nazwa");
                    return View(srp);
                }
                Samochod samochod = new Samochod(srp);
                _db.Samochody.Update(samochod);
                await _db.SaveChangesAsync();
                TempData["success"] = "Samochód został zedytowany!";
                return RedirectToAction(nameof(Index));
            }

            var obj2 = _db.RodzajPaliw.ToList();
            srp.RodzajPaliwaSelect = new SelectList(obj2, "IdRodzajuPaliwa", "Nazwa");
            return View(srp);
        }

        public IActionResult IndexZuzycie(int? id)
        {
            var iz = from z in _db.Zuzycia
                     join p in _db.Pracownicy on z.IdPracownika equals p.Id
                     where z.IdSamochodu == id
                     select new IndexZuzycia()
                     {
                         IdPracownika = z.IdPracownika,
                         Imie = p.Imie,
                         Nazwisko = p.Nazwisko,
                         Przebieg = z.Przebieg,
                         Data = z.Data,
                         IdZuzycia = z.IdZuzycia,
                         IdSamochodu = z.IdSamochodu
                     };
            var obj = iz.ToList();


            return View(iz);
        }

        //Get Method 
        public IActionResult DeleteZuzycie(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zuzycie = _db.Zuzycia.FirstOrDefault(x => x.IdZuzycia == id);
            IndexZuzycia iz = new IndexZuzycia(zuzycie.IdZuzycia, zuzycie.Przebieg, zuzycie.Data, zuzycie.IdSamochodu,
                 _db.Pracownicy.FirstOrDefault(x => x.Id == zuzycie.IdPracownika).Imie,
                 _db.Pracownicy.FirstOrDefault(x => x.Id == zuzycie.IdPracownika).Nazwisko);

            if (iz == null)
            {
                return NotFound();
            }
            return View(iz);
        }

        //Post Method 
        [HttpPost]
        public async Task<IActionResult> DeleteZuzycie(IndexZuzycia iz)
        {
            var zuzycie = _db.Zuzycia.FirstOrDefault(x => x.IdZuzycia == iz.IdZuzycia);
            if (zuzycie == null)
            {
                return NotFound();
            }

            _db.Zuzycia.Remove(zuzycie);
            await _db.SaveChangesAsync();
            TempData["success"] = "Raport zużycia został usunięty!";

            return RedirectToAction("IndexZuzycie", "Samochod", new { id = iz.IdSamochodu });
        }

        //Get Method
        public IActionResult IndexTankowanie(int? id)
        {
            var it = from t in _db.Tankowania
                     join z in _db.Zuzycia on t.Data equals z.Data
                     join p in _db.Pracownicy on z.IdPracownika equals p.Id
                     join rp in _db.RodzajPaliw on t.IdRodzajuPaliwa equals rp.IdRodzajuPaliwa
                     where t.IdSamochodu == id
                     select new IndexTankowania()
                     {
                         IdTankowania = t.IdTankowania,
                         Imie = p.Imie,
                         Nazwisko = p.Nazwisko,
                         Data = t.Data,
                         Cena = t.Cena,
                         Ilosc = t.Ilosc,
                         Wartosc = t.Wartosc,
                         NazwaRodzajuPaliwa=rp.Nazwa
                     };
            var obj = it.ToList();
            return View(it);
        }

        //Get Method 
        public IActionResult DeleteTankowanie(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tankowanie = _db.Tankowania.FirstOrDefault(x => x.IdTankowania == id);
            IndexTankowania it = new IndexTankowania(tankowanie.IdTankowania,
                _db.Pracownicy.FirstOrDefault(x => x.Id == _db.Zuzycia.FirstOrDefault(x => x.Data == tankowanie.Data).IdPracownika).Imie,
                _db.Pracownicy.FirstOrDefault(x => x.Id == _db.Zuzycia.FirstOrDefault(x => x.Data == tankowanie.Data).IdPracownika).Nazwisko,
                tankowanie.Data, tankowanie.Cena, tankowanie.Ilosc, tankowanie.Wartosc, tankowanie.IdSamochodu,
                _db.RodzajPaliw.FirstOrDefault(x=>x.IdRodzajuPaliwa==tankowanie.IdRodzajuPaliwa).Nazwa);


            if (it == null)
            {
                return NotFound();
            }
            return View(it);
        }

        //Post Method 
        [HttpPost]
        public async Task<IActionResult> DeleteTankowanie(IndexTankowania it)
        {
            var tankowanie = _db.Tankowania.FirstOrDefault(x => x.IdTankowania == it.IdTankowania);
            if (tankowanie == null)
            {
                return NotFound();
            }

            _db.Tankowania.Remove(tankowanie);
            await _db.SaveChangesAsync();
            TempData["success"] = "Raport tankowania został usunięty!";

            return RedirectToAction("IndexTankowanie", "Samochod", new { id = it.IdSamochodu });
        }
    }
}
