using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstagramManager.Classes;
using System.Collections.Specialized;
using BaseLib;
using System.Data;
using Globussoft;

namespace InstagramManager.Classes
{
    class Comments
    {
        #region Comment
        public string Comment(string commentId, string CommentMsg, ref InstagramAccountManager accountManager)
        {
            NameValueCollection namevalue = new NameValueCollection();
            string FollowedPageSource = string.Empty;
            try
            {
                string CommentIdlink = string.Empty;
                string commentIdLoggedInLink = string.Empty;
                if (commentId.Contains("http://websta.me/p/"))
                {
                    commentId = commentId.Replace("http://websta.me/p/", string.Empty);
                }

                if (!commentId.Contains("http://web.stagram.com/"))
                {
                    try
                    {

                        CommentIdlink = "http://web.stagram.com/p/" + commentId + "/";

                        commentIdLoggedInLink = "http://websta.me/p/" + commentId;
                    }
                    catch(Exception ex)
                    {
                        return ex.Message;
                    }
                }

                #region Change
                //GlobusHttpHelper _GlobusHttpHelper = new GlobusHttpHelper();

                //ChilkatHttpHelpr _ChilkatHttpHelpr = new ChilkatHttpHelpr();

                //InstagramAccountManager _InstagramAccountManager = new InstagramAccountManager(accountManager.Username, accountManager.Password, accountManager.proxyAddress, accountManager.proxyPassword, accountManager.proxyUsername, accountManager.proxyPassword);

                string url = "http://websta.me/api/comments/" + commentId;

                bool checkunicode = ContainsUnicodeCharacter(CommentMsg);

                string CmntMSG = string.Empty;
               

                if (checkunicode == false)
                {
                    try
                    {
                        CmntMSG = CommentMsg.Replace(" ", "+");
                       
                    }
                    catch(Exception ex)
                    {
                        return ex.Message;
                    };
                }
                else
                {
                    try
                    {
                        CmntMSG = Uri.EscapeDataString(CommentMsg);
                    }
                    catch(Exception ex) 
                    {
                        return ex.Message;
                    };
                }

              //  string commentPostData = "comment=+" + CmntMSG + "&media_id=" + commentId;
                try
                {
                    string commentPostData = "comment=+++" + CmntMSG + "&media_id=" + commentId;

                    FollowedPageSource = accountManager.httpHelper.postFormData(new Uri(url), commentPostData, CommentIdlink, "");
                }
                catch(Exception ex)
                {
                    return ex.Message;
                }

                if (FollowedPageSource.Contains("status\":\"OK\"") || FollowedPageSource.Contains("created_time"))
                {
                    try
                    {
                        FollowedPageSource = "Success";
                    }
                    catch(Exception ex) 
                    {
                        return ex.Message;
                    };
                }
                else
                {
                    try
                    {
                        FollowedPageSource = "Fail";
                    }
                    catch(Exception ex) 
                    {
                        return ex.Message;
                    };
                }
                #endregion


                #region commented
                //string firstUrl = "https://api.instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes";
                //string secondURL = "https://instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes";
                //string res_secondURL =_InstagramAccountManager.httpHelper.getHtmlfromUrlProxy(new Uri(secondURL), proxyAddress, 80, proxyUsername, proxyPassword);

                //string nextUrl = "https://instagram.com/accounts/login/?force_classic_login=&next=/oauth/authorize/%3Fclient_id%3D9d836570317f4c18bca0db6d2ac38e29%26redirect_uri%3Dhttp%3A//websta.me/%26response_type%3Dcode%26scope%3Dcomments%2Brelationships%2Blikes";
                //string res_nextUrl = _InstagramAccountManager.httpHelper.getHtmlfromUrlProxy(new Uri(nextUrl), proxyAddress, 80, proxyUsername, proxyPassword);

                //int FirstPointToken_nextUrl = res_nextUrl.IndexOf("csrfmiddlewaretoken");
                //string FirstTokenSubString_nextUrl = res_nextUrl.Substring(FirstPointToken_nextUrl);
                //int SecondPointToken_nextUrl = FirstTokenSubString_nextUrl.IndexOf("/>");
                //string Token = FirstTokenSubString_nextUrl.Substring(0, SecondPointToken_nextUrl).Replace("csrfmiddlewaretoken", string.Empty).Replace("value=", string.Empty).Replace("\"", string.Empty).Replace("'", string.Empty).Trim();
                //string Token = string.Empty;
                //try
                //{
                //    Token = getBetween(res_nextUrl, "accessToken', '", "')");
                //}
                //catch { }

                //string login = "https://instagram.com/accounts/login/?force_classic_login=&next=/oauth/authorize/%3Fclient_id%3D9d836570317f4c18bca0db6d2ac38e29%26redirect_uri%3Dhttp%3A//websta.me/%26response_type%3Dcode%26scope%3Dcomments%2Brelationships%2Blikes";
                //string postdata_Login = "csrfmiddlewaretoken=" + Token + "&username=" + Username + "&password=" + Password + "";

                //string res_postdata_Login = _InstagramAccountManager.httpHelper.postFormData(new Uri(login), postdata_Login, login, "");

                //string PageContent = string.Empty;
                //PageContent = _GlobusHttpHelper.getHtmlfromUrl(new Uri(commentIdLoggedInLink), "", "", _InstagramAccountManager.proxyPassword);
                ////if (res_postdata_Login.Contains("logout") || postdata_Login.Contains("LOG OUT"))
                ////{
                ////    PageContent = _InstagramAccountManager.httpHelper.getHtmlfromUrl(new Uri(CommentIdlink), "", "", accountManager.proxyPassword);
                ////PageContent = _InstagramAccountManager.httpHelper.getHtmlfromUrl(new Uri(commentIdLoggedInLink));

                //PageContent = _GlobusHttpHelper.getHtmlfromUrl(new Uri(CommentIdlink), "", "", accountManager.proxyPassword);
                //PageContent = _GlobusHttpHelper.getHtmlfromUrl(new Uri(commentIdLoggedInLink));
                ////}



                //string PageContent = accountManager.httpHelper.getHtmlfromUrl(new Uri(CommentIdlink), "", "", accountManager.proxyAddress);
                //string PageContent = accountManager.httpHelper.getHtmlfromUrl(new Uri(CommentIdlink), "", "", accountManager.proxyPassword);

                //  if (PageContent.Contains("id=\"textarea"))
                // if (PageContent.Contains("<div class=\"comments"))
                //{
                //check unicode character
                //if (success.Equals("Success"))
                //{
                //    bool checkunicode = ContainsUnicodeCharacter(CommentMsg);

                //    string CmntMSG = string.Empty;

                //    if (checkunicode == false)
                //    {
                //        CmntMSG = CommentMsg.Replace(" ", "+");
                //    }
                //    else
                //    {
                //        CmntMSG = Uri.EscapeDataString(CommentMsg);
                //    }

                //    string commentPostData = "comment=+" + CmntMSG + "&media_id=" + commentId;

                //    FollowedPageSource=_GlobusHttpHelper.postFormData(new Uri("http://websta.me/api/comments/" + commentId),commentPostData,commentIdLoggedInLink,"");

                //    //string commentPostData = ("message=" + CmntMSG + "&messageid=" + commentId + "&t=" + RandomNumber() + "").Trim();
                //    //string commentPostData = "comment=+" + CmntMSG + "&media_id="+commentId;
                //   // string commentPostData = ("comment=+" + CmntMSG + "&media_id=" + commentId + "".Trim());


                //   // // comment=+heloo&media_id=815573304185069562_3373974
                //   // //comment=+hi&media_id=815582504685487428_17999944
                //   // //namevalue.Add("Accept-Language", "en-us,en;q=0.5");
                //   // namevalue.Add("Accept-Language", "en-US,en;q=0.8");
                //   // namevalue.Add("Accept-Encoding", "gzip,deflate");
                //   // namevalue.Add("X-Requested-With", "XMLHttpRequest");
                //   // //namevalue.Add("Origin", "http://web.stagram.com");
                //   // namevalue.Add("Origin", "http://websta.me");
                //   // namevalue.Add("X-Requested-With", "XMLHttpRequest");

                //   //// FollowedPageSource = accountManager.httpHelper.postFormDataForFollowUser(new Uri("http://web.stagram.com/post_comment/"), commentPostData, CommentIdlink, namevalue);
                //   // //FollowedPageSource = accountManager.httpHelper.postFormDataForFollowUser(new Uri("http://websta.me/api/comments/"), commentPostData, CommentIdlink, namevalue);
                //   // //FollowedPageSource = _GlobusHttpHelper.postFormDataForFollowUser(new Uri("http://websta.me/api/comments/"), commentPostData, CommentIdlink, namevalue);
                //   // //FollowedPageSource = _InstagramAccountManager.httpHelper.postFormDataForFollowUser(new Uri("http://websta.me/api/comments/" + commentId), commentPostData, CommentIdlink, namevalue);
                //   // //FollowedPageSource = _GlobusHttpHelper.postFormDataForFollowUserNew(new Uri("http://websta.me/api/comments/" + commentId), commentPostData, commentIdLoggedInLink, namevalue);

                //} 
                #endregion

                #endregion

                try
                {
                    if(DemoStagramPro.ClGlobul.checkHashTagComment == true)
                    {
                        try
                        {
                        DataBaseHandler.InsertQuery("insert into comment_hash_tag (account_holder, photo_id, comment_date, comment_status) values ('" + accountManager.Username + "','" + commentId + "','" + Convert.ToString(DateTime.Now) + "','" + FollowedPageSource + "')", "comment_hash_tag");
                        }
                        catch(Exception ex)
                        {}
                    }
                }
                catch
                {}
            }
            catch
            {
                FollowedPageSource = string.Empty;
            }
            return FollowedPageSource;
        } 
        

        #region GetBetween
        /// <summary>
        /// Return string between start and end
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="strStart"></param>
        /// <param name="strEnd"></param>
        /// <returns></returns>
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


        #region LogInForComment
        //public string LogInForComment()
        //{
        //    //Log("[ " + DateTime.Now + " ] => [ Logging in with Account : " + Username + " ]");
        //    string Status = "Failed";
        //    try
        //    {
        //        string firstUrl = "https://api.instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes";

        //        //https://instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes

        //        string secondURL = "https://instagram.com/oauth/authorize/?client_id=9d836570317f4c18bca0db6d2ac38e29&redirect_uri=http://websta.me/&response_type=code&scope=comments+relationships+likes";
        //        string res_secondURL = httpHelper.getHtmlfromUrlProxy(new Uri(secondURL), proxyAddress, 80, proxyUsername, proxyPassword);

        //        string nextUrl = "https://instagram.com/accounts/login/?force_classic_login=&next=/oauth/authorize/%3Fclient_id%3D9d836570317f4c18bca0db6d2ac38e29%26redirect_uri%3Dhttp%3A//websta.me/%26response_type%3Dcode%26scope%3Dcomments%2Brelationships%2Blikes";
        //        string res_nextUrl = httpHelper.getHtmlfromUrlProxy(new Uri(nextUrl), proxyAddress, 80, proxyUsername, proxyPassword);

        //        #region << Get Token >>
        //        //Get Token Number of Id 
        //        int FirstPointToken_nextUrl = res_nextUrl.IndexOf("csrfmiddlewaretoken");
        //        string FirstTokenSubString_nextUrl = res_nextUrl.Substring(FirstPointToken_nextUrl);
        //        int SecondPointToken_nextUrl = FirstTokenSubString_nextUrl.IndexOf("/>");
        //        this.Token = FirstTokenSubString_nextUrl.Substring(0, SecondPointToken_nextUrl).Replace("csrfmiddlewaretoken", string.Empty).Replace("value=", string.Empty).Replace("\"", string.Empty).Replace("'", string.Empty).Trim();
        //        #endregion


        //        string login = "https://instagram.com/accounts/login/?force_classic_login=&next=/oauth/authorize/%3Fclient_id%3D9d836570317f4c18bca0db6d2ac38e29%26redirect_uri%3Dhttp%3A//websta.me/%26response_type%3Dcode%26scope%3Dcomments%2Brelationships%2Blikes";
        //        string postdata_Login = "csrfmiddlewaretoken=" + this.Token + "&username="+Username+"&password="+Password+"";

        //        string res_postdata_Login = httpHelper.postFormData(new Uri(login), postdata_Login, login, "");

        //        #region Commented Code
        //        //NameValueCollection nameval = new NameValueCollection();

        //        //int BasePort = 80;

        //        //if (!string.IsNullOrEmpty(this.proxyPort))
        //        //{
        //        //    BasePort = int.Parse(this.proxyPort);
        //        //}


        //        //string BasePageContent = string.Empty;
        //        //try
        //        //{
        //        //    BasePageContent = httpHelper.getHtmlfromUrlProxy(new Uri("http://web.stagram.com/"), proxyAddress, BasePort, proxyUsername, proxyPassword);
        //        //}
        //        //catch (Exception ex)
        //        //{
        //        //    if (ex.Message == ("Object reference not set to an instance of an object."))
        //        //    {
        //        //        BasePageContent = string.Empty;
        //        //        BasePageContent = httpHelper.getHtmlfromUrlProxy(new Uri("http://web.stagram.com/"), proxyAddress, BasePort, proxyUsername, proxyPassword);
        //        //    }
        //        //}

        //        //#region <<Get Client ID >>
        //        ////Get Client ID 
        //        //int FirstPointClientId = BasePageContent.IndexOf("client_id=");
              
        //        //string FirstClientIdSubString = BasePageContent.Substring(FirstPointClientId);
        //        //int SecondPointClientId = FirstClientIdSubString.IndexOf("&redirect_uri=");
        //        //this.ClientId = FirstClientIdSubString.Substring(0, SecondPointClientId).Replace("'", string.Empty).Replace("client_id=", string.Empty).Trim();
        //        //#endregion

        //        //#region << Get Login page >>
        //        //string LoginUrl = "https://instagram.com/accounts/login/?next=/oauth/authorize/%3Fclient_id%3D" + ClientId + "%26redirect_uri%3Dhttp%253A%252F%252Fweb.stagram.com%252F%26response_type%3Dcode%26scope%3Dlikes%2Bcomments%2Brelationships";
        //        //string LoginPageContent = httpHelper.getHtmlfromUrl(new Uri(LoginUrl), "http://web.stagram.com/", nameval);
        //        //#endregion

        //        //#region << Get Token >>
        //        ////Get Token Number of Id 
        //        //int FirstPointToken = LoginPageContent.IndexOf("csrfmiddlewaretoken");
        //        //string FirstTokenSubString = LoginPageContent.Substring(FirstPointToken);
        //        //int SecondPointToken = FirstTokenSubString.IndexOf("/>");
        //        //this.Token = FirstTokenSubString.Substring(0, SecondPointToken).Replace("csrfmiddlewaretoken", string.Empty).Replace("value=", string.Empty).Replace("\"", string.Empty).Replace("'", string.Empty).Trim();
        //        //#endregion

        //        //nameval.Add("Origin", "https://instagram.com");
        //        //string pass = this.Password;
        //        //string PostData = "csrfmiddlewaretoken=" + this.Token + "&username=" + this.Username + "&password=" + (pass.Replace("\0", string.Empty).Trim());
        //        //string PostedPageContent = httpHelper.postFormData(new Uri(LoginUrl), PostData, LoginUrl, nameval);
        //        #endregion

        //        #region << If ask For Autherization >>

        //        //If ask For Autherization..Then get to autherized Id..
        //        //if (postdata_Login.Contains("is requesting to do the following") && postdata_Login.Contains("value=\"Authorize"))
        //        //{
        //        //    postdata_Login = string.Empty;
        //        //    string AuthrizationPostURl = "https://instagram.com/oauth/authorize/?client_id=" + ClientId + "&redirect_uri=http://web.stagram.com/&response_type=code&scope=likes+comments+relationships";
        //        //    string AuthrizationPostData = "csrfmiddlewaretoken=" + Token + "&allow=Authorize";
        //        //    string redirectURL = "https://instagram.com/oauth/authorize/?client_id=" + ClientId + "&redirect_uri=http://web.stagram.com/&response_type=code&scope=likes+comments+relationships";
        //        //    postdata_Login = httpHelper.postFormData(new Uri(AuthrizationPostURl), AuthrizationPostData, redirectURL, nameval);
        //        //}
        //        #endregion

        //        if (res_postdata_Login.Contains("Please enter a correct username and password"))
        //        {
        //            Status = "Failed";
        //            this.LoggedIn = false;
        //        }
        //        else if (res_postdata_Login.Contains("requesting access to your Instagram account") || postdata_Login.Contains("is requesting to do the following"))
        //        {
        //            Status = "AccessIssue";
        //        }
        //        else if (res_postdata_Login.Contains("logout") || postdata_Login.Contains("LOG OUT"))
        //        {
        //            Log("[ " + DateTime.Now + " ] => [ Logged in with Account :" + Username + " ]");
        //            Status = "Success";
        //            this.LoggedIn = true;
        //        }

        //        //nameval.Clear();
        //        return Status;
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;

        //    }
        //}
        #endregion

        #region ContainsUnicodeCharacter
        public bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;

            return input.Any(c => c > MaxAnsiCode);
        } 
        #endregion

        #region RandomNumber
        public static int RandomNumber()
        {
            int min = 1910;
            int max = 6690;
            Random random = new Random();
            return random.Next(min, max);
        } 
        #endregion

       

    }
}
