using Microsoft.EntityFrameworkCore;

namespace FreeCourseApiNet5.Data
{
    public class MyDbContext:DbContext
    {
        public MyDbContext(DbContextOptions options):base(options)
        {

        }

        #region
        public DbSet<HangHoa> HangHoas { get; set; }
        public DbSet<Loai> Loais { get; set; }

        #endregion
    }
}
