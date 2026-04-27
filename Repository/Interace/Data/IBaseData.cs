namespace stok.Repository.Interace.Data
{
    public interface IBaseData
    {
        Task Save<T>(T data, CancellationToken cancellation = default) where T : class;
        Task SaveChanges(CancellationToken cancellation = default);
        IQueryable<TEntity> BaseQuery<TEntity>(bool withTracking) where TEntity : class;
    }
}
