using GastroApp.Data;
using GastroApp.Models.Pracownicy;
using GastroApp.Models.Pracownicy.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GastroApp.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class PracownikController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<PracownikLogin> _logger;
        public PracownikController(ApplicationDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            ILogger<PracownikLogin> logger)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }


        //Get Method
        public IActionResult Register()
        {
            return View();
        }

        //Post Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(PracownikRegister pracownikReg)
        {
            if (ModelState.IsValid)
            {
                if (pracownikReg.Haslo != pracownikReg.PotwierdzHaslo)
                {
                    ViewBag.mgs = "Hasła nie są takie same!";
                    return View(pracownikReg);
                }

                Pracownik pracownik = new Pracownik(pracownikReg);


                string firstLetter = pracownik.Imie.ToLower().Substring(0, 1);
                pracownik.UserName = firstLetter + pracownik.Nazwisko.ToLower();

                var obj = _db.Pracownicy.FirstOrDefault(x => x.UserName == pracownik.UserName);

                if (obj != null)
                {
                    int x = 0;
                    string y;
                    string newUserName = "";
                    while (obj != null)
                    {
                        x += 1;
                        y = x.ToString();
                        newUserName = pracownik.UserName + x;
                        obj = _db.Pracownicy.FirstOrDefault(x => x.UserName == newUserName);
                    }
                    pracownik.UserName = newUserName;
                }

                var result = await _userManager.CreateAsync(pracownik, pracownik.PasswordHash);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(pracownik, "Nowy");
                    return RedirectToAction("WinRegister", "Pracownik", pracownik);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }

        public IActionResult WinRegister(Pracownik pracownik)
        {
            if (pracownik == null)
            {
                return NotFound();
            }

            return View(pracownik);
        }

        //Get Method
        public IActionResult Login()
        {
            return View();
        }

        //Post Method
        [HttpPost]
        public async Task<IActionResult> Login(PracownikLogin pl)
        {
            var result = await _signInManager.PasswordSignInAsync(pl.UserName, pl.PasswordHash, pl.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return RedirectToAction("WinLogin", "Pracownik", pl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                ViewBag.msg = "Użytkownik o podanej nazwie został zwolniony i nie może się zalogować!";
                return View(pl);
            }
            else
            {
                ViewBag.msg = "Nieprawidłowa nazwa użytkownika lub hasło!";
                return View(pl);
            }
        }

        public IActionResult WinLogin(PracownikLogin pl)
        {
            if (pl == null)
            {
                return NotFound();
            }

            return View(pl);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pracownicy = from ur in _db.UserRoles
                             join r in _db.Roles on ur.RoleId equals r.Id
                             join au in _db.Pracownicy on ur.UserId equals au.Id
                             select new PracownikIndex()
                             {
                                 UserName = au.UserName,
                                 Roles = r.Name,

                                 Id = au.Id,
                                 Imie = au.Imie,
                                 Nazwisko = au.Nazwisko,
                                 Email = au.Email,
                                 PhoneNumber = au.PhoneNumber

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
        public IActionResult ChangePassword(string? id)
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
        public async Task<IActionResult> ChangePassword(PracownikRegister pracownikReg)
        {
            if (pracownikReg.Haslo != pracownikReg.PotwierdzHaslo)
            {
                ViewBag.mgs = "Hasła nie są takie same!";
                return View();
            }

            var pracownik = _db.Pracownicy.FirstOrDefault(x => x.Id == pracownikReg.Id);
            if (pracownik == null)
            {
                return View(NotFound());
            }
            
            var result = await _userManager.ChangePasswordAsync(pracownik, pracownikReg.PasswordHash, pracownikReg.Haslo);

            if (result.Succeeded)
            {
                TempData["success"] = "Hasło zostało zmienione!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.mgs = "Stare hasło jest nieprawidłowe!";
                return View();
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }

    }


}
