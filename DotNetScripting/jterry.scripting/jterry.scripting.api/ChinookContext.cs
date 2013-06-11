using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations.Schema;

namespace jterry.scripting.api
{
    public class ChinookContext : DbContext, IUnitOfWork
    {
        public DbSet<Customer> Customers { get; set; }

        public ChinookContext()
        {
            Database.SetInitializer<ChinookContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            var customerMap = modelBuilder.Entity<Customer>();
            customerMap.ToTable("Customer");
            customerMap.HasKey(c => c.Id);
            customerMap.Property(c => c.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("Customerid");
            base.OnModelCreating(modelBuilder);
        }

        public IQueryable<Customer> GetCustomers()
        {
            var query = from c in Customers select c;
            return query;
        }
    }
}
