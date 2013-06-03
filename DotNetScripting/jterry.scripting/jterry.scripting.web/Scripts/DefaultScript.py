import clr
clr.AddReference("System.Core")
import System
from System import *
from System.Collections.Generic import List
clr.ImportExtensions(System.Linq)

def getRepository(name):
    return factory.GetRepository(name)

def getAllEntities(repo):
    return repo.GetAll()

print DateTime.Now.ToString()
repo = getRepository("Customers")
customers = getAllEntities(repo)
print "Initial customer count: " + customers.Count.ToString()
customer1 = repo.CreateEntity()
customer1.Name = "My Customer"
print "Id: " + customer1.Id.ToString() + " Name: " + customer1.Name
print "Customer count after adding a customer: " + customers.Count.ToString()
existing = repo.Get(1)
print existing.Name
deleted = repo.Delete(existing.Id)
print "Customer Deleted: " + deleted.ToString()
customers = getAllEntities(repo)
print "Customer count after delete a customer: " + customers.Count.ToString()