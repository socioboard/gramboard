using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using BaseLib;
using InstagramManager.Classes;
using Globussoft;
using System.Data;
using System.Threading;
namespace DemoStagramPro.Classes
{
    class HashTag
    {
        #region GlobalVariables
        public static Events loggerHashTag = new Events();        
        int counterFollow = 0;
        int counterLike = 0;
        int counterComment = 0;
        int i = 0;
        #endregion GlobalVariables
        private static int unlikeCompletionCount = 0;
        static readonly Object _lockObject = new Object();
        
        public void HashTagFollow(ref InstagramAccountManager accountManager, List<string> Usercount)
        {

            try
            {
                GloBoardPro.HasTagListListThread.Add(Thread.CurrentThread);
                GloBoardPro.HasTagListListThread.Distinct();
                Thread.CurrentThread.IsBackground = true;
            }
            catch
            {
            }
            #region variables

            //List<string> Usercount = new List<string>();
            #endregion variables

            #region commented
           
            #endregion
            try
            {
                foreach (string urlToFollow in Usercount)
                {
                    FollowUrls(ref accountManager, urlToFollow);
                    Thread.Sleep(ClGlobul.hashTagDelay * 1000);

                    Log("[ " + DateTime.Now + "] " + "[Delay is " + ClGlobul.hashTagDelay + " sec. ]");
                   


                }
            }


            catch (Exception ex)
            {

            }
            finally
            { 
                //process completed.
                ClGlobul.countNoOFAccountHashFollower--;
                if (ClGlobul.countNoOFAccountHashFollower == 0)
                {
                    Log("[ " + DateTime.Now + "] " + "[Process completed for Follow Using HashTag. ]"); 
                }
            }
          
        }

        public void HashTagLike(ref InstagramAccountManager accountManager, List<string> hashTag)//string hashTag
        {
           
            

            try
            {
                foreach (string urlToLike in hashTag)//hashTag //snapsVideoUrl
                {


                    LikeSnapsVideos(ref accountManager, urlToLike);
                    Thread.Sleep(ClGlobul.hashTagFolloweDelay * 1000);

                    Log("[ " + DateTime.Now + "] " + "[Delay is " + ClGlobul.hashTagDelay + " sec. ]");

                }
            }
            
            catch(Exception ex)
            {
 
            }

            finally
            {
                //process completed.
                ClGlobul.countNOOfFollowersandImageDownload--;
                if (ClGlobul.countNOOfFollowersandImageDownload == 0)
                {
                    Log("[ " + DateTime.Now + "] " + "[Process completed for Like Using HashTag. ]");
                }
            }
        }

        public void HashTagComment(ref InstagramAccountManager accountManager, List<string> hashTag)
        {

            try
            {
                foreach (string urlToComment in hashTag)
                {
                    LikeSnapsVideos(ref accountManager, urlToComment);
                    Thread.Sleep(ClGlobul.hashTagFolloweDelay * 1000);

                    Log("[ " + DateTime.Now + "] " + "[Delay is " + ClGlobul.hashTagDelay + " sec. ]");
                 
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                //process completed.
                ClGlobul.countNoOFAccountHashComment--;
                if (ClGlobul.countNoOFAccountHashComment== 0)
                {
                    Log("[ " + DateTime.Now + "] " + "[Process completed for Comment Using HashTag. ]");
                }
            }
        }
        int mindelay = 5;
        int maxdelay = 10;
        public void FollowUrls(ref InstagramAccountManager accountManager, string url)
        {
            //Thread.Sleep(2000);
            string followStatus = string.Empty;
            try
            {
                string user = accountManager.Username;
                InstagramFollow objInstagramFollow = new InstagramFollow();
                try
                {
                    DataSet DS = DataBaseHandler.SelectQuery("Select FollowStatus from FollowInfo where AccountHolder='" + user + "' and FollowingUser='" + url + "'", "FollowInfo");
                    if (DS.Tables[0].Rows.Count != 0)
                    {
                        followStatus = DS.Tables[0].Rows[0].ItemArray[0].ToString();
                    }
                }
                catch(Exception ex)
                { }
                if (!(followStatus == "Followed"))
                {
                    //if (!(counterFollow == ClGlobul.NumberOfProfilesToFollow))
                    {
                        
                        string status = objInstagramFollow.Follow(url, ref accountManager);
                       // Thread.Sleep(ClGlobul.hashTagDelay * 1000);
                        counterFollow++;
                        if (status == "Followed")
                        {
                            Log("[ " + DateTime.Now + "] " + "[ Profile followed with url : " + url + " with User = "+user+" ]");
                        }
                        else
                        {
                            Log("[ " + DateTime.Now + "] " + "[ Profile not followed with url : " + url + " with User = " + user + " ]");
                            //Log("[ " + DateTime.Now + "] " + " [ " + ClGlobul.NumberOfProfilesToFollow + " profiles Unfollowed ]");
                        }
                    }
                   //else
                    {
                        //ClGlobul.isFollowLimitReached = true;
                        //Log("[ " + DateTime.Now + "] " + " [ " + ClGlobul.NumberOfProfilesToFollow + " profiles followed ]");
                       // return;
                    }
                }
            }
            catch (Exception ex)
            {
                GramBoardProFileHelper.AppendStringToTextfileNewLine("DateTime :- " + DateTime.Now + " :: Error --> btnMsgFrom_Click() --> " + ex.Message + "StackTrace --> >>>" + ex.StackTrace, GramBoardProFileHelper.ErrorLogFilePath);
            }
        }

        public void LikeSnapsVideos(ref InstagramAccountManager accountManager, string url)
        {
            string user = accountManager.Username;
            string likeStatus = string.Empty;
            ClGlobul.checkHashTagLiker = true;
            InstagramPhotoLike objInstagramPhotoLike = new InstagramPhotoLike();
            try
            {
                DataSet DS = DataBaseHandler.SelectQuery("select like_status from liker_hash_tag where account_holder ='" + user + "' and photo_id ='" + url.Replace("http://websta.me/p/", string.Empty) + "'", "liker_hash_tag");
                if (DS.Tables[0].Rows.Count != 0)
                {
                    likeStatus = DS.Tables[0].Rows[0].ItemArray[0].ToString();
                }
            }
            catch(Exception ex)
            { }
            try
            {
                if (!(likeStatus == "LIKED"))
                {
                    if (!(counterLike == ClGlobul.NumberofSnapsVideosToLike))
                    {
                        string response = objInstagramPhotoLike.photolike(url, ref accountManager);
                        Thread.Sleep(ClGlobul.hashTagDelay * 1000);
                        counterLike++;
                        if (response == "LIKED")
                        {
                            Log("[ " + DateTime.Now + "] " + "[ Snap/Video liked with url : " + url + " ]");

                        }
                        else
                        {
                            Log("[ " + DateTime.Now + "] " + "[ Snap/Video not liked with url : " + url + " ]");
                        }
                    }
                    else
                    {
                        ClGlobul.isLikeLimitReached = true;
                        Log("[ " + DateTime.Now + "] " + " [ " + ClGlobul.NumberofSnapsVideosToLike + " snaps/videos liked ]");
                        return;
                    }
                }

            }
            catch(Exception ex)
            {
            }
        }

        
        public void CommentOnSnapsVideos(ref InstagramAccountManager accountManager, string url)
        {
            Comments objComments = new Comments();
            InstagramPhotoLike objInstagramPhotoLike = new InstagramPhotoLike();
            string message = string.Empty;
            ClGlobul.checkHashTagComment = true;
            string user = accountManager.Username;
            string commentStatus = string.Empty;
            string likeStatus = string.Empty;
            try
            {
                try
                {
                    if (ClGlobul.HashCommentMessage.Count == ClGlobul.NumberofSnapsVideosToComment)
                    {
                        message = ClGlobul.HashCommentMessage[i];
                        i++;
                    }
                    else
                    {
                        message = ClGlobul.HashCommentMessage[RandomNumberGenerator.GenerateRandom(0, ClGlobul.HashCommentMessage.Count)];
                    }
                }
                catch (Exception ex)
                { }
                try
                {
                    try
                    {
                        DataSet commentDS = DataBaseHandler.SelectQuery("select comment_status from comment_hash_tag where account_holder ='" + user + "' and photo_id = '" + url.Replace("http://websta.me/p/", string.Empty) + "'", "comment_hash_tag");
                        if (commentDS.Tables[0].Rows.Count != 0)
                        {
                            commentStatus = commentDS.Tables[0].Rows[0].ItemArray[0].ToString();
                        }
                    }
                    catch(Exception ex)
                    { }
                    try
                    {
                        DataSet likeDS = DataBaseHandler.SelectQuery("select like_status from liker_hash_tag where account_holder ='" + user + "' and photo_id ='" + url.Replace("http://websta.me/p/", string.Empty) + "'", "liker_hash_tag");
                        if (likeDS.Tables[0].Rows.Count != 0)
                        {
                            likeStatus = likeDS.Tables[0].Rows[0].ItemArray[0].ToString();
                        }
                    }
                    catch (Exception ex)
                    { }

                }
                catch(Exception ex)
                {}

                if (!(commentStatus == "Success"))
                {
                    if (!(counterComment == ClGlobul.NumberofSnapsVideosToComment))
                    {
                        string status = objComments.Comment(url, message, ref accountManager);
                        Thread.Sleep(ClGlobul.hashTagDelay * 1000);
                        if (status == "Success")
                        {
                            Log("[ " + DateTime.Now + "] " + "[ Commented on snap/video with url : " + url + " with this message : " + message + " ]");
                        }
                        else
                        {
                            Log("[ " + DateTime.Now + "] " + "[ Could not comment on snap/video with url : " + url + " ]");
                        }

                        if (ClGlobul.isCommentAndLikeChecked == true)
                        {
                            if (!(likeStatus == "LIKED"))
                            {
                                ClGlobul.checkHashTagLiker = true;
                                string photoStatus = objInstagramPhotoLike.photolike(url, ref accountManager);
                                Thread.Sleep(ClGlobul.hashTagDelay * 1000);
                                if (photoStatus == "LIKED")
                                {
                                    Log("[ " + DateTime.Now + "] " + "[ Snap/Video liked with url : " + url + " ]");
                                }
                                else
                                {
                                    Log("[ " + DateTime.Now + "] " + "[ Snap/Video not liked with url : " + url + " ]");
                                }
                            }
                            else
                            {
                                Log("[ " + DateTime.Now + "] " + "[ Snap/video with url : " + url + " is already liked. ]");
                            }
                        }
                        counterComment++;
                    }
                    else
                    {
                        ClGlobul.isCommentLimitReached = true;
                        Log("[ " + DateTime.Now + "] " + "[ Commented on " + ClGlobul.NumberofSnapsVideosToLike + " snaps/videos. ");
                        return;
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        #region logger Log
        public Events logger = new Events();

        private void Log(string log)
        {
            try
            {
                EventsArgs loggerEventsArgs = new EventsArgs(log);
                loggerHashTag.LogText(loggerEventsArgs);
            }
            catch (Exception ex)
            {
               // GlobusFileHelper.AppendStringToTextfileNewLine(DateTime.Now + " --> Error --> PROFILE RANK -  LinkedInProfilerank()  --> " + ex.Message + "StackTrace --> >>>" + ex.StackTrace, Globals.Path_LinkedinErrorLogs);
               // GlobusFileHelper.AppendStringToTextfileNewLine("Error --> PROFILE RANK -  Log() >>>> " + ex.Message + "StackTrace --> >>>" + ex.StackTrace + " || DateTime :- " + DateTime.Now, Globals.Path_LinkedInProfileRankErrorLogs);
            }

        }
        #endregion

        #region GetBetween
        public string getBetween(string strSource, string strStart, string strEnd)
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
