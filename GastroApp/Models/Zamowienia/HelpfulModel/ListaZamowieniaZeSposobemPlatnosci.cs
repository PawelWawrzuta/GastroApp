namespace GastroApp.Models.Zamowienia.HelpfulModel
{
    public class ListaZamowieniaZeSposobemPlatnosci : ListaZamowienia
    {
        public string IdPracownika { get; set; }
        public string UserName { get; set; }
        public int IdSposobuPlatnosci { get; set; }
    }
}
