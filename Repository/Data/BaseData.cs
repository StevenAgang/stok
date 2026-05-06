using Microsoft.EntityFrameworkCore;
using stok.Repository.Interace.Data;

namespace stok.Repository.Data
{
    public class BaseData(DatabaseContext _context) : IBaseData
    {
        public async Task Save<T>(T data, CancellationToken cancellation = default) where T : class
        {
           _context.Add(data);
            await _context.SaveChangesAsync(cancellation);
        }

        public async Task SaveChanges(CancellationToken cancellation = default)
        {
            await _context.SaveChangesAsync(cancellation);
        }

        public IQueryable<TEntity> BaseQuery<TEntity>(bool withTracking) where TEntity : class
        {
            var query = _context.Set<TEntity>().AsQueryable();

            if (!withTracking)
            {
                query = query.AsNoTracking();
            }

            return query;
        }
    }
}
