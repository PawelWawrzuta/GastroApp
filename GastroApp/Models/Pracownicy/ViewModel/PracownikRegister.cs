using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GastroApp.Models.Pracownicy.ViewModel
{
    public class PracownikRegister : Pracownik
    {
        public PracownikRegister()
        {

        }
        public PracownikRegister(string imie, string nazwisko, string email, string telefon)
        {
            Imie = imie;
            Nazwisko = nazwisko;
            Email = email;
            Telefon = telefon;
        }

        [Required(ErrorMessage = "Pole Email jest wymagane.")]
        [RegularExpression("^[{a-zA-Z0-9}.,+-<>?:;{}|!#$%^&*]+@([{a-zA-Z0-9}.-]+.)+[{a-zA-Z0-9}]{2,4}$", ErrorMessage = "To nie jest Email.")]
        public override string Email { get; set; }

        [Required(ErrorMessage = "Pole Telefon jest wymagane.")]
        [RegularExpression("^([1-9]{1})([0-9]{2})((?:[-| ]*[0-9]){3}){2}$", ErrorMessage = "Pole Telefon zawiera 9 cyfr. Dostępne formaty: xxxxxxxxx, xxx xxx xxx, xxx-xxx-xxx.")]
        [MaxLength(11)]
        public string Telefon { get; set; }

        [Required(ErrorMessage = "Pole Hasło jest wymagane.")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Hasło musi zawierać minimum 8 znaków: minimum 1 wielką literę, minimum 1 małą literę, minimum 1 cyfrę oraz minimum 1 znak specialny.")]
        [Display(Name = "Hasło")]
        public string Haslo { get; set; }

        [Required(ErrorMessage = "Pole Potwierdz hasło jest wymagane.")]
        [Display(Name = "Potwierdz hasło")]
        public string PotwierdzHaslo { get; set; }
    }
}
