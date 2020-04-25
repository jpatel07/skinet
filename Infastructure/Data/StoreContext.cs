using System.Linq;
using System.Reflection;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {

                //for sqllite conver decimel to double 
                foreach (var entityTYpe in modelBuilder.Model.GetEntityTypes())
                {
                    var properties = entityTYpe.ClrType.GetProperties().Where(p => p.PropertyType
                   == typeof(decimal));

                    foreach (var property in properties)
                    {
                        modelBuilder.Entity(entityTYpe.Name).Property(property.Name)
                        .HasConversion<double>();
                    }
                }
            }
        }
    }
}