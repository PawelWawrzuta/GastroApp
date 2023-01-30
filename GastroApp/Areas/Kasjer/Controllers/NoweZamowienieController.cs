using GastroApp.Data;
using GastroApp.Models;
using GastroApp.Models.Zamowienia;
using GastroApp.Models.Zamowienia.ViewModel;
using GastroApp.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace GastroApp.Areas.Kasjer.Controllers
{
    [Area("Kasjer")]
    [Authorize(Roles = "Kasjer")]
    public class NoweZamowienieController : Controller
    {
        private ApplicationDbContext _db;
        public NoweZamowienieController(ApplicationDbContext db)
        {
            _db = db;
        }
        //Get Method
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

            var daneKlienta = HttpContext.Session.Get<Klient>("daneKlienta");
            if (daneKlienta == null)
            {
                return RedirectToAction(nameof(DaneKlienta));
            }
            else
            {
                return RedirectToAction(nameof(Podsumowanie));
            }
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
            return RedirectToAction(nameof(Podsumowanie));
        }

        //Get Method
        public IActionResult DaneKlienta()
        {
            return View();
        }
        //Post Method
        [HttpPost]
        public async Task<IActionResult> DaneKlienta(Klient daneKlienta)
        {
            if (ModelState.IsValid)
            {
                if (daneKlienta.Miasto == null)
                {
                    daneKlienta.Miasto = "Bielsko-Biała";
                }
                HttpContext.Session.Set("daneKlienta", daneKlienta);
                return RedirectToAction(nameof(Podsumowanie));
            }
            return RedirectToAction(nameof(DaneKlienta));
        }

        //Get Method
        public IActionResult Podsumowanie()
        {
            var listaZamowienia = HttpContext.Session.Get<List<ListaZamowienia>>("listaZamowienia");
            if (listaZamowienia == null)
            {
                listaZamowienia = new List<ListaZamowienia>();
            }

            List<ListaZamowieniaZNazwa> lzn = new List<ListaZamowieniaZNazwa>();
            foreach (var item in listaZamowienia)
            {
                string nazwa = _db.Produkty.FirstOrDefault(x => x.IdProduktu == item.IdProduktu).Nazwa;
                lzn.Add(new ListaZamowieniaZNazwa(item, nazwa));
            }

            var daneKlienta = HttpContext.Session.Get<Klient>("daneKlienta");
            if (daneKlienta == null)
            {
                daneKlienta = new Klient();
            }


            var podsumowanie = new Podsumowanie(lzn, daneKlienta);
            podsumowanie.NrZamowienia = _db.Zamowienia.Where(x => x.DataOne >= DateTime.Today).Count() + 1;
            var sposobPlatnosci = _db.SposobPlatnosci.ToList();
            podsumowanie.SposobPlatnosciSelect = new SelectList(sposobPlatnosci, "IdSposobuPlatnosci", "Nazwa");

            return View(podsumowanie);
        }

        //Post Method
        [HttpPost]
        public async Task<IActionResult> Podsumowanie(Podsumowanie podsumowanie)
        {
            var listaZamowienia = HttpContext.Session.Get<List<ListaZamowienia>>("listaZamowienia");
            var daneKlienta = HttpContext.Session.Get<Klient>("daneKlienta");

            if (listaZamowienia == null || daneKlienta == null)
            {
                return RedirectToAction(nameof(Podsumowanie));
            }

            var staryKlient = _db.Klienci.FirstOrDefault(x => x.Adres == daneKlienta.Adres && x.Telefon == daneKlienta.Telefon);
            if (staryKlient == null)
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

                _db.Klienci.Add(daneKlienta);
                await _db.SaveChangesAsync();
            }
            Klient klient = _db.Klienci.FirstOrDefault(x => x.Adres == daneKlienta.Adres && x.Telefon == daneKlienta.Telefon);

            //var idPracownika = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (idPracownika == null)
            //{
            //    TempData["fail"] = "Zamówienie nie zostało dodane!";
            //    return RedirectToAction("Index", "Zamowienia");
            //}


            Zamowienie zamowienie = new Zamowienie(klient.IdKlienta, podsumowanie, 1);
            _db.Zamowienia.Add(zamowienie);
            await _db.SaveChangesAsync();

            DateTime now = DateTime.Now.AddMinutes(-1);
            int idZamowienia = _db.Zamowienia.FirstOrDefault(x => x.DataOne >= now && x.IdKlienta == klient.IdKlienta).IdZamowienia;

            if (idZamowienia == null)
            {
                TempData["fail"] = "Zamówienie nie zostało dodane!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var item in listaZamowienia)
            {
                item.IdZamowienia = idZamowienia;
                _db.ListaZamowienia.Add(item);
                await _db.SaveChangesAsync();
            }

            listaZamowienia = null;
            HttpContext.Session.Set("listaZamowienia", listaZamowienia);
            daneKlienta = null;
            HttpContext.Session.Set("daneKlienta", daneKlienta);

            TempData["success"] = "Zamówienie zostało dodane!";
            return RedirectToAction("Index", "Zamowienia", new { area = "Kasjer" });
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

    }
}
