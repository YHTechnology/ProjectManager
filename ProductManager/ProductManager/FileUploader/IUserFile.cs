using System.IO;
using System.Windows.Threading;
using System.ComponentModel;

namespace ProductManager.FileUploader
{
    public interface IUserFile
    {
        string FileName { get; set; }
        string FolderName { get; set; }

        double FileSize { get; }
        Stream FileStream { get; set; }
        FileStates State { get; set; }

        double BytesUploaded { get; set; }
        double BytesUploadedFinished { get; set; }

        float Percentage { get; set; }
        float PercentageFinished { get; set; }



        string ErrorMessage { get; set; }

        void Upload(string initParams, Dispatcher uiDispatcher);
        void CancelUpload();

        event PropertyChangedEventHandler PropertyChanged;
    }
}