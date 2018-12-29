using log4net;
using MCTech.OpenApi.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApiSample
{
    class Program
    {
        private const string accessId = "accessId";
        private const string secretKey = "secretKey";

        private const string baseUrl = "https://api.mctech.vip";

        private const string projectsApi = "/org-api/projects?start={0}&limit={1}";
        private const string pcrApi = "/external/project-construction-record?startId={0}&startVersion={1}&limit={2}&orgId={3}";
        private static ILog logger = LogManager.GetLogger("logger");
        private static string outputFile = "project-construction-record.txt";


        private static object lockObj = new object();
        private static int threadCount = 20;
        // 10线程
        private static SemaphoreSlim sem = new SemaphoreSlim(threadCount, threadCount);

        private static OpenApiClient client = new OpenApiClient(baseUrl, accessId, secretKey);

        private static int totalCount = 0;

        static void Main(string[] args)
        {
            File.Delete(outputFile);

            List<long> list = getProjects();
            foreach (long id in list)
            {
                sem.Wait();
                ThreadPool.QueueUserWorkItem((object state) =>
                {
                    try
                    {
                        logger.Info("start project: " + id);
                        GetPcr(id);
                    }
                    finally
                    {
                        sem.Release();
                    }
                });
            }

            // 等待所有线程结束
            while (sem.CurrentCount < threadCount)
            {
                Thread.Sleep(1000);
            }
        }

        private static void GetPcr(long orgId) {
            long startId = 0;
            long startVersion = 0;
            int pageSize = 50;
            while (true) 
            {
                 string apiUrl = String.Format(pcrApi, startId, startVersion, pageSize, orgId);
                 try
                 {
                     using (RequestResult result = client.Get(apiUrl))
                     {
                         // 调用result.GetJsonObject方法可以使用强类型的实体，也可以使用Newtonsoft.json.dll中的JArray类型
                         JArray arr = (JArray)result.GetJsonObject(typeof(JArray));
                         int count = arr.Count;
                         Interlocked.Add(ref totalCount, count);
                         WritePcrFile(arr);
                         logger.Info(String.Format("get {0} records on project {1}", totalCount, orgId));
                         if (count < pageSize)
                         {
                             break;
                         }
                         startId = arr[arr.Count - 1].Value<long>("id");
                         startVersion = arr[arr.Count - 1].Value<long>("version");
                     }
                 }
                 catch (Exception ex)
                 {
                     logger.Info(String.Format("call api error:  {0} ", ex.Message));
                     WebException webEx = ex as WebException;
                     if (webEx != null)  
                     {
                        using (HttpWebResponse resp = (HttpWebResponse)webEx.Response)
                        {
                            using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                            {
                                logger.Info(String.Format("response body: {0} ", reader.ReadToEnd()));
                            }
                        }
                     }
                 }
            }
        }

        private static void WritePcrFile(JToken token)
        {
            lock (lockObj)
            {
                using (StreamWriter writer = File.AppendText(outputFile))
                {
                    writer.WriteLine(token.ToString(Formatting.None));
                    writer.Flush();
                }
            }
        }

        private static List<long> getProjects()
        {
            List<long> list = new List<long>();
            long startId = 0;
            int pageSize = 200;
            while (true)
            {
                string apiUrl = String.Format(projectsApi, startId, pageSize);
                using (RequestResult result = client.Get(apiUrl))
                {
                    // 调用result.GetJsonObject方法可以使用强类型的实体，例如ProjectInfo，也可以使用Newtonsoft.json.dll中的JArray类型
                    ProjectInfo[] projects = (ProjectInfo[])result.GetJsonObject(typeof(ProjectInfo[]));
                    foreach (var proj in projects)
                    {
                        list.Add(proj.Id);
                    }
                    if (projects.Length < pageSize)
                    {
                        break;
                    }
                    startId = projects[projects.Length - 1].Version;
                }
            }
            return list;
        }

        
    }

    class ProjectInfo
    {
        public long Id { get; set; }
        public string address { get; set; }

        public long Version { get; set; }
    }
}
