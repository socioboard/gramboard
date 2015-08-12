using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BaseLib;
using Globussoft;
using DemoStagramPro;


namespace StagramProxyChecker.classes
{
    public class ClProxyChecker
    {
        #region global declaration
        public static BaseLib.Events ProxyLogger = new BaseLib.Events();
        public static readonly object lockerforProxies = new object();
        public static readonly object lockerforNonWorkingProxies = new object();
        int countParseProxiesThreads = 0;
        int count_ThreadController = 0;
        frm_stagram frm = new frm_stagram();
        #endregion

        #region LoggerProxy
        private void LoggerProxy(string message)
        {
            EventsArgs eventArgs = new EventsArgs(message);
            ProxyLogger.LogText(eventArgs);
        } 
        #endregion

        public static List<Thread> lstProxyThread = new List<Thread>();
        public static bool proxyStop = false;

        #region proxycheckStart
        public void proxycheckStart()
        {
            Thread.CurrentThread.IsBackground = true;
            lstProxyThread.Add(Thread.CurrentThread);
            lstProxyThread = lstProxyThread.Distinct().ToList();
            Proxystatus = DemoStagramPro.ClGlobul.ProxyList.Count;
            
            LoggerProxy("[ " + DateTime.Now + " ] => [ Process For Proxy Checking ]");

            int numberOfThreads = DemoStagramPro.ClGlobul.ProxyCheckNoOfThread;

            List<List<string>> list_Proxy = new List<List<string>>();

            list_Proxy = ListUtilities.Split(DemoStagramPro.ClGlobul.ProxyList, numberOfThreads);
           

            #region Modified Proxy Check
            ThreadPool.SetMaxThreads(50, 50);
            int counter = 0;
            try
            {
                foreach (string itemProxy in DemoStagramPro.ClGlobul.ProxyList)
                {
                    try
                    {
                        if (proxyStop)
                            return;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(getpageSourceFromProxy), new object[] { itemProxy });                        
                    }
                    catch { }
                }
               
            }
            catch (Exception ex)
            { }
            #endregion

            #region Previous Proxy Check
            //foreach (List<string> list_Proxy_ in list_Proxy)
            //{
            //    foreach (string list_Proxy_Item in list_Proxy_)
            //    {
            //        lock (lockr_ThreadController)
            //        {
            //            try
            //            {
            //                if (count_ThreadController >= list_Proxy_.Count)
            //                {
            //                    Monitor.Wait(lockr_ThreadController);
            //                }

            //                string Proxy_Item = list_Proxy_Item.Remove(list_Proxy_Item.IndexOf(':'));

            //                Thread likerThread = new Thread(getpageSourceFromProxy);
            //                likerThread.Name = "workerThread_Liker_" + Proxy_Item;
            //                likerThread.IsBackground = true;

            //                likerThread.Start(new object[] { list_Proxy_Item });

            //                count_ThreadController++;
            //            }
            //            catch (Exception ex)
            //            {

            //            }
            //        }
            //    }
            //}
            #endregion
        } 
        #endregion

        static int Proxystatus = 0;

        #region getpageSourceFromProxy
        public void getpageSourceFromProxy(object item)
        {
            if (proxyStop)
                return;
            try
            {
                Thread.CurrentThread.IsBackground = true;
                lstProxyThread.Add(Thread.CurrentThread);
                lstProxyThread = lstProxyThread.Distinct().ToList();
            }
            catch { }

            countParseProxiesThreads++;

            Array Item_value = (Array)item;
            string ClGlobul_ProxyList_item = (string)Item_value.GetValue(0);
            DemoStagramPro.frm_stagram frm1 = new DemoStagramPro.frm_stagram();
            Globussoft.GlobDramProHttpHelper GlobusHttpHelper = new Globussoft.GlobDramProHttpHelper();
            ChilkatHttpHelpr objchilkat = new ChilkatHttpHelpr();
            string proxyad = string.Empty;
            string proxyport = string.Empty;
            string proxyusername = string.Empty;
            string proxyPassword = string.Empty;
            string pagesource1 = string.Empty;
            string pagesource = string.Empty;


            try
            {
                string[] proxyLst = ClGlobul_ProxyList_item.Split(':');
                if (proxyLst.Count() > 3)
                {
                    proxyad = proxyLst[0];
                    proxyport = proxyLst[1];
                    proxyusername = proxyLst[2];
                    proxyPassword = proxyLst[3];
                }
                else if (proxyLst.Count() > 0 && proxyLst.Count() < 3)
                {
                    proxyad = proxyLst[0];
                    proxyport = proxyLst[1];
                }
                else
                {
                    return;
                }

                try
                {
                    if (proxyStop)
                        return;
                    //pagesource1 = GlobusHttpHelper.getHtmlfromUrlProxy(new Uri("http://websta.me/login"), proxyad, Convert.ToInt16(proxyport), proxyusername, proxyPassword);
                    pagesource1 = GlobusHttpHelper.getHtmlfromUrlProxy(new Uri("http://web.stagram.com/"), proxyad, Convert.ToInt16(proxyport), proxyusername, proxyPassword);
                }
                catch { };

                try
                {
                    // pagesource1 = GlobusHttpHelper.getHtmlfromUrlProxy(new Uri("http://web.stagram.com/"), proxyad, Convert.ToInt16(proxyport), proxyusername, proxyPassword);

                    pagesource1 = objchilkat.GetHtmlProxy("http://web.stagram.com/", proxyad, proxyport, proxyusername, proxyPassword);
                }
                catch { };


                //int FirstPointClientId = pagesource1.IndexOf("client_id=");
                //string FirstClientIdSubString = pagesource1.Substring(FirstPointClientId);
                //int SecondPointClientId = FirstClientIdSubString.IndexOf("&redirect_uri=");
                //string ClientId = FirstClientIdSubString.Substring(0, SecondPointClientId).Replace("'", string.Empty).Replace("client_id=", string.Empty).Trim();

                //string LoginUrl = "https://instagram.com/accounts/login/?next=/oauth/authorize/%3Fclient_id%3D" + ClientId + "%26redirect_uri%3Dhttp%253A%252F%252Fweb.stagram.com%252F%26response_type%3Dcode%26scope%3Dlikes%2Bcomments%2Brelationships";

                //pagesource = GlobusHttpHelper.getHtmlfromUrlProxy(new Uri(LoginUrl), proxyad, Convert.ToInt16(proxyport), proxyusername, proxyPassword);

                //if (string.IsNullOrEmpty(pagesource))
                //{
                //    pagesource = string.Empty;
                //    pagesource = GlobusHttpHelper.getHtmlfromUrlProxy(new Uri(LoginUrl), proxyad, Convert.ToInt16(proxyport), proxyusername, proxyPassword);
                //}

                //ADD in List list of Finally chacked.....
                if (!string.IsNullOrEmpty(pagesource1))
                {
                    if (proxyStop)
                        return;
                    addInFinalCheckedProxyist(proxyad, proxyport, proxyusername, proxyPassword, pagesource1);
                }
                else
                {
                    if (proxyStop)
                        return;
                    DemoStagramPro.ClGlobul.isProxyCheckComplete = true;
                    LoggerProxy("[ " + DateTime.Now + " ] => [ Proxy Is not Working : " + proxyad + ":" + proxyport + ":" + proxyusername + ":" + proxyPassword + " ]");
                    lock (lockerforNonWorkingProxies)
                    {
                        GramBoardProFileHelper.AppendStringToTextfileNewLine(proxyad + ":" + proxyport + ":" + proxyusername + ":" + proxyPassword, GramBoardProFileHelper.NonWorkingProxiesList);
                    }
                }
            }
            catch (Exception)
            {
                if (proxyStop)
                    return;
                LoggerProxy("[ " + DateTime.Now + " ] => [ Proxy Is not Working : " + proxyad + ":" + proxyport + ":" + proxyusername + ":" + proxyPassword + " ]");

                lock (lockerforNonWorkingProxies)
                {
                    GramBoardProFileHelper.AppendStringToTextfileNewLine(proxyad + ":" + proxyport + ":" + proxyusername + ":" + proxyPassword, GramBoardProFileHelper.NonWorkingProxiesList);
                }
            }
            finally
            {

                lock (lockerforProxies)
                {
                    countParseProxiesThreads--;
                    Monitor.Pulse(lockerforProxies);
                }
                Proxystatus--;
                if (Proxystatus == 0)
                {
                    LoggerProxy("[ " + DateTime.Now + " ] => [ PROCESS COMPLETED ]");
                    LoggerProxy("-----------------------------------------------------------------------------------------------------------------------------------");
                }
            }
            
        } 
        #endregion

        #region addInFinalCheckedProxyist
        public static readonly object locker_finalProxyList = new object();
        public void addInFinalCheckedProxyist(string proxyad, string proxyport, string proxyusername, string proxyPassword, string pagesource)
        {
            if (checkStatuse(pagesource))
            {
                if (proxyStop)
                    return;
                try
                {
                    if (proxyStop)
                        return;
                    LoggerProxy("[ " + DateTime.Now + " ] => [ Working IP : " + proxyad + ":" + proxyport + " ]");
                    DemoStagramPro.ClGlobul.isProxyCheckComplete = true;
                }
                catch { };
              

                if (!string.IsNullOrEmpty(proxyusername) && !string.IsNullOrEmpty(proxyPassword) && !string.IsNullOrEmpty(proxyad) && !string.IsNullOrEmpty(proxyport))
                {
                    if (proxyStop)
                        return;
                    try
                    {
                        string add = proxyad + ":" + proxyport + ":" + proxyusername + ":" + proxyPassword;
                        lock (lockerforProxies)
                        {
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(add, GramBoardProFileHelper.WorkingProxiesList);
                        }
                        lock (locker_finalProxyList)
                        {
                            DemoStagramPro.ClGlobul.finalProxyList.Add(add);
                            Monitor.Pulse(locker_finalProxyList);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                
                  
                else if (string.IsNullOrEmpty(proxyusername) && string.IsNullOrEmpty(proxyPassword) && !string.IsNullOrEmpty(proxyad) && !string.IsNullOrEmpty(proxyport))
                {
                    try
                    {
                        if (proxyStop)
                            return;
                        string add = proxyad + ":" + proxyport;
                        //lock (lockerforNonWorkingProxies)
                        //{
                            if (proxyStop)
                                return;
                            GramBoardProFileHelper.AppendStringToTextfileNewLine(add, GramBoardProFileHelper.WorkingProxiesList);
                            
                        //}
                        //lock (locker_finalProxyList)
                        //{
                            DemoStagramPro.ClGlobul.finalProxyList.Add(add);
                            Monitor.Pulse(locker_finalProxyList);
                        //}
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {

                }
            }
        } 

        
        #endregion

        #region checkStatuse
        public bool checkStatuse(string page)
        {
            //if (page.Contains("<input type=\"submit\" class=\"button-green\" value=\"Log in\" />"))
            if(page.Contains("<a href=\"/login\">LOG IN</a></li>"))
            {
                return true;
            }

            return false;
        } 
        #endregion

        #region Stop Checking Proxy
        public void stopProxy()
        {
            LoggerProxy("[ " + DateTime.Now + " ] => [ Process For IP Checking Stopped ]");
            List<Thread> lstTemp = new List<Thread>();
            lstTemp = lstProxyThread.Distinct().ToList();
            foreach(Thread itemThreads in lstTemp)
            {
                try
                {
                    itemThreads.Abort();
                }
                catch { }
            }
        }
        #endregion

    }
}
