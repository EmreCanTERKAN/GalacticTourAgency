using GalacticTourAgency.Models;
using Microsoft.EntityFrameworkCore;

namespace GalacticTourAgency.Data
{
    // Dbcontex olduğunu belirtmek için öncelikle dbcontext sınıfından miras almaktır.
    public class GalacticContext : DbContext
    {
        // Çoğul isim kullanılır.
        public DbSet<GalacticProduct> GalacticProducts { get; set; }

        // OnConfiguring metodunu ezerek bu database'in hangi databesi kullanacağını belirtebiliriz.
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=Galactic;Truested_Connection=true");
        //}
    }
}
