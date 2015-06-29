using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ProductManager
{
    public class NotifyPropertyChanged : INotifyPropertyChanged , IDataErrorInfo
    {
#region INotifyPropertyChanged Member
        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateChanged(String aPropertyName)
        {
            if (aPropertyName != null && PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(aPropertyName));
            }
        }
#endregion

#region IDataErrorInfo Member

        private string _dataError = string.Empty;
        private Dictionary<string, string> _dataErrors = new Dictionary<string, string>();

        public string Error
        {
            get { return _dataError; }
        }

        public string this[string columnName]
        {
            get
            {
                if (_dataErrors.ContainsKey(columnName))
                    return _dataErrors[columnName];
                else
                    return null;
            }
        }
#endregion

        public void AddError(string name, string error)
        {
            _dataErrors[name] = error;
            this.UpdateChanged(name);
        }

        public void RemoveError(string name)
        {
            if (_dataErrors.ContainsKey(name))
            {
                _dataErrors.Remove(name);
                this.UpdateChanged(name);
            }
        }

        public void ClearError()
        {
            var keys = new string[_dataErrors.Count];
            _dataErrors.Keys.CopyTo(keys, 0);
            foreach (var key in keys)
            {
                this.RemoveError(key);
            }
        }

        public bool Validate()
        {
            //this.ClearError();
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(this, new ValidationContext(this, null, null), results, true))
            {
                foreach (var result in results)
                {
                    this.AddError(result.MemberNames.First(), result.ErrorMessage);
                }
                return false;
            }
            if (_dataErrors.Count > 0)
            {
                return false;
            }
            
            return true;
        }

    }
}
