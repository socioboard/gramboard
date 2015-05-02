using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoStagramPro.Classes
{
    public class HashLogger
    {
        public static frm_stagram _frm_stagram = null;
     
       
        public static  void printLogger(string msg)
        {
            try
            {
                _frm_stagram.LoggerProperty = msg;
            }
            catch { }
        }

        public static void printImageLogger(string msg)
        {
            try
            {
                _frm_stagram.LoggerImageProperty = msg;
            }
            catch { }
        }
       

       
       
    }
}
