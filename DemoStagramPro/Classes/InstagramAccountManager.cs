using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseLib;
using Globussoft;
using System.Collections.Specialized;
using DemoStagramPro.Classes;

namespace InstagramManager.Classes
{
    public class InstagramAccountManager
    {

        public GlobDramProHttpHelper httpHelper = new GlobDramProHttpHelper();
        //public GlobusHttpHelper httpHelper;
        public string Username { get; set; }
        public string Password { get; set; }
        public string proxyAddress { get; set; }
        public string proxyPort { get; set; }
        public string proxyUsername { get; set; }
        public string proxyPassword { get; set; }
        public string Token { get; set; }
        public string ClientId { get; set; }
        public bool LoggedIn { get; set; }





        public InstagramAccountManager(string Username, string Password, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword)
        {
            this.Username = Username;
            this.Password = Password;
            this.proxyAddress = proxyAddress;
            this.proxyPort = proxyPort;
            this.proxyUsername = proxyUsername;
            this.proxyPassword = proxyPassword;

        }


       
        public BaseLib.Events logEvents = new BaseLib.Events();
        private void Log(string message)
        {
            EventsArgs eventArgs = new EventsArgs(message);
            //logEvents_static.LogText(eventArgs);
            logEvents.LogText(eventArgs);
        }
       

       
        public string Login()
        {
            Log("[ " + DateTime.Now + " ] => [ Logging in with Account : " + Username + " ]");
            string Status = "Failed";
            try
            {
                string firstUrl = "https://api.instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes";

                //https://instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes

                string secondURL = "https://instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes";
                string res_secondURL = string.Empty;
                if (!string.IsNullOrEmpty(proxyAddress) && !string.IsNullOrEmpty(proxyPort))
                {
                    try
                    {
                        res_secondURL = httpHelper.getHtmlfromUrlProxy(new Uri(secondURL), proxyAddress, int.Parse(proxyPort), proxyUsername, proxyPassword);
                    }
                    catch { };


                }
                else
                {

                    res_secondURL = httpHelper.getHtmlfromUrl(new Uri(secondURL), "");
                   
                }
                       
                   
                //string authlogin = "https://instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes";
                //string res_nextUrl1 = httpHelper.getHtmlfromUrl(new Uri(authlogin), "");

                string nextUrl = "https://instagram.com/accounts/login/?force_classic_login=&next=/oauth/authorize/%3Fclient_id%3D9d836570317f4c18bca0db6d2ac38e29%26redirect_uri%3Dhttp%3A//websta.me/%26response_type%3Dcode%26scope%3Dcomments%2Brelationships%2Blikes";
               
                string res_nextUrl = httpHelper.getHtmlfromUrl(new Uri(nextUrl), "");//postFormDataProxy
                



                //int FirstPointToken_nextUrl1 = res_nextUrl.IndexOf("authorize");
                //string FirstTokenSubString_nextUrl1 = res_nextUrl.Substring(FirstPointToken_nextUrl1);
                //int SecondPointToken_nextUrl1 = FirstTokenSubString_nextUrl1.IndexOf("%");
                //this.Token = FirstTokenSubString_nextUrl1.Substring(0, SecondPointToken_nextUrl1).Replace("Authorize", string.Empty).Replace("value=", string.Empty).Replace("\"", string.Empty).Replace("'", string.Empty).Trim();



                try
                {
                    int FirstPointToken_nextUrl = res_nextUrl.IndexOf("csrfmiddlewaretoken");//csrfmiddlewaretoken
                    string FirstTokenSubString_nextUrl = res_nextUrl.Substring(FirstPointToken_nextUrl);
                    int SecondPointToken_nextUrl = FirstTokenSubString_nextUrl.IndexOf("/>");
                    this.Token = FirstTokenSubString_nextUrl.Substring(0, SecondPointToken_nextUrl).Replace("csrfmiddlewaretoken", string.Empty).Replace("value=", string.Empty).Replace("\"", string.Empty).Replace("'", string.Empty).Trim();
                }
                catch { };
               
                
                string login = "https://instagram.com/accounts/login/?force_classic_login=&next=/oauth/authorize/%3Fclient_id%3D9d836570317f4c18bca0db6d2ac38e29%26redirect_uri%3Dhttp%3A//websta.me/%26response_type%3Dcode%26scope%3Dcomments%2Brelationships%2Blikes";
               //string authlogin=  "https://instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes";

                //string postauthorize = "csrfmiddlewaretoken=" + this.Token + "&allow=Authorize";
                //http://websta.me/?code=90786edc0a4844b79a3d9289d27360d5

                 
                string postdata_Login = "csrfmiddlewaretoken=" + this.Token + "&username=" + Username + "&password=" + Password + "";

                string res_postdata_Login = httpHelper.postFormData(new Uri(login), postdata_Login, login, "");



            
                string autho = "https://instagram.com/oauth/authorize/?scope=comments+likes+relationships&redirect_uri=http%3A%2F%2Fwww.gramfeed.com%2Foauth%2Fcallback%3Fpage%3D&response_type=code&client_id=b59fbe4563944b6c88cced13495c0f49";
               
                if (res_postdata_Login.Contains("Please enter a correct username and password"))
                {
                    Status = "Failed";
                    this.LoggedIn = false;
                }
                else if (res_postdata_Login.Contains("requesting access to your Instagram account") || postdata_Login.Contains("is requesting to do the following"))
                {
                    Status = "AccessIssue";
                }
                else if (res_postdata_Login.Contains("logout") || postdata_Login.Contains("LOG OUT"))
                {
                    Log("[ " + DateTime.Now + " ] => [ Logged in with Account :" + Username + " ]");
                    Status = "Success";
                    this.LoggedIn = true;
                }

                //nameval.Clear();
                return Status;
            }
            catch (Exception ex)
            {
                return ex.Message;

            }

        }


        public string LoginNew(ref GlobDramProHttpHelper _GlobusHttpHelper)
        {

            if (string.IsNullOrEmpty(proxyPort))
            {
                proxyPort = "0";
            }

            Log("[ " + DateTime.Now + " ] => [ Logging in with Account : " + Username + " ]");
            string Status = "Failed";
            try
            {

                //string firstUrl = "https://api.instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes";

                //https://instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes


                string secondURL = "https://instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes";
                //string res_secondURL = _GlobusHttpHelper.getHtmlfromUrlProxy(new Uri(secondURL), proxyAddress, 80, proxyUsername, proxyPassword);
                string res_secondURL = string.Empty;

                if (!string.IsNullOrEmpty(proxyAddress) && !string.IsNullOrEmpty(proxyPort))
                {
                    try
                    {
                        res_secondURL = _GlobusHttpHelper.getHtmlfromUrlProxy(new Uri(secondURL), proxyAddress, Convert.ToInt32(proxyPort), proxyUsername, proxyPassword);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {

                    try
                    {
                        res_secondURL = _GlobusHttpHelper.getHtmlfromUrlProxy(new Uri(secondURL), "", 80, "", "");
                    }
                    catch { }
                }


                string res_nextUrl = _GlobusHttpHelper.getHtmlfromUrlProxy(new Uri("http://websta.me/login"), proxyAddress, Convert.ToInt32(proxyPort), proxyUsername, proxyPassword);

                //Get Token Number of Id 
                int FirstPointToken_nextUrl = res_nextUrl.IndexOf("csrfmiddlewaretoken");
                string FirstTokenSubString_nextUrl = res_nextUrl.Substring(FirstPointToken_nextUrl);
                int SecondPointToken_nextUrl = FirstTokenSubString_nextUrl.IndexOf("/>");
                this.Token = FirstTokenSubString_nextUrl.Substring(0, SecondPointToken_nextUrl).Replace("csrfmiddlewaretoken", string.Empty).Replace("value=", string.Empty).Replace("\"", string.Empty).Replace("'", string.Empty).Trim();



                string login = "https://instagram.com/accounts/login/?force_classic_login=&next=/oauth/authorize/%3Fclient_id%3D9d836570317f4c18bca0db6d2ac38e29%26redirect_uri%3Dhttp%3A//websta.me/%26response_type%3Dcode%26scope%3Dcomments%2Brelationships%2Blikes";

                string postdata_Login = "csrfmiddlewaretoken=" + this.Token + "&username=" + Username + "&password=" + Password + "";

                //string res_postdata_Login = _GlobusHttpHelper.postFormData(new Uri(login), postdata_Login, login, "");
                string res_postdata_Login = _GlobusHttpHelper.postFormDataRefererProxy(new Uri(login), postdata_Login, login, proxyAddress, Convert.ToInt32(proxyPort), proxyUsername, proxyPassword);

                if (res_postdata_Login.Contains("Please enter a correct username and password"))
                {
                    Status = "Failed";
                    this.LoggedIn = false;
                }
                else if (res_postdata_Login.Contains("requesting access to your Instagram account") || postdata_Login.Contains("is requesting to do the following"))
                {
                    Status = "AccessIssue";
                }
                else if (res_postdata_Login.Contains("logout") || postdata_Login.Contains("LOG OUT"))
                {



                    Status = "Success";
                    Log("[ " + DateTime.Now + " ] => [ Logged in with Account :" + Username + " ]");
                    this.LoggedIn = true;

                }

                //nameval.Clear();
                return Status;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public string LoginNewScraperFollower(ref InstagramAccountManager _GlobusHttpHelper)
        {

            if (string.IsNullOrEmpty(proxyPort))
            {
                proxyPort = "0";
            }
            //proxyAddress = "60.169.78.218";
            //proxyPort = "808";

            Log("[ " + DateTime.Now + " ] => [ Logging in with Account : " + Username + " ]");
            string Status = "Failed";
            try
            {

                string secondURL = "https://instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes";
                //string res_secondURL = _GlobusHttpHelper.getHtmlfromUrlProxy(new Uri(secondURL), proxyAddress, 80, proxyUsername, proxyPassword);
                string res_secondURL = string.Empty;




                string res_nextUrl = _GlobusHttpHelper.httpHelper.getHtmlfromUrlProxy(new Uri("http://websta.me/login"), proxyAddress, Convert.ToInt32(proxyPort), proxyUsername, proxyPassword);

                //Get Token Number of Id 
                int FirstPointToken_nextUrl = res_nextUrl.IndexOf("csrfmiddlewaretoken");
                string FirstTokenSubString_nextUrl = res_nextUrl.Substring(FirstPointToken_nextUrl);
                int SecondPointToken_nextUrl = FirstTokenSubString_nextUrl.IndexOf("/>");
                this.Token = FirstTokenSubString_nextUrl.Substring(0, SecondPointToken_nextUrl).Replace("csrfmiddlewaretoken", string.Empty).Replace("value=", string.Empty).Replace("\"", string.Empty).Replace("'", string.Empty).Trim();



                string login = "https://instagram.com/accounts/login/?force_classic_login=&next=/oauth/authorize/%3Fclient_id%3D9d836570317f4c18bca0db6d2ac38e29%26redirect_uri%3Dhttp%3A//websta.me/%26response_type%3Dcode%26scope%3Dcomments%2Brelationships%2Blikes";
                string postdata_Login = "csrfmiddlewaretoken=" + this.Token + "&username=" + Username + "&password=" + Password + "";

                //string res_postdata_Login = _GlobusHttpHelper.postFormData(new Uri(login), postdata_Login, login, "");
                string res_postdata_Login = _GlobusHttpHelper.httpHelper.postFormDataRefererProxy(new Uri(login), postdata_Login, login, proxyAddress, Convert.ToInt32(proxyPort), proxyUsername, proxyPassword);

                if (res_postdata_Login.Contains("Please enter a correct username and password"))
                {
                    Status = "Failed";
                    this.LoggedIn = false;
                }
                else if (res_postdata_Login.Contains("requesting access to your Instagram account") || postdata_Login.Contains("is requesting to do the following"))
                {
                    Status = "AccessIssue";
                }
                else if (res_postdata_Login.Contains("logout") || postdata_Login.Contains("LOG OUT"))
                {


                    Status = "Success";
                    Log("[ " + DateTime.Now + " ] => [ Logged in with Account :" + Username + " ]");
                    this.LoggedIn = true;

                }

                //nameval.Clear();
                return Status;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

       
        public string MyLoginandComment(ref GlobDramProHttpHelper _GlobusHttpHelper, string url, string commentPostData, string referrer)
        {

            Log("[ " + DateTime.Now + " ] => [ Logging in with Account : " + Username + " ]");
            string Status = "Failed";
            try
            {

                //string firstUrl = "https://api.instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes";

                //https://instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes


                string secondURL = "https://instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes";
                //string res_secondURL = _GlobusHttpHelper.getHtmlfromUrlProxy(new Uri(secondURL), proxyAddress, 80, proxyUsername, proxyPassword);
                string res_secondURL = string.Empty;

                if (!string.IsNullOrEmpty(proxyAddress) && !string.IsNullOrEmpty(proxyPort))
                {
                    try
                    {
                        res_secondURL = _GlobusHttpHelper.getHtmlfromUrlProxy(new Uri(secondURL), proxyAddress, Convert.ToInt32(proxyPort), proxyUsername, proxyPassword);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {

                    try
                    {
                        res_secondURL = _GlobusHttpHelper.getHtmlfromUrlProxy(new Uri(secondURL), "", 80, "", "");
                    }
                    catch { }
                }


                //string nextUrl = "https://instagram.com/accounts/login/?force_classic_login=&next=/oauth/authorize/%3Fclient_id%3D9d836570317f4c18bca0db6d2ac38e29%26redirect_uri%3Dhttp%3A//websta.me/%26response_type%3Dcode%26scope%3Dcomments%2Brelationships%2Blikes";
                string res_nextUrl = _GlobusHttpHelper.getHtmlfromUrl(new Uri("http://websta.me/login"), "");

                //Get Token Number of Id 
                int FirstPointToken_nextUrl = res_nextUrl.IndexOf("csrfmiddlewaretoken");
                string FirstTokenSubString_nextUrl = res_nextUrl.Substring(FirstPointToken_nextUrl);
                int SecondPointToken_nextUrl = FirstTokenSubString_nextUrl.IndexOf("/>");
                this.Token = FirstTokenSubString_nextUrl.Substring(0, SecondPointToken_nextUrl).Replace("csrfmiddlewaretoken", string.Empty).Replace("value=", string.Empty).Replace("\"", string.Empty).Replace("'", string.Empty).Trim();



                string login = "https://instagram.com/accounts/login/?force_classic_login=&next=/oauth/authorize/%3Fclient_id%3D9d836570317f4c18bca0db6d2ac38e29%26redirect_uri%3Dhttp%3A//websta.me/%26response_type%3Dcode%26scope%3Dcomments%2Brelationships%2Blikes";
                string postdata_Login = "csrfmiddlewaretoken=" + this.Token + "&username=" + Username + "&password=" + Password + "";

                string res_postdata_Login = _GlobusHttpHelper.postFormData(new Uri(login), postdata_Login, login, "");

                if (res_postdata_Login.Contains("Please enter a correct username and password"))
                {
                    Status = "Failed";
                    this.LoggedIn = false;
                }
                else if (res_postdata_Login.Contains("requesting access to your Instagram account") || postdata_Login.Contains("is requesting to do the following"))
                {
                    Status = "AccessIssue";
                }
                else if (res_postdata_Login.Contains("logout") || postdata_Login.Contains("LOG OUT"))
                {
                    Status = "Success";
                    Log("[ " + DateTime.Now + " ] => [ Logged in with Account :" + Username + " ]");
                    this.LoggedIn = true;

                }

                //nameval.Clear();
                return Status;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }


       
        public string MyLoginForUnfollow(ref GlobDramProHttpHelper _GlobusHttpHelper, string url, string commentPostData, string referrer)
        {

            Log("[ " + DateTime.Now + " ] => [ Logging in with Account : " + Username + " ]");
            string Status = string.Empty;
            try
            {
                string firstUrl = "https://api.instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes";

                //https://instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes

                string secondURL = "https://instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes";

                string res_secondURL = string.Empty;

                if (!string.IsNullOrEmpty(proxyAddress) && !string.IsNullOrEmpty(proxyPort))
                {
                    try
                    {
                        res_secondURL = _GlobusHttpHelper.getHtmlfromUrlProxy(new Uri(secondURL), proxyAddress, Convert.ToInt32(proxyPort), proxyUsername, proxyPassword);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {

                    try
                    {
                        res_secondURL = _GlobusHttpHelper.getHtmlfromUrlProxy(new Uri(secondURL), "", 80, "", "");
                    }
                    catch { }
                }

                string nextUrl = "https://instagram.com/accounts/login/?force_classic_login=&next=/oauth/authorize/%3Fclient_id%3D9d836570317f4c18bca0db6d2ac38e29%26redirect_uri%3Dhttp%3A//websta.me/%26response_type%3Dcode%26scope%3Dcomments%2Brelationships%2Blikes";
                string res_nextUrl = _GlobusHttpHelper.getHtmlfromUrl(new Uri("http://websta.me/login"), "");


                //Get Token Number of Id 
                int FirstPointToken_nextUrl = res_nextUrl.IndexOf("csrfmiddlewaretoken");
                string FirstTokenSubString_nextUrl = res_nextUrl.Substring(FirstPointToken_nextUrl);
                int SecondPointToken_nextUrl = FirstTokenSubString_nextUrl.IndexOf("/>");
                this.Token = FirstTokenSubString_nextUrl.Substring(0, SecondPointToken_nextUrl).Replace("csrfmiddlewaretoken", string.Empty).Replace("value=", string.Empty).Replace("\"", string.Empty).Replace("'", string.Empty).Trim();



                string login = "https://instagram.com/accounts/login/?force_classic_login=&next=/oauth/authorize/%3Fclient_id%3D9d836570317f4c18bca0db6d2ac38e29%26redirect_uri%3Dhttp%3A//websta.me/%26response_type%3Dcode%26scope%3Dcomments%2Brelationships%2Blikes";
                string postdata_Login = "csrfmiddlewaretoken=" + this.Token + "&username=" + Username + "&password=" + Password + "";

                string res_postdata_Login = _GlobusHttpHelper.postFormData(new Uri(login), postdata_Login, login, "");

                if (res_postdata_Login.Contains("Please enter a correct username and password"))
                {
                    //Status = "Failed";
                    this.LoggedIn = false;
                }
                else if (res_postdata_Login.Contains("requesting access to your Instagram account") || postdata_Login.Contains("is requesting to do the following"))
                {
                    //Status = "AccessIssue";
                }
                else if (res_postdata_Login.Contains("logout") || postdata_Login.Contains("LOG OUT"))
                {
                    //Status = "Success";
                    string zone = string.Empty;
                    if (res_postdata_Login.Contains("data('userid'") && res_postdata_Login.Contains(")"))
                    {
                        try
                        {
                            zone = ScrapUserName.getBetween(res_postdata_Login, "data('userid'", ")");
                            if (!string.IsNullOrEmpty(zone))
                            {
                                try
                                {
                                    int start = zone.IndexOf("'");
                                    int end = zone.LastIndexOf("'");
                                    Status = zone.Substring(start + 1, end - start - 1);
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                    Log("[ " + DateTime.Now + " ] => [ Logged in with Account :" + Username + " ]");
                    this.LoggedIn = true;

                }

                //nameval.Clear();
                return Status;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
            
