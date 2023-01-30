using Microsoft.AspNetCore.Mvc.Rendering;

namespace GastroApp.Models.Pracownicy.ViewModel
{
    public class PracownikRole:Pracownik
    {
        public PracownikRole()
        {

        }
        public PracownikRole(Pracownik pracownik)
        {
            Imie = pracownik.Imie;
            Nazwisko = pracownik.Nazwisko;
            Email = pracownik.Email;
            PasswordHash = pracownik.PasswordHash;
            PhoneNumber = pracownik.PhoneNumber;
        }
        public SelectList? RoleSelect { get; set; }

        public string? IdRole { get; set; }

    }
}
