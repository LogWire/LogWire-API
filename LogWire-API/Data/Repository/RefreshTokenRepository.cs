using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using LogWire.API.Data.Context;
using LogWire.API.Data.Model;

namespace LogWire.API.Data.Repository
{
    public class RefreshTokenRepository : IDataRepository<RefreshTokenEntry>
    {

        readonly DataContext _context;

        public RefreshTokenRepository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<RefreshTokenEntry> GetAll()
        {
            return _context.RefreshTokens.ToList();
        }

        public RefreshTokenEntry Get(string key)
        {
            return _context.RefreshTokens
                .FirstOrDefault(e => e.Token == key);
        }

        public void Add(RefreshTokenEntry entity)
        {
            _context.RefreshTokens.Add(entity);
            _context.SaveChanges();
        }

        public void Update(RefreshTokenEntry dbEntity, RefreshTokenEntry entity)
        {
            dbEntity.Token = entity.Token;
            dbEntity.CreatedAt = entity.CreatedAt;
            dbEntity.UserId = entity.UserId;

            _context.SaveChanges();
        }

        public void Delete(RefreshTokenEntry entity)
        {
            _context.RefreshTokens.Remove(entity);
            _context.SaveChanges();
        }

        public IEnumerable<RefreshTokenEntry> GetExpiredTokens(int mins)
        {
            return _context.RefreshTokens.Where(e => e.CreatedAt.AddMinutes(mins) < DateTime.UtcNow ).ToList();
        }
    }
}
