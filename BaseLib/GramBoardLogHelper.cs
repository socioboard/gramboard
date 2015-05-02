using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;

namespace BaseLib
{
    public sealed class GramBoardLogHelper
    {
        private static volatile GramBoardLogHelper globusLogHelper = null;
        private static object syncRoot = new object();
        private static log4net.ILog logger = null;

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static log4net.ILog log
        {
            get
            {
                lock (syncRoot)
                {
                    if (globusLogHelper == null || logger==null)
                    {
                        globusLogHelper = new GramBoardLogHelper();
                        logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                     }
                        
                }
                return logger;
            }
        }

        /// <summary>
        /// Private Constructer for Singleton Implementation
        /// </summary>
        private GramBoardLogHelper()
        { 
        
        }
    }
}
