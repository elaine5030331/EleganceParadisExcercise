using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly EleganceParadisContext _context;
        private IDbContextTransaction _transaction;
        private readonly IServiceProvider _serviceProvider;

        public EFUnitOfWork(EleganceParadisContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }

        public async Task BeginAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _transaction.CommitAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return _serviceProvider.GetRequiredService<IRepository<TEntity>>();
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
        }
    }
}
