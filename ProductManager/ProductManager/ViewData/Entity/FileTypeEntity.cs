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

namespace ProductManager.ViewData.Entity
{
    public class FileTypeEntity : NotifyPropertyChanged
    {
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

        public void Update()
        {
            this.fileTypeID = FileType.file_type_id;
            this.fileTypeName = FileType.file_type_name;
        }

        public void DUpdate()
        {
            FileType.file_type_id = this.fileTypeID;
            FileType.file_type_name = this.fileTypeName;
        }

        public void RaisALL()
        {
            UpdateChanged("FileTypeID");
            UpdateChanged("FileTypeName");
        }

        public ProductManager.Web.Model.filetype FileType { get; set; }
    }
}
