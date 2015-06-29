using System;

namespace ProductManager.FileUploader
{
    public class CustomUri : Uri
    {
        public CustomUri(string uri)
            : base(GetAbsoluteUrl(uri))
        {

        }

        public static string GetAbsoluteUrl(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                return relativePath;
            }

            string fullUrl;

            if (relativePath.StartsWith("http:", StringComparison.OrdinalIgnoreCase)
             || relativePath.StartsWith("https:", StringComparison.OrdinalIgnoreCase)
             || relativePath.StartsWith("file:", StringComparison.OrdinalIgnoreCase)
                )
            {
                fullUrl = relativePath;
            }
            else
            {
                //  http://localhost:25258/ClientBin/ProductManager.xap
                fullUrl = System.Windows.Application.Current.Host.Source.AbsoluteUri;
                if (fullUrl.IndexOf("ClientBin") > 0)
                {
                    fullUrl = fullUrl.Substring(0, fullUrl.IndexOf("ClientBin")) + relativePath;
                }
                else
                {
                    fullUrl = fullUrl.Substring(0, fullUrl.LastIndexOf("/") + 1) + relativePath;
                }
            }

            return fullUrl;

        }
    }
}