using WebApp.Strategy.Models.Enum;

namespace WebApp.Strategy.Models.Settings
{
    public class Settings
    {
        //Bu  string değere çoğu yerde ihtiyacım olduğu için const ile static olarak bu şekilde tutuyorum. 
        public static string claimDatabaseType = "databasetype";

        public DatabaseType DatabaseType;
        /// <summary>
        ///Default olarak , hiçbir seçenek şeçilmediği zaman SqlServer olarak tabloya eklenir.
        ///Burada => işareti, özellikle get erişimcileri, metotlar veya indexer'lar gibi tek bir ifade içeren üye tanımlamalarında kullanılır.
        ///</summary>>
        public DatabaseType GetDefaultDatabaseType => DatabaseType.SqlServer;
        //Bunun yerine yukarıdaki gibi bir tanımlama yapabilir miyim?
    }
}
