using Nest;
using SisDAL.Consts;
using SisDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SisWebApi.Controllers
{
    public class MeasurmentsController : ApiController
    {
        public static Uri EsNode;
        public static ConnectionSettings EsConfig;
        public static ElasticClient EsClient;
        public static IndexState IndexConfig;

        private static void InitElastic()
        {
            try
            {
                EsNode = new Uri(LogInConstData.URL);
                EsConfig = new ConnectionSettings(EsNode).DefaultIndex(LogInConstData.indexName).DisableDirectStreaming().PrettyJson()
                .BasicAuthentication(LogInConstData.UserName, LogInConstData.Password);

                EsClient = new ElasticClient(EsConfig);
                var settings = new IndexSettings { NumberOfReplicas = 1, NumberOfShards = 2 };

                IndexConfig = new IndexState
                {
                    Settings = settings
                };
            }
            catch (Exception ex)
            {
                
            }
        }

        // GET: api/Measurments
        public IEnumerable<MeasurmentsData> Get()
        {
            if (EsNode == null)
            {
                InitElastic();
            }
            var searchResponse = EsClient.Search<MeasurmentsData>(s => s
                .Size(10000)
                .MatchAll(x => x).Sort(y => y.Descending(z => z.Time))
                );
            if (searchResponse.Documents.Count > 0)
            {
                return searchResponse.Documents.ToList();
            }
            else
            {
                return null;
            }
        }


        // GET: api/Measurments/1
        public IHttpActionResult Get([FromUri]string id)
        {
            if (EsNode == null)
            {
                InitElastic();
            }
            var searchResponse = EsClient.Search<MeasurmentsData>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.UserId)
                        .Query(id)
                    )
                ).Size(10000).Sort(x => x.Descending(f => f.Time))
            );
            if (searchResponse.Documents.Count > 0)
            {
                return Ok(searchResponse.Documents.ToList());
            }
            else
            {
                return NotFound();
            }
        }

        

        // POST: api/Measurments
        public IHttpActionResult Post([FromBody]MeasurmentsData md)
        {
            if (EsNode == null)
            {
                InitElastic();
            }
            try
            {
                var x = EsClient.Index(md, i => i.Index(LogInConstData.indexName));
                return Content(HttpStatusCode.OK, x);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.ExpectationFailed, ex);
            }
        }
    }
}
