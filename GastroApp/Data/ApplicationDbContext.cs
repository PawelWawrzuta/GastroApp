using GastroApp.Models;
using GastroApp.Models.Pracownicy;
using GastroApp.Models.Samochody;
using GastroApp.Models.Zamowienia;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GastroApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Produkt> Produkty { get; set; }
       
        public DbSet<Pracownik> Pracownicy { get; set; }

        public DbSet<RodzajPaliwa> RodzajPaliw { get; set; }
        public DbSet<Samochod> Samochody { get; set; }
        public DbSet<Zuzycie> Zuzycia { get; set; }
        public DbSet<Tankowanie> Tankowania { get; set; }
        public DbSet<SposobPlatnosci> SposobPlatnosci { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Klient> Klienci { get; set; }        
        public DbSet<Zamowienie> Zamowienia { get; set; }
        public DbSet<ListaZamowienia> ListaZamowienia { get; set; }
        
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var foreignKey in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}