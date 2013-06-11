import clr
import System

clr.AddReference("System.Core")
clr.AddReference("System.Windows.Forms")
clr.ImportExtensions(System.Linq)

customers = unitOfWork.GetCustomers()
print customers.Count()