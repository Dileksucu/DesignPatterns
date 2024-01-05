using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Strategy.Models.Entities;
using WebApp.Strategy.Repositories;

namespace WebApp.Strategy.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        /// <summary>
        ///Burada direkt contextten haberleşmesi iyi bir şey değil 
        ///ORM aracını biliyor olması iyi bir durum değil 
        ///Kısacası: controllar , DbContextimi biliyor ben bu durmu istemiyorum. 
        /// İleride desem ki mongo db ye geçmek istiyorum. Bundan dolayı hangi veritabanı ile çalıştığımızı bilmeyecek.
        /// Soyutlama yapmamız ve ınterface kullanmamız lazım. Bu sayede verilerin hangi db'den geldiğini bilmeyecektir . 
        /// </summary>
      
        private readonly IProductRepository _repository;
        private readonly UserManager<AppUser> _userManager;
     
        /// <summary>
        /// Burada biz Repository üzerinden haberleşiyoruz . Bunu haberleşmeyi Service üzerinden de yapabiliriz (CQRS yapısını kullanarak mesela), katmanlı bir mimariye oluşturarak. 
        /// Fakat önemli olan strategy pattern amacını anlamak olduğu için bu şekilde yazdım.
        /// </summary>
       
        public ProductsController(IProductRepository repository, UserManager<AppUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var user = await _userManager
                .FindByNameAsync(User.Identity.Name);

            return View(await _repository.GetAllByUserIdAsync(user.Id));
            //user'dan gelen Name ile methoddan ile gelen o name'in ıd'ni gösterdik.
            //GetAllByUserIdAsync() --> buradam tüm userın ıd'si geliyor ve eşleştirme yapılıyor.

        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
                return NotFound();

            var product = await _repository
                .GetByIdAsync(id);

            if(product == null)
                return NotFound();

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Stock,UserId,CreatedDate")] Product product)
        {
            var user = await _userManager
              .FindByNameAsync(User.Identity.Name); //user'ın name bilgisini bulduk.

            if (ModelState.IsValid)
            {
                //Yukarıda user'ın name bilgisine ulaştım - (FindByNameAsync ile).
                //Product'ın userıd ile  gelen Id'yi mapledim. Aynı işlemi CreatedDate için de yaptım.
                //Service katmanım olmadığı için bu işlemi yaptım.
                product.UserId = user?.Id; 
                product.CreatedDate = DateTime.Now;

                await _repository.Save(product); // Generic Repository
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
                return NotFound();

            var product = await _repository
                .GetByIdAsync(id);

            if (product == null)
                return NotFound();
            
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Price,Stock,UserId,CreatedDate")] Product product)
        {
            if (id != product.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                   await _repository.Update(product); //Generic Repository
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index)); //Yönlendirmede kullanılır.
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null )
                return NotFound();

            var product = await _repository
                .GetByIdAsync(id);  //Generic Repository

            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var product = await _repository.GetByIdAsync(id); //id'yi buldum
             
            if (product != null)
               await _repository.Delete(product); //Generic Repository

            return RedirectToAction(nameof(Index));
            //RedirectToAction() --> controller'ı sayfaya yönlendirmek için kullanılan bir methoddur.
        }

        private bool ProductExists(string id)
        {
            return _repository.GetByIdAsync(id) != null;
            //id null değilse, varsa --> true döner
            //id değeri yoksa ---> false döndürür

        }
    }
}
