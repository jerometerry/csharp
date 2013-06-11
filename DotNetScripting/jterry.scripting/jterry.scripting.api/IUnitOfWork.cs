using System.Linq;

namespace jterry.scripting.api
{
    public interface IUnitOfWork
    {
        IQueryable<Customer> GetCustomers();
        IQueryable<Employee> GetEmployees();
        IQueryable<Invoice> GetInvoices();
        IQueryable<InvoiceLine> GetInvoiceLines();
        IQueryable<Track> GetTracks();
        IQueryable<Playlist> GetPlaylists();
    }
}
