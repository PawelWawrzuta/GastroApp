using GastroApp.Data;
using GastroApp.Models;
using GastroApp.Models.Zamowienia;
using GastroApp.Models.Zamowienia.HelpfulModel;
using GastroApp.Models.Zamowienia.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GastroApp.Areas.Kierowca.Controllers
{
    [Area("Kierowca")]
    [Authorize(Roles = "Kierowca")]
    public class ZamowieniaController : Controller
    {
        private ApplicationDbContext _db;
        public ZamowieniaController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var idPracownika = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idPracownika == null)
            {
                return NotFound();
            }

            var zamowienia = from z in _db.Zamowienia
                             join p in _db.Pracownicy on z.IdPracownika equals p.Id
                             join k in _db.Klienci on z.IdKlienta equals k.IdKlienta
                             join s in _db.Status on z.IdStatusu equals s.IdStatusu
                             join sp in _db.SposobPlatnosci on z.IdSposobuPlatnosci equals sp.IdSposobuPlatnosci
                             where z.DataOne >= DateTime.Today && p.Id == idPracownika
                             select new ZamowienieZKlientem
                             {
                                 IdZamowienia = z.IdZamowienia,
                                 NrZamowienia = z.NrZamowienia,
                                 DataOne = z.DataOne,
                                 DataTwo = z.DataTwo,
                                 Adres = k.Adres,
                                 Telefon = k.Telefon,
                                 SposobPlatnosciNazwa = sp.Nazwa,
                                 IdStatusu = s.IdStatusu
                             };

            var zamowieniaPrzypisane = zamowienia.Where(x => x.IdStatusu == 2);
            var zamowieniaWDrodze = zamowienia.Where(x => x.IdStatusu == 3);
            var zamowieniaProblem = zamowienia.Where(x => x.IdStatusu == 5);

            ZamowieniaIndex model = new ZamowieniaIndex()
            {
                ZamowieniaPrzypisane = zamowieniaPrzypisane.ToList(),
                ZamowieniaWDrodze = zamowieniaWDrodze.ToList(),
                ZamowieniaProblem = zamowieniaProblem.ToList(),
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult ChangeStatus(ZamowieniaIndex zamowieniaIndex)
        {
            var idPracownika = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idPracownika == null)
            {
                return NotFound();
            }
            var zamowieniaZaznaczone = zamowieniaIndex.ZamowieniaPrzypisane.Where(x => x.Zaznacz == true).ToList();

            foreach (var item in zamowieniaZaznaczone)
            {
                var zamowienie = _db.Zamowienia.FirstOrDefault(x => x.IdZamowienia == item.IdZamowienia);

                zamowienie.IdStatusu = 3;
                zamowienie.IdPracownika = idPracownika;
                zamowienie.DataTwo = DateTime.Now;
                _db.Zamowienia.Update(zamowienie);
                _db.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var zamowienie = _db.Zamowienia.FirstOrDefault(x => x.IdZamowienia == id);
            var daneKlienta = _db.Klienci.FirstOrDefault(x => x.IdKlienta == zamowienie.IdKlienta);

            var listaZamowienia = _db.ListaZamowienia.Where(x => x.IdZamowienia == id);
            List<ListaZamowieniaZNazwa> lzn = new List<ListaZamowieniaZNazwa>();
            foreach (var item in listaZamowienia)
            {
                string nazwa = _db.Produkty.FirstOrDefault(x => x.IdProduktu == item.IdProduktu).Nazwa;
                lzn.Add(new ListaZamowieniaZNazwa(item, nazwa));
            }

            string nazwaSposobuPlatnosci = _db.SposobPlatnosci.FirstOrDefault(x => x.IdSposobuPlatnosci == zamowienie.IdSposobuPlatnosci).Nazwa;
            Podsumowanie podsumowanie = new Podsumowanie(lzn, daneKlienta, zamowienie, nazwaSposobuPlatnosci);

            podsumowanie.NazwaStatusu = _db.Status.FirstOrDefault(x => x.IdStatusu == zamowienie.IdStatusu).Nazwa;
            return View(podsumowanie);
        }

        public IActionResult Delivered()
        {
            var idPracownika = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idPracownika == null)
            {
                return NotFound();
            }

            var zamowienia = from z in _db.Zamowienia
                             join p in _db.Pracownicy on z.IdPracownika equals p.Id
                             join k in _db.Klienci on z.IdKlienta equals k.IdKlienta
                             join s in _db.Status on z.IdStatusu equals s.IdStatusu
                             join sp in _db.SposobPlatnosci on z.IdSposobuPlatnosci equals sp.IdSposobuPlatnosci
                             where z.DataOne >= DateTime.Today && p.Id == idPracownika
                             select new ZamowienieZKlientem
                             {
                                 IdZamowienia = z.IdZamowienia,
                                 NrZamowienia = z.NrZamowienia,
                                 DataOne = z.DataOne,
                                 DataTwo = z.DataTwo,
                                 Adres = k.Adres,
                                 SposobPlatnosciNazwa = sp.Nazwa,
                                 IdStatusu = s.IdStatusu
                             };

            var zamowieniaDostarczone = zamowienia.Where(x => x.IdStatusu == 4).ToList();

            foreach (var item in zamowieniaDostarczone)
            {
                item.WartoscCalkowita = _db.ListaZamowienia.Where(x => x.IdZamowienia == item.IdZamowienia).Sum(a => a.Wartosc);
            }

            int iloscZamowien = zamowieniaDostarczone.Count();

            var listyzamowien = from lz in _db.ListaZamowienia
                                join z in _db.Zamowienia on lz.IdZamowienia equals z.IdZamowienia
                                join s in _db.Status on z.IdStatusu equals s.IdStatusu
                                where z.DataOne >= DateTime.Today && z.IdPracownika == idPracownika && s.IdStatusu == 4
                                select new ListaZamowieniaZeSposobemPlatnosci
                                {
                                    Wartosc = lz.Wartosc,
                                    IdSposobuPlatnosci = z.IdSposobuPlatnosci
                                };
            List<PodsumowanieDniaKierowcy> pdn = new List<PodsumowanieDniaKierowcy>();

            decimal wartoscZamowien = listyzamowien.Sum(x => x.Wartosc);
            decimal wartoscGotowka = listyzamowien.Where(x => x.IdSposobuPlatnosci == 1).Sum(x => x.Wartosc);
            decimal wartoscKarta = listyzamowien.Where(x => x.IdSposobuPlatnosci == 2).Sum(x => x.Wartosc);
            decimal wartoscOnline = listyzamowien.Where(x => x.IdSposobuPlatnosci == 3).Sum(x => x.Wartosc);

            int idSamochodu = _db.Zuzycia.FirstOrDefault(x => x.IdPracownika == idPracownika && x.Data == DateTime.Now.Date).IdSamochodu;
            decimal wartoscTankowan = _db.Tankowania.Where(x => x.IdSamochodu == idSamochodu && x.Data == DateTime.Now.Date).Sum(x => x.Wartosc);


            pdn.Add(new PodsumowanieDniaKierowcy(iloscZamowien, wartoscZamowien, wartoscGotowka, wartoscKarta, wartoscOnline,wartoscTankowan));

            ZamowieniaIndex model = new ZamowieniaIndex()
            {
                ZamowieniaDostarczone = zamowieniaDostarczone,
                PodsumowanieDniaKierowcy = pdn
            };

            return View(model);
        }

        public IActionResult ConfirmOrder(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var zamowienie = _db.Zamowienia.FirstOrDefault(x => x.IdZamowienia == id);
            var daneKlienta = _db.Klienci.FirstOrDefault(x => x.IdKlienta == zamowienie.IdKlienta);

            var listaZamowienia = _db.ListaZamowienia.Where(x => x.IdZamowienia == id);
            List<ListaZamowieniaZNazwa> lzn = new List<ListaZamowieniaZNazwa>();
            foreach (var item in listaZamowienia)
            {
                string nazwa = _db.Produkty.FirstOrDefault(x => x.IdProduktu == item.IdProduktu).Nazwa;
                lzn.Add(new ListaZamowieniaZNazwa(item, nazwa));
            }

            string nazwaSposobuPlatnosci = _db.SposobPlatnosci.FirstOrDefault(x => x.IdSposobuPlatnosci == zamowienie.IdSposobuPlatnosci).Nazwa;
            Podsumowanie podsumowanie = new Podsumowanie(lzn, daneKlienta, zamowienie, nazwaSposobuPlatnosci);

            podsumowanie.NazwaStatusu = _db.Status.FirstOrDefault(x => x.IdStatusu == zamowienie.IdStatusu).Nazwa;
            var sposobPlatnosci = _db.SposobPlatnosci.ToList();
            podsumowanie.SposobPlatnosciSelect = new SelectList(sposobPlatnosci, "IdSposobuPlatnosci", "Nazwa");
            return View(podsumowanie);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(Podsumowanie podsumowanie)
        {

            if (podsumowanie == null)
            {
                return RedirectToAction(nameof(Index));
            }
            int idZamowienia = podsumowanie.IdZamowienia;


            var zamowienieStare = _db.Zamowienia.AsNoTracking().FirstOrDefault(x => x.IdZamowienia == podsumowanie.IdZamowienia);
            var zamowienie = new Zamowienie()
            {
                IdZamowienia = idZamowienia,
                IdKlienta = zamowienieStare.IdKlienta,
                IdPracownika = zamowienieStare.IdPracownika,
                DataOne = zamowienieStare.DataOne,
                DataTwo = DateTime.Now,
                NrZamowienia = zamowienieStare.NrZamowienia,
                IdSposobuPlatnosci = podsumowanie.IdSposobuPlatnosci,
                IdStatusu = 4
            };

            _db.Zamowienia.Update(zamowienie);
            _db.SaveChanges();

            TempData["success"] = "Zamówienie zostało dostarczone!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Deliver(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var zamowienie = _db.Zamowienia.FirstOrDefault(x => x.IdZamowienia == id);

            zamowienie.IdStatusu = 4;
            zamowienie.DataTwo = DateTime.Now;
            _db.Zamowienia.Update(zamowienie);
            _db.SaveChanges();

            TempData["success"] = "Zamówienie zostało dostarczone!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var zamowienie = _db.Zamowienia.FirstOrDefault(x => x.IdZamowienia == id);

            zamowienie.IdStatusu = 5;
            zamowienie.DataTwo = DateTime.Now;
            _db.Zamowienia.Update(zamowienie);
            _db.SaveChanges();

            TempData["success"] = "Problem z tym zamówieniem został zgłoszony!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Map()
        {
            return View();
        }

        public JsonResult GetCoordinates(int idStatusu)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var zamowienia = _db.Zamowienia.Where(x => x.IdStatusu == idStatusu && x.IdPracownika==id).ToList();

            List<Coordinates> coordinates = new List<Coordinates>();
            foreach (var item in zamowienia)
            {
                var klient = _db.Klienci.FirstOrDefault(x => x.IdKlienta == item.IdKlienta);
                var dataOne = item.DataOne.ToString("HH:mm");
                
                Coordinates c = new Coordinates(klient.SzerokoscGeograficzna, klient.DlugoscGeograficzna, klient.Adres, dataOne);

                if (item.NrZamowienia!=null)
                {
                    c.NrZamowienia = Convert.ToInt32(item.NrZamowienia);
                }

                coordinates.Add(c);
            }
            return new JsonResult(Ok(coordinates));
        }
    }
}
