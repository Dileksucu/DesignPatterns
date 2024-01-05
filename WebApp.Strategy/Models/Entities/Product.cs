using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Strategy.Models.Entities
{
    /// <summary>
    ///Bu product hem MongoDb tarafında hem de Sql tarafına karşılık gelecek tablodur.
    /// </summary>
    public class Product
    {
        //[Key] //Sql için de Pk alan belirleme Attributte'ü. (bunu vermesek de algılar sistem)
        [BsonId] //MongoDb de kullanılan bir attribute'dür. Pk alanı olarak belirlemede kullanılır.
        //[BsonRepresentation(BsonType.ObjectId)] //Mongo Db de string değeri bir key olarak temsil edileceği için bunun type belirlenmeli. Biz bu alanın mongo db de objectıd tipine dönüştürülmesini istiyoruz.
        public string? Id { get; set; }
        public string? Name { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public string? UserId { get; set; }
        //Ürünlerin hangi kullanıcıya ait olacağını tutuyoruz burada
        public DateTime CreatedDate { get; set; }
    }
}
