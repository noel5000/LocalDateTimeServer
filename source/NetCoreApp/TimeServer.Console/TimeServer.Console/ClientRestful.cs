using System;
using System.Collections.Generic;
using System.Text;

namespace TimeServer.Console
{
    public enum HttpVerb
    {
        GET,
        POST,
        PUT,
        DELETE,
        PATCH
    }

    public enum Authorization
    {
        NONE = 0,
        BASIC = 1
    }
    public class ClientRestFul
    {
        public string EndPoint { get; set; }
        public HttpVerb Method { get; set; }
        public Authorization Authorization { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public ClientRestFul()
        {
            EndPoint = "";
            Method = HttpVerb.GET;
            ContentType = "application/xml";
            PostData = "";
            this.Authorization = Authorization.NONE;
        }
        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public ClientRestFul(string endpoint)
        {
            EndPoint = endpoint;
            Method = HttpVerb.GET;
            ContentType = "application/xml";
            PostData = "";
            this.Authorization = Authorization.NONE;
        }

        public ClientRestFul(string endpoint, HttpVerb method)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/xml";
            PostData = "";
            this.Authorization = Authorization.NONE;
        }

        public ClientRestFul(string endpoint, HttpVerb method, string postData)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/xml";
            PostData = postData;
            this.Authorization = Authorization.NONE;
        }
        public ClientRestFul(string endpoint, HttpVerb method, Authorization authorization, string username, string password)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/xml";
            this.Authorization = authorization;
            this.Username = username;
            this.Password = password;
        }
        public ClientRestFul(string endpoint, HttpVerb method, string postData, Authorization authorization, string username, string password)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/xml";
            PostData = postData;
            this.Authorization = authorization;
            this.Username = username;
            this.Password = password;
        }

        public string MakeRequest()
        {
            return MakeRequest("");
        }

        public string MakeRequest(string parameters)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
            var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);
            request.Method = Method.ToString();
            request.ContentLength = 0;
            request.ContentType = ContentType;
            switch (this.Authorization)
            {
                case Authorization.BASIC:
                    //  request.Headers.Add($"authorization: basic MjM6YWlsYTIz");
                    request.Headers.Add($"authorization: basic {StringUtility.ConvertToBase64($"{this.Username}:{this.Password}")}");
                    break;
            }


            if (!string.IsNullOrEmpty(PostData) && (Method == HttpVerb.POST || Method == HttpVerb.PUT || Method == HttpVerb.PATCH))
            {
                var encoding = new UTF8Encoding();
                var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PostData);
                request.ContentLength = bytes.Length;

                using (var writeStream = request.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }

            //using (var response = (HttpWebResponse)request.GetResponse())
            //{
            var responseValue = string.Empty;

            /*
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                throw new ApplicationException(message);
            }
            */
            // grab the response
            if (response != null)
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                }
            return responseValue;
            //}
        }

    }

    public static class StringUtility
    {
        public static string ConvertToBase64(string original)
        {
            byte[] byt = System.Text.Encoding.UTF8.GetBytes(original);

            // convert the byte array to a Base64 string

            return Convert.ToBase64String(byt);
        }

        public static string ConvertFromBase64(string original)
        {
            byte[] byt = Convert.FromBase64String(original);

            // convert the byte array to a Base64 string

            return Encoding.UTF8.GetString(byt);
        }

        public static string Serialize<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(T));
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, value, ns);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        public static string SerializeJson<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                return JsonConvert.SerializeObject(value);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }
    }
}
