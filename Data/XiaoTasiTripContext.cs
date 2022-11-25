using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace xiaotasi.Data
{
    public class XiaoTasiTripContext : DbContext
    {
        public XiaoTasiTripContext(DbContextOptions<XiaoTasiTripContext> options)
            : base(options)
        {
        }

        public DbSet<xiaotasi.Models.TripViewModel> TripViewModels { get; set; }
    }
}
