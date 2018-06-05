using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisDAL.Consts;
using SisDAL.Models;

namespace SisBatchApp
{
    class Program
    {
        public static Uri EsNode;
        public static ConnectionSettings EsConfig;
        public static ElasticClient EsClient;
        public static IndexState IndexConfig;

        static void Main(string[] args)
        {
            InitElastic();
            CreateIndexByConstName();
            InsertMockData();
        }

        public static void InitElastic()
        {
            try
            {
                EsNode = new Uri(LogInConstData.URL);
                EsConfig = new ConnectionSettings(EsNode).DisableDirectStreaming().PrettyJson()
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
                Console.WriteLine(ex.Message);
            }
        }

        public static bool CreateIndexByConstName()
        {
            try
            {
                if (!EsClient.IndexExists(LogInConstData.indexName).Exists)
                {
                    EsClient.CreateIndex(LogInConstData.indexName, c => c
                    .InitializeUsing(IndexConfig)
                    .Mappings(m => m.Map<MeasurmentsData>(mp => mp.AutoMap())));
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool InsertMockData()
        {
            MeasurmentsData md = new MeasurmentsData
            {
                BuzzerVal = 0,
                HudimityPrecent = 0,
                HudimityVal = 0,
                IdrVal = 0,
                Time = DateTime.Now,
                UserId = 12345678,
                UserName = "Sis Admin",
                WaterVal = 0,
            };
            try
            {
                var x = EsClient.Index(md, i => i.Index(LogInConstData.indexName));
                Console.WriteLine(x.DebugInformation);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
