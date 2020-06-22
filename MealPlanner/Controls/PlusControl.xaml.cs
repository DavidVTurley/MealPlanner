using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MealPlanner.Controls
{
    /// <summary>
    /// Interaction logic for PlusControl.xaml
    /// </summary>
    public partial class PlusControl : UserControl
    {
        public static readonly DependencyProperty BackgroundColourProperty = DependencyProperty.Register("BackgroundColour", typeof(SolidColorBrush), typeof(PlusControl), new PropertyMetadata(default(SolidColorBrush)));
        public SolidColorBrush BackgroundColour
        {
            get { return (SolidColorBrush)GetValue(BackgroundColourProperty); }
            set { SetValue(BackgroundColourProperty, value); }
        }


        public new static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(Int32), typeof(PlusControl), new PropertyMetadata(default(Int32)));
        public new Int32 Width
        {
            get { return (Int32) GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value);
                base.Width = value;
            }
        }


        public new static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(Int32), typeof(PlusControl), new PropertyMetadata(default(Int32)));
        public new Int32 Height
        {
            get { return (Int32) GetValue(HeightProperty); }
            set
            {
                SetValue(HeightProperty, value);
                base.Height = value;
            }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(String), typeof(PlusControl), new PropertyMetadata(default(String)));
        public String Text
        {
            get { return (String) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }



        public PlusControl()
        {
            InitializeComponent();
            DataContext = this;
            BackgroundColour = new SolidColorBrush(Colors.White);
            Width = 100;
            Height = 100;
        }

        public static readonly RoutedEvent ButtonClickedEvent = EventManager.RegisterRoutedEvent("OnButtonPressed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PlusControl));
        public event RoutedEventHandler OnButtonPressed
        {
            add { AddHandler(ButtonClickedEvent, value); }
            remove { RemoveHandler(ButtonClickedEvent, value); }
        }

        private void OnButtonPressedHandler(Object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ButtonClickedEvent));
        }
    }
}
