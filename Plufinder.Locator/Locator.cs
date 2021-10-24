using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MongoDB.Driver;
using MongoDB.Bson;
using Plufinder.Locator.Models;

namespace Plufinder.Locator
{
    public static class Locator
    {
        [FunctionName("PlufinderLocator")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Inicializando Azure Function . . .");
            int usuario = int.Parse(req.Query["usuario"]);
            int localizacao = int.Parse(req.Query["localizacao"]);
            int cargo = int.Parse(req.Query["cargo"]);

            log.LogInformation("Realizando conexão com o MongoDB . . .");
            var client = new MongoClient("mongodb+srv://plufinderatlas:fiap123@plufindertracker.l6ath.mongodb.net/Plufinder?connect=replicaSet");
            var session = client.StartSession();
            var database = client.GetDatabase("Plufinder");
            var collection = database.GetCollection<UsuarioLocalizacao>("UserLocation");
            UsuarioLocalizacao usuarioLocalizado = new UsuarioLocalizacao();

            try
            {
                log.LogInformation("Gerando os filtros para busca . . .");
                var filterCargo = new FilterDefinitionBuilder<UsuarioLocalizacao>().Where(u => u.IdCargo == cargo);
                var filterUsuario = new FilterDefinitionBuilder<UsuarioLocalizacao>().Where(u => u.IdUsuario == usuario);
                var filterLocalizacao = new FilterDefinitionBuilder<UsuarioLocalizacao>().Where(u => u.IdLocalizacao == localizacao);

                if (usuario != default)
                {
                    log.LogInformation("Realizando busca por usuário . . .");
                    usuarioLocalizado = await collection.Find(filterUsuario).FirstOrDefaultAsync();
                }
                else if (cargo != default)
                {
                    log.LogInformation("Realizando busca por usuário . . .");
                    usuarioLocalizado = await collection.Find(filterCargo).FirstOrDefaultAsync();
                }
                else if (localizacao != default)
                {
                    log.LogInformation("Realizando busca por localização . . .");
                    usuarioLocalizado = await collection.Find(filterLocalizacao).FirstOrDefaultAsync();
                }
            }
            catch (Exception e)
            {
                session.AbortTransaction();
                return new OkObjectResult("Erro: " + e.Message);
                throw;
            } finally
            {
                if (usuarioLocalizado == null) log.LogInformation("Nenhum usuário encontrado.");
                else log.LogInformation("Retornando usuário localizado . . .");
            }

            log.LogInformation("C# HTTP trigger function processed a request.");

            string responseMessage = usuarioLocalizado == null ? "Nenhum usuário localizado." : $"Usuário localizado: {usuarioLocalizado.IdUsuario}";
            return new JsonResult(usuarioLocalizado);
            return new OkObjectResult(responseMessage);
        }
    }
}
