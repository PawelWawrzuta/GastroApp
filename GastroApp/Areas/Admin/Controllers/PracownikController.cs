using GastroApp.Data;
using GastroApp.Models.Pracownicy;
using GastroApp.Models.Pracownicy.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace GastroApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class PracownikController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public PracownikController(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var pracownicy = from ur in _db.UserRoles
                             join r in _db.Roles on ur.RoleId equals r.Id
                             join p in _db.Pracownicy on ur.UserId equals p.Id
                             where p.LockoutEnd == null || p.LockoutEnd <= DateTime.Now
                             select new PracownikIndex()
                             {
                                 UserName = p.UserName,
                                 Roles = r.Name,

                                 Id = p.Id,
                                 Imie = p.Imie,
                                 Nazwisko = p.Nazwisko,
                                 Email = p.Email,
                                 PhoneNumber = p.PhoneNumber

                             };

            StanowiskaIndex model = new StanowiskaIndex
            {
                Pracownicy = pracownicy.ToList()
            };

            return View(model);
        }

        //Get Method 
        public IActionResult Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pracownik = _db.Pracownicy.FirstOrDefault(x => x.Id == id);
            if (pracownik == null)
            {
                return NotFound();
            }

            return View(pracownik);
        }

        //Post Method 
        [HttpPost]
        public async Task<IActionResult> Delete(Pracownik pracownik)
        {
            var pracownikInfo = _db.Pracownicy.FirstOrDefault(c => c.Id == pracownik.Id);
            if (pracownikInfo == null)
            {
                return NotFound();

            }

            var deleteRole = await _userManager.RemoveFromRoleAsync(pracownikInfo, "Nowy");
            if (deleteRole.Succeeded)
            {
                _db.Pracownicy.Remove(pracownikInfo);
                _db.SaveChanges();
                TempData["success"] = "Pracownik został usunięty!";
                return RedirectToAction(nameof(Index));

            }
            return View(pracownikInfo);
        }

        public IActionResult Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pracownicy = from ur in _db.UserRoles
                             join r in _db.Roles on ur.RoleId equals r.Id
                             join p in _db.Pracownicy on ur.UserId equals p.Id
                             select new PracownikIndex()
                             {
                                 UserName = p.UserName,
                                 Roles = r.Name,

                                 Id = p.Id,
                                 Imie = p.Imie,
                                 Nazwisko = p.Nazwisko,
                                 Email = p.Email,
                                 PhoneNumber = p.PhoneNumber

                             };
            var obj = pracownicy.ToList();
            var model = obj.FirstOrDefault(x => x.Id == id);


            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        //Get Method 
        public IActionResult Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var pracownik = _db.Pracownicy.FirstOrDefault(x => x.Id == id);
            PracownikRegister model = new PracownikRegister(pracownik.Imie, pracownik.Nazwisko, pracownik.Email, pracownik.PhoneNumber);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        //Post Method 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PracownikRegister pracownik)
        {
            var pracownikInfo = _db.Pracownicy.FirstOrDefault(c => c.Id == pracownik.Id);
            if (pracownikInfo == null)
            {
                return NotFound();
            }
            pracownikInfo.Imie = pracownik.Imie;
            pracownikInfo.Nazwisko = pracownik.Nazwisko;
            pracownikInfo.Email = pracownik.Email;
            pracownikInfo.PhoneNumber = pracownik.Telefon;

            var result = await _userManager.UpdateAsync(pracownikInfo);
            if (result.Succeeded)
            {
                TempData["success"] = "Pracownik został zedytowany!";
                return RedirectToAction(nameof(Index));
            }
            return View(pracownikInfo);
        }

        //Get Method 
        public IActionResult SetRole(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pracownik = _db.Pracownicy.FirstOrDefault(x => x.Id == id);
            if (pracownik == null)
            {
                return NotFound();
            }

            PracownikRole pracownikRole = new PracownikRole(pracownik);

            var obj = _roleManager.Roles.Where(x => x.Name != "Nowy").ToList();

            pracownikRole.RoleSelect = new SelectList(obj, "Name", "Name");

            return View(pracownikRole);
        }
        //Post Method
        [HttpPost]
        public async Task<IActionResult> SetRole(PracownikRole pracownikRole)
        {
            Pracownik pracownik = _db.Pracownicy.FirstOrDefault(x => x.Id == pracownikRole.Id);

            var role = await _userManager.AddToRoleAsync(pracownik, pracownikRole.IdRole);
            var deleteRole = await _userManager.RemoveFromRoleAsync(pracownik, "Nowy");

            if (role.Succeeded && deleteRole.Succeeded)
            {
                pracownik.EmailConfirmed = true;
                await _userManager.UpdateAsync(pracownik);
                TempData["success"] = "Stanowisko zostało przypisane do pracownika!";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }


        //Get Method
        public IActionResult Lockout(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pracownicy = from ur in _db.UserRoles
                             join r in _db.Roles on ur.RoleId equals r.Id
                             join p in _db.Pracownicy on ur.UserId equals p.Id
                             select new PracownikIndex()
                             {
                                 UserName = p.UserName,
                                 Roles = r.Name,

                                 Id = p.Id,
                                 Imie = p.Imie,
                                 Nazwisko = p.Nazwisko,
                                 Email = p.Email,
                                 PhoneNumber = p.PhoneNumber

                             };
            var obj = pracownicy.ToList();
            var model = obj.FirstOrDefault(x => x.Id == id);
            var idAdmina = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (idAdmina == id)
            {
                TempData["fail"] = "Administrator nie może zwolnić siebie!";
                return RedirectToAction(nameof(Index));
            }

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        //Post Method
        [HttpPost]
        public async Task<IActionResult> Lockout(PracownikIndex pracownik)
        {
            var pracownikInfo = _db.Pracownicy.FirstOrDefault(c => c.Id == pracownik.Id);
            if (pracownikInfo == null)
            {
                return NotFound();

            }
            pracownikInfo.LockoutEnd = DateTime.Now.AddYears(1000);
            int rowAffected = _db.SaveChanges();
            if (rowAffected > 0)
            {
                TempData["success"] = "Pracownik został zwolniony!";
                return RedirectToAction(nameof(Index));
            }
            return View(pracownikInfo);
        }

    }
}
