using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace jterry.scripting.api
{
    public interface IUnitOfWork
    {
        IQueryable<Customer> GetCustomers();
    }
}
