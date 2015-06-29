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
using ProductManager.Resource;

namespace ProductManager
{
    public sealed class ApplicationResource
    {
        private static readonly ApplicationString applicationString = new ApplicationString();

        private static readonly ModuleString moduleString = new ModuleString();

        public ApplicationString ApplicationString
        {
            get { return applicationString; }
        }

        public ModuleString ModuleString
        {
            get { return moduleString; }
        }
    }
}
