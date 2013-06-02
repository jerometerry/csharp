using System;

namespace jterry.scripting.api
{
    public interface IEntity
    {
        string EntityType { get; }

        int Id { get; }
        string Name { get; set; }

        DateTime Created { get; set; }
        DateTime Modified { get; set; }
        
    }
}
