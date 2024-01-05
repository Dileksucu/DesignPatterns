using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BaseProject.Models
{
    // IdentityDbContext --> bu context'in içine dbset oluşturmadım nedeni de kullandığımız "IdentityDbContext" sınfından tüm tabloların gelmesi .
    public class AppIdentityDbContext:IdentityDbContext<AppUser>
    {
        /// <summary>
        /// IdentityDbContext<> --> üyelik sistemini de içeren bir db context olduğu için .
        /// Aşağıda ctor oluşturma sebebi; bu dbcontext'i program.cs de tanımlayacağımız için bu şekilde oluşturuyoruz. 
        /// </summary>
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options):base(options) 
        {
            
        }

    }
}
