using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ProductManager
{
    public class DoubleClickBehavior : Behavior<FrameworkElement>
    {
        private const int ClickThresholdInMiliseconds = 300;

        private DateTime? LastClick { get; set; }

        private object LastSource { get; set; }
        public event EventHandler<MouseButtonEventArgs> DoubleClick;

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(DoubleClickBehavior), new PropertyMetadata(CommandParameterChanged));

        private static void CommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public ICommand DoubleClickCommand
        {
            get { return (ICommand)GetValue(DoubleClickCommandProperty); }
            set { SetValue(DoubleClickCommandProperty, value); }
        }

        public static readonly DependencyProperty DoubleClickCommandProperty = DependencyProperty.Register("DoubleClickCommand", typeof(ICommand), typeof(DoubleClickBehavior), new PropertyMetadata(DoubleClickCommandChanged));

        private static void DoubleClickCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public DoubleClickBehavior()
        {
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseLeftButtonUp += new MouseButtonEventHandler(this.AssociatedObject_MouseLeftButtonUp);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.MouseLeftButtonUp -= new MouseButtonEventHandler(this.AssociatedObject_MouseLeftButtonUp);
        }

        void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.LastSource == null || !object.Equals(this.LastSource, e.OriginalSource))
            {
                this.LastSource = e.OriginalSource;
                this.LastClick = DateTime.Now;
            }
            else if ((DateTime.Now - this.LastClick.Value).Milliseconds <= DoubleClickBehavior.ClickThresholdInMiliseconds)
            {
                this.LastClick = null;
                this.LastSource = null;
                if (this.DoubleClick != null)
                    this.DoubleClick(sender, e);
                if (DoubleClickCommand != null)
                {
                    DoubleClickCommand.Execute(sender);
                }
            }
            else
            {
                this.LastClick = null;
                this.LastSource = null;
            }
        }
    } 
}
