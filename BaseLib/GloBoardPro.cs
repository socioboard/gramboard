using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace BaseLib
{
    public class GloBoardPro
    {
        public static List<string> listAccounts = new List<string>();

        public static AccountMode accountMode;

        public static string DeCaptcherHost = string.Empty;
        public static string DeCaptcherPort = string.Empty;
        public static string DeCaptcherUsername = string.Empty;
        public static string DeCaptcherPassword = string.Empty;

        public static string DBCUsername = string.Empty;
        public static string DBCPassword = string.Empty;

        public static string EmailsFilePath = string.Empty;

        public static List<Thread> HasTagListListThread = new List<Thread>();
        public static List<Thread> lstThread = new List<Thread>();
        
        public static string FbAccountDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\FaceDominatorFbAccount";
        public static string imageDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\HashtagImageSave";


        public static List<string> lstDesktopFilePaths = new List<string>() { Path.Combine(FbAccountDesktopPath, "DisableFbAccount.txt"), Path.Combine(FbAccountDesktopPath, "IncorrectFbAccount.txt"), Path.Combine(FbAccountDesktopPath, "PhoneVerifyFbAccount.txt"), Path.Combine(FbAccountDesktopPath, "CorrectFbAccount.txt"), Path.Combine(FbAccountDesktopPath, "TemporarilyFbAccount.txt"), Path.Combine(FbAccountDesktopPath, "AccountNotConfirmed.txt"), Path.Combine(FbAccountDesktopPath, "CorrectFbAccount.txt"), Path.Combine(FbAccountDesktopPath, "Uploadimages.txt") };

        Regex IdCheck = new Regex("^[0-9]*$");
    }

    public enum AccountMode
    {
        NoProxy, PublicProxy, PrivateProxy
    }
}
