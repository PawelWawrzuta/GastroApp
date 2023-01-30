using GastroApp.Data;
using GastroApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GastroApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class ProduktController : Controller
    {
        private ApplicationDbContext _db;

        public ProduktController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.Produkty.Where(x=>x.Dostepnosc==true).ToList());
        }

        //Get Method 
        public IActionResult Create()
        {
            return View();
        }

        //Post Method 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Produkt produkt)
        {
            if (ModelState.IsValid)
            {
                var searchProduct = _db.Produkty.FirstOrDefault(c => c.Nazwa == produkt.Nazwa);
                if (searchProduct != null)
                {
                    ViewBag.mgs = "Taki produkt już istnieje!";
                    return View(produkt);
                }

                _db.Produkty.Add(produkt);
                await _db.SaveChangesAsync();
                TempData["success"] = "Produkt został dodany!";
                return RedirectToAction(nameof(Index));
            }
            return View(produkt);
        }

        //Get Method 
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produkt = _db.Produkty.FirstOrDefault(x => x.IdProduktu == id);
            if (produkt == null)
            {
                return NotFound();
            }

            return View(produkt);
        }

        //Post Method 
        [HttpPost]
        public async Task<IActionResult> Delete(Produkt produkt)
        {
            produkt.Dostepnosc = false;
            _db.Produkty.Update(produkt);
            await _db.SaveChangesAsync();
            TempData["success"] = "Produkt został usunięty!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produkt = _db.Produkty.FirstOrDefault(x => x.IdProduktu == id);
            if (produkt == null)
            {
                return NotFound();
            }

            return View(produkt);
        }

        //Get Method 
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produkt = _db.Produkty.FirstOrDefault(x => x.IdProduktu == id);
            if (produkt == null)
            {
                return NotFound();
            }

            return View(produkt);
        }

        //Post Method 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Produkt produkt)
        {
            if (ModelState.IsValid)
            {
                _db.Produkty.Update(produkt);
                await _db.SaveChangesAsync();
                TempData["success"] = "Produkt został zedytowany!";
                return RedirectToAction(nameof(Index));
            }
            return View(produkt);
        }
    }
}
