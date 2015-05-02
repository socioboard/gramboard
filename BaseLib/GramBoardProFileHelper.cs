using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections;

namespace Globussoft
{
    public static class GramBoardProFileHelper
    {
        //Common path For every Module ...
        public static string path_AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Gram BoardPro";

        //Follow Module Log Files...
        public static string FollowIDFilePath = path_AppDataFolder + "\\FollowedByID.txt";
        public static string UnFollowIdFilePath = path_AppDataFolder + "\\NotFollowed.txt";
        public static string ErrorLogFilePath = path_AppDataFolder + "\\FollowedErrorLog.txt";
        public static string AllReadyFollowedIdFilePath = path_AppDataFolder + "\\AlReadyFollowedByID.txt";
        public static string FollowedOptionNotAvailableFilePath = path_AppDataFolder + "\\FollowedOptionNotAvailableByID.txt";

        public static string successfullyLoggedaccounts = path_AppDataFolder + "\\SuccessfullyLoggedaccounts.txt";
        public static string AccessIssuesAccounts = path_AppDataFolder + "\\AccessIssuesAccounts.txt";
        public static string FailedAccounts = path_AppDataFolder + "\\FailedToLoginAccounts.txt";
        public static string PostRequesterror = path_AppDataFolder + "\\StreamNotReadableAccounts.txt";
        public static string WorkingProxiesList = path_AppDataFolder + "\\WorkingProxiesList.txt";
        public static string NonWorkingProxiesList = path_AppDataFolder + "\\NonWorkingProxiesList.txt";

        //Photo like Module Log Files ...
        public static string LikePhotoAccountIdFilePath = path_AppDataFolder + "\\SuccesfullyLikePhotoAccounts.txt";
        public static string NotlikePhotoAccountIdFilePath = path_AppDataFolder + "\\NotLikePhotoAccounts.txt";
         public static string AllReadylikePhotoAccountIdFilePath = path_AppDataFolder + "\\AllReadylikePhotoAccounts.txt";
        public static string ErrorLogFilePathForPhotolike = path_AppDataFolder + "\\PhotoLikesErrorLog.txt";

        //Comment Module Log Files ....
        public static string SuccesfullyCommentedFilePath = path_AppDataFolder + "\\SuccessFullyCommentedAccounts.txt";
        public static string NotCommentedFilePath = path_AppDataFolder + "\\NotComment.txt";
        public static string ErrorLogFilePathForComment = path_AppDataFolder + "\\CommentErrorLog.txt";

        public static string ErrorLogFilePathScrapeFollowers = path_AppDataFolder + "\\ScrapeFollowers.txt";

        public static String ReadStringFromTextfile(string filepath)
        {
            StreamReader reader = new StreamReader(filepath);
            string fileText=reader.ReadToEnd();
            reader.Close();
            return fileText;
        }

        private static int _bufferSize = 16384;

        public static List<string> ReadFile(string filename)
        {
            List<string> listFileContent = new List<string>();

            StringBuilder stringBuilder = new StringBuilder();
            using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    char[] fileContents = new char[_bufferSize];
                    int charsRead = streamReader.Read(fileContents, 0, _bufferSize);

                    while (charsRead > 0)
                    {
                        stringBuilder.Append(fileContents);
                        charsRead = streamReader.Read(fileContents, 0, _bufferSize);
                    }

                    string[] contentArray = stringBuilder.ToString().Split(new char[] { '\r', '\n' });
                    foreach (string line in contentArray)
                    {
                      //  listFileContent.Add(line.Replace("#", "").Replace("\0", ""));
                        listFileContent.Add(line.Replace("\0", ""));
                    }
                    listFileContent.RemoveAll(s => string.IsNullOrEmpty(s));
                }
            }
            return listFileContent;
        }

        public static List<string> ReadFiletoStringList(string filepath)
        {
            List<string> list = new List<string>();
            StreamReader reader = new StreamReader(filepath);
            string text="";
            while ((text = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    list.Add(text); 
                }
            }
            reader.Close();
            return list;

        }

        public static void WriteStringToTextfile(string content,string filepath)
        {
           StreamWriter writer = new StreamWriter(filepath);
           writer.Write(content);
           writer.Close();
           
        }

        public static void ExportDataCSVFile(string CSV_Header, string CSV_Content, string CSV_FilePath)
        {
            try
            {
                if (!File.Exists(CSV_FilePath))
                {
                    GramBoardProFileHelper.AppendStringToTextFile(CSV_FilePath, CSV_Header);
                }

                GramBoardProFileHelper.AppendStringToTextFile(CSV_FilePath, CSV_Content);
            }
            catch (Exception)
            {

            }
        }

        public static void AppendStringToTextFile(string FilePath, string content)
        {
            using (StreamWriter sw = new StreamWriter(FilePath, true))
            {
                sw.WriteLine(content);
            }
        }

        public static void WriteStringToTextfileNewLine(String content, string filepath)
        {

            StreamWriter writer = new StreamWriter(filepath);

            StringReader reader = new StringReader(content);

            while(reader.ReadLine()!=null)
            { 
                writer.WriteLine(content);
            }           

            writer.Close();
        }

        public static void AppendStringToTextfileNewLine(String content, string filepath)
        {
            try
            {
                StreamWriter writer = new StreamWriter(filepath, true);

                StringReader reader = new StringReader(content);

                string temptext = "";

                while ((temptext = reader.ReadLine()) != null)
                {
                    writer.WriteLine(temptext);
                }

                writer.Close();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        public static void AppendStringToTextfileNewLineWithCarat(String content, string filepath)
        {

            try
            {
                StreamWriter writer = new StreamWriter(filepath, true);

                StringReader reader = new StringReader(content);

                string temptext = "";

                while ((temptext = reader.ReadLine()) != null)
                {
                    writer.WriteLine(temptext);
                }
                for (int i = 0; i < 80; i++)
                {
                    writer.Write("-");
                }
                writer.WriteLine();

                writer.Close();
            }
            catch (Exception ex)
            { 
            
            }
        }
        
        public static void WriteListtoTextfile(List<string> list,string filepath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filepath))
                {
                    foreach (string listitem in list)
                    {
                        writer.WriteLine(listitem);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
        
        public static List<string> readcsvfile(string filpath)
        {
            List<string> tempdata = new List<string>();
            StreamReader sr = new StreamReader(filpath, Encoding.GetEncoding(1250));
            string strline = "";
            int x = 0;
            while (!sr.EndOfStream)
            {
                x++;
                strline = sr.ReadLine();
                tempdata.Add(strline);
            }
            sr.Close();
            return tempdata;
        }

        public static void ReplaceStringFromCsv(string filePath, string searchText, string replaceText)
        {
            StreamReader reader = new StreamReader(filePath);
            string content = reader.ReadToEnd();
            reader.Close();

            content = Regex.Replace(content, searchText + "\r\n", replaceText);

            StreamWriter writer = new StreamWriter(filePath);
            writer.Write(content);
            writer.Close();
        }
               
        public static List<string> GetSpinnedComments(string RawComment)
        {
            /// <summary>
            /// Hashtable that stores (DataInsideBraces) as Key and DataInsideBracesArray as Value
            /// </summary>
            Hashtable commentsHashTable = new Hashtable();

            ///This is final possible cominations of comments
            List<string> listModComments = new List<string>();

            ///Put braces data in list of string array
            List<string[]> listDataInsideBracesArray = new List<string[]>();

            ///This Regex will fetch data within braces and put it in list of string array
            var regex = new Regex(@"\(([^)]*)\)", RegexOptions.Compiled);
            foreach (Match Data in regex.Matches(RawComment))
            {
                string data = Data.Value.Replace("(", "").Replace(")", "");
                string[] DataInsideBracesArray = data.Split('/');
                commentsHashTable.Add(Data, DataInsideBracesArray);
                listDataInsideBracesArray.Add(DataInsideBracesArray);
            }

            string ModifiedComment = RawComment;

            IDictionaryEnumerator en = commentsHashTable.GetEnumerator();

            List<string> listModifiedComment = new List<string>();

            listModifiedComment.Add(ModifiedComment);

            //en.Reset();

            string ModifiedComment1 = ModifiedComment;

            foreach (string[] item in listDataInsideBracesArray)
            {
                en.MoveNext();
                foreach (string modItem in listModifiedComment)
                {
                    foreach (string innerItem in item)
                    {
                        string ModComment = modItem.Replace(en.Key.ToString(), innerItem);
                        listModComments.Add(ModComment);
                    }
                }
                listModifiedComment.AddRange(listModComments);
                //string ModComment = ModifiedComment1.Replace(en.Key, item
            }

            List<string> listRequiredComments = listModifiedComment.FindAll(s => !s.Contains("("));

            //listComments.AddRange(listRequiredComments);
            return listRequiredComments;
        }
    }
}
