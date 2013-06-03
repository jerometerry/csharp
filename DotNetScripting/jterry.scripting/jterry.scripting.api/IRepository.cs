namespace jterry.scripting.api
{
    public interface IRepository
    {
        IEntity CreateEntity();
        bool Delete(int id);
        string EntityType { get; }
        IEntity Get(int id);
        System.Collections.Generic.IEnumerable<IEntity> Get(string name);
        System.Collections.Generic.IEnumerable<IEntity> GetAll();
    }
}
