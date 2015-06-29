using System;

namespace ProductManager.FileUploader
{
    public interface IFileUploader
    {
        void StartUpload(string initParams);
        void CancelUpload();

        event EventHandler UploadFinished;
    }

}