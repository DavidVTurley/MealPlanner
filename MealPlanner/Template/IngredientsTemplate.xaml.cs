using System;
using System.Windows;
using System.Windows.Controls;

namespace MealPlanner.Template
{
    /// <summary>
    /// Interaction logic for IngredientsTemplate.xaml
    /// </summary>
    public partial class IngredientsTemplate : UserControl
    {
        public static readonly RoutedEvent RefreshMealEvent = EventManager.RegisterRoutedEvent("RefreshMeal", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(IngredientsTemplate));
        public event RoutedEventHandler RefreshMeal
        {
            add { AddHandler(RefreshMealEvent, value); }
            remove { RemoveHandler(RefreshMealEvent, value); }
        }

        public static readonly DependencyProperty RefreshButtonEnabledProperty = DependencyProperty.Register("RefreshButtonEnabled", typeof(Visibility), typeof(IngredientsTemplate), new PropertyMetadata(Visibility.Hidden));
        public Visibility RefreshButtonEnabled
        {
            get { return (Visibility) GetValue(RefreshButtonEnabledProperty); }
            set { SetValue(RefreshButtonEnabledProperty, value); }
        }

        public IngredientsTemplate()
        {
            InitializeComponent();
        }

        private void VegitairianCheckBox_OnClick(Object sender, RoutedEventArgs e)
        {
            DisplayReplaceMeatValue((CheckBox)sender);
        }
        public void DisplayReplaceMeatValue(CheckBox chenBox)
        {
            switch (VegetarianCheckBox.IsChecked)
            {
                case true:
                    ReplaceTheMeatText.Visibility = Visibility.Visible;
                    ReplaceTheMeatCheckBox.Visibility = Visibility.Visible;
                    break;
                case false:
                    ReplaceTheMeatText.Visibility = Visibility.Hidden;
                    ReplaceTheMeatCheckBox.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void RefreshButtonClick(Object sender, RoutedEventArgs e)
        {
            RefreshMealEventArgs newEventArgs = new RefreshMealEventArgs();
            RaiseEvent(newEventArgs);
        }
    }

    public class RefreshMealEventArgs : RoutedEventArgs
    {
        public Int32 IngredientId;
    }
}
