using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using SESG.UserWebService.Core;

namespace SESG.UserWebService.Data
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UserContext()
        {
        }

        public UserContext(string cs)
            : base(cs)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            var userMap = modelBuilder.Entity<User>();
            userMap.ToTable("Users");
            userMap.HasKey(p => p.UserID)
                .Property(p => p.UserID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            userMap.HasOptional(p => p.ProfilePicture)
                .WithMany()
                .Map(x => x.MapKey("ProfilePictureId"));

            var fileMap = modelBuilder.Entity<File>();
            fileMap.ToTable("Files");
            fileMap.HasKey(p => p.FileID)
                .Property(p => p.FileID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            fileMap.HasOptional(p => p.CreatedBy)
                .WithMany()
                .Map(x => x.MapKey("CreatedBy"));

            base.OnModelCreating(modelBuilder);
        }
    }
}
