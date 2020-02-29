using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogWire.API.Data.Repository
{
    public interface IDataRepository<TEntity>
    {

        IEnumerable<TEntity> GetAll();
        TEntity Get(string key);
        void Add(TEntity entity);
        void Update(TEntity dbEntity, TEntity entity);
        void Delete(TEntity entity);

    }
}
