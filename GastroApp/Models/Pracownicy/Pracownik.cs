using GastroApp.Models.Pracownicy.ViewModel;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GastroApp.Models.Pracownicy
{
    public class Pracownik : IdentityUser
    {
        public Pracownik()
        {

        }
        public Pracownik(PracownikRegister pracownikRegister)
        {
            Imie = pracownikRegister.Imie;
            Nazwisko = pracownikRegister.Nazwisko;
            Email = pracownikRegister.Email;
            PasswordHash = pracownikRegister.Haslo;
            PhoneNumber = pracownikRegister.Telefon;
        }

        [Required(ErrorMessage = "Pole Imię jest wymagane.")]
        [RegularExpression("^[A-ZĄĆĘŁŃÓŚŹŻ][a-z][a-ząćęłńóśźż]*$", ErrorMessage = "Pole Imie może być tylko jednoczłonowe, zaczynające się od wielkiej litery i zawierające tylko litery.")]
        [MaxLength(30)]
        [Display(Name = "Imię")]
        public string Imie { get; set; }

        [Required(ErrorMessage = "Pole Nazwisko jest wymagane.")]
        [RegularExpression("^(([A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]{2,})(-[A-ZŻŹĆĄŚĘŁÓŃ][a-ząćęłńóśźż]{2,})?)*$", ErrorMessage = "Pole Nazwisko zaczyna się od wielkiej litery i może zawierać tylko litery.")]
        [MaxLength(30)]
        public string Nazwisko { get; set; }
    }
}
