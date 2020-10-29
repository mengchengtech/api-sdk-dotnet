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
        private const string ACCESS_ID = "YOUR_ACCESS_ID";
        private const string SECRET_KEY = "YOUR_SECRET_KEY";
        private const string BASE_URL = "https://api.mctech.vip";

        private static string outputFile = "result.txt";
        private static ILog logger = LogManager.GetLogger("logger");

        static void Main(string[] args)
        {
            OpenApiClient client = new OpenApiClient(BASE_URL, ACCESS_ID, SECRET_KEY);
            string url = "/v2/biz-data";
            int pageSize = 500;
            string body = "{{\"tableName\": \"projectRecordProgressQuantity\", \"offset\": {0}, \"limit\": {1} }}";
            int offset = 0;
            while (true)
            {
                try
                {
                    using (RequestResult result = client.Post(url, string.Format(body, offset, pageSize)))
                    {
                        JArray jsonResult = (JArray)result.GetJsonObject(typeof(JArray));
                        SaveResult(jsonResult);
                        logger.Info(string.Format("{0} records received", jsonResult.Count()));
                        offset += jsonResult.Count();
                        if (jsonResult.Count() < pageSize)
                        {
                            break;
                        }
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
                    break;
                }
            }
            logger.Info(string.Format("totally received {0} records, saved to {1}", offset, outputFile));
        }

        private static void SaveResult(JArray jsonResult)
        {
            using (StreamWriter writer = File.AppendText(outputFile))
            {
                writer.WriteLine(jsonResult.ToString());
                writer.Flush();
            }
        }
    }

}
