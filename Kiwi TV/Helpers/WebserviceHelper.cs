using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Kiwi_TV.Helpers
{
    /// <summary>
    /// A helper to interact with various webservices
    /// </summary>
    class WebserviceHelper
    {
        /* Makes a request to a JSON webservice */
        public async static Task<object> MakeJSONRequest(string requestUrl, Type responseType)
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

        /* Makes a request to a XML webservice */
        public async static Task<object> MakeXMLRequest(string requestUrl, Type responseType)
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
                    XmlSerializer xmlSerializer = new XmlSerializer(responseType);
                    object objResponse = xmlSerializer.Deserialize(response.GetResponseStream());

                    return objResponse;
                }
            }
            catch
            {
                return null;
            }
        }

        /* Pings a given URL to see if it responds */
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
