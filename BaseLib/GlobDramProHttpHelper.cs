using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Globussoft
{
    public class GlobDramProHttpHelper
    {

        CookieCollection gCookies;
        HttpWebRequest gRequest;
        HttpWebResponse gResponse;

        string proxyAddress = string.Empty;
        int port = 80;
        string proxyUsername = string.Empty;
        string proxyPassword = string.Empty;

        string UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:32.0) Gecko/20100101 Firefox/32.0";

        public Uri GetResponseData()
        {
            return gResponse.ResponseUri;
        }
        public string getHtmlfromUrl1(Uri url, string referer)
        {
            string responseString = string.Empty;
            try
            {
                //setExpect100Continue();
                gRequest = (HttpWebRequest)WebRequest.Create(url);
                gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.115 Safari/537.36";
                gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                //gRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
                gRequest.Headers["Accept-Language"] = "en-US,en;q=0.8";
                if (!string.IsNullOrEmpty(referer))
                {
                    gRequest.Referer = referer;
                }
                gRequest.KeepAlive = true;

                gRequest.AllowAutoRedirect = true;

                gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;

                gRequest.Method = "GET";

                #region CookieManagment

                if (this.gCookies != null && this.gCookies.Count > 0)
                {
                    setExpect100Continue();
                    gRequest.CookieContainer.Add(gCookies);

                }

                if (this.gCookies == null)
                {
                    this.gCookies = new CookieCollection();
                }



                //Get Response for this request url

                setExpect100Continue();
                gResponse = (HttpWebResponse)gRequest.GetResponse();

                //check if the status code is http 200 or http ok
                if (gResponse.StatusCode == HttpStatusCode.OK)
                {
                    //get all the cookies from the current request and add them to the response object cookies
                    setExpect100Continue();
                    gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);


                    //check if response object has any cookies or not
                    if (gResponse.Cookies.Count > 0)
                    {
                        //check if this is the first request/response, if this is the response of first request gCookies
                        //will be null
                        if (this.gCookies == null)
                        {
                            gCookies = gResponse.Cookies;
                        }
                        else
                        {
                            foreach (Cookie oRespCookie in gResponse.Cookies)
                            {
                                bool bMatch = false;
                                foreach (Cookie oReqCookie in this.gCookies)
                                {
                                    if (oReqCookie.Name == oRespCookie.Name)
                                    {
                                        oReqCookie.Value = oRespCookie.Value;
                                        bMatch = true;
                                        break; // 
                                    }
                                }
                                if (!bMatch)
                                    this.gCookies.Add(oRespCookie);
                            }
                        }
                    }
                #endregion

                    string responseURI = gResponse.ResponseUri.AbsoluteUri;

                    StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                    responseString = reader.ReadToEnd();
                    reader.Close();
                    return responseString;
                }
                else
                {
                    return "Error";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);

                if (ex.Message.Contains("The remote server returned an error: (999)"))
                {
                    // GlobusLogHelper.log.Info("Yahoo trace your IP ,Plz change your proxy IP");
                    responseString = "The remote server returned an error: (999)";
                }
            }
            return responseString;
        }

        public string getHtmlfromUrl(Uri url)
        {
            string responseString = string.Empty;
            try
            {
                //setExpect100Continue();
                gRequest = (HttpWebRequest)WebRequest.Create(url);
                gRequest.UserAgent = "User-Agent: Mozilla/5.0 (Windows NT 6.1; rv:27.0) Gecko/20100101 Firefox/27.0";
                gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                gRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
                gRequest.Headers["Accept-Language"] = "en-us,en;q=0.5";

                gRequest.KeepAlive = true;

                gRequest.AllowAutoRedirect = true;

                gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;

                gRequest.Method = "GET";

                #region CookieManagment

                if (this.gCookies != null && this.gCookies.Count > 0)
                {
                    setExpect100Continue();
                    gRequest.CookieContainer.Add(gCookies);

                }

                if (this.gCookies == null)
                {
                    this.gCookies = new CookieCollection();
                }



                //Get Response for this request url

                setExpect100Continue();
                gResponse = (HttpWebResponse)gRequest.GetResponse();

                //check if the status code is http 200 or http ok
                if (gResponse.StatusCode == HttpStatusCode.OK)
                {
                    //get all the cookies from the current request and add them to the response object cookies
                    setExpect100Continue();
                    gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);


                    //check if response object has any cookies or not
                    if (gResponse.Cookies.Count > 0)
                    {
                        //check if this is the first request/response, if this is the response of first request gCookies
                        //will be null
                        if (this.gCookies == null)
                        {
                            gCookies = gResponse.Cookies;
                        }
                        else
                        {
                            foreach (Cookie oRespCookie in gResponse.Cookies)
                            {
                                bool bMatch = false;
                                foreach (Cookie oReqCookie in this.gCookies)
                                {
                                    if (oReqCookie.Name == oRespCookie.Name)
                                    {
                                        oReqCookie.Value = oRespCookie.Value;
                                        bMatch = true;
                                        break; // 
                                    }
                                }
                                if (!bMatch)
                                    this.gCookies.Add(oRespCookie);
                            }
                        }
                    }
                #endregion

                    string responseURI = gResponse.ResponseUri.AbsoluteUri;

                    StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                    responseString = reader.ReadToEnd();
                    reader.Close();
                    return responseString;
                }
                else
                {
                    return "Error";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);

                if (ex.Message.Contains("The remote server returned an error: (999)"))
                {
                    // GlobusLogHelper.log.Info("Yahoo trace your IP ,Plz change your proxy IP");
                    responseString = "The remote server returned an error: (999)";
                }
            }
            return responseString;
        }


        public string getHtmlfromUrl(Uri url, string Referes, string Token, string proxy)
        {
            if (string.IsNullOrEmpty(proxyAddress))
            {
                proxyAddress = proxy;
            }
            setExpect100Continue();
            gRequest = (HttpWebRequest)WebRequest.Create(url);

            //gRequest.ServicePoint.Expect100Continue = false;

            gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2062.124 Safari/537.36"; //"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.24) Gecko/20111103 Firefox/3.6.24";
            gRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            gRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
            //gRequest.Headers["Cache-Control"] = "max-age=0";
            gRequest.Headers["Accept-Language"] = "en-US,en;q=0.8";
            gRequest.Headers["Content-Type"] = "application/x-www-form-urlencoded; charset=UTF-8";
            gRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            gRequest.Headers.Add("Origin", "http://websta.me");
            gRequest.Host = "http://websta.me";
            
           
            //gRequest.Connection = "keep-alive";

            gRequest.KeepAlive = true;
            
            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;

            gRequest.Method = "GET";
            //gRequest.AllowAutoRedirect = false;
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            if (!string.IsNullOrEmpty(Referes))
            {
                gRequest.Referer = Referes;
            }
            if (!string.IsNullOrEmpty(Token))
            {
                //gRequest.Headers.Add("X-CSRFToken", Token);
                //gRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");

                gRequest.Headers.Add("Origin", Token);
               // gRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            }



            #region CookieManagment

            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);

            }

            if (this.gCookies == null)
            {
                this.gCookies = new CookieCollection();
            }



            //Get Response for this request url

            setExpect100Continue();
            gResponse = (HttpWebResponse)gRequest.GetResponse();

            //check if the status code is http 200 or http ok
            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);


                //check if response object has any cookies or not
                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion

                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                return responseString;
            }
            else
            {
                return "Error";
            }

        }

        public string getHtmlfromUrl(Uri url,string Referes, NameValueCollection nameValueCollection)
        {
            setExpect100Continue();
            gRequest = (HttpWebRequest)WebRequest.Create(url);

            //gRequest.ServicePoint.Expect100Continue = false;
            gRequest.UserAgent = UserAgent;
            gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            gRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
            //gRequest.Headers["Cache-Control"] = "max-age=0";
            gRequest.Headers["Accept-Language"] = "en-us,en;q=0.5";
            //gRequest.Connection = "keep-alive";

            gRequest.KeepAlive = true;

            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;

            gRequest.Method = "GET";
            //gRequest.AllowAutoRedirect = false;
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            if (!string.IsNullOrEmpty(Referes))
            {
                gRequest.Referer = Referes;
            }

            if (nameValueCollection != null)
            {
                foreach (string item in nameValueCollection.Keys)
                {
                    gRequest.Headers.Add(item, nameValueCollection[item]);
                }
            }




            #region CookieManagment

            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);

                try
                {
                    //gRequest.CookieContainer.Add(url, new Cookie("__qca", "P0 - 2078004405 - 1321685323158", "/"));
                    //gRequest.CookieContainer.Add(url, new Cookie("__utma", "101828306.1814567160.1321685324.1322116799.1322206824.9", "/"));
                    //gRequest.CookieContainer.Add(url, new Cookie("__utmz", "101828306.1321685324.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)", "/"));
                    //gRequest.CookieContainer.Add(url, new Cookie("__utmb", "101828306.2.10.1321858563", "/"));
                    //gRequest.CookieContainer.Add(url, new Cookie("__utmc", "101828306", "/"));
                }
                catch
                {

                }
            }
            //Get Response for this request url

            setExpect100Continue();
            string html;
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (WebException ex)
            {
               Console.WriteLine(ex.Message);
                ////Logger.LogText("Response from " + url + ":" + ex.Message, null);
            }

            //check if the status code is http 200 or http ok
            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);


                //check if response object has any cookies or not
                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion

                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                return responseString;
            }
            else
            {
                return "Error";
            }

        }

        public string postFormData(Uri url, string postData, string Referes, NameValueCollection nameValueCollection)
        {

            gRequest = (HttpWebRequest)WebRequest.Create(url);

            //gRequest.ServicePoint.Expect100Continue = false;

            gRequest.UserAgent = UserAgent;
            gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            gRequest.Method = "POST";
            gRequest.KeepAlive = true;
            gRequest.ContentType = @"text/html; charset=utf-8";
            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            if (!string.IsNullOrEmpty(Referes))
            {
                gRequest.Referer = Referes;
            }

            try
            {
                if (nameValueCollection != null)
                {
                    foreach (string item in nameValueCollection.Keys)
                    {
                        gRequest.Headers.Add(item, nameValueCollection[item]);
                    }
                }
            }
            catch (Exception)
            {
            }
            
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // //Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends

            //Get Response for this request url

            string html;

            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (WebException ex)
            {
               Console.WriteLine(ex.Message);
               // //Logger.LogText("Response from " + url + ":" + ex.Message, null);
            }



            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion


                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }

        }

       
        
      

        public string getHtmlfromUrlProxy(Uri url, string proxyAddress, int port, string proxyUsername1, string proxyPassword1)
        {

            setExpect100Continue();
            gRequest = (HttpWebRequest)WebRequest.Create(url);

            //gRequest.ServicePoint.Expect100Continue = false;

            gRequest.UserAgent = UserAgent;
            gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            gRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
            gRequest.Headers["Accept-Language"] = "en-us,en;q=0.5";
           // gRequest.Headers.Add("Origin", "http://websta.me");//http://websta.me/?t=lo
            gRequest.Headers.Add("Origin", "http://websta.me/?t=lo");
            

            gRequest.KeepAlive = true;

            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            /////Set Proxy
            //this.proxyAddress = proxyAddress;
            //this.port = port;
            //this.proxyUsername = proxyUsername1;
            //this.proxyPassword = proxyPassword1;
            //this.proxyUsername = proxyUsername1;
            //this.proxyPassword = proxyPassword1;

            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;

            gRequest.Method = "GET";
            //gRequest.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            #region CookieManagment

            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);

                //try
                //{
                //    gRequest.CookieContainer.Add(url, new Cookie("__qca", "P0-2078004405-1321685323158", "/"));
                //    gRequest.CookieContainer.Add(url, new Cookie("__utma", "101828306.1814567160.1321685324.1321697619.1321858563.3", "/"));
                //    gRequest.CookieContainer.Add(url, new Cookie("__utmz", "101828306.1321685324.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)", "/"));
                //    gRequest.CookieContainer.Add(url, new Cookie("__utmb", "101828306.2.10.1321858563", "/"));
                //    gRequest.CookieContainer.Add(url, new Cookie("__utmc", "101828306", "/"));
                //}
                //catch (Exception ex)
                //{

                //}
            }
            //Get Response for this request url

            setExpect100Continue();
            string html;
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (WebException ex)
            {
               Console.WriteLine(ex.Message);
               // //Logger.LogText("Response from " + url + ":" + ex.Message, null);
            }

            //check if the status code is http 200 or http ok
            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);


                //check if response object has any cookies or not
                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion

                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                return responseString;
            }
            else
            {
                return "Error";
            }

        }

        public string getHtmlfromAsx(Uri url)
        {
            setExpect100Continue();
            gRequest = (HttpWebRequest)WebRequest.Create(url);
            //gRequest.ServicePoint.Expect100Continue = false;


            gRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.0.4) Gecko/2008102920 Firefox/3.0.4";
            gRequest.CookieContainer = new CookieContainer() ;//new CookieContainer();
            gRequest.ContentType = "video/x-ms-asf";

            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                gRequest.CookieContainer.Add(gCookies);
                setExpect100Continue();
            }
            //Get Response for this request url
            string html;
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (WebException ex)
            {
               Console.WriteLine(ex.Message);
               // Logger.LogText("Response from " + url + ":" + ex.Message, null);
            }

            setExpect100Continue();
            //check if the status code is http 200 or http ok
            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                setExpect100Continue();
                //check if response object has any cookies or not
                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }

                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error";
            }
        }


        public string HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc, bool IsLocalFile)
        {

            ////log.Debug(string.Format("Uploading {0} to {1}", file, url));
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString();
            //string boundary = "---------------------------" + DateTime.Now.Ticks.ToString();
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            gRequest = (HttpWebRequest)WebRequest.Create(url);

            //gRequest.ServicePoint.Expect100Continue = false;

            gRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            gRequest.Method = "POST";
            gRequest.KeepAlive = true;
            gRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;

            //ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;

            #region CookieManagment

            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                gRequest.CookieContainer.Add(gCookies);
            }
            #endregion

            Stream rs = gRequest.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

          

            if (IsLocalFile)
            {
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, paramName, file, contentType);
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);

                FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();
            }

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            #region CookieManagment

            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                gRequest.CookieContainer.Add(gCookies);
            }

            #endregion

            WebResponse wresp = null;
            try
            {
                //wresp = gRequest.GetResponse();
                gResponse = (HttpWebResponse)gRequest.GetResponse();
                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                //reader.Close();
                return responseString;

            }
            catch (Exception ex)
            {
                //log.Error("Error uploading file", ex);
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
                return null;
            }
            finally
            {
                gRequest = null;
            }
            //}

        }

        public void MultiPartImageUpload(string Username, string Password, string localImagePath)
        {
            /////Login to FB

            //string valueLSD = "name=" + "\"lsd\"";
            //string pageSource = getHtmlfromUrl(new Uri("https://www.facebook.com/login.php"));
            ////string pageSource = getHtmlfromUrlProxy(new Uri("https://www.facebook.com/login.php"));
            //int startIndex = pageSource.IndexOf(valueLSD) + 18;
            //string value = pageSource.Substring(startIndex, 5);

            //string ResponseLogin = postFormData(new Uri("https://www.facebook.com/login.php?login_attempt=1"), "charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + value + "&locale=en_US&email=" + Username.Split('@')[0] + "%40" + Username.Split('@')[1] + "&pass=" + Password + "&persistent=1&default_persistent=1&charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + value + "");


            //string profileSource = getHtmlfromUrl(new Uri("http://www.facebook.com/editprofile.php?sk=basic"));

            //string valuepost_form_id = "name=" + "\"post_form_id\"";
            //string valuefb_dtsg = "name=" + "\"fb_dtsg\"";
            /////Setting Post Data Params...
            //int sIndex = profileSource.IndexOf(valuepost_form_id) + 27;
            //string post_form_id = profileSource.Substring(sIndex, 32);

            //int s1Index = profileSource.IndexOf(valuefb_dtsg) + 22;
            //string fb_dtsg = profileSource.Substring(s1Index, 8);

            //int s2Index = profileSource.IndexOf("user") + 5;
            //string userId = profileSource.Substring(s2Index, 15);

            //if (ResponseLogin.Contains("http://www.facebook.com/profile.php?id="))
            //{
            //    string[] arrUser = System.Text.RegularExpressions.Regex.Split(ResponseLogin, "href");
            //    foreach (String itemUser in arrUser)
            //    {
            //        if (!itemUser.Contains("<!DOCTYPE"))
            //        {
            //            if (itemUser.Contains("http://www.facebook.com/profile.php?id="))
            //            {

            //                string[] arrhre = itemUser.Split('"');
            //                userId = arrhre[1].Replace("http://www.facebook.com/profile.php?id=", "");
            //                if (userId.Contains("&"))
            //                {
            //                    string[] Arr = userId.Split('&');
            //                    userId = Arr[0];
            //                }

            //                break;

            //            }
            //        }
            //    }
            //}

            //NameValueCollection nvc = new NameValueCollection();
            //nvc.Add("post_form_id", post_form_id);
            //nvc.Add("fb_dtsg", fb_dtsg);
            //nvc.Add("id", userId);
            //nvc.Add("type", "profile");
            //nvc.Add("return", "/ajax/profile/picture/upload_iframe.php?pic_type=1&id=" + userId);

            //UploadFilesToRemoteUrl("http://upload.facebook.com/pic_upload.php ", new string[] { @"C:\Users\Globus-n2\Desktop\Windows Photo Viewer Wallpaper.jpg" }, "", nvc);
            //HttpUploadFile("http://upload.facebook.com/pic_upload.php ", localImagePath, "file", "image/jpeg", nvc, proxyAddress, );
        }

        public bool MultiPartImageUpload(string Username, string Password, string localImagePath, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword)
        {
            /////Login to FB

            //string valueLSD = "name=" + "\"lsd\"";

            //int intProxyPort = 80;

            ////Regex IdCheck = new Regex("^[0-9]*$");

            ////if (!string.IsNullOrEmpty(proxyPort) && IdCheck.IsMatch(proxyPort))
            ////{
            ////    intProxyPort = int.Parse(proxyPort);
            ////}

            //string pageSource = getHtmlfromUrl(new Uri("https://www.facebook.com/login.php"),"","");
            //int startIndex = pageSource.IndexOf(valueLSD) + 18;
            //string value = pageSource.Substring(startIndex, 5);

            //string ResponseLogin = postFormData(new Uri("https://www.facebook.com/login.php?login_attempt=1"), "charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + value + "&locale=en_US&email=" + Username.Split('@')[0] + "%40" + Username.Split('@')[1] + "&pass=" + Password + "&persistent=1&default_persistent=1&charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + value + "","","");


            //string profileSource = getHtmlfromUrl(new Uri("http://www.facebook.com/editprofile.php?sk=basic"),"","");

            //string valuepost_form_id = "name=" + "\"post_form_id\"";
            //string valuefb_dtsg = "name=" + "\"fb_dtsg\"";
            /////Setting Post Data Params...
            //int sIndex = profileSource.IndexOf(valuepost_form_id) + 27;
            //string post_form_id = profileSource.Substring(sIndex, 32);

            //int s1Index = profileSource.IndexOf(valuefb_dtsg) + 22;
            //string fb_dtsg = profileSource.Substring(s1Index, 8);

            //int s2Index = profileSource.IndexOf("user") + 5;
            //string userId = profileSource.Substring(s2Index, 15);

            //if (ResponseLogin.Contains("http://www.facebook.com/profile.php?id="))
            //{
            //    string[] arrUser = System.Text.RegularExpressions.Regex.Split(ResponseLogin, "href");
            //    foreach (String itemUser in arrUser)
            //    {
            //        if (!itemUser.Contains("<!DOCTYPE"))
            //        {
            //            if (itemUser.Contains("http://www.facebook.com/profile.php?id="))
            //            {

            //                string[] arrhre = itemUser.Split('"');
            //                userId = arrhre[1].Replace("http://www.facebook.com/profile.php?id=", "");
            //                if (userId.Contains("&"))
            //                {
            //                    string[] arr = userId.Split('&');
            //                    userId = arr[0];
            //                }
           
            //                break;

            //            }
            //        }
            //    }
            //}

            //NameValueCollection nvc = new NameValueCollection();
            //nvc.Add("post_form_id", post_form_id);
            //nvc.Add("fb_dtsg", fb_dtsg);
            //nvc.Add("id", userId);
            //nvc.Add("type", "profile");
            //nvc.Add("return", "/ajax/profile/picture/upload_iframe.php?pic_type=1&id=" + userId);

            ////UploadFilesToRemoteUrl("http://upload.facebook.com/pic_upload.php ", new string[] { @"C:\Users\Globus-n2\Desktop\Windows Photo Viewer Wallpaper.jpg" }, "", nvc);
            ////HttpUploadFile("http://upload.facebook.com/pic_upload.php ", localImagePath, "file", "image/jpeg", nvc);
            //if (HttpUploadFile("http://upload.facebook.com/pic_upload.php ", localImagePath, "file", "image/jpeg", nvc, proxyAddress, intProxyPort, proxyUsername, proxyPassword))
            //{
            //    return true;
            //}
            return false;
            
        }

        public string postFormData(Uri url, string postData, string Referes,string Token)
        {
           
            gRequest = (HttpWebRequest)WebRequest.Create(url);

            //gRequest.ServicePoint.Expect100Continue = false;

           gRequest.UserAgent = UserAgent;
           
            gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            gRequest.Method = "POST";
            gRequest.Accept = " text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";
            gRequest.KeepAlive = true;
            gRequest.ContentType = @"application/x-www-form-urlencoded";
           


           
           

            if (!string.IsNullOrEmpty(Referes))
            {
                gRequest.Referer = Referes;
            }
            if (!string.IsNullOrEmpty(Token))
            {
                gRequest.Headers.Add("Origin", Token);
                gRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            }
            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
               // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends

            //Get Response for this request url
            string html;
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (WebException ex)
            {
               Console.WriteLine(ex.Message);
                //Logger.LogText("Response from " + url + ":" + ex.Message, null);
            }



            //check if the status code is http 200 or http ok
          
                if (gResponse.StatusCode == HttpStatusCode.OK)
                {
                    //get all the cookies from the current request and add them to the response object cookies
                    setExpect100Continue();
                    gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                    //check if response object has any cookies or not
                   //Added by sandeep pathak
                    //gCookiesContainer = gRequest.CookieContainer;  
                    
                    if (gResponse.Cookies.Count > 0)
                    {
                        //check if this is the first request/response, if this is the response of first request gCookies
                        //will be null
                        if (this.gCookies == null)
                        {
                            gCookies = gResponse.Cookies;
                        }
                        else
                        {
                            foreach (Cookie oRespCookie in gResponse.Cookies)
                            {
                                bool bMatch = false;
                                foreach (Cookie oReqCookie in this.gCookies)
                                {
                                    if (oReqCookie.Name == oRespCookie.Name)
                                    {
                                        oReqCookie.Value = oRespCookie.Value;
                                        bMatch = true;
                                        break; // 
                                    }
                                }
                                if (!bMatch)
                                    this.gCookies.Add(oRespCookie);
                            }
                        }
                    }
            #endregion


                    StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                    string responseString = reader.ReadToEnd();
                    reader.Close();
                    //Console.Write("Response String:" + responseString);
                    return responseString;
                }
                else
                {
                    return "Error in posting data";
                } 
            
        }

        public string postFormDataProxy(Uri url, string postData, string proxyAddress, int port, string proxyUsername, string proxyPassword)
        {

            gRequest = (HttpWebRequest)WebRequest.Create(url);

            //gRequest.ServicePoint.Expect100Continue = false;


            gRequest.UserAgent = "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.16) Gecko/20110319 Firefox/3.6.16";

            gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            gRequest.Method = "POST";
            gRequest.Accept = " text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";
            gRequest.KeepAlive = true;
            gRequest.ContentType = @"application/x-www-form-urlencoded";

            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends

            //Get Response for this request url
            string html;
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (WebException ex)
            {
               Console.WriteLine(ex.Message);
                //Logger.LogText("Response from " + url + ":" + ex.Message, null);
            }



            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion

                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }

        }

        public string postFormDataRefererProxy(Uri url, string postData, string referer, string proxyAddress, int port, string proxyUsername, string proxyPassword)
        {

            gRequest = (HttpWebRequest)WebRequest.Create(url);

            //gRequest.ServicePoint.Expect100Continue = false;


            //gRequest.UserAgent = "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.16) Gecko/20110319 Firefox/3.6.16";
            gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.111 Safari/537.36";
   
            gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            gRequest.Method = "POST";
            gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            gRequest.KeepAlive = true;
            //gRequest.ContentType = @"application/x-www-form-urlencoded";
            if (!string.IsNullOrEmpty(referer))
            {
                gRequest.Referer = referer;
            }

            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends

            //Get Response for this request url
            string html;
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                //Logger.LogText("Response from " + url + ":" + ex.Message, null);
            }



            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion

                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }

        }

        public void setExpect100Continue()
        {
            if (ServicePointManager.Expect100Continue==true)
            {
                ServicePointManager.Expect100Continue = false; 
            }
        }

        public void setExpect100ContinueToTrue()
        {
            if (ServicePointManager.Expect100Continue==false)
            {
                ServicePointManager.Expect100Continue = true; 
            }
        }

        public void ChangeProxy(string proxyAddress, int port, string proxyUsername, string proxyPassword)
        {
            try
            {
                WebProxy myproxy = new WebProxy(proxyAddress, port);
                myproxy.BypassProxyOnLocal = false;

                if (!string.IsNullOrEmpty(proxyUsername) && !string.IsNullOrEmpty(proxyPassword))
                {
                    myproxy.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
                }
                gRequest.Proxy = myproxy;
            }
            catch (Exception ex)
            {
              
            }

        }

        public static string GetParamValue(string pgSrc, string paramName)
        {
            try
            {
                if (pgSrc.Contains("name='" + paramName + "'"))
                {
                    string param = "name='" + paramName + "'";
                    int startparamName = pgSrc.IndexOf(param) + param.Length;
                    startparamName = pgSrc.IndexOf("value=", startparamName) + "value=".Length + 1;
                    int endparamName = pgSrc.IndexOf("'", startparamName);
                    string valueparamName = pgSrc.Substring(startparamName, endparamName - startparamName);
                    return valueparamName;
                }
                else if (pgSrc.Contains("name=\"" + paramName + "\""))
                {
                    string param = "name=\"" + paramName + "\"";
                    int startparamName = pgSrc.IndexOf(param) + param.Length;
                    startparamName = pgSrc.IndexOf("value=", startparamName) + "value=".Length + 1;
                    int endcommentPostID = pgSrc.IndexOf("\"", startparamName);
                    string valueparamName = pgSrc.Substring(startparamName, endcommentPostID - startparamName);
                    return valueparamName;
                }
                else if (pgSrc.Contains("name=\\\"" + paramName + "\\\""))
                {
                    string param = "name=\\\"" + paramName + "\\\"";
                    int startparamName = pgSrc.IndexOf(param) + param.Length;
                    startparamName = pgSrc.IndexOf("value=\\", startparamName) + "value=\\".Length + 1;
                    int endcommentPostID = pgSrc.IndexOf("\\\"", startparamName);
                    string valueparamName = pgSrc.Substring(startparamName, endcommentPostID - startparamName);
                    return valueparamName;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string ParseJson(string data, string paramName)
        {
            int startIndx = data.IndexOf(paramName) + paramName.Length + 3;
            int endIndx = data.IndexOf("\"", startIndx);

            string value = data.Substring(startIndx, endIndx - startIndx);
            return value;
        }

        public static string ParseEncodedJson(string data, string paramName)
        {
            data = data.Replace("&quot;", "\"");
            int startIndx = data.IndexOf("\"" + paramName + "\"") + ("\"" + paramName + "\"").Length + 1;
            int endIndx = data.IndexOf("\"", startIndx);

            string value = data.Substring(startIndx, endIndx - startIndx);
            return value;
        }

        public string postFormDataAAAAAA(Uri url, string postData)
        {

            gRequest = (HttpWebRequest)WebRequest.Create(url);

            //gRequest.ServicePoint.Expect100Continue = false;

            gRequest.UserAgent = "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.16) Gecko/20110319 Firefox/3.6.16";

            gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            gRequest.Method = "POST";
            gRequest.Accept = " text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";
            gRequest.KeepAlive = true;
            gRequest.ContentType = @"application/x-www-form-urlencoded";

            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends

            //Get Response for this request url
            string html;
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (WebException ex)
            {
               Console.WriteLine(ex.Message);
                //Logger.LogText("Response from " + url + ":" + ex.Message, null);
            }


            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion



                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }

        }


        //Test Post Request For Follow 

        public string postFormDataForFollowUser(Uri url, string postData, string Referes, NameValueCollection nameValueCollection)
        {
            
            gRequest = (HttpWebRequest)WebRequest.Create(url);

            //gRequest.ServicePoint.Expect100Continue = false;

            gRequest.UserAgent = UserAgent; //@"Mozilla/5.0 (Windows NT 6.1; rv:14.0) Gecko/20100101 Firefox/14.0.1";
            gRequest.Accept = "*/*";
            
            gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            gRequest.Method = "POST";
            gRequest.KeepAlive = true;
            gRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            gRequest.Headers["Accept-Language"] = "en-us,en;q=0.5";
            gRequest.Headers["X-Requested-With"] = "XMLHttpRequest";

            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            if (!string.IsNullOrEmpty(Referes))
            {
                gRequest.Referer = Referes;
            }

            try
            {
                if (nameValueCollection != null)
                {
                    foreach (string item in nameValueCollection.Keys)
                    {
                        gRequest.Headers.Add(item, nameValueCollection[item]);
                    }
                }
            }
            catch (Exception)
            {
            }

            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // //Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends

            //Get Response for this request url

            string html;

            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                // //Logger.LogText("Response from " + url + ":" + ex.Message, null);
            }



            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion


                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }

        }

        public string postFormDataForFollowUserNew(Uri url, string postData, string Referes, NameValueCollection nameValueCollection,string user,string pass)
        {

            GlobDramProHttpHelper _GlobusHttpHelper = new GlobDramProHttpHelper();

            string Username = user;
            string Password = pass;

            string firstUrl = "https://api.instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes";

            //https://instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes

            string secondURL = "https://instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes";
            string res_secondURL = _GlobusHttpHelper.getHtmlfromUrlProxy(new Uri(secondURL), proxyAddress, 80, proxyUsername, proxyPassword);

            string nextUrl = "https://instagram.com/accounts/login/?force_classic_login=&next=/oauth/authorize/%3Fclient_id%3D9d836570317f4c18bca0db6d2ac38e29%26redirect_uri%3Dhttp%3A//websta.me/%26response_type%3Dcode%26scope%3Dcomments%2Brelationships%2Blikes";
            string res_nextUrl = _GlobusHttpHelper.getHtmlfromUrlProxy(new Uri(nextUrl), proxyAddress, 80, proxyUsername, proxyPassword);

            #region << Get Token >>
            //Get Token Number of Id 
            int FirstPointToken_nextUrl = res_nextUrl.IndexOf("csrfmiddlewaretoken");
            string FirstTokenSubString_nextUrl = res_nextUrl.Substring(FirstPointToken_nextUrl);
            int SecondPointToken_nextUrl = FirstTokenSubString_nextUrl.IndexOf("/>");
            string Token = FirstTokenSubString_nextUrl.Substring(0, SecondPointToken_nextUrl).Replace("csrfmiddlewaretoken", string.Empty).Replace("value=", string.Empty).Replace("\"", string.Empty).Replace("'", string.Empty).Trim();
            #endregion


            string login = "https://instagram.com/accounts/login/?force_classic_login=&next=/oauth/authorize/%3Fclient_id%3D9d836570317f4c18bca0db6d2ac38e29%26redirect_uri%3Dhttp%3A//websta.me/%26response_type%3Dcode%26scope%3Dcomments%2Brelationships%2Blikes";
            string postdata_Login = "csrfmiddlewaretoken=" + Token + "&username=" + Username + "&password=" + Password + "";

            string res_postdata_Login = _GlobusHttpHelper.postFormData(new Uri(login), postdata_Login, login, "");
            gRequest = (HttpWebRequest)WebRequest.Create(url);

            //gRequest.ServicePoint.Expect100Continue = false;

            gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2062.124 Safari/537.36";
            gRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            

            gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            gRequest.Method = "POST";
            gRequest.KeepAlive = true;
            gRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            gRequest.Headers["Accept-Language"] = "en-us,en;q=0.5";
            gRequest.Headers["X-Requested-With"] = "XMLHttpRequest";

            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            if (!string.IsNullOrEmpty(Referes))
            {
                gRequest.Referer = Referes;
            }

            try
            {
                if (nameValueCollection != null)
                {
                    foreach (string item in nameValueCollection.Keys)
                    {
                        gRequest.Headers.Add(item, nameValueCollection[item]);
                    }
                }
            }
            catch (Exception)
            {
            }

            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // //Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends

            //Get Response for this request url

            string html;

            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                // //Logger.LogText("Response from " + url + ":" + ex.Message, null);
            }



            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion


                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }

        }


        //public string getHtmlfromUrl(Uri url, string referer)
        //{
        //    string responseString = string.Empty;
        //    try
        //    {
        //        //setExpect100Continue();
        //        gRequest = (HttpWebRequest)WebRequest.Create(url);
        //        gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.115 Safari/537.36";
        //        gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        //        //gRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
        //        gRequest.Headers["Accept-Language"] = "en-US,en;q=0.8";
        //        if (!string.IsNullOrEmpty(referer))
        //        {
        //            gRequest.Referer = referer;
        //        }
        //        gRequest.KeepAlive = true;

        //        gRequest.AllowAutoRedirect = true;

        //        gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        //        gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;

        //        gRequest.Method = "GET";

        //        #region CookieManagment

        //        if (this.gCookies != null && this.gCookies.Count > 0)
        //        {
        //            setExpect100Continue();
        //            gRequest.CookieContainer.Add(gCookies);

        //        }

        //        if (this.gCookies == null)
        //        {
        //            this.gCookies = new CookieCollection();
        //        }



        //        //Get Response for this request url

        //        setExpect100Continue();
        //        gResponse = (HttpWebResponse)gRequest.GetResponse();

        //        //check if the status code is http 200 or http ok
        //        if (gResponse.StatusCode == HttpStatusCode.OK)
        //        {
        //            //get all the cookies from the current request and add them to the response object cookies
        //            setExpect100Continue();
        //            gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);


        //            //check if response object has any cookies or not
        //            if (gResponse.Cookies.Count > 0)
        //            {
        //                //check if this is the first request/response, if this is the response of first request gCookies
        //                //will be null
        //                if (this.gCookies == null)
        //                {
        //                    gCookies = gResponse.Cookies;
        //                }
        //                else
        //                {
        //                    foreach (Cookie oRespCookie in gResponse.Cookies)
        //                    {
        //                        bool bMatch = false;
        //                        foreach (Cookie oReqCookie in this.gCookies)
        //                        {
        //                            if (oReqCookie.Name == oRespCookie.Name)
        //                            {
        //                                oReqCookie.Value = oRespCookie.Value;
        //                                bMatch = true;
        //                                break; // 
        //                            }
        //                        }
        //                        if (!bMatch)
        //                            this.gCookies.Add(oRespCookie);
        //                    }
        //                }
        //            }
        //        #endregion

        //            string responseURI = gResponse.ResponseUri.AbsoluteUri;

        //            StreamReader reader = new StreamReader(gResponse.GetResponseStream());
        //            responseString = reader.ReadToEnd();
        //            reader.Close();
        //            return responseString;
        //        }
        //        else
        //        {
        //            return "Error";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.StackTrace);

        //        if (ex.Message.Contains("The remote server returned an error: (999)"))
        //        {
        //            // GlobusLogHelper.log.Info("Yahoo trace your IP ,Plz change your proxy IP");
        //            responseString = "The remote server returned an error: (999)";
        //        }
        //    }
        //    return responseString;
        //}


        public string getHtmlfromUrl(Uri url, string referer)
        {
            string responseString = string.Empty;
            try
            {
                //setExpect100Continue();
                gRequest = (HttpWebRequest)WebRequest.Create(url);
                gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.115 Safari/537.36";
                gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                //gRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
                gRequest.Headers["Accept-Language"] = "en-US,en;q=0.8";
                if (!string.IsNullOrEmpty(referer))
                {
                    gRequest.Referer = referer;
                }
                gRequest.KeepAlive = true;

                gRequest.AllowAutoRedirect = true;

                gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;

                gRequest.Method = "GET";

                #region CookieManagment

                if (this.gCookies != null && this.gCookies.Count > 0)
                {
                    setExpect100Continue();
                    gRequest.CookieContainer.Add(gCookies);

                }

                if (this.gCookies == null)
                {
                    this.gCookies = new CookieCollection();
                }



                //Get Response for this request url

                setExpect100Continue();
                try
                {
                    gResponse = (HttpWebResponse)gRequest.GetResponse();
                }
                catch { };

                //check if the status code is http 200 or http ok
                if (gResponse.StatusCode == HttpStatusCode.OK)
                {
                    //get all the cookies from the current request and add them to the response object cookies
                    setExpect100Continue();
                    gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);


                    //check if response object has any cookies or not
                    if (gResponse.Cookies.Count > 0)
                    {
                        //check if this is the first request/response, if this is the response of first request gCookies
                        //will be null
                        if (this.gCookies == null)
                        {
                            gCookies = gResponse.Cookies;
                        }
                        else
                        {
                            foreach (Cookie oRespCookie in gResponse.Cookies)
                            {
                                bool bMatch = false;
                                foreach (Cookie oReqCookie in this.gCookies)
                                {
                                    if (oReqCookie.Name == oRespCookie.Name)
                                    {
                                        oReqCookie.Value = oRespCookie.Value;
                                        bMatch = true;
                                        break; // 
                                    }
                                }
                                if (!bMatch)
                                    this.gCookies.Add(oRespCookie);
                            }
                        }
                    }
                #endregion

                    string responseURI = gResponse.ResponseUri.AbsoluteUri;

                    StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                    responseString = reader.ReadToEnd();
                    reader.Close();
                    return responseString;
                }
                else
                {
                    return "Error";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);

                if (ex.Message.Contains("The remote server returned an error: (999)"))
                {
                    // GlobusLogHelper.log.Info("Yahoo trace your IP ,Plz change your proxy IP");
                    responseString = "The remote server returned an error: (999)";
                }
            }
            return responseString;
        }



        public List<string> GetTextDataByTagAndAttributeName(string pageSrcHtml, string TagName, string AttributeName)
        {
            List<string> lstData = new List<string>();
            try
            {
                bool success = false;
                string xHtml = string.Empty;

                Chilkat.HtmlToXml htmlToXml = new Chilkat.HtmlToXml();

                //*** Check DLL working or not **********************
                success = htmlToXml.UnlockComponent("THEBACHtmlToXml_7WY3A57sZH3O");
                if ((success != true))
                {
                    Console.WriteLine(htmlToXml.LastErrorText);
                    return null;
                }

                htmlToXml.Html = pageSrcHtml;

                //** Convert Data Html to XML ******************************************* 
                xHtml = htmlToXml.ToXml();

                //******************************************
                Chilkat.Xml xNode = default(Chilkat.Xml);
                Chilkat.Xml xBeginSearchAfter = default(Chilkat.Xml);
                Chilkat.Xml xml = new Chilkat.Xml();
                xml.LoadXml(xHtml);

                #region Data Save in list From using XML Tag and Attribut
                string DescriptionMain = string.Empty;
                string dataDescription = string.Empty;

                xBeginSearchAfter = null;

                xNode = xml.SearchForAttribute(xBeginSearchAfter, TagName, "class", AttributeName);
                while ((xNode != null))
                {
                    //** Get Data Under Tag only Text Value**********************************
                    dataDescription = xNode.GetXml();//.AccumulateTagContent("text", "script|style");

                    string text = xNode.AccumulateTagContent("text", "script|style");
                    lstData.Add(text);

                    //** Get Data Under Tag All  Html value * *********************************
                    //dataDescription = xNode.GetXml();

                    xBeginSearchAfter = xNode;
                    xNode = xml.SearchForAttribute(xBeginSearchAfter, TagName, "class", AttributeName);
                }
                #endregion
                return lstData;
            }
            catch (Exception)
            {
                return lstData = null;

            }
        }


        public bool CheckAttributeexsist(string pageSrcHtml, string TagName, string AttributeName)
        {
            bool IsContain = false;
            try
            {
                bool success = false;
                string xHtml = string.Empty;

                Chilkat.HtmlToXml htmlToXml = new Chilkat.HtmlToXml();

                //*** Check DLL working or not **********************
                success = htmlToXml.UnlockComponent("THEBACHtmlToXml_7WY3A57sZH3O");
                if ((success != true))
                {
                    Console.WriteLine(htmlToXml.LastErrorText);
                    return IsContain;
                }

                htmlToXml.Html = pageSrcHtml;

                //** Convert Data Html to XML ******************************************* 
                xHtml = htmlToXml.ToXml();

                //******************************************
                Chilkat.Xml xNode = default(Chilkat.Xml);
                Chilkat.Xml xBeginSearchAfter = default(Chilkat.Xml);
                Chilkat.Xml xml = new Chilkat.Xml();
                xml.LoadXml(xHtml);

                #region Data Save in list From using XML Tag and Attribut
                string DescriptionMain = string.Empty;
                string dataDescription = string.Empty;

                xBeginSearchAfter = null;

                xNode = xml.SearchForAttribute(xBeginSearchAfter, TagName, "class", AttributeName);
                while ((xNode != null))
                {
                    IsContain = true;
                    return IsContain;
                }
                #endregion               
            }
            catch (Exception)
            {
                IsContain = false;
            }
            return IsContain;
        }

    }
}
