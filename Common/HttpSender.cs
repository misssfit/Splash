using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Splash.Common
{
    public class HttpSender
    {
        private static readonly CookieContainer CookieContainer;


        /// <summary>
        ///     Static Constructor used to add non default remote certificate validation callback to ServicePointManager
        /// </summary>
        static HttpSender()
        {
            CookieContainer = new CookieContainer();
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
        }

        /// <summary>
        ///     Method gets HttpWebResponse from HttpWebRequest
        /// </summary>
        /// <param name="request">HttpWebRequest should be built correctly before calling this method</param>
        /// <returns></returns>
        /// <exception cref="ProtocolViolationException">
        ///     Method is GET or HEAD, and either ContentLength is greater or equal to zero or SendChunked is true.
        ///     KeepAlive is true, AllowWriteStreamBuffering is false, ContentLength is -1, SendChunked is false, and Method is POST or PUT.
        ///     The HttpWebRequest has an entity body but the GetResponse method is called without calling the GetRequestStream method.
        ///     The ContentLength is greater than zero, but the application does not write all of the promised data.
        /// </exception>
        /// <exception cref="WebException">Timeout or aborted or an error ocurred</exception>
        /// <exception cref="InvalidOperationException">The stream is already in use by a previous call to BeginGetResponse.</exception>
        public HttpWebResponse Send(HttpWebRequest request)
        {
            request.CookieContainer = CookieContainer;
            HttpWebResponse response;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException e)
            {
                using (response = e.Response as HttpWebResponse)
                {
                    //string text = string.Empty;
                    //using (Stream data = response.GetResponseStream())
                    //{
                    //    text = new StreamReader(data).ReadToEnd();
                    //}
                    //Exception exception = new Exception(response.StatusCode, response.StatusDescription, text);
                    //throw e;
                }
            }
            return response;
        }

        /// <summary>
        ///     Metod creates HttpWebResponse object based on input parameters and AccessCredentials, it calls Send(HttpWebRequest)
        /// </summary>
        /// <param name="uri">SystemObject URL on UCI Server</param>
        /// <param name="method"></param>
        /// <param name="body">optional HttpWebRequest body message</param>
        /// <returns></returns>
        /// <exception cref="Exception">throws Send(HttpWebRequest request) exceptions</exception>
        public HttpWebResponse Send(Uri uri, string method, string body)
        {
            HttpWebResponse httpResponse = null;
            var request = (HttpWebRequest) WebRequest.Create(uri);

            //set http web request method
            request.Method = method;
            //set basic authentiaction credentials

            //set http web request body
            SetBody(body, request);

            //set content type
            request.ContentType = "application/xml";
            //set accept type
            request.Accept = "application/xml";

            httpResponse = Send(request);

            return httpResponse;
        }


        /// <summary>
        ///     Method sets optional body message to HttpWebRequest if body's length is greater than 0
        /// </summary>
        /// <param name="body">Body to set</param>
        /// <param name="request">HttpWebRequest that will be filled with body</param>
        protected void SetBody(string body, HttpWebRequest request)
        {
            if (body.Length > 0)
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    using (var writer = new StreamWriter(requestStream))
                    {
                        writer.Write(body);
                        writer.Close();
                    }
                    requestStream.Close();
                }
            }
        }


        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain,
                                                     SslPolicyErrors sslPolicyErrors)
        {
#if DEBUG
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                Debug.WriteLine("Certificate error: {0}", sslPolicyErrors);
            }
#endif
            // !! Allow this client to communicate with unauthenticated servers.
            //return false;
            return true;
        }
    }
}