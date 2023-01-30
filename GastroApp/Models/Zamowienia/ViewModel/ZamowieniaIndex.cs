using GastroApp.Models.Zamowienia.HelpfulModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace GastroApp.Models.Zamowienia.ViewModel
{
    public class ZamowieniaIndex
    {
        public List<ZamowienieZKlientem> ZamowieniaPrzyjete { get; set; }
        public List<ZamowienieZKlientem> ZamowieniaPrzypisane { get; set; }
        public List<ZamowienieZKlientem> ZamowieniaWDrodze { get; set; }
        public List<ZamowienieZKlientem> ZamowieniaPrzypisaneWDrodzeProblem { get; set; }
        public List<ZamowienieZKlientem> ZamowieniaDostarczone { get; set; }
        public List<ZamowienieZKlientem> ZamowieniaProblem { get; set; }

        public SelectList? PracownikSelect { get; set; }
        public string IdPracownika { get; set; }
        public List<PodsumowanieDniaKierowcy> PodsumowanieDniaKierowcy { get; set; }

    }
}
