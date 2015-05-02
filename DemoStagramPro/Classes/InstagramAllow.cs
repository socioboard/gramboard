using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace InstagramManager.Classes
{
    public class InstagramAllow
    {
        public string Allow(ref InstagramAccountManager accountManager)
        {
            NameValueCollection nameval = new NameValueCollection();

            string UserName = accountManager.Username;

            if (!UserName.Contains("http://web.stagram.com/"))
            {
                UserName = "http://web.stagram.com/n/" + UserName + "/";
            }

            //string UserPageContent = accountManager.httpHelper.getHtmlfromUrl(new Uri(UserName), "", "");

            //int FirstPointToken = UserPageContent.IndexOf("follow_button\"><span");
            //string FirstTokenSubString = UserPageContent.Substring(FirstPointToken);
            //int SecondPointToken = FirstTokenSubString.IndexOf(">Follow");
            //string PK = FirstTokenSubString.Substring(0, SecondPointToken).Replace("csrfmiddlewaretoken", string.Empty).Replace("value=", string.Empty).Replace("\"", string.Empty).Replace("'", string.Empty).Replace("follow_button>", string.Empty).Replace("<span", string.Empty).Replace("class=", string.Empty).Trim();

            string OauthUrl = "https://instagram.com/oauth/authorize/?client_id=" + accountManager.ClientId + "&redirect_uri=http://web.stagram.com/&response_type=code&scope=likes+comments+relationships";

            string PostData = "csrfmiddlewaretoken=" + accountManager.Token + "&allow=Authorize";

            //nameval.Add("Origin", "https://instagram.com");

            string AllowedPageSource = accountManager.httpHelper.postFormData(new Uri(OauthUrl), PostData, OauthUrl, nameval);

            string UserPageContent = accountManager.httpHelper.getHtmlfromUrl(new Uri(UserName), "", "", accountManager.proxyAddress);

            if (UserPageContent.Contains("LOGOUT"))
            {
                return "Allowed";
            }
            else
            {
                return "NotAllowed";
            }
        }
    }
}
