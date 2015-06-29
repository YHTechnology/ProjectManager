using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Hosting;

// using pdftron;
// using pdftron.Common;
// using pdftron.Filters;
// using pdftron.SDF;
// using pdftron.PDF;

namespace ProductManager.Web
{
    /// <summary>
    /// HttpUploadFilesHandler 的摘要说明
    /// </summary>
    public class HttpUploadFilesHandler : IHttpHandler
    {
        private HttpContext _context;
        private string _fileName;
        private string _path;

        public void ProcessRequest(HttpContext context)
        {
            _context = context;
            _context.Response.ContentType = "text/plain";

            if (_context.Request.InputStream.Length == 0)
            {
                throw new ArgumentException("No file input");
            }

            else
            {
                try
                {
                    _fileName = _context.Request.QueryString["file"];
                    _path = _context.Request.QueryString["path"];

                    SetUpLoadPath(_path);
                    string targetPath = GetTargetFilePath(_fileName);

                    if (File.Exists(targetPath))
                    {
                        File.Delete(targetPath);
                    }

                    using (FileStream fs = File.Open(targetPath, FileMode.CreateNew))
                    {
                        SaveFile(_context.Request.InputStream, fs);
                        fs.Close();
//                         try
//                         {
//                             int lastDot = targetPath.LastIndexOf(".");
//                             String lExt = targetPath.Substring(lastDot, targetPath.Length - lastDot).ToLower();
//                             if (lExt == ".pdf"
//                                 || lExt == ".doc"
//                                 || lExt == ".docx"
//                                 || lExt == ".xls"
//                                 || lExt == ".xlsx"
//                                 || lExt == ".jpeg"
//                                 || lExt == ".png"
//                                 || lExt == ".bmp"
//                                 || lExt == ".tiff"
//                                 || lExt == ".gif")
//                             {

//                                pdftron.PDF.Convert.ToXod(targetPath, targetPath + ".xod");
//                             }
//                             
//                         }
//                         catch (System.Exception ex)
//                         {
//                             _context.Response.Write(ex.Message);
//                         }
                    }

                    _context.Response.Write("upload success" + targetPath + ".xod");
                }
                catch (System.Exception ex)
                {
                    _context.Response.Write(ex.Message);
                }

            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        protected void SetUpLoadPath(string strPathName)
        {
            string filePath = Path.Combine(@HostingEnvironment.ApplicationPhysicalPath, Path.Combine(strPathName));

            if (Directory.Exists(filePath))
            {
                return;
            }
            
            try
            {
                Directory.CreateDirectory(filePath);
            }
            catch (System.Exception ex)
            {
                _context.Response.Write(ex.Message);
            }
        }


        protected virtual string GetUploadFolder()
        {
            return _path;
        }

        protected string GetTargetFilePath(string fileName)
        {
            return Path.Combine(@HostingEnvironment.ApplicationPhysicalPath, Path.Combine(GetUploadFolder(), fileName));
        }

        private void SaveFile(Stream stream, FileStream fs)
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                fs.Write(buffer, 0, bytesRead);
            }
        }
    }
}