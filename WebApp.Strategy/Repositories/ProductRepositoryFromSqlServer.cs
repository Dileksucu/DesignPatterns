using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;
using WebApp.Strategy.Models.Context;
using WebApp.Strategy.Models.Entities;
using WebApp.Strategy.UnitOfWork;

namespace WebApp.Strategy.Repositories
{
    //Altı çizili alanlara bakmalısın.
    public class ProductRepositoryFromSqlServer : IProductRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppIdentityDbContext _identityDbContext;

        
        public ProductRepositoryFromSqlServer(IUnitOfWork unitOfWork, AppIdentityDbContext identityDbContext)
        {
            _unitOfWork = unitOfWork;
            _identityDbContext = identityDbContext;
        }

        public Task Delete(Product product)
        {
           _identityDbContext.Products
                .Remove(product);

             _unitOfWork.SaveChangeAsync();
            return Task.FromResult(product);
            // methodun asekron çalışmasını sağlar.
            //Bu durumda, senkron olarak alınan product değeri, bir Task içinde asenkron bir dönüş türü olarak sunulur.
        }

        public async Task<List<Product>> GetAllByUserIdAsync(string userId)
        {
            return await _identityDbContext
                 .Products
                 .Where(x => x.UserId == userId)
                 .ToListAsync();
        }

        //Burada idleri çekmek için FirstOrDefault() gibi işlev görür.
        public async Task<Product> GetByIdAsync(string id)
        {
            return await _identityDbContext
                 .Products
                 .FindAsync(id);
            //Burada productın Id si ile , dışarıdan gelen id aynı mı diye bakıyor 
            //Bir tek değer döndürür
        }

        public async Task<Product> Save(Product product)
        {
            product.Id= Guid.NewGuid().ToString(); 
            // sql server için bu işlemi yapıyoruz .MongoDb kendi eklediği ürün için kendi oluşturuyor guid bir ıd .
            //Bunu yapmamızın sebebi , db de olmayan eklenen yeni ürünün uniq bir ıd'sinin olmasını istememiz.

           await _identityDbContext.Products
                .AddAsync(product);

           await _unitOfWork.SaveChangeAsync(); 
            return product;
        }

        public Task Update(Product product)
        {
             _identityDbContext.Products
                .Update(product);

             _unitOfWork.SaveChangeAsync();
              return Task.FromResult(product);
            // methodun asekron çalışmasını sağlar. 
        }

        //return Task.CompletedTask;
        // Bu, geri döndürülmek istenen işlemin tamamlandığını, fakat bir değer döndürme gereksinimi olmadığını ifade eder.
    }
}
