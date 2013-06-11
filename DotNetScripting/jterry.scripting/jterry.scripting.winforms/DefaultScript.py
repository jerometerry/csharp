import clr
import System

clr.AddReference("System.Core")
clr.AddReference("System.Windows.Forms")
clr.ImportExtensions(System.Linq)

customers = unitOfWork.GetCustomers()
print "Found " + customers.Count().ToString() + " customers"

employees = unitOfWork.GetEmployees();
print "Found " + employees.Count().ToString() + " employees"

customer = customers.FirstOrDefault()
print "Customer: " + customer.FirstName + " " + customer.LastName

rep = customer.SupportRep
print "Support Rep: " + rep.FirstName + " " + rep.LastName

reportsTo = rep.ReportsTo
if (reportsTo != None):
    print "Reports To: " + reportsTo.FirstName + " " + reportsTo.LastName
else:
    print "Employee has no boss"

