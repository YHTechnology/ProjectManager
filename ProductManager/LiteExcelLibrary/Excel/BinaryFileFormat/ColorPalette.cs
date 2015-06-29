using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;

namespace Lite.ExcelLibrary.BinaryFileFormat
{
    public class ColorPalette
    {
        public Dictionary<int, Color> Palette = new Dictionary<int, Color>();

        public ColorPalette()
        {
            Palette.Add(0, Colors.Black);
            Palette.Add(1, Colors.White);
            Palette.Add(2, Colors.Red);
            Palette.Add(3, Colors.Green);
            Palette.Add(4, Colors.Blue);
            Palette.Add(5, Colors.Yellow);
            Palette.Add(6, Colors.Magenta);
            Palette.Add(7, Colors.Cyan);
            // 0x08-0x3F: user-defined colour from the PALETTE record
            Palette.Add(0x1F, Color.FromArgb(204, 204, 255,255));

            Palette.Add(0x40, SystemColors.WindowColor);
            Palette.Add(0x41, SystemColors.WindowTextColor);
            Palette.Add(0x43, SystemColors.WindowFrameColor);//dialogue background colour
            Palette.Add(0x4D, SystemColors.ControlTextColor);//text colour for chart border lines
            Palette.Add(0x4E, SystemColors.ControlColor); //background colour for chart areas
            Palette.Add(0x4F, Colors.Black); //Automatic colour for chart border lines
            Palette.Add(0x50, SystemColors.InfoColor);
            Palette.Add(0x51, SystemColors.InfoTextColor);
            Palette.Add(0x7FFF, SystemColors.WindowTextColor);
        }

        public Color this[int index]
        {
            get
            {
                if (Palette.ContainsKey(index))
                {
                    return Palette[index];
                }
                return Colors.White;
            }
            set
            {
                Palette[index] = value;
            }
        }
    }
}
