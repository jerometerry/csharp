using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jterry.scripting.api
{
    public class Factory : jterry.scripting.api.IFactory
    {
        public IRepository GetRepository(string entityType)
        {
            return new Repository(entityType);
        }
    }
}
