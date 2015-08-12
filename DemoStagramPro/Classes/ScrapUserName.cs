using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Globussoft;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using DemoStagramPro.Classes;
using InstagramManager.Classes;
using BaseLib;
using System.Threading;

namespace DemoStagramPro.Classes
{
    class ScrapUserName
    {
        public static List<Thread> lstScrapThread = new List<Thread>();
        public static bool stopScrapBool = false;
        private const string mainUrl = "http://websta.me/";
        private const string CSVHeader = "HashTag,UserName,UserLink";
        private const string userLink = "http://websta.me/n/";
        private string CSVPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Gram BoardPro\\scrapedUserDetails.csv";

        //public static string filepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Instagram\\Downloaded_Image";

        private static readonly Object _lockObject = new Object();


        public void startHash(int delay)
        {

            try
            {
                GloBoardPro.lstThread.Add(Thread.CurrentThread);
                Thread.CurrentThread.IsBackground = true;
                GloBoardPro.lstThread = GloBoardPro.lstThread.Distinct().ToList();
            }
            catch { };

            try
            {

                foreach (string itemHash in ClGlobul.HashTagForScrap)
                {
                    startUserScraper(itemHash,delay);

                    Thread.Sleep(5000);
                }

                GramBoardLogHelper.log.Info("[" + DateTime.Now + " ]=>[Process Completed]");
                //HashLogger.printLogger("[  Process Completed ]");
               // AddTophotoLogger("[ " + DateTime.Now + " ] =>[Process completed.");
            }
            catch { }
        }

        Dictionary<string, string> duplicateurl = new Dictionary<string, string>();
        int mindelay = 5;
        int maxdelay = 10;
        public void startUserScraper(string itemHash, int delay)

        {
            string pageSource = string.Empty;
            string response = string.Empty;
            string postData = string.Empty;
            List<string> lstCountScrapUser = new List<string>();

            if (stopScrapBool) return;
            try
            {
                GlobDramProHttpHelper _GlobusHttpHelper = new GlobDramProHttpHelper();
                pageSource = _GlobusHttpHelper.getHtmlfromUrl(new Uri(mainUrl),"");
                if (!string.IsNullOrEmpty(pageSource))
                {
                    //if (itemHash.StartsWith("#"))
                    //{
                    //    postData = "q=" + Uri.EscapeDataString(itemHash);
                    //}
                    //else
                    //{
                    //    postData = "q=" + Uri.EscapeDataString("#") + itemHash;
                    //}

                    postData = "q=" + Uri.EscapeDataString(itemHash);
                    string url = "http://websta.me/search/" + postData.Substring(postData.IndexOf("=") + 1);
                    string referer = "http://websta.me/";
                    response = _GlobusHttpHelper.postFormData(new Uri(url), postData, referer, "");

                    if (!string.IsNullOrEmpty(response))
                    {
                        if (response.Contains("class=\"username\""))
                        {
                            try
                            {
                                string[] arrOfUserName = Regex.Split(response, "class=\"username\"");

                                if (arrOfUserName.Length > 0)
                                {
                                    arrOfUserName = arrOfUserName.Skip(1).ToArray();
                                    foreach (string itemArray in arrOfUserName)
                                    {
                                        if (stopScrapBool) return;
                                        try
                                        {
                                            string startString = "href=\"/n/";
                                            string endString = "\">";
                                            if (itemArray.Contains(startString) && itemArray.Contains(endString))
                                            {
                                                string userName = string.Empty;
                                                try
                                                {
                                                    userName = getBetween(itemArray, startString, endString);
                                                    lstCountScrapUser.Add(userName);
                                                    lstCountScrapUser = lstCountScrapUser.Distinct().ToList();


                                                    if (!string.IsNullOrEmpty(userLink))
                                                    {
                                                        duplicateurl.Add(userLink + userName, userLink + userName);
                                                        #region CSV Write
                                                        try
                                                        {

                                                            string CSVData = itemHash.Replace(",", string.Empty) + "," + userName.Replace(",", string.Empty) + "," + (userLink + userName).Replace(",", string.Empty);
                                                            GramBoardProFileHelper.ExportDataCSVFile(CSVHeader, CSVData, CSVPath);
                                                        }
                                                        catch { }
                                                        try
                                                        {

                                                        GramBoardLogHelper.log.Info("["+ userName +","+ "itemHash:" +","+ itemHash +","+ "userName:" +","+ userName +","+ "userLink:" +","+ userLink + ","+" UserName" +userName +"]");

                                                        }
                                                        catch { };

                                                        #endregion

                                                        try
                                                        {
                                                            if (stopScrapBool) return;
                                                            lock (_lockObject)
                                                            {
                                                                try
                                                                {
                                                                    GramBoardLogHelper.log.Info("=> [  UserName" + userName + " ]");
                                                                    //HashLogger.printLogger("[ " + DateTime.Now + " ] => [ Delay for " + delay + " seconds ]");
                                                                    //GramBoardLogHelper.log.Info(" => [ Delay for " + delay + " seconds ]");
                                                                    //Thread.Sleep(delay * 1000);

                                                                    frm_stagram objfrm_stagram = (frm_stagram)Application.OpenForms["frm_stagram"];
                                                                   
                                                                    if (!string.IsNullOrEmpty(objfrm_stagram.txtDelayHashTag.Text) && NumberHelper.ValidateNumber(objfrm_stagram.txtDelayHashTag.Text))
                                                                    {
                                                                        mindelay = Convert.ToInt32(objfrm_stagram.txtDelayHashTag.Text);
                                                                    }
                                                                    if (!string.IsNullOrEmpty(objfrm_stagram.MinHashTagMinDelay.Text) && NumberHelper.ValidateNumber(objfrm_stagram.MinHashTagMinDelay.Text))
                                                                    {
                                                                        maxdelay = Convert.ToInt32(objfrm_stagram.MinHashTagMinDelay.Text);
                                                                    }

                                                                    Random obj_rn = new Random();
                                                                    int delay1 = RandomNumberGenerator.GenerateRandom(mindelay, maxdelay);
                                                                    delay1 = obj_rn.Next(mindelay, maxdelay);
                                                                    GramBoardLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Delay For " + delay1 + " Seconds ]");
                                                                    Thread.Sleep(delay1 * 1000);
                                                                }
                                                                catch { }
                                                            }
                                                            if (lstCountScrapUser.Count >= ClGlobul.countNOOfFollowersandImageDownload)
                                                            {
                                                                return;
                                                            }
                                                        }
                                                        catch { }
                                                    }
                                                }
                                                catch { }
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                            catch { }
                        }
                    }//End of if (!string.IsNullOrEmpty(response))
                    else
                    {
                        //Do Nothing yet
                    }

                }//End of if (!string.IsNullOrEmpty(pageSource))
            }
            catch { }
            //GlobusLogHelper.log.Info("[" + DateTime.Now + " ]=>[Process Completed]");
            //HashLogger.printLogger("[ " + DateTime.Now + " ] => [  Process Completed ]");
        }//End of for-each (string itemHash in ClGlobul.HashTagForScrap)
              
        private void AddToImageTagLogger(string log)
        {
            //try
            //{
            //    if (lstImageLogger.InvokeRequired)
            //    {
            //        lstImageLogger.Invoke(new MethodInvoker(delegate
            //        {
            //            lstImageLogger.Items.Add(log);
            //            lstImageLogger.SelectedIndex = lstHashLogger.Items.Count - 1;
            //        }));
            //    }
            //    else
            //    {
            //        lstImageLogger.Items.Add(log);
            //        lstImageLogger.SelectedIndex = lstHashLogger.Items.Count - 1;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // AddToCommentLogger("Error : AddToImageTagLogger :-" + ex.Message);
            //}
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
    }
}
