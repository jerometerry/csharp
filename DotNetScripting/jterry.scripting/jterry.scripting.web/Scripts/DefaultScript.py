import clr
import System

clr.AddReference("System.Core")
clr.ImportExtensions(System.Linq)

from System import DateTime
from System.Collections.Generic import List

def getRepository(name):
    # factory is set when the script host is initialized by the server
    return factory.GetRepository(name)

def getAllEntities(repo):
    return repo.GetAll()
    
def printCurrentTime():
    print DateTime.Now.ToString()
    return    

def doBasicTests():
    printCurrentTime()

    repo = getRepository("Customers")
    
    customers = getAllEntities(repo)
    print "Initial customer count: " + customers.Count.ToString()

    customer = repo.CreateEntity()
    customer.Name = "My Customer"
    print "Id: " + customer.Id.ToString() + " Name: " + customer.Name
    
    print "Customer count after adding a customer: " + customers.Count.ToString()

    existing = repo.Get(customer.Id)
    print existing.Name

    deleted = repo.Delete(existing.Id)
    print "Customer Deleted: " + deleted.ToString()

    customers = getAllEntities(repo)
    print "Customer count after delete a customer: " + customers.Count.ToString()
    return
    
doBasicTests()