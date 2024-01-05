using BaseProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BaseProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login() 
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var emailFind= await _userManager.FindByEmailAsync(email);
            
            if (emailFind is null) 
               return View();

            //SignInManager<> --> sınıfında bulunan "PasswordSignInAsync" methodunu kullanıyoruz.
            //Bu methodun kullanım amacı - metod, kullanıcı adı ve şifre ile kimlik doğrulama yapmak için kullanılır. 

            var signInResult = await _signInManager
                .PasswordSignInAsync(emailFind,password,true,false);

            if (!signInResult.Succeeded)
            { return View(); }

            //Bu kod, bir kullanıcının oturum açma işlemi başarılı olduğunda, yönlendirme (redirect) yaparak kullanıcıyı belirli bir sayfaya yönlendirmek için kullanılır.
            //RedirectToAction(action name, controller name) parametrelerini alır .
            //nameof kullanmamızın amacı -> Özellikle, kodun belirli bir noktasında kullanılan bir öğenin adını almak için nameof kullanılır. Bu, adın kodun farklı yerlerinde değişmesi durumunda, hata ayıklama ve bakım süreçlerinde hataları azaltmak için kullanışlı olabilir.
            return RedirectToAction(nameof(HomeController.Index), "Home");

        }

        public async Task<IActionResult> Logout()
        {
            //SignOutAsync() methodu da kütüphaneden gelen methodlardan biri 
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");

        }
    }
}
