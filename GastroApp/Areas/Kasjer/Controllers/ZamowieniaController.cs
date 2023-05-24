using GastroApp.Data;
using GastroApp.Models.Zamowienia.ViewModel;
using GastroApp.Models.Zamowienia;
using GastroApp.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GastroApp.Models.Pracownicy;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using GastroApp.Models.Zamowienia.HelpfulModel;
using GastroApp.Models;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;

namespace GastroApp.Areas.Kasjer.Controllers
{
    [Area("Kasjer")]
    [Authorize(Roles = "Kasjer")]
    public class ZamowieniaController : Controller
    {
        private ApplicationDbContext _db;
        public ZamowieniaController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var zamowienia = from z in _db.Zamowienia
                             join k in _db.Klienci on z.IdKlienta equals k.IdKlienta
                             join s in _db.Status on z.IdStatusu equals s.IdStatusu
                             where z.DataOne >= DateTime.Today
                             select new ZamowienieZKlientem
                             {
                                 IdZamowienia = z.IdZamowienia,
                                 NrZamowienia = z.NrZamowienia,
                                 DataOne = z.DataOne,
                                 DataTwo = z.DataTwo,
                                 Adres = k.Adres,
                                 Telefon = k.Telefon,
                                 IdStatusu = s.IdStatusu,
                                 StatusNazwa = s.Nazwa
                             };

            var zamowieniaZPracownikiem = from z in _db.Zamowienia
                                          join p in _db.Pracownicy on z.IdPracownika equals p.Id
                                          join k in _db.Klienci on z.IdKlienta equals k.IdKlienta
                                          join s in _db.Status on z.IdStatusu equals s.IdStatusu
                                          where z.DataOne >= DateTime.Today
                                          select new ZamowienieZKlientem
                                          {
                                              IdZamowienia = z.IdZamowienia,
                                              NrZamowienia = z.NrZamowienia,
                                              DataOne = z.DataOne,
                                              DataTwo = z.DataTwo,
                                              Adres = k.Adres,
                                              Telefon = k.Telefon,
                                              UserName = p.UserName,
                                              IdStatusu = s.IdStatusu,
                                              StatusNazwa = s.Nazwa
                                          };

            var pracownicy = from p in _db.Pracownicy
                             join ur in _db.UserRoles on p.Id equals ur.UserId
                             join r in _db.Roles on ur.RoleId equals r.Id
                             where r.Id == "4" && (p.LockoutEnd == null || p.LockoutEnd <= DateTime.Now)
                             select new Pracownik
                             {
                                 Id = p.Id,
                                 UserName = p.UserName
                             };

            var zamowieniaPrzyjete = zamowienia.Where(x => x.IdStatusu == 1);
            var zamowieniaPrzypisaneWDrodzeProblem = zamowieniaZPracownikiem.Where(x => x.IdStatusu == 2 || x.IdStatusu == 3 || x.IdStatusu == 5);


            ZamowieniaIndex model = new ZamowieniaIndex()
            {
                ZamowieniaPrzyjete = zamowieniaPrzyjete.ToList(),
                ZamowieniaPrzypisaneWDrodzeProblem = zamowieniaPrzypisaneWDrodzeProblem.ToList(),
                PracownikSelect = new SelectList(pracownicy.ToList(), "Id", "UserName")
            };

            ListaZamowienia listaZamowienia = null;
            HttpContext.Session.Set("listaZamowienia", listaZamowienia);
            Klient daneKlienta = null;
            HttpContext.Session.Set("daneKlienta", daneKlienta);
            return View(model);
        }

        [HttpPost]
        public IActionResult SignToDriver(ZamowieniaIndex zamowieniaIndex)
        {
            var zamowieniaZaznaczone = zamowieniaIndex.ZamowieniaPrzyjete.Where(x => x.Zaznacz == true).ToList();

            foreach (var item in zamowieniaZaznaczone)
            {
                var zamowienie = _db.Zamowienia.FirstOrDefault(x => x.IdZamowienia == item.IdZamowienia);

                zamowienie.IdStatusu = 2;
                zamowienie.IdPracownika = zamowieniaIndex.IdPracownika;
                _db.Zamowienia.Update(zamowienie);
                _db.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult BackSignToDriver(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var zamowienie = _db.Zamowienia.FirstOrDefault(x => x.IdZamowienia == id);

            zamowienie.IdStatusu = 1;
            zamowienie.IdPracownika = null;
            zamowienie.DataTwo = null;
            _db.Zamowienia.Update(zamowienie);
            _db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
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

            return View(podsumowanie);
        }

        [HttpPost]
        public IActionResult Delete(Podsumowanie podsumowanie)
        {
            if (podsumowanie == null)
            {
                return NotFound();
            }
            var zamowienie = _db.Zamowienia.FirstOrDefault(x => x.IdZamowienia == podsumowanie.IdZamowienia);
            var listaZamowienia = _db.ListaZamowienia.Where(x => x.IdZamowienia == podsumowanie.IdZamowienia).ToList();

            foreach (var item in listaZamowienia)
            {
                _db.ListaZamowienia.Remove(item);
            }

            _db.Zamowienia.Remove(zamowienie);
            _db.SaveChanges();

            TempData["success"] = "Zamówienie zostało usunięte!";
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

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var zamowienie = _db.Zamowienia.FirstOrDefault(x => x.IdZamowienia == id);

            var daneKlienta = HttpContext.Session.Get<Klient>("daneKlienta");
            if (daneKlienta == null || daneKlienta.IdKlienta != zamowienie.IdKlienta)
            {
                daneKlienta = _db.Klienci.FirstOrDefault(x => x.IdKlienta == zamowienie.IdKlienta);
            }

            var listaZamowienia = HttpContext.Session.Get<List<ListaZamowienia>>("listaZamowienia");
            if (listaZamowienia == null)
            {
                listaZamowienia = _db.ListaZamowienia.Where(x => x.IdZamowienia == id).ToList();
            }

            List<ListaZamowieniaZNazwa> lzn = new List<ListaZamowieniaZNazwa>();
            foreach (var item in listaZamowienia)
            {
                string nazwa = _db.Produkty.FirstOrDefault(x => x.IdProduktu == item.IdProduktu).Nazwa;
                lzn.Add(new ListaZamowieniaZNazwa(item, nazwa));
            }

            string nazwaSposobuPlatnosci = _db.SposobPlatnosci.FirstOrDefault(x => x.IdSposobuPlatnosci == zamowienie.IdSposobuPlatnosci).Nazwa;
            Podsumowanie podsumowanie = new Podsumowanie(lzn, daneKlienta, zamowienie, nazwaSposobuPlatnosci);

            var sposobPlatnosci = _db.SposobPlatnosci.ToList();
            podsumowanie.SposobPlatnosciSelect = new SelectList(sposobPlatnosci, "IdSposobuPlatnosci", "Nazwa");

            HttpContext.Session.Set("listaZamowienia", listaZamowienia);
            HttpContext.Session.Set("daneKlienta", daneKlienta);
            HttpContext.Session.Set("idZamowienia", zamowienie.IdZamowienia);

            return View(podsumowanie);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Podsumowanie podsumowanie)
        {

            if (podsumowanie == null)
            {
                return RedirectToAction(nameof(Index));
            }
            int idZamowienia = podsumowanie.IdZamowienia;
            var listaZamowienia = HttpContext.Session.Get<List<ListaZamowienia>>("listaZamowienia");
            var listaZamowieniaDb = _db.ListaZamowienia.Where(x => x.IdZamowienia == idZamowienia).ToList();


            foreach (var item in listaZamowienia)
            {
                item.IdZamowienia = idZamowienia;
                var isInDb = _db.ListaZamowienia.AsNoTracking().FirstOrDefault(x => x.IdZamowienia == item.IdZamowienia &&
                    x.IdProduktu == item.IdProduktu &&
                    x.Ilosc == item.Ilosc);

                if (isInDb == null)
                {
                    _db.ListaZamowienia.Add(item);
                    _db.SaveChanges();
                }
            }
            foreach (var item in listaZamowieniaDb)
            {
                var isInDb = listaZamowienia.FirstOrDefault(x => x.IdZamowienia == item.IdZamowienia &&
                    x.IdProduktu == item.IdProduktu &&
                    x.Ilosc == item.Ilosc);

                if (isInDb == null)
                {
                    _db.ListaZamowienia.Remove(item);
                    _db.SaveChanges();
                }
            }

            var daneKlienta = HttpContext.Session.Get<Klient>("daneKlienta");
            var daneKlientaStare = _db.Klienci.AsNoTracking().FirstOrDefault(x => x.IdKlienta == daneKlienta.IdKlienta);

            if (daneKlientaStare != daneKlienta)
            {
                string ulica = daneKlienta.Adres;
                int index = ulica.IndexOf('/');
                string sub;
                if (index >= 0)
                {
                    sub = ulica.Substring(0, index);
                }
                else
                {
                    sub = ulica;
                }

                string address = sub + ", " + daneKlienta.Miasto;
                daneKlienta.SzerokoscGeograficzna = FindCoordinates(address).latitude;
                daneKlienta.DlugoscGeograficzna = FindCoordinates(address).longitude;

                _db.Klienci.Update(daneKlienta);
                _db.SaveChanges();
            }


            var zamowienieStare = _db.Zamowienia.AsNoTracking().FirstOrDefault(x => x.IdZamowienia == podsumowanie.IdZamowienia);
            var zamowienie = new Zamowienie()
            {
                IdZamowienia = idZamowienia,
                IdKlienta = daneKlienta.IdKlienta,
                IdPracownika = zamowienieStare.IdPracownika,
                DataOne = zamowienieStare.DataOne,
                DataTwo = zamowienieStare.DataTwo,
                NrZamowienia = podsumowanie.NrZamowienia,
                IdSposobuPlatnosci = podsumowanie.IdSposobuPlatnosci,
                IdStatusu = zamowienieStare.IdStatusu
            };
            if (zamowienie.IdStatusu == 5)
            {
                zamowienie.IdStatusu = 3;
                zamowienie.DataTwo = DateTime.Now;
            }
            if (zamowienie != zamowienieStare)
            {
                _db.Update(zamowienie);
                _db.SaveChanges();
            }

            listaZamowienia = null;
            HttpContext.Session.Set("listaZamowienia", listaZamowienia);
            daneKlienta = null;
            HttpContext.Session.Set("daneKlienta", daneKlienta);
            idZamowienia = 0;
            HttpContext.Session.Set("idZamowienia", idZamowienia);

            TempData["success"] = "Zamówienie zostało zedytowane!";
            return RedirectToAction(nameof(Index));

        }

        public IActionResult ProduktyIndex()
        {
            var obj = _db.Produkty.Where(x => x.Dostepnosc == true).ToList();
            List<ProduktyZIloscia> model = new List<ProduktyZIloscia>();

            foreach (var item in obj)
            {
                model.Add(new ProduktyZIloscia(item));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCard(List<ProduktyZIloscia> pzi)
        {
            foreach (var item in pzi.ToList())
            {
                if (item.IloscString == null)
                {
                    item.IloscString = "0";
                }
                item.IloscString = item.IloscString.Replace('.', ',');
                item.Ilosc = Convert.ToSingle(item.IloscString);
            }

            var listaZamowienia = HttpContext.Session.Get<List<ListaZamowienia>>("listaZamowienia");
            if (listaZamowienia == null)
            {
                listaZamowienia = new List<ListaZamowienia>();
            }

            foreach (var item in pzi)
            {
                var obj = listaZamowienia.FirstOrDefault(x => x.IdProduktu == item.IdProduktu);
                if (obj == null)
                {
                    if (item.Ilosc > 0)
                    {
                        ListaZamowienia produkt = new ListaZamowienia(item.IdProduktu, item.Cena, item.Ilosc, item.Opis);
                        decimal ilosc = Convert.ToDecimal(item.Ilosc);
                        produkt.Wartosc = ilosc * item.Cena;

                        listaZamowienia.Add(produkt);
                    }
                }
                else
                {
                    obj.Ilosc = item.Ilosc;
                    decimal ilosc = Convert.ToDecimal(item.Ilosc);
                    obj.Wartosc = ilosc * item.Cena;
                    if (obj.Ilosc <= 0)
                    {
                        listaZamowienia.Remove(obj);
                    }
                }
            }

            if (listaZamowienia == null) return RedirectToAction(nameof(ProduktyIndex));
            HttpContext.Session.Set("listaZamowienia", listaZamowienia);

            int idZamowienia = HttpContext.Session.Get<int>("idZamowienia");
            return RedirectToAction("Edit", new { id = idZamowienia });
        }

        public IActionResult ProduktDetails(int? id)
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

        public async Task<IActionResult> RemoveToCard(int? id)
        {
            if (id == null) return NotFound();
            var listaZamowienia = HttpContext.Session.Get<List<ListaZamowienia>>("listaZamowienia");
            if (listaZamowienia != null)
            {
                var toRemove = listaZamowienia.FirstOrDefault(c => c.IdProduktu == id);

                if (toRemove != null)
                {
                    listaZamowienia.Remove(toRemove);
                    HttpContext.Session.Set("listaZamowienia", listaZamowienia);
                }

            }

            int idZamowienia = HttpContext.Session.Get<int>("idZamowienia");
            return RedirectToAction("Edit", new { id = idZamowienia });
        }

        public IActionResult DaneKlienta()
        {
            var daneKlienta = HttpContext.Session.Get<Klient>("daneKlienta");
            return View(daneKlienta);
        }
        [HttpPost]
        public async Task<IActionResult> DaneKlienta(Klient daneKlienta)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.Set("daneKlienta", daneKlienta);
                int idZamowienia = HttpContext.Session.Get<int>("idZamowienia");
                return RedirectToAction("Edit", new { id = idZamowienia });
            }
            return RedirectToAction(nameof(DaneKlienta));
        }

        public IActionResult Delivered()
        {
            var zamowienia = from z in _db.Zamowienia
                             join p in _db.Pracownicy on z.IdPracownika equals p.Id
                             join k in _db.Klienci on z.IdKlienta equals k.IdKlienta
                             join s in _db.Status on z.IdStatusu equals s.IdStatusu
                             join sp in _db.SposobPlatnosci on z.IdSposobuPlatnosci equals sp.IdSposobuPlatnosci
                             where z.DataOne >= DateTime.Today
                             select new ZamowienieZKlientem
                             {
                                 IdZamowienia = z.IdZamowienia,
                                 NrZamowienia = z.NrZamowienia,
                                 DataOne = z.DataOne,
                                 DataTwo = z.DataTwo,
                                 Adres = k.Adres,
                                 SposobPlatnosciNazwa = sp.Nazwa,
                                 IdStatusu = s.IdStatusu,
                                 UserName = p.UserName
                             };

            var zamowieniaDostarczone = zamowienia.Where(x => x.IdStatusu == 4).ToList();
            foreach (var item in zamowieniaDostarczone)
            {
                item.WartoscCalkowita = _db.ListaZamowienia.Where(x => x.IdZamowienia == item.IdZamowienia).Sum(a => a.Wartosc);
            }


            var listyzamowien = from lz in _db.ListaZamowienia
                                join z in _db.Zamowienia on lz.IdZamowienia equals z.IdZamowienia
                                join p in _db.Pracownicy on z.IdPracownika equals p.Id
                                join s in _db.Status on z.IdStatusu equals s.IdStatusu
                                where z.DataOne >= DateTime.Today && s.IdStatusu == 4
                                select new ListaZamowieniaZeSposobemPlatnosci
                                {
                                    IdPracownika = z.IdPracownika,
                                    UserName = p.UserName,
                                    Wartosc = lz.Wartosc,
                                    IdSposobuPlatnosci = z.IdSposobuPlatnosci
                                };
            List<PodsumowanieDniaKierowcy> pdn = new List<PodsumowanieDniaKierowcy>();
            var listaLista = listyzamowien.ToList();
            var kierowcy = listaLista.DistinctBy(x => x.IdPracownika).ToList();

            foreach (var item in kierowcy)
            {
                string idPracownika = item.IdPracownika;
                string userName = item.UserName;
                int iloscZamowien = listyzamowien.Where(x => x.IdPracownika == idPracownika).Count();
                decimal wartoscZamowien = listyzamowien.Where(x => x.IdPracownika == idPracownika).Sum(x => x.Wartosc);
                decimal wartoscGotowka = listyzamowien.Where(x => x.IdSposobuPlatnosci == 1 && x.IdPracownika == idPracownika).Sum(x => x.Wartosc);
                decimal wartoscKarta = listyzamowien.Where(x => x.IdSposobuPlatnosci == 2 && x.IdPracownika == idPracownika).Sum(x => x.Wartosc);
                decimal wartoscOnline = listyzamowien.Where(x => x.IdSposobuPlatnosci == 3 && x.IdPracownika == idPracownika).Sum(x => x.Wartosc);


                int idSamochodu = _db.Zuzycia.FirstOrDefault(x => x.IdPracownika == idPracownika && x.Data == DateTime.Now.Date).IdSamochodu;
                decimal wartoscTankowan = _db.Tankowania.Where(x => x.IdSamochodu == idSamochodu && x.Data == DateTime.Now.Date).Sum(x => x.Wartosc);

                pdn.Add(new PodsumowanieDniaKierowcy(idPracownika, userName, iloscZamowien, wartoscZamowien, wartoscGotowka, wartoscKarta, wartoscOnline, wartoscTankowan));

            }

            ZamowieniaIndex model = new ZamowieniaIndex()
            {
                ZamowieniaDostarczone = zamowieniaDostarczone,
                PodsumowanieDniaKierowcy = pdn
            };

            return View(model);
        }

        public IActionResult Map()
        {
            return View();
        }

        public JsonResult GetCoordinates()
        {
            var zamowienia = _db.Zamowienia.Where(x => x.IdStatusu == 1).ToList();

            List<Coordinates> coordinates = new List<Coordinates>();
            foreach (var item in zamowienia)
            {
                var klient = _db.Klienci.FirstOrDefault(x => x.IdKlienta == item.IdKlienta);
                var dataOne = item.DataOne.ToString("HH:mm");
                Coordinates c = new Coordinates(klient.SzerokoscGeograficzna, klient.DlugoscGeograficzna, klient.Adres, dataOne);

                if (item.NrZamowienia != null)
                {
                    c.NrZamowienia = Convert.ToInt32(item.NrZamowienia);
                }

                coordinates.Add(c);
            }
            return new JsonResult(Ok(coordinates));
        }

        public Coordinates FindCoordinates(string fullAddress)
        {
            var requestUri = string.Format("http://api.positionstack.com/v1/forward?access_key=1b2e130c0f829639331467b6b9335ddd&query={0}", Uri.EscapeDataString(fullAddress));

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(requestUri).Result;
                Coordinates coordinates = new Coordinates();
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;

                    string responseString = responseContent.ReadAsStringAsync().Result;

                    var lat = JObject.Parse(responseString).SelectToken("$.data[0].latitude").ToString();
                    var lon = JObject.Parse(responseString).SelectToken("$.data[0].longitude").ToString();
                    lat = lat.Replace(",", ".");
                    lon = lon.Replace(",", ".");

                    coordinates.latitude = lat;
                    coordinates.longitude = lon;
                    return coordinates;
                }
                return coordinates;
            }
        }

        public IActionResult MapDriver(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var pracownik = _db.Pracownicy.FirstOrDefault(x => x.UserName == id);
            return View(pracownik);
        }

        public JsonResult GetCoordinatesDriver(string idPracownika, int idStatusu)
        {
            
            var zamowienia = _db.Zamowienia.Where(x => x.IdStatusu == idStatusu && x.IdPracownika == idPracownika).ToList();

            List<Coordinates> coordinates = new List<Coordinates>();
            foreach (var item in zamowienia)
            {
                var klient = _db.Klienci.FirstOrDefault(x => x.IdKlienta == item.IdKlienta);
                var dataOne = item.DataOne.ToString("HH:mm");
                Coordinates c = new Coordinates(klient.SzerokoscGeograficzna, klient.DlugoscGeograficzna, klient.Adres, dataOne);

                if (item.NrZamowienia != null)
                {
                    c.NrZamowienia = Convert.ToInt32(item.NrZamowienia);
                }

                coordinates.Add(c);
            }
            return new JsonResult(Ok(coordinates));
        }
    }
}