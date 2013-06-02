using System;
namespace jterry.scripting.api
{
    public interface IFactory
    {
        IRepository GetRepository(string entityType);
    }
}
