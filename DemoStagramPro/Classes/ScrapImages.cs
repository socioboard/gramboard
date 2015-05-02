using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Globussoft;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BaseLib;

namespace DemoStagramPro.Classes
{
    class ScrapImages
    {
        public static List<Thread> lstScrapImageThread = new List<Thread>();
        public static bool stopScrapImageBool = false;
        private const string mainUrl = "http://websta.me/";
        private const string userLink = "http://websta.me/n/";
        private const string CSVHeader = "HashTag,Image Link,Image Url,Status";
        private string CSVPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Gram BoardPro\\ImageDetails.csv";

        private static readonly Object _lockObject = new Object();



        public void DownloadingImage(int delay)
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

                foreach (string itemImageTag in ClGlobul.ImageTagForScrap)
                {
                    startDownloadingImage(itemImageTag,delay);

                    Thread.Sleep(5000);
                }
                classsforlogger objclasssforlogger1 = new classsforlogger();
                objclasssforlogger1.AddToImageTagLogger("[ " + DateTime.Now + " ] => [ Process Completed ]");
                // AddTophotoLogger("[ " + DateTime.Now + " ] =>[Process completed.");
            }
            catch { }
        }
        public void startDownloadingImage(string itemImageTag, int delay )
        {
            string pageSource = string.Empty;
            List<string> lstCountScrape = new List<string>();
              if (stopScrapImageBool) return;
                try
                {
                    GlobDramProHttpHelper _GlobusHttpHelper = new GlobDramProHttpHelper();
                    try
                    {
                        pageSource = _GlobusHttpHelper.getHtmlfromUrl(new Uri(mainUrl));
                    }
                    catch { }
                    if (!string.IsNullOrEmpty(pageSource))
                    {
                        string url = mainUrl + "tag/" + itemImageTag;
                        try
                        {
                            pageSource = _GlobusHttpHelper.getHtmlfromUrl(new Uri(url));
                        }
                        catch { }
                        if (!string.IsNullOrEmpty(pageSource))
                        {
                            if (pageSource.Contains("<div class=\"mainimg_wrapper\">"))
                            {
                                try
                                {
                                    string[] arr = Regex.Split(pageSource, "<div class=\"mainimg_wrapper\">");

                                    if (arr.Length > 1)
                                    {
                                        arr = arr.Skip(1).ToArray();
                                        foreach (string itemarr in arr)
                                        {
                                            if (stopScrapImageBool) return;
                                            try
                                            {
                                                string startString = "<a href=\"";
                                                string endString = "\" class=\"mainimg\"";
                                                string imageId = string.Empty;
                                                string imageSrc = string.Empty;
                                                if (itemarr.Contains("<a href=\"/p/"))
                                                {
                                                    int indexStart = itemarr.IndexOf("<a href=\"/p/");
                                                    string itemarrNow = itemarr.Substring(indexStart);
                                                    if (itemarrNow.Contains(startString) && itemarrNow.Contains(endString))
                                                    {
                                                        try
                                                        {
                                                            imageId = ScrapUserName.getBetween(itemarrNow, startString, endString);

                                                        }
                                                        catch { }
                                                        if (!string.IsNullOrEmpty(imageId))
                                                        {
                                                            imageId = "http://websta.me" + imageId;
                                                        }
                                                    }

                                                    if (itemarrNow.Contains("<img src=\""))
                                                    {
                                                        try
                                                        {
                                                            imageSrc = ScrapUserName.getBetween(itemarrNow, "<img src=\"", "\"");
                                                            lstCountScrape.Add(imageSrc);
                                                            lstCountScrape = lstCountScrape.Distinct().ToList();
                                                        }
                                                        catch { }
                                                    }
                                                }

                                                #region Get Comments
                                                string comments = string.Empty;
                                                try
                                                {
                                                    comments = getComments(itemarr);
                                                }
                                                catch { }
                                                #endregion



                                                #region CSV Write
                                                if (stopScrapImageBool) return;
                                                try
                                                {
                                                    string CSVData = itemImageTag.Replace(",", string.Empty) + "," + imageId.Replace(",", string.Empty) + "," + imageSrc.Replace(",", string.Empty) + "," + comments.Replace(",", string.Empty);
                                                    GramBoardProFileHelper.ExportDataCSVFile(CSVHeader, CSVData, CSVPath);
                                                    classsforlogger objclasssforlogger = new classsforlogger();
                                                    objclasssforlogger.AddToImageTagLogger("[ " + DateTime.Now + " ] => [ itemImageTag : " + itemImageTag + "   imageId : " + imageId + "   imageSrc : " + imageSrc + "  comments : " + comments);
                                                    //frm_stagram objbbbFrmMain = (frm_stagram)Application.OpenForms["frm_stagram"];
                                                    //objbbbFrmMain.lstImageLogger.Items.Add("hey ram");


                                                }
                                                catch { }
                                                #endregion

                                                #region Logger Show
                                                if (stopScrapImageBool) return;
                                                try
                                                {
                                                    classsforlogger objclasssforlogger = new classsforlogger();
                                                    //HashLogger.printLogger("[ " + DateTime.Now + " ] => [ " + imageId + " ]");
                                                    //HashLogger.printLogger("[ " + DateTime.Now + " ] => [ Delay for " + delay + " seconds ]");
                                                    objclasssforlogger.AddToImageTagLogger("[ " + DateTime.Now + " ] => [ " + imageId + " ]");
                                                    objclasssforlogger.AddToImageTagLogger("[ " + DateTime.Now + " ] => [ Delay for " + delay + " seconds ]");
                                                    Thread.Sleep(delay * 1000);

                                                    if (lstCountScrape.Count >= ClGlobul.countNOOfFollowersandImageDownload)
                                                    {
                                                        return;
                                                    }

                                                }
                                                catch { }
                                                #endregion

                                            }
                                            catch { }
                                        }

                                        if (lstCountScrape.Count >= ClGlobul.countNOOfFollowersandImageDownload)
                                        {
                                            return;
                                        }
                                        classsforlogger objclasssforlogger2 = new classsforlogger();
                                        objclasssforlogger2.AddToImageTagLogger("[ " + DateTime.Now + " ] => [ Process Completed ]");

                                        #region pagination
                                        string pageLink = string.Empty;
                                        while (true)
                                        {
                                            if (stopScrapImageBool) return;
                                            string startString = "<a href=\"";
                                            string endString = "\" class=\"mainimg\"";
                                            string imageId = string.Empty;
                                            string imageSrc = string.Empty;

                                            if (!string.IsNullOrEmpty(pageLink))
                                            {
                                                pageSource = _GlobusHttpHelper.getHtmlfromUrl(new Uri(pageLink));
                                            }

                                            if (pageSource.Contains("<ul class=\"pager\">") && pageSource.Contains("rel=\"next\">"))
                                            {
                                                try
                                                {
                                                    pageLink = ScrapUserName.getBetween(pageSource, "<ul class=\"pager\">", "rel=\"next\">");
                                                }
                                                catch { }
                                                if (!string.IsNullOrEmpty(pageLink))
                                                {
                                                    try
                                                    {
                                                        int len = pageLink.IndexOf("<a href=\"");
                                                        len = len + ("<a href=\"").Length;
                                                        pageLink = pageLink.Substring(len);
                                                        pageLink = pageLink.Trim();
                                                        pageLink = pageLink.TrimEnd(new char[] { '"' });
                                                        pageLink = "http://websta.me/" + pageLink;
                                                    }
                                                    catch { }
                                                    if (!string.IsNullOrEmpty(pageLink))
                                                    {
                                                        string response = string.Empty;
                                                        try
                                                        {
                                                            response = _GlobusHttpHelper.getHtmlfromUrl(new Uri(pageLink));
                                                        }
                                                        catch { }
                                                        if (!string.IsNullOrEmpty(response))
                                                        {
                                                            if (response.Contains("<div class=\"mainimg_wrapper\">"))
                                                            {
                                                                try
                                                                {
                                                                    string[] arr1 = Regex.Split(response, "<div class=\"mainimg_wrapper\">");
                                                                    if (arr1.Length > 1)
                                                                    {
                                                                        arr1 = arr1.Skip(1).ToArray();
                                                                        foreach (string items in arr1)
                                                                        {
                                                                            try
                                                                            {
                                                                                if (stopScrapImageBool) return;
                                                                                if (items.Contains("<a href=\"/p/"))
                                                                                {
                                                                                    int indexStart = items.IndexOf("<a href=\"/p/");
                                                                                    string itemarrNow = items.Substring(indexStart);

                                                                                    if (itemarrNow.Contains(startString) && itemarrNow.Contains(endString))
                                                                                    {
                                                                                        try
                                                                                        {
                                                                                            imageId = ScrapUserName.getBetween(itemarrNow, startString, endString);
                                                                                        }
                                                                                        catch { }
                                                                                        if (!string.IsNullOrEmpty(imageId))
                                                                                        {
                                                                                            imageId = "http://websta.me" + imageId;
                                                                                        }
                                                                                    }

                                                                                    if (itemarrNow.Contains("<img src=\""))
                                                                                    {
                                                                                        try
                                                                                        {
                                                                                            imageSrc = ScrapUserName.getBetween(itemarrNow, "<img src=\"", "\"");
                                                                                            lstCountScrape.Add(imageSrc);
                                                                                            lstCountScrape = lstCountScrape.Distinct().ToList();
                                                                                        }
                                                                                        catch { }
                                                                                    }

                                                                                    #region Get Comments
                                                                                    string comments = string.Empty;
                                                                                    try
                                                                                    {
                                                                                        comments = getComments(itemarrNow);
                                                                                    }
                                                                                    catch { }
                                                                                    #endregion

                                                                                    #region CSV Write
                                                                                    if (stopScrapImageBool) return;
                                                                                    try
                                                                                    {
                                                                                        string CSVData = itemImageTag.Replace(",", string.Empty) + "," + imageId.Replace(",", string.Empty) + "," + imageSrc.Replace(",", string.Empty) + "," + comments.Replace(",", string.Empty);
                                                                                        GramBoardProFileHelper.ExportDataCSVFile(CSVHeader, CSVData, CSVPath);
                                                                                    }
                                                                                    catch { }
                                                                                    #endregion

                                                                                    #region Logger Show
                                                                                    if (stopScrapImageBool) return;
                                                                                    try
                                                                                    {
                                                                                        classsforlogger objclasssforlogger = new classsforlogger();
                                                                                        objclasssforlogger.AddToImageTagLogger("[ " + DateTime.Now + " ] => [ " + imageId + " ]");
                                                                                        objclasssforlogger.AddToImageTagLogger("[ " + DateTime.Now + " ] => [ Delay for " + delay + " seconds ]");
                                                                                        Thread.Sleep(delay * 1000);

                                                                                        if (lstCountScrape.Count >= ClGlobul.countNOOfFollowersandImageDownload)
                                                                                        {
                                                                                            return;
                                                                                        }

                                                                                    }
                                                                                    catch { }
                                                                                    #endregion

                                                                                }

                                                                            }
                                                                            catch { }
                                                                        }
                                                                    }
                                                                }
                                                                catch { }

                                                            }
                                                        }
                                                        else
                                                        {

                                                        }

                                                    }
                                                    else
                                                    {
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        if (lstCountScrape.Count >= ClGlobul.countNOOfFollowersandImageDownload)
                                        {
                                            return;
                                        }
                                        #endregion
                                    }
                                }
                                catch { }
                            }
                        }//End of 2nd if (!string.IsNullOrEmpty(pageSource)) of tagged pageSource
                        else
                        {
                            //Tag PageSource is empty
                        }
                    }
                    else
                    {
                        //PageSource is empty
                    }
                }
            
                catch { };

                classsforlogger objclasssforlogger1 = new classsforlogger();
                objclasssforlogger1.AddToImageTagLogger("[ " + DateTime.Now + " ] => [ Process Completed ]");
            }
            
        #region Get Comments of Images
        /// <summary>
        /// Get Comments of the particular images
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private string getComments(string source)
        {
            string comments = string.Empty;
            string startString = "<div class=\"comment_body\">";
            string endString = "<span class=\"time\">";
            string zone = string.Empty;
            try
            {
                if (source.Contains(startString) && source.Contains(endString))
                {
                    try
                    {
                        zone = ScrapUserName.getBetween(source, startString, endString);
                    }
                    catch { }
                }
                if (!string.IsNullOrEmpty(zone))
                {
                    try
                    {
                        zone = ScrapUserName.getBetween(zone, "<strong>", "</strong>");
                        if (zone.Contains("<span"))
                        {
                            string badText = string.Empty;
                            try
                            {
                                badText = ScrapUserName.getBetween(zone, "<span", "</span>");
                            }
                            catch { }
                            if (!string.IsNullOrEmpty(badText))
                            {
                                zone = zone.Replace(badText, string.Empty);
                                zone = zone.Substring(0, zone.IndexOf("<span"));
                            }
                        }
                    }
                    catch { }
                }
                if (!string.IsNullOrEmpty(zone))
                {
                    if (!zone.Contains("<a href"))
                    {
                        comments = zone;
                        comments = comments.Trim();
                    }
                    else
                    {
                        string[] arrZone = Regex.Split(zone, "<a href");
                        comments = arrZone[0];
                        comments = comments.Trim();
                        arrZone = arrZone.Skip(1).ToArray();
                        foreach (string item_arrZone in arrZone)
                        {
                            try
                            {
                                comments = comments + " " + ScrapUserName.getBetween(item_arrZone, ">", "</a>");
                            }
                            catch { }
                        }
                        comments.Trim();
                    }
                }//End of if (!string.IsNullOrEmpty(zone))
            }
            catch { }
            if (comments.Contains("<br/>"))
            {
                comments = comments.Replace("<br/>", " ");
            }

            if (comments.Contains("\n"))
            {
                comments = comments.Replace("\n", string.Empty);
            }
            return comments;
        }
        #endregion

    }
}
