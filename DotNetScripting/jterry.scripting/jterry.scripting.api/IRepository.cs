using System.Collections.Generic;

namespace jterry.scripting.api
{
    public interface IRepository
    {
        IEntity CreateEntity();
        bool Delete(int id);
        string EntityType { get; }
        IEntity Get(int id);
        IEnumerable<IEntity> Get(string name);
        IEnumerable<IEntity> GetAll();
    }
}
