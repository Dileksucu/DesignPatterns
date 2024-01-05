namespace WebApp.Strategy.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangeAsync();
    }
}
