using System.ComponentModel;

namespace GastroApp.Models.Zamowienia.HelpfulModel
{
    public class PodsumowanieDniaKierowcy
    {
        public PodsumowanieDniaKierowcy()
        {

        }
        public PodsumowanieDniaKierowcy(int iloscZamowien, decimal wartoscZamowien, decimal wartoscGotowka, decimal wartoscKarta, decimal wartoscOnline, decimal wartoscTankowan)
        {
            IloscZamowien = iloscZamowien;
            WartoscZamowien = wartoscZamowien;
            WartoscGotowka = wartoscGotowka;
            WartoscKarta = wartoscKarta;
            WartoscOnline = wartoscOnline;
            WartoscTankowan = wartoscTankowan;
        }
        public PodsumowanieDniaKierowcy(string idPracownika, string userName, int iloscZamowien, decimal wartoscZamowien, decimal wartoscGotowka, decimal wartoscKarta, decimal wartoscOnline,decimal wartoscTankowan)
        {
            IdPracownika = idPracownika;
            UserName = userName;
            IloscZamowien = iloscZamowien;
            WartoscZamowien = wartoscZamowien;
            WartoscGotowka = wartoscGotowka;
            WartoscKarta = wartoscKarta;
            WartoscOnline = wartoscOnline;
            WartoscTankowan = wartoscTankowan;
        }

        public string IdPracownika { get; set; }
        public string UserName { get; set; }

        [DisplayName("Ilość zamówień")]
        public int IloscZamowien { get; set; }
        [DisplayName("Wartość zamówień")]
        public decimal WartoscZamowien { get; set; }
        [DisplayName("Wartość zamówień w gotówce")]
        public decimal WartoscGotowka { get; set; }
        [DisplayName("Wartość zamówień w karcie")]
        public decimal WartoscKarta { get; set; }
        [DisplayName("Wartość zamówień online")]
        public decimal WartoscOnline { get; set; }

        [DisplayName("Wartość tankowań")]
        public decimal WartoscTankowan { get; set; }

    }
}
