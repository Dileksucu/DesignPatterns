using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApp.Strategy.Models.Context;
using WebApp.Strategy.Models.Entities;
using WebApp.Strategy.Models.Enum;
using WebApp.Strategy.Models.Settings;
using WebApp.Strategy.Repositories;
using WebApp.Strategy.UnitOfWork;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Bu �nterfaceleri ve �mplementlerini projeye entegre ediyorum.
        //AddScoped, servislerin ya�am d�ng�s�n� belirtmek i�in kullan�lan bir metoddur.
        //Bu metot, servislerin hangi kapsamda oldu�unu ve nas�l ya�am d�ng�s�ne sahip olaca��n� tan�mlar. Interface ile �mplemente class� birbirine ba�lar .

        //Bu service �zerinden herhangi bir classa eri�ebilriim.
        builder.Services.AddHttpContextAccessor();

        //Bu �ekilde dinamik olan bir entegrasyon sa�lam�� oldum. 
        //Bu �nterface i�in dinamik olarak �e�ilen enum'a g�re class� ayarlayacakt�r.
        builder.Services.AddScoped<IProductRepository>(sc =>
        {
            //GetRequiredService() ,Eri�meye �al��t���m service yoksa geriye hata f�rlat�r.GetService(), eri�ilmeye �al��t���m service yoksa geriye hata d�nd�rmez
            //Bu service �zerinden httpContext'e eri�ebilirim.

            var httpContexAccessor = sc.GetRequiredService<IHttpContextAccessor>();

            //Claim tablosundan , Enumlar� �ekiyor yani kullan�c�n�n se�ti�i databaseleri.
            //Bunu IHttpContextAccessor ile yap�yoruz.

            var claim = httpContexAccessor.HttpContext.User.Claims
            .Where(x => x.Type == Settings.claimDatabaseType).FirstOrDefault();

            //Contexti ald�k burada , sql veya mongo db de contextin paramaetresini istiyor i�ine.
            var context = sc.GetRequiredService<AppIdentityDbContext>();
            var unitOfWork=sc.GetService<UnitOfWork>(); //s�k�nt� var ??

            //Kullan�c� taraf�ndan hi�bir enum se�ilmediyse default olarak sql d�nd�r. ??
            if (claim is null) 
                return new ProductRepositoryFromSqlServer(unitOfWork, context);

            var databaseType = (DatabaseType)int.Parse(claim.Value); //claim bilgilerini int parse edilir.

            return databaseType switch
            {
                DatabaseType.SqlServer => new ProductRepositoryFromSqlServer(unitOfWork, context),
                DatabaseType.MongoDb => new ProductRepositoryFromMongoDb(builder.Configuration),
            };
        });

        builder.Services.AddScoped<IUnitOfWork,UnitOfWork>(); //Burada bir s�k�nt� var ??
      
        //Db ba�lant�s�
        builder.Services.AddDbContext<AppIdentityDbContext>(options =>
        {

            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
        });

        //Identity entegrasyonu
        builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true; //Email Uniq olsun, e�siz olsun.
        }).AddEntityFrameworkStores<AppIdentityDbContext>();

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        //Ara�t�r !
        var app = builder.Build();

        using var scope = app.Services.CreateScope();

        var identityDbContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();


        if (!userManager.Users.Any())
        {
            userManager.CreateAsync(new AppUser() { UserName = "user1", Email = "user1@outlook.com" }, "Password12*").Wait();

            userManager.CreateAsync(new AppUser() { UserName = "user2", Email = "user2@outlook.com" }, "Password12*").Wait();
            userManager.CreateAsync(new AppUser() { UserName = "user3", Email = "user3@outlook.com" }, "Password12*").Wait();
            userManager.CreateAsync(new AppUser() { UserName = "user4", Email = "user4@outlook.com" }, "Password12*").Wait();
            userManager.CreateAsync(new AppUser() { UserName = "user5", Email = "user5@outlook.com" }, "Password12*").Wait();
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }

}