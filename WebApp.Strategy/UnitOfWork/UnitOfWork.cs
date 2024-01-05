using MongoDB.Driver;
using WebApp.Strategy.Models.Context;

namespace WebApp.Strategy.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppIdentityDbContext _identityDbContext;

        public UnitOfWork(AppIdentityDbContext identityDbContext)
        {
            _identityDbContext = identityDbContext;
        }

        public async Task<int> SaveChangeAsync()
         => await _identityDbContext.SaveChangesAsync();
    }
}
