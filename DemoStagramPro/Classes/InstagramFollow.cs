using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using BaseLib;
using DemoStagramPro.Classes;
using System.Threading;

namespace InstagramManager.Classes
{
    public class InstagramFollow
    {
        public InstagramFollow()
        {
        }

        #region Follow
        public string Follow(string UserName, ref InstagramAccountManager accountManager)
        {
            NameValueCollection nameval = new NameValueCollection();
            //ChilkatHttpHelpr chilaktHttpHelper = new ChilkatHttpHelpr();
            if (!UserName.Contains("http://websta.me/n/"))
            {
                UserName = "http://websta.me/n/" + UserName + "/";
            }
            string UserPageContent=string.Empty;


            if (!string.IsNullOrEmpty(accountManager.proxyAddress) && !string.IsNullOrEmpty(accountManager.proxyPort))
                {
                    try
                    {
                       UserPageContent = accountManager.httpHelper.getHtmlfromUrlProxy(new Uri(UserName), accountManager.proxyAddress, Convert.ToInt32(accountManager.proxyPort), accountManager.proxyUsername, accountManager.proxyPassword);
                    }
                    catch (Exception ex)
                    {

                    }
                    if (string.IsNullOrEmpty(UserPageContent))
                    {
                        Thread.Sleep(1000);
                        try
                        {
                            UserPageContent = accountManager.httpHelper.getHtmlfromUrlProxy(new Uri(UserName), accountManager.proxyAddress, Convert.ToInt32(accountManager.proxyPort), accountManager.proxyUsername, accountManager.proxyPassword);
                        }
                        catch (Exception ex)
                        {

                        }
                        
                    }
                }
                else
                {

                    try
                    {
                        UserPageContent = accountManager.httpHelper.getHtmlfromUrlProxy(new Uri(UserName), "", 80, "", "");
                    }
                    catch { };

                    if (string.IsNullOrEmpty(UserPageContent))
                    {
                        Thread.Sleep(1000);
                        try
                        {
                            UserPageContent = accountManager.httpHelper.getHtmlfromUrlProxy(new Uri(UserName), "", 80, "", "");
                        }
                        catch { };
                    }
                    
                }
        
          
            try
            {
                //if (UserPageContent.Contains("This user is private."))
                //{
                //    return "private";
                //}
                string PK = string.Empty;
                if (UserPageContent.Contains(""))
                {
                    PK = getBetween(UserPageContent, "id=\"follow_btn_wrapper\"", ">").Replace("data-target=", "").Replace("\"","").Trim();
                }

                if (string.IsNullOrEmpty(PK))
                {
                    PK = getBetween(UserPageContent, "id=\"message_user_id", ">").Replace(">", "").Replace("value=",string.Empty).Replace("\"",string.Empty).Trim();//.Replace("\"", "").Trim();
                }

                string PostData = "action=follow";//"&pk=" + PK + "&t=9208";
                string FollowedPageSource=string.Empty;

                if (!string.IsNullOrEmpty(PK))
                {
                    try
                    {
                        FollowedPageSource = accountManager.httpHelper.postFormData(new Uri("http://websta.me/api/relationships/" + PK), PostData, UserName, "http://websta.me");
                    }
                    catch { }
                }
                if (string.IsNullOrEmpty(FollowedPageSource))
                {

                }
                nameval.Add("Origin", "http://web.stagram.com");
                nameval.Add("X-Requested-With", "XMLHttpRequest");

               
                if (FollowedPageSource.Contains("OK"))
                {
                    //return "Followed";
                    string status = string.Empty;
                    try
                    {
                        status = QueryExecuter.getFollowStatus(accountManager.Username, UserName);
                    }
                    catch { }
                    switch (status)
                    {
                        case "Followed": status = "Already Followed";
                            break;
                        case "Unfollowed": status = "Unfollowed";
                            QueryExecuter.updateFollowStatus(accountManager.Username, UserName, "Followed");
                            break;
                        default: status = "Followed";                            
                            try
                            {
                                QueryExecuter.insertFollowInfo(accountManager.Username, UserName,"Followed");
                            }
                            catch { }                           
                            break;
                    }
                    return status;
                }
                else
                {
                    return "UnFollowed";
                }
            }
            catch (Exception)
            {
                return "Follow option is not available In page...!!";
            }

        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }
        #endregion


      








    }
}
