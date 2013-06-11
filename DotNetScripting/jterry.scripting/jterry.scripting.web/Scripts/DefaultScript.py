import clr
import System
clr.AddReference("System.Core")
clr.AddReference("System.Windows.Forms")
clr.ImportExtensions(System.Linq)

customers = unitOfWork.GetCustomers()
employees = unitOfWork.GetEmployees()
invoices = unitOfWork.GetInvoices()
playlists = unitOfWork.GetPlaylists()

customer = customers.FirstOrDefault()
rep = customer.SupportRep
reportsTo = rep.ReportsTo

print "Found " + customers.Count().ToString() + " customers"
print "Found " + employees.Count().ToString() + " employees"
print "Found " + invoices.Count().ToString() + " invoices"

print "Customer: " + customer.FirstName + " " + customer.LastName
print "Support Rep: " + rep.FirstName + " " + rep.LastName

if (reportsTo != None):
    print "Reports To: " + reportsTo.FirstName + " " + reportsTo.LastName
else:
    print "Employee has no boss"

invoice = invoices.FirstOrDefault()
lines = invoice.Lines

print "Tracks"
for l in lines:
    print l.Track.Name

playlist = playlists.FirstOrDefault()
print playlist.Name