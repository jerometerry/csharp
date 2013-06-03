using System;

namespace jterry.scripting.api
{
    public class Entity : IEntity
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public string EntityType { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public Entity()
        {
        }

        public Entity(int id)
        {
            this.Id = id;
        }

        public override bool Equals(object obj)
        {
            return this.Id == ((IEntity)obj).Id;
        }

        public override int GetHashCode()
        {
            return this.Id;
        }
    }
}
