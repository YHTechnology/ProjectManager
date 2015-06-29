using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;

namespace BIS.Library
{
    public static class ConversionHelper
    {
        /// <summary>
        /// Abubakar
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(String value)
        {
            byte[] plainText = new byte[value.Length];
            Int16 i = 0;
            foreach (char c in value)
            {
                plainText[i] = (byte)System.Convert.ToInt32(c);
                i++;
            }
            return plainText;
        }
        /// <summary>
        /// Abubakar
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytesUnicode(String value)
        {
            byte[] plainText = new byte[value.Length * 2];
            Int16 i = 0;
            foreach (char c in value)
            {
                plainText[i] = (byte)System.Convert.ToInt32(c);
                i++;
                plainText[i] = (byte)0;
                i++;
            }
            return plainText;
        }
        /// <summary>
        /// Abubakar
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetString(byte[] value)
        {


            char[] plainText = new char[value.Length];
            Int16 i = 0;
            foreach (byte c in value)
            {
                plainText[i] = (char)System.Convert.ToInt32(c);
                i++;
            }

            return new String(plainText);
        }

        /// <summary>
        /// Abubakar
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetStringUnicode(byte[] value)
        {

            String text = (new UnicodeEncoding()).GetString(value, 0, value.Length);
            return text;
            /*char[] plainText = new char[value.Length / 2];
            Int32 i = 0;
            Int32 j = 0;
            for (; i < value.Length; i += 2)
            {
                plainText[j] = (char)System.Convert.ToInt32(value[i]);
                j++;
            }

            return new String(plainText);*/
        }
    }
}
