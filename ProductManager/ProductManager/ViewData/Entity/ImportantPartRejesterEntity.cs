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
    public class ImportantPartRejesterEntity : NotifyPropertyChanged
    {
        private int importantPartRejesterID;
        public int ImportantPartRejesterID
        {
            get
            {
                return importantPartRejesterID;
            }
            set
            {
                if (importantPartRejesterID != value)
                {
                    importantPartRejesterID = value;
                    UpdateChanged("ImportantPartRejesterID");
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

        private String importantPartType;
        public String ImportantPartType
        {
            get
            {
                return importantPartType;
            }
            set
            {
                if (importantPartType != value)
                {
                    importantPartType = value;
                    UpdateChanged("ImportantPartType");
                }
            }
        }

        private String outputNumber;
        public String OutputNumber
        {
            get
            {
                return outputNumber;
            }
            set
            {
                if (outputNumber != value)
                {
                    outputNumber = value;
                    UpdateChanged("OutputNumber");
                }
            }
        }

        private DateTime outputTime;
        public DateTime OutputTime
        {
            get
            {
                return outputTime;
            }
            set
            {
                if (outputTime != value)
                {
                    outputTime = value;
                    UpdateChanged("OutputTime");
                    UpdateChanged("OurputTimeString");
                }
            }
        }

        public String OutputTimeString
        {
            get
            {
                if (outputTime != DateTime.MinValue)
                {
                    return outputTime.ToShortDateString();
                }
                else
                {
                    return "";
                }
            }
        }

        private String importantPartManu;
        public String ImportantPartManu
        {
            get
            {
                return importantPartManu;
            }
            set
            {
                if (importantPartManu != value)
                {
                    importantPartManu = value;
                    UpdateChanged("ImportantPartManu");
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

        private int inputUserID;
        public int InputUserID
        {
            get
            {
                return inputUserID;
            }
            set
            {
                if (inputUserID != value)
                {
                    inputUserID = value;
                    UpdateChanged("InputUserID");
                    UpdateChanged("InputUserString");
                }
            }
        }

        public String InputUserString
        {
            get
            {
                UserEntity aUerEntity;
                if (UserEntityDictionary != null && UserEntityDictionary.TryGetValue(inputUserID, out aUerEntity))
                {
                    return aUerEntity.CUserName;
                }
                return "";
            }
        }

        private DateTime inputDateTime;
        public DateTime InputDateTime
        {
            get
            {
                return inputDateTime;
            }
            set
            {
                if (inputDateTime != value)
                {
                    inputDateTime = value;
                    UpdateChanged("InputDateTime");
                    UpdateChanged("InputDateTimeString");
                }
            }
        }

        public String InputDateTimeString
        {
            get
            {
                if (inputDateTime != DateTime.MinValue)
                {
                    return inputDateTime.ToShortDateString();
                }
                else
                {
                    return "";
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

        private DateTime deleteDateTime;
        public DateTime DeleteDateTime
        {
            get
            {
                return deleteDateTime;
            }
            set
            {
                if (deleteDateTime != value)
                {
                    deleteDateTime = value;
                    UpdateChanged("DeleteDateTime");
                    UpdateChanged("DeleteDateTimeString");
                }
            }
        }

        public String DeleteDateTimeString
        {
            get
            {
                if (deleteDateTime != DateTime.MinValue)
                {
                    return deleteDateTime.ToShortDateString();
                }
                else
                {
                    return "";
                }
            }
        }

        private String deleteReason;
        public String DeleteReason
        {
            get
            {
                return deleteReason;
            }
            set
            {
                if (deleteReason != value)
                {
                    deleteReason = value;
                    UpdateChanged("DeleteReason");
                }
            }
        }

        public void Update()
        {
            this.importantPartRejesterID = ImportantPartRejester.important_part_rejester_id;
            this.manufactureNumber = ImportantPartRejester.manufacture_number;
            this.importantPartName = ImportantPartRejester.important_part_name;
            this.importantPartManufacturers = ImportantPartRejester.important_part_manufacturers;
            this.importantPartType = ImportantPartRejester.important_part_type;
            this.outputNumber = ImportantPartRejester.output_number;
            this.outputTime = ImportantPartRejester.output_time.GetValueOrDefault();
            this.importantPartManu = ImportantPartRejester.important_part_manu;
            this.note = ImportantPartRejester.note;
            this.isDelete = ImportantPartRejester.isDelete.GetValueOrDefault(false);
            this.inputUserID = ImportantPartRejester.input_user_id.GetValueOrDefault(-1);
            this.inputDateTime = ImportantPartRejester.input_datetime.GetValueOrDefault();
            this.modifyUserID = ImportantPartRejester.modify_user_id.GetValueOrDefault(-1);
            this.modifyDateTime = ImportantPartRejester.modify_datetime.GetValueOrDefault();
            this.deleteUserID = ImportantPartRejester.delete_user_id.GetValueOrDefault(-1);
            this.deleteDateTime = ImportantPartRejester.delete_datetime.GetValueOrDefault();
            this.deleteReason = ImportantPartRejester.delete_reason;
        }

        public void DUpdate()
        {
            ImportantPartRejester.important_part_rejester_id = this.importantPartRejesterID;
            ImportantPartRejester.manufacture_number = this.manufactureNumber;
            ImportantPartRejester.important_part_name = this.importantPartName;
            ImportantPartRejester.important_part_manufacturers = this.importantPartManufacturers;
            ImportantPartRejester.important_part_type = this.importantPartType;
            ImportantPartRejester.output_number = this.outputNumber;
            ImportantPartRejester.output_time = this.outputTime;
            ImportantPartRejester.important_part_manu = this.importantPartManu;
            ImportantPartRejester.note = this.note;
            ImportantPartRejester.isDelete = this.isDelete;
            ImportantPartRejester.input_user_id = this.inputUserID;
            ImportantPartRejester.input_datetime = this.inputDateTime;
            ImportantPartRejester.modify_user_id = this.modifyUserID;
            ImportantPartRejester.modify_datetime = this.modifyDateTime;
            ImportantPartRejester.delete_user_id = this.deleteUserID;
            ImportantPartRejester.delete_datetime = this.deleteDateTime;
            ImportantPartRejester.delete_reason = this.deleteReason;
        }

        public void RaisALL()
        {
            UpdateChanged("ImportantPartRejesterID");
            UpdateChanged("ManufactureNumber");
            UpdateChanged("ImportantPartName");
            UpdateChanged("ImportantPartManufacturers");
            UpdateChanged("ImportantPartType");
            UpdateChanged("OutputNumber");
            UpdateChanged("OutputTime");
            UpdateChanged("OurputTimeString");
            UpdateChanged("ImportantPartManu");
            UpdateChanged("Note");
            UpdateChanged("IsDelete");
            UpdateChanged("IsDeleteString");
            UpdateChanged("InputUserID");
            UpdateChanged("InputUserString");
            UpdateChanged("InputDateTime");
            UpdateChanged("InputDateTimeString");
            UpdateChanged("ModifyUserID");
            UpdateChanged("ModifyUserString");
            UpdateChanged("ModifyDateTime");
            UpdateChanged("ModifyDateTimeString");
            UpdateChanged("DeleteUserID");
            UpdateChanged("DeleteUserString");
            UpdateChanged("DeleteDateTime");
            UpdateChanged("DeleteDateTimeString");
            UpdateChanged("DeleteReason");
            
        }

        public ProductManager.Web.Model.important_part_rejester ImportantPartRejester { get; set; }

        public Dictionary<int, UserEntity> UserEntityDictionary { get; set; }
    }
}
