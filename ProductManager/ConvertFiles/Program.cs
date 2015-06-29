using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            String lDictrionary = args[0];
            DirectoryInfo lRootDirectory = new DirectoryInfo(lDictrionary);
            foreach (DirectoryInfo dirinfo in lRootDirectory.GetDirectories())
            {
                foreach (FileInfo fileInfo in dirinfo.GetFiles())
                {
                    String lFileName = fileInfo.FullName;
                    int lastDot = lFileName.LastIndexOf(".");
                    if (lastDot == -1)
                    {
                        continue;
                    }
                    String lExt = lFileName.Substring(lastDot, lFileName.Length - lastDot).ToLower();
                    if (lExt == ".pdf"
                        || lExt == ".doc"
                        || lExt == ".docx"
                        || lExt == ".xls"
                        || lExt == ".xlsx"
                        || lExt == ".jpeg"
                        || lExt == ".jpg"
                        || lExt == ".png"
                        || lExt == ".bmp"
                        || lExt == ".tiff"
                        || lExt == ".tif"
                        || lExt == ".gif")
                    {
                        try
                        {
                            if (!File.Exists(lFileName + ".xod"))
                            {
                                System.Console.WriteLine("[StartConvert]" + lFileName);
                                pdftron.PDF.Convert.ToXod(lFileName, lFileName + ".xod");
                                System.Console.WriteLine("[Success]" + lFileName);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            System.Console.WriteLine("[Fail]" + lFileName +"  " + ex.Message);
                        }
                    }
                }
            }
        }
    }
}
