using System;
using System.Collections.Generic;
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
    public class ImportantPartEntity : NotifyPropertyChanged
    {
        private int importantPartID;
        public int ImportantPartID
        {
            get
            {
                return importantPartID;
            }
            set
            {
                if (importantPartID != value)
                {
                    importantPartID = value;
                    UpdateChanged("ImportantPartID");
                }
            }
        }

        private String manufactureNumber;
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

        public String ProjectName { get; set; }

        private String importantPartName;
        public String ImportantPartName
        {
            get
            {
                return importantPartName;
            }
            set
            {
                if (importantPartName != value)
                {
                    importantPartName = value;
                    UpdateChanged("ImportantPartName");
                }
            }
        }

        private String importantPartManufacturers;
        public String ImportantPartManufacturers
        {
            get
            {
                return importantPartManufacturers;
            }
            set
            {
                if (importantPartManufacturers != value)
                {
                    importantPartManufacturers = value;
                    UpdateChanged("ImportantPartManufacturers");
                }
            }
        }

        private Nullable<DateTime> ariveTime;
        public Nullable<DateTime> AriveTime
        {
            get
            {
                return ariveTime;
            }
            set
            {
                if (ariveTime != value)
                {
                    ariveTime = value;
                    UpdateChanged("AriveTime");
                }
            }
        }

        private int ariveUserId;
        public int AriveUserId
        {
            get
            {
                return ariveUserId;
            }
            set
            {
                if (ariveUserId != value)
                {
                    ariveUserId = value;
                    UpdateChanged("AriveUserId");
                    UpdateChanged("AriveUserString");
                }
            }
        }

        public String AriveUserString
        {
            get
            {
                UserEntity aUerEntity;
                if (UserEntityDictionary != null && UserEntityDictionary.TryGetValue(ariveUserId, out aUerEntity))
                {
                    return aUerEntity.CUserName;
                }
                return "";
            }
        }

        private Nullable<DateTime> ariveInputTime;
        public Nullable<DateTime> AriveInputTime
        {
            get
            {
                return ariveInputTime;
            }
            set
            {
                if (ariveInputTime != value)
                {
                    ariveInputTime = value;
                    UpdateChanged("AriveInputTime");
                }
            }
        }

        private int modifyUserID;
        public int ModifyUserID
        {
            get
            {
                return modifyUserID;
            }
            set
            {
                if (modifyUserID != value)
                {
                    modifyUserID = value;
                    UpdateChanged("ModifyUserID");
                    UpdateChanged("ModifyUserString");
                }
            }
        }

        public String ModifyUserString
        {
            get
            {
                UserEntity aUerEntity;
                if (UserEntityDictionary != null && UserEntityDictionary.TryGetValue(modifyUserID, out aUerEntity))
                {
                    return aUerEntity.CUserName;
                }
                return "";
            }
        }

        private DateTime modifyDateTime;
        public DateTime ModifyDateTime
        {
            get
            {
                return modifyDateTime;
            }
            set
            {
                if (modifyDateTime != value)
                {
                    modifyDateTime = value;
                    UpdateChanged("ModifyDateTime");
                    UpdateChanged("ModifyDateTimeString");
                }
            }
        }

        public String ModifyDateTimeString
        {
            get
            {
                if (modifyDateTime != DateTime.MinValue)
                {
                    return modifyDateTime.ToShortDateString();
                }
                else
                {
                    return "";
                }
            }
        }

        private String note;
        public String Note
        {
            get
            {
                return note;
            }
            set
            {
                if (note != value)
                {
                    note = value;
                    UpdateChanged("Note");
                }
            }
        }

        private bool isDelete;
        public bool IsDelete
        {
            get
            {
                return isDelete;
            }
            set
            {
                if (isDelete != value)
                {
                    isDelete = value;
                    UpdateChanged("IsDelete");
                    UpdateChanged("IsDeleteString");
                }
            }
        }

        public String IsDeleteString
        {
            get
            {
                if (isDelete)
                {
                    return "是";
                }
                else
                {
                    return "否";
                }
            }
        }

        private int deleteUserID;
        public int DeleteUserID
        {
            get
            {
                return deleteUserID;
            }
            set
            {
                if (deleteUserID != value)
                {
                    deleteUserID = value;
                    UpdateChanged("DeleteUserID");
                    UpdateChanged("DeleteUserString");
                }
            }
        }

        public String DeleteUserString
        {
            get
            {
                UserEntity aUerEntity;
                if (UserEntityDictionary != null && UserEntityDictionary.TryGetValue(deleteUserID, out aUerEntity))
                {
                    return aUerEntity.CUserName;
                }
                return "";
            }
        }

        private DateTime deleteTime;
        public DateTime DeleteTime
        {
            get
            {
                return deleteTime;
            }
            set
            {
                if (deleteTime != value)
                {
                    deleteTime = value;
                    UpdateChanged("DeleteTime");
                    UpdateChanged("DeleteTimeString");
                }
            }
        }

        public String DeleteTimeString
        {
            get
            {
                if (deleteTime != DateTime.MinValue)
                {
                    return deleteTime.ToShortDateString();
                }
                else
                {
                    return "";
                }
            }
        }

        private String deleteNote;
        public String DeleteNote
        {
            get
            {
                return deleteNote;
            }
            set
            {
                if (deleteNote != value)
                {
                    deleteNote = value;
                    UpdateChanged("DeleteNote");
                }
            }
        }

        public void Update()
        {
            this.importantPartID = ImportantPart.important_part_id;
            this.manufactureNumber = ImportantPart.manufacture_number;
            this.importantPartName = ImportantPart.important_part_name;
            this.importantPartManufacturers = ImportantPart.important_part_manufacturers;
            this.ariveTime = ImportantPart.arive_time;
            this.ariveUserId = ImportantPart.arive_user_id.GetValueOrDefault(-1);
            this.ariveInputTime = ImportantPart.arive_input_time;
            this.modifyUserID = ImportantPart.modify_user_id.GetValueOrDefault(-1);
            this.modifyDateTime = ImportantPart.modify_time.GetValueOrDefault();
            this.note = ImportantPart.note;
            this.isDelete = ImportantPart.isDelete.GetValueOrDefault(false);
            this.deleteUserID = ImportantPart.delete_user_id.GetValueOrDefault(-1);
            this.deleteTime = ImportantPart.delete_time.GetValueOrDefault();
            this.deleteNote = ImportantPart.delete_note;
        }

        public void DUpdate()
        {
            ImportantPart.important_part_id = this.importantPartID;
            ImportantPart.manufacture_number = this.manufactureNumber;
            ImportantPart.important_part_name = this.importantPartName;
            ImportantPart.important_part_manufacturers = this.importantPartManufacturers;
            ImportantPart.arive_time = this.ariveTime;
            ImportantPart.arive_user_id = this.ariveUserId;
            ImportantPart.arive_input_time = this.ariveInputTime;
            ImportantPart.modify_user_id = this.modifyUserID;
            ImportantPart.modify_time = this.modifyDateTime;
            ImportantPart.note = this.note;
            ImportantPart.isDelete = this.isDelete;
            ImportantPart.delete_user_id = this.deleteUserID;
            ImportantPart.delete_time = this.deleteTime;
            ImportantPart.delete_note = this.deleteNote;
        }

        public void RaisALL()
        {
            UpdateChanged("ImportantPartID");
            UpdateChanged("ManufactureNumber");
            UpdateChanged("ImportantPartName");
            UpdateChanged("ImportantPartManufacturers");
            UpdateChanged("AriveTime");
            UpdateChanged("AriveUserId");
            UpdateChanged("AriveUserString");
            UpdateChanged("AriveInputTime");
            UpdateChanged("ModifyUserID");
            UpdateChanged("ModifyUserString");
            UpdateChanged("ModifyDateTime");
            UpdateChanged("ModifyDateTimeString");
            UpdateChanged("Note");
            UpdateChanged("IsDelete");
            UpdateChanged("IsDeleteString");
            UpdateChanged("DeleteUserID");
            UpdateChanged("DeleteUserString");
            UpdateChanged("DeleteTime");
            UpdateChanged("DeleteTimeString");
            UpdateChanged("DeleteNote");
        }

        public ProductManager.Web.Model.important_part ImportantPart { get; set; }

        public Dictionary<int, UserEntity> UserEntityDictionary { get; set; }
    }
}
