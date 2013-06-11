using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace jterry.scripting.api
{
    public class ChinookContext : DbContext, IUnitOfWork
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceLine> InvoiceLines { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MediaType> MediaTypes { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistTrack> PlaylistTracks { get; set; }

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

            var invoiceMap = modelBuilder.Entity<Invoice>();
            invoiceMap.ToTable("Invoice");
            invoiceMap.HasKey(e => e.Id);
            invoiceMap.Property(e => e.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("InvoiceId");
            invoiceMap.HasOptional(i => i.Customer)
                .WithMany().Map(x => x.MapKey("CustomerId"));

            var invoiceLineMap = modelBuilder.Entity<InvoiceLine>();
            invoiceLineMap.ToTable("InvoiceLine");
            invoiceLineMap.HasKey(l => l.Id);
            invoiceLineMap.Property(l => l.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("InvoiceLineId");
            invoiceLineMap.HasRequired(l => l.Invoice)
                .WithMany(i => i.Lines).Map(x => x.MapKey("InvoiceId"));
            invoiceLineMap.HasOptional(l => l.Track)
                .WithMany().Map(x => x.MapKey("TrackId"));

            var trackMap = modelBuilder.Entity<Track>();
            trackMap.ToTable("Track");
            trackMap.HasKey(t => t.Id);
            trackMap.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("TrackId");
            trackMap.HasOptional(t => t.Genre)
                .WithMany().Map(x => x.MapKey("GenreId"));
            trackMap.HasOptional(t => t.Album)
                .WithMany().Map(x => x.MapKey("AlbumId"));
            trackMap.HasOptional(t => t.MediaType)
                .WithMany().Map(x => x.MapKey("MediaTypeId"));

            var artistMap = modelBuilder.Entity<Artist>();
            artistMap.ToTable("Artist");
            artistMap.HasKey(a => a.Id);
            artistMap.Property(a => a.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ArtistId");

            var genreMap = modelBuilder.Entity<Genre>();
            genreMap.ToTable("Genre");
            genreMap.HasKey(g => g.Id);
            genreMap.Property(g => g.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("GenreId");

            var albumMap = modelBuilder.Entity<Album>();
            albumMap.ToTable("Album");
            albumMap.HasKey(a => a.Id);
            albumMap.Property(a => a.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("AlbumId");
            albumMap.HasOptional(a => a.Artist)
                .WithMany().Map(x => x.MapKey("ArtistId"));

            var mediaTypeMap = modelBuilder.Entity<MediaType>();
            mediaTypeMap.ToTable("Media");
            mediaTypeMap.HasKey(m => m.Id);
            mediaTypeMap.Property(m => m.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("MediaTypeId");

            var playlistMap = modelBuilder.Entity<Playlist>();
            playlistMap.ToTable("Playlist");
            playlistMap.HasKey(p => p.Id);
            playlistMap.Property(p => p.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("PlaylistId");

            var playlistTrackMap = modelBuilder.Entity<PlaylistTrack>();
            playlistTrackMap.ToTable("PlaylistTrack");
            playlistTrackMap.HasKey(p => p.Id);
            playlistTrackMap.Property(p => p.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("PlaylistTrackId");
            playlistTrackMap.HasOptional(p => p.Playlist)
                .WithMany(p => p.Tracks).Map(x => x.MapKey("PlaylistId"));
            playlistTrackMap.HasOptional(p => p.Track)
                .WithMany().Map(x => x.MapKey("TrackId"));

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

        public IQueryable<Invoice> GetInvoices()
        {
            var query = from c in Invoices select c;
            return query;
        }

        public IQueryable<InvoiceLine> GetInvoiceLines()
        {
            var query = from c in InvoiceLines select c;
            return query;
        }

        public IQueryable<Track> GetTracks()
        {
            var query = from c in Tracks select c;
            return query;
        }

        public IQueryable<Playlist> GetPlaylists()
        {
            var query = from c in Playlists select c;
            return query;
        }
    }
}
