using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Configuration;
using System.Security.Claims;
using WebApp.Strategy.Models.Entities;
using WebApp.Strategy.Models.Enum;
using Settings = WebApp.Strategy.Models.Settings.Settings;

namespace WebApp.Strategy.Controllers
{
    [Authorize] //Kullanıcı sisteme giriş yapmış olması gereklidir bu işlemi yapabilmesi için. 
    public class SettingsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public SettingsController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            Settings settings = new();
            if (User.Claims.Where(x=>x.Type == Settings.claimDatabaseType).FirstOrDefault()!= null) 
            { 
                //claimValue kolonu nvarchar olduğu için ben enumları int olarak tutuyorum ? 
                //Bundan dolayı integer'a çevirmem lazım.
                settings.DatabaseType=(DatabaseType)int.Parse(User.Claims.First(x=>x.Type == Settings.claimDatabaseType).Value);
            }
            //eğer kullanıcı vt işaretlemediyse de default değeri döndüm.
            settings.DatabaseType = settings.GetDefaultDatabaseType;

            return View();
        }

        /// <summary>
        ///Claim'ler, genellikle kimlik doğrulama ve yetkilendirme süreçlerinde kullanılır ve kullanıcının izinlerini veya özel bilgilerini temsil eder.
        /// </summary>
      
        [HttpPost]
        public async Task<IActionResult> ChangeDatabase(int databaseType) 
        {
            var user = await _userManager
                .FindByNameAsync(User.Identity.Name); 
            //userın name bilgilerine ulaştım, name alanı uniq'dir.


            var newClaim = new Claim(Settings.claimDatabaseType,databaseType.ToString());
           //Veritabanındaki yeni bir claim oluşturuyoruz, bu ınstance ile.
           
            var claims=await _userManager.GetClaimsAsync(user);
            //Vt'nındaki tüm claimları GetClaimAsync() methodu ile alıyoruz.
            //Bu metot, kullanıcıların yetkilerini, özelleştirilmiş verilerini veya uygulama içindeki diğer özelliklerini temsil eden claim'leri çekmek için kullanılır. 

            //Bu claim var mı diye kontrol edicem .
            //Var olan claimi bulmak için.
            var hasDatabaseTypeClaim = claims
                .FirstOrDefault(x=>x.Type==Settings.claimDatabaseType);

            if (hasDatabaseTypeClaim != null)
            {
                await _userManager
                    .ReplaceClaimAsync(user,hasDatabaseTypeClaim,newClaim);

                //Burada kullanıcının claimini değiştirdim . Var olan ile , kullanıcı bilgilerini alarak yeni claim yarattım.

                //Replace methodu ile 3 parametre alır. Bunlar;
                //(User'ı ister,var olan claimi ister, yeni oluşturmak için claim ister)
                
            }

            //Null  ise direkt yeni bir claim oluşturuyor ve user bilgilerini veriyorum.
            await _userManager.AddClaimAsync(user,newClaim);

            //Önemli kısım
            //Veritabanında claimleri değiştirdin fakat cookie de hala eskisi olabilir.
            //İlk önce kullanıcıya logout yapıp daha sonra login yapmalıyım.
            //Bu sayede kullanıcı çıkış yapıp tekrar giriş yaptığında, cookie'ye yeni claim bilgilerini alınmış olacak.

            await _signInManager.SignOutAsync(); //Kullanıcı bu çıkış işlemini hissetmeyecek.

            //cookie ile tutulan var olan bilgileri almak gerekir. Bu sayede propertylere ulaşabilirim. 
            //Cookie de bulunabilecek bilgiler --> Beni hatırla checkbox tutulması, token kaydedilmiş olabilir..vb durumlar.
            var result = await HttpContext.AuthenticateAsync(); // Propertyleri almak için cookiedeki farklı bilgileri çekmemize yaradı.

            //Tekrar sisteme giriş yaptık ve cookie bilgileri yeni bir şekilde kaydedildi.
            await _signInManager.SignInAsync(user,result.Properties);


            return RedirectToAction(nameof(Index));
        }
    }
}
 