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
        public DbSet<Employee> Employees { get; set; }

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
                .HasColumnName("CustomerId");
            customerMap.HasOptional(c => c.SupportRep)
                .WithMany().Map(x => x.MapKey("SupportRepId"));

            var employeeMap = modelBuilder.Entity<Employee>();
            employeeMap.ToTable("Employee");
            employeeMap.HasKey(e => e.Id);
            employeeMap.Property(e => e.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("EmployeeId");
            employeeMap.HasOptional(c => c.ReportsTo)
                .WithMany().Map(x => x.MapKey("ReportsTo"));

            base.OnModelCreating(modelBuilder);
        }

        public IQueryable<Customer> GetCustomers()
        {
            var query = from c in Customers select c;
            return query;
        }

        public IQueryable<Employee> GetEmployees()
        {
            var query = from c in Employees select c;
            return query;
        }
    }
}
