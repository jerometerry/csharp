using System;
using System.Collections.Generic;
using System.Linq;

namespace jterry.scripting.api
{
    public class Repository : IRepository
    {
        int nextId = 1;
        List<IEntity> entities = new List<IEntity>();

        public string EntityType { get; private set; }

        public Repository(string entityType)
        {
            this.EntityType = entityType;
        }

        public IEnumerable<IEntity> GetAll()
        {
            return entities;
        }

        public IEntity CreateEntity()
        {
            var entity = new Entity(nextId++);
            entity.EntityType = this.EntityType;
            entity.Created = DateTime.Now;
            entity.Modified = DateTime.Now;
            entities.Add(entity);
            return entity;
        }

        public IEntity Get(int id)
        {
            return entities.Where(e => e.Id == id).FirstOrDefault();
        }

        public IEnumerable<IEntity> Get(string name)
        {
            return entities.Where(e => e.Name == name);
        }

        public bool Delete(int id)
        {
            var entity = Get(id);
            return entities.Remove(entity);
        }
    }
}
