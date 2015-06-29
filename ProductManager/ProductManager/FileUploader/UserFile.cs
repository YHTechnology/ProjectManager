using System.ComponentModel;
using ProductManager;
using System.IO;
using System;

namespace ProductManager.FileUploader
{
    /// <summary>
    /// 
    /// </summary>
    public class UserFile : INotifyPropertyChanged, IUserFile
    {
        private string _fileName;
        private string _folderName;
        private double _fileSize;
        private Stream _fileStream;
        private double _bytesUploaded;
        private double _bytesUploadedFinished;
        private FileStates _state;


        private float _percentage = 0;
        private float _percentageFinished = 0;

        private IFileUploader _fileUploader;

        public delegate void FinishUpdateHandler( object sender, EventArgs e );
        public event FinishUpdateHandler FinishUpdate;

        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                UpdateChanged("FileName");
            }
        }

        public string FolderName
        {
            get
            {
                return _folderName;
            }
            set
            {
                _folderName = value;
                UpdateChanged("FolderName");
            }

        }

        public double FileSize
        {
            get
            {
                return _fileSize;
            }
        }

        public Stream FileStream
        {
            get
            {
                return _fileStream;
            }
            set
            {
                _fileStream = value;
                if (_fileStream != null)
                {
                    _fileSize = _fileStream.Length;
                }
            }
        }

        public double BytesUploaded
        {
            get
            {
                return _bytesUploaded;
            }
            set
            {
                _bytesUploaded = value;
                UpdateChanged("BytesUploaded");
                Percentage = (float)(value / FileSize);
            }
        }

        public FileStates State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                UpdateChanged("State");
            }
        }

        public double BytesUploadedFinished
        {
            get
            {
                return _bytesUploadedFinished;
            }
            set
            {
                _bytesUploadedFinished = value;
                UpdateChanged("BytesUploadedFinished");
                PercentageFinished = (float)(value / FileSize) * 100;
            }
        }

        public float Percentage
        {
            get
            {
                return _percentage;
            }
            set
            {
                _percentage = value;
                UpdateChanged("Percentage");
            }
        }

        public float PercentageFinished
        {
            get
            {
                return _percentageFinished;
            }
            set
            {
                _percentageFinished = value;
                UpdateChanged("PercentageFinished");
                UpdateChanged("PercentageFinished2");
            }
        }

        public int PercentageFinished2
        {
            get
            {
                int rs = (int)(_percentageFinished * 100);
                return rs;
            }
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public void Upload(string initParams, System.Windows.Threading.Dispatcher uiDispatcher)
        {
            State = FileStates.Uploading;

            _fileUploader = new HttpFileUploader(this, uiDispatcher);
            _fileUploader.StartUpload(initParams);
            _fileUploader.UploadFinished += new EventHandler(fileUploader_UploadFinished);
        }

        private void fileUploader_UploadFinished(object sender, EventArgs e)
        {
            _fileUploader = null;
            if (State != FileStates.Deleted && State != FileStates.Error)
            {
                State = FileStates.Finished;

                if (FileStream != null)
                {
                    FileStream.Close();
                    FileStream.Dispose();
                    FileStream = null;
                }
                FinishUpdate(sender, e);
            }
        }

        public void CancelUpload()
        {
            if (this._fileStream != null)
            {
                this._fileStream.Close();
                this._fileStream.Dispose();
                this._fileStream = null;
            }

            if (_fileUploader != null && this.State == FileStates.Uploading)
            {
                _fileUploader.CancelUpload();
            }

            _fileUploader = null;
        }

        private void UpdateChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}