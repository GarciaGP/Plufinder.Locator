using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Plufinder.Locator.Models
{
    class UsuarioLocalizacao
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("id_usuario")]
        public int IdUsuario { get; set; }

        [BsonElement("id_localizacao")]
        public int IdLocalizacao { get; set; }
    }
}
