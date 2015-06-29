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
using System.Windows.Media.Imaging;

namespace Lite.Library
{
    public class ImageTranslator
    {
        public static Byte[] TranslateImageToBytes(System.Windows.Controls.Image ImageControl)
        {
        
                 WriteableBitmap wBitmap = new WriteableBitmap(ImageControl, null);

                int hgt = wBitmap.PixelHeight;
                int wdth = wBitmap.PixelWidth;

                
                EditableImage ei = new EditableImage(wdth, hgt);

                for (int y = 0; y < hgt; y++)
                {
                    for (int x = 0; x < wdth; x++)
                    {
                        int pixel = wBitmap.Pixels[((y * wdth) + x)];
                        ei.SetPixel(x, y, (byte)((pixel >> 16) & 0xff), (byte)((pixel >> 8) & 0xff), (byte)(pixel & 0xff), (byte)((pixel >> 24) & 0xff));
                    }
                }

            return ei.GetStream().GetAllBytes();
        }
    }
}
