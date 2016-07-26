using System;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace Kiwi_TV.Helpers
{
    class WebserviceHelper
    {
        public async static Task<object> MakeRequest(string requestUrl, Type responseType)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                    }
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(responseType);
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    
                    return objResponse;
                }
            }
            catch
            {
                return null;
            }
        }

        public async static Task<bool> Ping(Uri requestUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                
                using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
