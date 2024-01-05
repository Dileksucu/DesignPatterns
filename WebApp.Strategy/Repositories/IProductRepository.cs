using WebApp.Strategy.Models.Entities;

namespace WebApp.Strategy.Repositories
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(string id);

        Task<List<Product>> GetAllByUserIdAsync(string userId);

        Task<Product> Save(Product product); //ekleme

        //Bunlar bir product almasına gerek yok , var olan veri gelecek ve dönüş için parametre yeterkli olaaktır.
        Task Update(Product product);
        Task Delete(Product product);
    }


}
