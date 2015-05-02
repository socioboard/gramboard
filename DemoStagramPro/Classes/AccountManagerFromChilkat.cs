using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using BaseLib;

namespace DemoStagramPro.Classes
{
    class AccountManagerFromChilkat
    {

        public ChilkatHttpHelpr ChilkathttpHelper = new ChilkatHttpHelpr();

        public string Username { get; set; }
        public string Password { get; set; }
        public string proxyAddress { get; set; }
        public string proxyPort { get; set; }
        public string proxyUsername { get; set; }
        public string proxyPassword { get; set; }
        public string Token { get; set; }
        public string ClientId { get; set; }
        public bool LoggedIn { get; set; }


        public AccountManagerFromChilkat(string Username, string Password, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword)
        {
            this.Username = Username;
            this.Password = Password;
            this.proxyAddress = proxyAddress;
            this.proxyPort = proxyPort;
            this.proxyUsername = proxyUsername;
            this.proxyPassword = proxyPassword;
        }


        public string getlogin()
        {
            NameValueCollection nameval = new NameValueCollection();

            string Status = "Failed";



            string BasePageContent = ChilkathttpHelper.GetHtmlProxy(("http://web.stagram.com/"), proxyAddress, proxyPort, proxyUsername, proxyPassword);

            //**********************
            int FirstPointClientId = BasePageContent.IndexOf("client_id=");
            string FirstClientIdSubString = BasePageContent.Substring(FirstPointClientId);
            int SecondPointClientId = FirstClientIdSubString.IndexOf("&redirect_uri=");
            this.ClientId = FirstClientIdSubString.Substring(0, SecondPointClientId).Replace("'", string.Empty).Replace("client_id=", string.Empty).Trim();

            string LoginUrl = "https://instagram.com/accounts/login/?next=/oauth/authorize/%3Fclient_id%3D" + ClientId + "%26redirect_uri%3Dhttp%253A%252F%252Fweb.stagram.com%252F%26response_type%3Dcode%26scope%3Dlikes%2Bcomments%2Brelationships";

            string LoginPageContent = ChilkathttpHelper.GetHtmlProxy(LoginUrl, proxyAddress, proxyPort, proxyUsername, proxyPassword);

            //************Get data prost For Login***************



            int FirstPointToken = LoginPageContent.IndexOf("csrfmiddlewaretoken");
            string FirstTokenSubString = LoginPageContent.Substring(FirstPointToken);
            int SecondPointToken = FirstTokenSubString.IndexOf("/>");
            this.Token = FirstTokenSubString.Substring(0, SecondPointToken).Replace("csrfmiddlewaretoken", string.Empty).Replace("value=", string.Empty).Replace("\"", string.Empty).Replace("'", string.Empty).Trim();

            nameval.Add("Origin", "https://instagram.com");

            string PostData = "csrfmiddlewaretoken=" + this.Token + "&username=" + this.Username + "&password=" + this.Password;
            string PostedPageContent = ChilkathttpHelper.PostData(LoginUrl, PostData, LoginUrl);

            if (PostedPageContent.Contains("Please enter a correct username and"))
            {
                Status = "Failed";
                this.LoggedIn = false;
            }
            else if (PostedPageContent.Contains("requesting access to your Instagram account") || PostedPageContent.Contains("is requesting to do the following"))
            {
                Status = "AccessIssue";
            }
            else if (PostedPageContent.Contains("LOGOUT") || PostedPageContent.Contains("LOG OUT"))
            {
                Status = "Success";
                this.LoggedIn = true;
            }

            return Status;

        }


    }
}
