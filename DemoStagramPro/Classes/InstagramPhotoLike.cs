using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Data;
using BaseLib;
using DemoStagramPro.Classes;

namespace InstagramManager.Classes
{
    class InstagramPhotoLike
    {
        #region photolike
        public string photolike(string PhotoId, ref InstagramAccountManager accountManager)
        {
            NameValueCollection namevalue = new NameValueCollection();
            string Photolink = string.Empty;
            string FollowedPageSource = string.Empty;
            string like = string.Empty;

            try
            {
                if (PhotoId.Contains("http://websta.me/p/"))
                {
                    PhotoId = PhotoId.Replace("http://websta.me/p/", string.Empty);
                }
                if (!PhotoId.Contains("http://web.stagram.com/"))
                {
                    Photolink = "http://websta.me/api/like/" + PhotoId + "/".Replace("http://websta.me/p/", "");                    
                }
                else
                {
                    Photolink = PhotoId;                    
                }
                

                //string PageContent = accountManager.httpHelper.getHtmlfromUrl(new Uri(Photolink), "", "", accountManager.proxyAddress);
                string PageContent = string.Empty;
                if (string.IsNullOrEmpty(accountManager.proxyPort))
                {
                    accountManager.proxyPort = "80";
                }
                try
                {
                    if (DemoStagramPro.ClGlobul.checkHashTagLiker == true)
                    {
                        PageContent = accountManager.httpHelper.getHtmlfromUrl(new Uri(Photolink));
                    }
                    else
                    {
                        PageContent = accountManager.httpHelper.getHtmlfromUrlProxy(new Uri(Photolink), accountManager.proxyAddress, Convert.ToInt32(accountManager.proxyPort), accountManager.proxyUsername, accountManager.proxyPassword);
                    }
                    
                }
                catch { }
                if (string.IsNullOrEmpty(PageContent))
                {
                    if (DemoStagramPro.ClGlobul.checkHashTagLiker == true)
                    {
                        PageContent = accountManager.httpHelper.getHtmlfromUrl(new Uri(Photolink));
                    }
                    else
                    {
                        PageContent = accountManager.httpHelper.getHtmlfromUrlProxy(new Uri(Photolink), "", 80, "", "");
                    }
                }

                if (PageContent.Contains("message\":\"LIKED\""))
                {
                    #region commented code
                    //bool isContain = accountManager.httpHelper.CheckAttributeexsist(PageContent, "span", "dislike_button");

                    //if (PageContent.Contains("img/liked.png"))
                    //{
                    //    FollowedPageSource = "All ready LIKED";
                    //}

                    //if (lstLikes.Count > 0)
                    //{
                    //    try
                    //    {
                    //        like = lstLikes[0];
                    //    }
                    //    catch
                    //    {
                    //    }
                    //}
                    //if (like == "1")
                    //{
                    //    FollowedPageSource = "All ready LIKED";
                    //}
                    //if (isContain)
                    //{
                    //    FollowedPageSource = "All ready LIKED";
                    //}
                    //else
                    //{
                    //string PostData = "&pk=" + PhotoId + "&t=653"; 
                    #endregion

                    #region commented
                    //string PostData = "&pk=" + PhotoId + "&t=" + RandomNumber() + "";
                    //namevalue.Add("Accept-Language", "en-us,en;q=0.5");
                    //namevalue.Add("X-Requested-With", "XMLHttpRequest");
                    //namevalue.Add("Origin", "http://web.stagram.com");
                    //namevalue.Add("X-Requested-With", "XMLHttpRequest");
                    //FollowedPageSource = accountManager.httpHelper.postFormDataForFollowUser(new Uri("http://web.stagram.com/do_like/"), PostData.Trim(), Photolink, namevalue);
                    //if (FollowedPageSource.Contains("\"message\":null}"))
                    //{
                    //    FollowedPageSource = "All ready LIKED";
                    //} 
                    #endregion
                                        
                    FollowedPageSource = "LIKED";

                    try
                    {
                        if (DemoStagramPro.ClGlobul.checkHashTagLiker == true)
                        {
                            try
                            {
                                DataBaseHandler.InsertQuery("insert into liker_hash_tag (account_holder, photo_id, like_date, like_status) values ('" + accountManager.Username + "','" + PhotoId + "','" + Convert.ToString(DateTime.Now) + "','" + FollowedPageSource + "')", "liker_hash_tag");
                            }
                            catch
                            { }
                        }
                    }
                    catch(Exception ex)
                    { }
                   

                }
                else if (string.IsNullOrEmpty(FollowedPageSource))
                {
                    FollowedPageSource = "Already LIKED";
                }
            }
            catch
            {
            }
            return FollowedPageSource;
        }
        #endregion

        #region RandomNumber
        public static int RandomNumber()
        {
            int min = 600;
            int max = 750;
            Random random = new Random();
            return random.Next(min, max);
        }
        #endregion

    }
}
