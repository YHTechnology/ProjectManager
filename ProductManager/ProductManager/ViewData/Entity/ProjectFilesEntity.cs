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
    public class ProjectFilesEntity : NotifyPropertyChanged
    {
        private int departmentID;
        public int DepartmentID
        {
            get
            {
                return departmentID;
            }
            set
            {
                if (departmentID != value)
                {
                    departmentID = value;
                    UpdateChanged("DepartmentID");
                }
            }
        }

        private String departmentName;
        public String DepartmentName
        {
            get
            {
                return departmentName;
            }
            set
            {
                if (departmentName != value)
                {
                    departmentName = value;
                    UpdateChanged("DepartmentName");
                }
            }
        }

        private String fileDiscript;
        public String FileDiscript
        {
            get
            {
                return fileDiscript;
            }
            set
            {
                if (fileDiscript != value)
                {
                    fileDiscript = value;
                    UpdateChanged("FileDiscript");
                }
            }
        }

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

        public String fileType;
        public String FileType
        {
            get
            {
                return fileType;
            }
            set
            {
                if (fileType != value)
                {
                    fileType = value;
                    UpdateChanged("FileType");
                }
            }
        }

        public String manufactureNumber;
        public String ManufactureNumber
        {
            get
            {
                return manufactureNumber;
            }
            set
            {
                if (manufactureNumber != value)
                {
                    manufactureNumber = value;
                    UpdateChanged("ManufactureNumber");
                }
            }
        }

        public String rmanufactureNumber;
        public String RManufactureNumber
        {
            get
            {
                return rmanufactureNumber;
            }
            set
            {
                if (rmanufactureNumber != value)
                {
                    rmanufactureNumber = value;
                    UpdateChanged("RManufactureNumber");
                }
            }
        }

        public int projectFilesID;
        public int ProjectFilesID
        {
            get
            {
                return projectFilesID;
            }
            set
            {
                if (projectFilesID != value)
                {
                    projectFilesID = value;
                    UpdateChanged("ProjectFilesID");
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

        private Nullable<DateTime> fileUpdateTime;
        public Nullable<DateTime> FileUpdateTime
        {
            get
            {
                return fileUpdateTime;
            }
            set
            {
                if (fileUpdateTime != value)
                {
                    fileUpdateTime = value;
                    UpdateChanged("FileUpdateTime");
                }
            }
        }

        public String FileUpdateTimeString
        {
            get
            {
                if (fileUpdateTime.HasValue)
                {
                    return fileUpdateTime.Value.ToShortDateString();
                }
                else
                {
                    return "";
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

        private bool fileDelete;
        public bool FileDelete
        {
            get
            {
                return fileDelete;
            }
            set
            {
                if (fileDelete != value)
                {
                    fileDelete = value;
                    UpdateChanged("FileDelete");
                    UpdateChanged("FileDeleteString");
                }
            }
        }

        public bool CanDownload
        {
            get
            {
                return !fileDelete;
            }
        }

        public String FileDeleteString
        {
            get
            {
                if (fileDelete)
                {
                    return "是";
                }
                else
                {
                    return "否";
                }
            }
        }

        private String fileDeleteDescript;
        public String FileDeleteDescript
        {
            get
            {
                return fileDeleteDescript;
            }
            set
            {
                if (fileDeleteDescript != value)
                {
                    fileDeleteDescript = value;
                    UpdateChanged("FileDeleteDescript");
                }
            }
        }

        private Nullable<DateTime> fileDeleteTime;
        public Nullable<DateTime> FileDeleteTime
        {
            get
            {
                return fileDeleteTime;
            }
            set
            {
                if (fileDeleteTime != value)
                {
                    fileDeleteTime = value;
                    UpdateChanged("FileDeleteTime");
                }
            }
        }

        public String FileDeleteTimeString
        {
            get
            {
                if (fileDeleteTime.HasValue)
                {
                    return fileDeleteTime.Value.ToShortDateString();
                }
                else
                {
                    return "";
                }
            }
        }

        private int fileDeletePersionID;
        public int FileDeletePersionID
        {
            get
            {
                return fileDeletePersionID;
            }
            set
            {
                if (fileDeletePersionID != value)
                {
                    fileDeletePersionID = value;
                    UpdateChanged("FileDeletePersionID");
                }
            }
        }

        private String fileDeletePersionName;
        public String FileDeletePersionName
        {
            get
            {
                return fileDeletePersionName;
            }
            set
            {
                if (fileDeletePersionName != value)
                {
                    fileDeletePersionName = value;
                    UpdateChanged("FileDeletePersionName");
                }
            }
        }

        private int fileTypeID;
        public int FileTypeID
        {
            get
            {
                return fileTypeID;
            }
            set
            {
                if (fileTypeID != value)
                {
                    fileTypeID = value;
                    UpdateChanged("FileTypeID");
                }
            }
        }

        private String fileTypeName;
        public String FileTypeName
        {
            get
            {
                return fileTypeName;
            }
            set
            {
                if (fileTypeName != value)
                {
                    fileTypeName = value;
                    UpdateChanged("FileTypeName");
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
                    return (fileBytes / (1024*1024)).ToString() + " M";
                }
            }
        }

        public void Update()
        {
            this.departmentID = ProjectFiles.department_id.GetValueOrDefault(0);
            this.fileDiscript = ProjectFiles.file_discript;
            this.fileName = ProjectFiles.file_name;
            this.fileType = ProjectFiles.file_type;
            this.manufactureNumber = ProjectFiles.manufacture_number;
            this.rmanufactureNumber = ProjectFiles.r_manufacture_number;
            this.projectFilesID = ProjectFiles.project_files_id;
            this.userID = ProjectFiles.user_id.GetValueOrDefault(0);
            this.fileUpdateTime = ProjectFiles.file_updatetime;
            this.fileDelete = ProjectFiles.file_delete.GetValueOrDefault(false);
            this.fileDeleteDescript = ProjectFiles.file_delete_descript;
            this.fileDeleteTime = ProjectFiles.file_delete_time;
            this.fileDeletePersionID = ProjectFiles.file_delete_persion_id.GetValueOrDefault(-1);
            this.fileTypeID = ProjectFiles.file_type_id.GetValueOrDefault(-1);
            this.fileBytes = ProjectFiles.file_bytes.GetValueOrDefault(0);

            string manufactureNumberTemp = manufactureNumber.TrimEnd();
            string rmanufactureNumberTemp = "";
            if (!String.IsNullOrEmpty(rmanufactureNumber))
            {
                rmanufactureNumberTemp = rmanufactureNumber.TrimEnd();
            }
            this.fileUrl = (CustomUri.GetAbsoluteUrl(String.IsNullOrEmpty(rmanufactureNumber) ? manufactureNumberTemp : rmanufactureNumberTemp) + "/" + fileName);

            //this.fileUrl = (CustomUri.GetAbsoluteUrl(String.IsNullOrEmpty(rmanufactureNumber) ? manufactureNumber : rmanufactureNumber) + "/" + fileName);
        }

        public void DUpdate()
        {
            ProjectFiles.department_id = this.departmentID;
            ProjectFiles.file_discript = this.fileDiscript;
            ProjectFiles.file_name = this.fileName;
            ProjectFiles.file_type = this.fileType;
            ProjectFiles.manufacture_number = this.manufactureNumber;
            ProjectFiles.r_manufacture_number = this.rmanufactureNumber;
            ProjectFiles.project_files_id = this.projectFilesID;
            ProjectFiles.user_id = this.userID;
            ProjectFiles.file_updatetime = this.fileUpdateTime;
            ProjectFiles.file_delete = this.fileDelete;
            ProjectFiles.file_delete_descript = this.fileDeleteDescript;
            ProjectFiles.file_delete_time = this.fileDeleteTime;
            ProjectFiles.file_delete_persion_id = this.fileDeletePersionID;
            ProjectFiles.file_type_id = this.fileTypeID;
            ProjectFiles.file_bytes = this.fileBytes;
        }

        public void RaisALL()
        {
            UpdateChanged("DepartmentID");
            UpdateChanged("FileDiscript");
            UpdateChanged("FileName");
            UpdateChanged("FileType");
            UpdateChanged("ManufactureNumber");
            UpdateChanged("ProjectFilesID");
            UpdateChanged("UserID");
            UpdateChanged("FileUpdateTime");
            UpdateChanged("FileDelete");
            UpdateChanged("FileDeleteDescript");
            UpdateChanged("FileDeleteTime");
            UpdateChanged("FileDeletePersionID");
            UpdateChanged("FileTypeID");
            UpdateChanged("FileDeleteString");
            UpdateChanged("RManufactureNumber");
        }

        public ProductManager.Web.Model.project_files ProjectFiles { get; set; }


        public ICommand OnDownload { get; private set; }

        public ICommand OnView { get; private set; }

        public ProjectFilesEntity()
        {
            OnDownload = new DelegateCommand(OnDownloadCommand, CanDownloadCommand);
            OnView = new DelegateCommand(OnViewCommand, CanViewCommand);
        }

        private SaveFileDialog saveFileDialog;

        private void OnDownloadCommand()
        {
            //MessageBox.Show(FileUrl);
            if (FileUrl == null)
            {
                string manufactureNumberTemp = manufactureNumber.TrimEnd();
                string rmanufactureNumberTemp = "";
                if (!String.IsNullOrEmpty(rmanufactureNumber))
                {
                    rmanufactureNumberTemp = rmanufactureNumber.TrimEnd();
                }
                FileUrl = (CustomUri.GetAbsoluteUrl(String.IsNullOrEmpty(rmanufactureNumber) ? manufactureNumberTemp : rmanufactureNumberTemp) + "/" + fileName);
                //FileUrl = (CustomUri.GetAbsoluteUrl(String.IsNullOrEmpty(rmanufactureNumber) ? manufactureNumber.TrimEnd() : rmanufactureNumber.TrimEnd()) + "/" + fileName);
            }
            //MessageBox.Show(FileUrl);
            try
            {
                saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "All Files|*.*";
                saveFileDialog.GetType().GetMethod("set_DefaultFileName").Invoke(saveFileDialog, new object[] { this.FileName });
                //MessageBox.Show("3");
                bool? dialogResult = saveFileDialog.ShowDialog();
                if (dialogResult != true) return;
                //MessageBox.Show("4");
                WebClient client = new WebClient();
                Uri uri = new Uri(FileUrl, UriKind.RelativeOrAbsolute);
                client.OpenReadCompleted += new OpenReadCompletedEventHandler(OpenReadCompleted);
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);
                client.OpenReadAsync(uri);
                DownLoading = Visibility.Visible;
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
            
        }

        private void OnViewCommand()
        {
            //MessageBox.Show(FileUrl + ".xod");
            ReviewWindow review = new ReviewWindow(FileUrl + ".xod");
            review.Show();
        }

        private bool CanViewCommand(object aObject)
        {
            if (FileUrl == null)
            {
                string manufactureNumberTemp = manufactureNumber.TrimEnd();
                string rmanufactureNumberTemp = "";
                if(!String.IsNullOrEmpty(rmanufactureNumber))
                {
                    rmanufactureNumberTemp = rmanufactureNumber.TrimEnd();
                }
                FileUrl = (CustomUri.GetAbsoluteUrl(String.IsNullOrEmpty(rmanufactureNumber) ? manufactureNumberTemp : rmanufactureNumberTemp) + "/" + fileName);
            }

            if (FileUrl != null)
            {
                int lastDot = FileUrl.LastIndexOf(".");
                if (lastDot <= 0)
                {
                    return false;
                }
                String lExt = FileUrl.Substring(lastDot, FileUrl.Length - lastDot).ToLower();
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
                    DateTime lToday = DateTime.Now;
                    if (lToday.Year == fileUpdateTime.Value.Year && lToday.Month == fileUpdateTime.Value.Month && lToday.Day == fileUpdateTime.Value.Day)
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

        private bool CanDownloadCommand(object aObject)
        {
            if (CanDownload)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2010400);
            }
            else
            {
                return false;
            }
            
        }

        public void RaiseCommand()
        {
            (OnDownload as DelegateCommand).RaiseCanExecuteChanged();
            (OnView as DelegateCommand).RaiseCanExecuteChanged();
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadPer = e.ProgressPercentage;
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
