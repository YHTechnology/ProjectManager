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
using System.IO;

namespace Lite.Library
{
    public static class StreamExtensions
    {
        public static MemoryStream CloneToMemoryStream(this Stream obj)
        {
            var m = new MemoryStream();

            var bytes = new byte[obj.Length];
            obj.Read(bytes, 0, bytes.Length);
            obj.Seek(0, SeekOrigin.Begin);

            return new MemoryStream(bytes);
        }

        public static byte[] GetAllBytes(this Stream obj)
        {
            obj.Seek(0, SeekOrigin.Begin);

            return new BinaryReader(obj).ReadBytes((int)obj.Length);
        }
    }
}
