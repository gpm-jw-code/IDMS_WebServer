using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace IDMSWebServer.Models.DataModels
{
    public class IDMSContext : DbContext
    {
        public string Schema = "";
        private IConfiguration _config;

        public DbSet<Vibration_Energy> vibration_energy { get; set; }
        public DbSet<Health_Score> health_score { get; set; }
        public DbSet<Alert_Index> alert_index { get; set; }
        public DbSet<Vibration_raw_data> vibration_raw_data { get; set; }
        public DbSet<Physical_quantity> physical_quantity { get; set; }
        public DbSet<Side_Band> side_band { get; set; }
        public DbSet<Frequency_doubling> frequency_doubling { get; set; }


        public IDMSContext(IConfiguration config, string sensorIP)
        {
            _config = config;
            Schema = "sensor_" + sensorIP.Replace(".", "_");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var sqlcon = _config.GetConnectionString("IDMSDb");

            optionsBuilder.UseNpgsql(sqlcon)
                .ReplaceService<IModelCacheKeyFactory, MyModelCacheKeyFactory>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);
            modelBuilder.UseSerialColumns();
            //modelBuilder.Entity<Vibration_Energy>().HasKey(v => v.datetime);
            modelBuilder.Entity<Vibration_Energy>().Property(i => i.datetime).HasColumnType("timestamp without time zone");
            modelBuilder.Entity<Health_Score>().Property(i => i.datetime).HasColumnType("timestamp without time zone");
            modelBuilder.Entity<Alert_Index>().Property(i => i.datetime).HasColumnType("timestamp without time zone");
            modelBuilder.Entity<Vibration_raw_data>().Property(i => i.datetime).HasColumnType("timestamp without time zone");
            modelBuilder.Entity<Physical_quantity>().Property(i => i.datetime).HasColumnType("timestamp without time zone");
            modelBuilder.Entity<Side_Band>().Property(i => i.datetime).HasColumnType("timestamp without time zone");
            modelBuilder.Entity<Frequency_doubling>().Property(i => i.datetime).HasColumnType("timestamp without time zone");

            modelBuilder.Entity<Side_Band>().Property(i => i.severity).HasColumnType("json");
            modelBuilder.Entity<Frequency_doubling>().Property(i => i.severity).HasColumnType("json");
        }
    }


    class MyModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
            => new MyModelCacheKey(context);
    }

    class MyModelCacheKey : ModelCacheKey
    {
        string _schema;

        public MyModelCacheKey(DbContext context)
            : base(context)
        {
            _schema = (context as IDMSContext)?.Schema;
        }

        protected override bool Equals(ModelCacheKey other)
            => base.Equals(other)
                && (other as MyModelCacheKey)?._schema == _schema;

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode() * 397;
            if (_schema != null)
            {
                hashCode ^= _schema.GetHashCode();
            }

            return hashCode;
        }
    }
}
