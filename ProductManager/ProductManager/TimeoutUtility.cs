using System;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.DomainServices.Client;
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
    public static class TimeoutUtility
    {
        public static void ChangeSentTimeout(DomainContext aContext, TimeSpan aSendTimeout)
        {
            PropertyInfo lChangeFactoryProperty = aContext.DomainClient.GetType().GetProperty("ChannelFactory");
            if (lChangeFactoryProperty == null)
            {
                throw new InvalidOperationException("There is no ChannelFactory property on the DomainClient.");
            }

            ChannelFactory lFactory = (ChannelFactory)lChangeFactoryProperty.GetValue(aContext.DomainClient, null);
            lFactory.Endpoint.Binding.ReceiveTimeout = aSendTimeout;
            lFactory.Endpoint.Binding.ReceiveTimeout = aSendTimeout;
        }
    }
}
