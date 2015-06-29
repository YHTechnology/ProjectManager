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

namespace ProductManager.Controls
{
    public static class Message
    {

        /// <summary>
        /// Outputs a user-friendly error message.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public static void ErrorMessage(string message)
        {
           new CustomMessage(message, CustomMessage.MessageType.Error).Show();
        }

        /// <summary>
        /// Outputs a user-friendly info message.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public static void InfoMessage(string message)
        {
            new CustomMessage(message, CustomMessage.MessageType.Info).Show();
        }


    }
}
