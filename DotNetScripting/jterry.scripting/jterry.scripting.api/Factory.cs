namespace jterry.scripting.api
{
    public class Factory : IFactory
    {
        public IRepository GetRepository(string entityType)
        {
            return new Repository(entityType);
        }
    }
}
