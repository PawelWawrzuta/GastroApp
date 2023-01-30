using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GastroApp.Models.Samochody.ViewModel
{
    public class ZuzycieZSamochodem : Zuzycie
    {

        public SelectList? SamochodSelect { get; set; }

        [Required(ErrorMessage = "Pole Przebieg jest wymagane.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Pole Przebieg może zawierać tylko cyfry.")]
        public string PrzebiegString { get; set; }

    }
}
