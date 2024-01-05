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

        //Bu ýnterfaceleri ve ýmplementlerini projeye entegre ediyorum.
        //AddScoped, servislerin yaþam döngüsünü belirtmek için kullanýlan bir metoddur.
        //Bu metot, servislerin hangi kapsamda olduðunu ve nasýl yaþam döngüsüne sahip olacaðýný tanýmlar. Interface ile ýmplemente classý birbirine baðlar .

        //Bu service üzerinden herhangi bir classa eriþebilriim.
        builder.Services.AddHttpContextAccessor();

        //Bu þekilde dinamik olan bir entegrasyon saðlamýþ oldum. 
        //Bu ýnterface için dinamik olarak þeçilen enum'a göre classý ayarlayacaktýr.
        builder.Services.AddScoped<IProductRepository>(sc =>
        {
            //GetRequiredService() ,Eriþmeye çalýþtýðým service yoksa geriye hata fýrlatýr.GetService(), eriþilmeye çalýþtýðým service yoksa geriye hata döndürmez
            //Bu service üzerinden httpContext'e eriþebilirim.

            var httpContexAccessor = sc.GetRequiredService<IHttpContextAccessor>();

            //Claim tablosundan , Enumlarý çekiyor yani kullanýcýnýn seçtiði databaseleri.
            //Bunu IHttpContextAccessor ile yapýyoruz.

            var claim = httpContexAccessor.HttpContext.User.Claims
            .Where(x => x.Type == Settings.claimDatabaseType).FirstOrDefault();

            //Contexti aldýk burada , sql veya mongo db de contextin paramaetresini istiyor içine.
            var context = sc.GetRequiredService<AppIdentityDbContext>();
            var unitOfWork=sc.GetService<UnitOfWork>(); //sýkýntý var ??

            //Kullanýcý tarafýndan hiçbir enum seçilmediyse default olarak sql döndür. ??
            if (claim is null) 
                return new ProductRepositoryFromSqlServer(unitOfWork, context);

            var databaseType = (DatabaseType)int.Parse(claim.Value); //claim bilgilerini int parse edilir.

            return databaseType switch
            {
                DatabaseType.SqlServer => new ProductRepositoryFromSqlServer(unitOfWork, context),
                DatabaseType.MongoDb => new ProductRepositoryFromMongoDb(builder.Configuration),
            };
        });

        builder.Services.AddScoped<IUnitOfWork,UnitOfWork>(); //Burada bir sýkýntý var ??
      
        //Db baðlantýsý
        builder.Services.AddDbContext<AppIdentityDbContext>(options =>
        {

            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
        });

        //Identity entegrasyonu
        builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true; //Email Uniq olsun, eþsiz olsun.
        }).AddEntityFrameworkStores<AppIdentityDbContext>();

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        //Araþtýr !
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