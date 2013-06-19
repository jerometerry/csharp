#< imports
import clr
import System
clr.AddReference("System.Core")
clr.AddReference("System.Windows.Forms")
clr.ImportExtensions(System.Linq)
from System import String
#> end imports

#< methods
def TestIronPython():
    CountEntities()
    TestCustomers()
    TestInvoices()
    TestPlaylists()
    TestSearchTracks()

def GetAllCustomers():
    return unitOfWork.GetCustomers()

def GetAllEmployees():
    return unitOfWork.GetEmployees()

def GetAllInvoices():
    return unitOfWork.GetInvoices()

def GetAllPlaylists():
    return unitOfWork.GetPlaylists()

def GetAllTracks():
    return unitOfWork.GetTracks()

def CountEntities():
    WriteLine(String.Format("Found {0} customers", GetAllCustomers().Count()))
    WriteLine(String.Format("Found {0} employees", GetAllEmployees().Count()))
    WriteLine(String.Format("Found {0} invoices", GetAllInvoices().Count()))
    WriteLine()

def TestCustomers():
    WriteLine("Testing Customers...")
    customer = GetAllCustomers().FirstOrDefault()
    rep = customer.SupportRep
    reportsTo = rep.ReportsTo
    
    WriteLine(String.Format("Customer: {0} {1}", customer.FirstName, customer.LastName))
    WriteLine(String.Format("Support Rep: {0} {1}", rep.FirstName, rep.LastName))

    if (reportsTo != None):
        WriteLine(String.Format("Reports To: {0} {1}", reportsTo.FirstName, reportsTo.LastName))
    else:
        WriteLine("Employee has no boss")

    WriteLine("Testing Customers complete");
    WriteLine()

def WriteLine(line = ""):
    print line

def TestPlaylists():
    WriteLine("Testing Playlists...")
    playlist = GetAllPlaylists().FirstOrDefault()
    WriteLine(playlist.Name)
    WriteLine("Testing Playlists complete")
    WriteLine()

def TestInvoices():
    WriteLine("Testing Invoices...")
    invoice = GetAllInvoices().FirstOrDefault()
    WriteLine(String.Format("Invoice Id: {0} Date: {1}", invoice.Id, invoice.InvoiceDate))
    lines = invoice.Lines
    WriteLine(String.Format("Found {0} tracks", lines.Count))
    for l in lines:
        WriteLine(String.Format("Invoice Line Id: {0} Track Id: {1} Track Name: {2} Price: {3}", 
                                l.Id, l.Track.Id, l.Track.Name, l.Track.UnitPrice))
    WriteLine("Testing Invoices complete")
    WriteLine()

def TestSearchTracks():
    WriteLine("Testing Search Tracks...");
    SearchTracks("AC/DC")
    WriteLine("Testing Search Tracks Complete");
    WriteLine()

def SearchTracks(composer):
    WriteLine(String.Format("Searching for tracks by '{0}'...", composer))
    tracks = GetAllTracks().Where(lambda t: t.Composer == composer)
    WriteLine(String.Format("Found {0} tracks by '{1}'", tracks.Count(), composer))
    for t in tracks:
        WriteLine(String.Format("Track Id: {0} Name: {1}", t.Id, t.Name))
    WriteLine("Search complete")
#> end methods

TestIronPython()