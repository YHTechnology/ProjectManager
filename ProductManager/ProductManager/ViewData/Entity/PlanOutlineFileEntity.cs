using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ProductManager.Controls;
using ProductManager.FileUploader;

namespace ProductManager.ViewData.Entity
{
    public class PlanOutlineFileEntity : NotifyPropertyChanged
    {        
        public String fileName;
        public String FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    UpdateChanged("FileName");
                }
            }
        }

        private int userID;
        public int UserID
        {
            get
            {
                return userID;
            }
            set
            {
                if (userID != value)
                {
                    userID = value;
                    UpdateChanged("UserID");
                }
            }
        }

        private String userName;
        public String UserName
        {
            get
            {
                return userName;
            }
            set
            {
                if (userName != value)
                {
                    userName = value;
                    UpdateChanged("UserName");
                }
            }
        }

        private DateTime fileUploadTime;
        public DateTime FileUploadTime
        {
            get
            {
                return fileUploadTime;
            }
            set
            {
                if (fileUploadTime != value)
                {
                    fileUploadTime = value;
                    UpdateChanged("FileUploadTime");
                }
            }
        }
        
        private String fileUrl;
        public String FileUrl
        {
            get
            {
                return fileUrl;
            }
            set
            {
                if (fileUrl != value)
                {
                    fileUrl = value;
                    UpdateChanged("FileUrl");
                }
            }
        }        
        
        private Visibility downLoading = Visibility.Collapsed;
        public Visibility DownLoading
        {
            get
            {
                return downLoading;
            }
            set
            {
                if (downLoading != value)
                {
                    downLoading = value;
                    UpdateChanged("DownLoading");
                }
            }
        }

        private long fileBytes;
        public long FileBytes
        {
            get
            {
                return fileBytes;
            }
            set
            {
                if (fileBytes != value)
                {
                    fileBytes = value;
                    UpdateChanged("FileBytes");
                }
            }
        }

        public String FileBytesString
        {
            get
            {
                if (fileBytes < 1024)
                {
                    return fileBytes.ToString() + " bytes";
                }
                else if (fileBytes >= 1024 && fileBytes < 1024 * 1024)
                {
                    return (fileBytes / 1024).ToString() + " K";
                }
                else
                {
                    return (fileBytes / (1024 * 1024)).ToString() + " M";
                }
            }
        }

        public void Update()
        {        
            this.fileName = PlanFiles.file_name;
            this.userID = PlanFiles.user_id.GetValueOrDefault(0);
            this.fileUploadTime = PlanFiles.file_uploadtime.GetValueOrDefault();
            this.fileBytes = PlanFiles.file_bytes.GetValueOrDefault(0);
            this.fileUrl = CustomUri.GetAbsoluteUrl("PlanOutline/" + fileName);
        }

        public void DUpdate()
        {
            if (null == PlanFiles)
            {
                PlanFiles = new ProductManager.Web.Model.plan_outline_files();
            }

            PlanFiles.file_name = this.fileName;
            PlanFiles.user_id = this.userID;
            PlanFiles.file_uploadtime = this.fileUploadTime;
            PlanFiles.file_bytes = this.fileBytes;
        }

        public void RaisALL()
        {
            UpdateChanged("FileName");
            UpdateChanged("UserID");
            UpdateChanged("FileUploadTime");
            UpdateChanged("FileTypeID");
        }

        public ProductManager.Web.Model.plan_outline_files PlanFiles { get; set; }


        public ICommand OnDownload { get; private set; }

        public ICommand OnView { get; private set; }

        public PlanOutlineFileEntity()
        {
            OnDownload = new DelegateCommand(OnDownloadCommand);
            OnView = new DelegateCommand(OnViewCommand, CanViewCommand);
        }

        private SaveFileDialog saveFileDialog;

        private void OnDownloadCommand()
        {
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "All Files|*.*";
            saveFileDialog.GetType().GetMethod("set_DefaultFileName").Invoke(saveFileDialog, new object[] { this.FileName });

            bool? dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult != true) return;

            WebClient client = new WebClient();
            Uri uri = new Uri(FileUrl, UriKind.RelativeOrAbsolute);
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(OpenReadCompleted);
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);
            client.OpenReadAsync(uri);
            DownLoading = Visibility.Visible;
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadPer = e.ProgressPercentage;
        }

        private void OnViewCommand()
        {
            ReviewWindow review = new ReviewWindow(FileUrl + ".xod");
            review.Show();
        }

        private bool CanViewCommand(object aObject)
        {
            if (FileUrl == null)
            {
                FileUrl = CustomUri.GetAbsoluteUrl("PlanOutline/" + fileName);
            }

            if (FileUrl != null)
            {
                int lastDot = FileUrl.LastIndexOf(".");
                String lExt = FileUrl.Substring(lastDot, FileUrl.Length - lastDot).ToLower();
                if (lExt == ".pdf"
                   || lExt == ".doc"
                   || lExt == ".docx"
                   || lExt == ".xls"
                   || lExt == ".xlsx"
                   || lExt == ".jpeg"
                   || lExt == ".png"
                   || lExt == ".bmp"
                   || lExt == ".tiff"
                   || lExt == ".gif")
                {
                    DateTime lToday = DateTime.Now;
                    if (lToday.Year == fileUploadTime.Year && lToday.Month == fileUploadTime.Month && lToday.Day == fileUploadTime.Day)
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        void OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                using (Stream sf = (Stream)saveFileDialog.OpenFile())
                {
                    e.Result.CopyTo(sf);
                    sf.Flush();
                    sf.Close();
                    DownLoading = Visibility.Collapsed;
                    NotifyWindow notifyWindow = new NotifyWindow("下载完成", "下载完成！");
                    notifyWindow.Show();
                }
            }
        }

        private int downloadPer;
        public int DownloadPer
        {
            get
            {
                return downloadPer;
            }
            set
            {
                if (downloadPer != value)
                {
                    downloadPer = value;
                    UpdateChanged("DownloadPer");
                }
            }
        }

    }
}
