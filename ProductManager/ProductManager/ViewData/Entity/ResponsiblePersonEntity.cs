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
    public class ResponsiblePersonEntity : NotifyPropertyChanged
    {
        public String ResponsiblePerson { get; set; }
        public String ManufactureNumber { get; set; }
        public String ProjectName { get; set; }
        public String ProjectNote { get; set; }
        public Nullable<DateTime> RecoderDateTime { get; set; }
        public Nullable<DateTime> OutputDateTime { get; set; }
    }
}
