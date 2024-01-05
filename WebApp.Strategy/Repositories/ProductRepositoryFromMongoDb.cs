using MongoDB.Driver;
using WebApp.Strategy.Models.Entities;

namespace WebApp.Strategy.Repositories
{
    public class ProductRepositoryFromMongoDb : IProductRepository
    {
        //Ben bu Interface üzerinden mongoDb bağlanacağım.
        //Generic olarak product üzerinden işlem yapacağım.
        private readonly IMongoCollection<Product> _products;

        public ProductRepositoryFromMongoDb(IConfiguration configuration)
        {
            //appsetting.json dosyasının içinde olan MongoDb yolunu okuduk.
            var connectionString = configuration.GetConnectionString("MongoDb");
            var client = new MongoClient(connectionString);

            var database = client.GetDatabase("ProductDb");
            //MongoDb de varsa bu db kalır, yoksa bu iismde bir db oluştururmasını sağladık.

            _products = database.GetCollection<Product>("Products");
            //Product üzeirnden , GetCollection yapıyoruz ve bu tablomun  ismini Products olarak veriyorum.
            //Yukarıda tanımladığımız ınterface alanının içini doldurmak için de database de tanımladığımız alanları, oluşturacağımız tabloya mapliyoruz.
        }

        public async Task Delete(Product product)
        {
            await _products
                .DeleteOneAsync(x=>x.Id==product.Id);
            //Bu ıd'yi bul ve sil dedik.
        }

        public async Task<List<Product>> GetAllByUserIdAsync(string userId)
        {
            return await _products
               .Find(x=>x.UserId == userId)
               .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(string id)
        {
            return await _products
                .Find(x => x.Id == id) //id den bir tane bul diyoruz, db deki ıd ile gelen id yi eşit olanı getir.
                .FirstOrDefaultAsync(); //Bul veya geriye null dönmesi için ekledik bunu.
                
        }

        public async Task<Product> Save(Product product)
        {
            await _products.InsertOneAsync(product);
            return product;
        }

        public async Task Update(Product product)
        {
            //FindOneAndReplace --> bulduğumuz ıd ye sahip product ile 2.alandaki product'ı değiştir dedik
            await _products.FindOneAndReplaceAsync(x=>x.Id==product.Id, product);
              
        }
    }
}
