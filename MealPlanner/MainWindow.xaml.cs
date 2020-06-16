using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MealPlanner.Model;
using MealPlanner.View;

namespace MealPlanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow MainWindowAccessor;

        public MainWindow()
        {
            InitializeComponent();
            MainWindowAccessor = this;
        }

        public void ChangeContentWindow(UserControl control)
        {
            ContentStackPanel.Children.Clear();
            ContentStackPanel.Children.Add(control);
        }

        private void MealsNavButton_OnClick(Object sender, RoutedEventArgs e)
        {
            ChangeContentWindow(new View.MealPlannerView());
        }

        private void ManageMealsNavButton_OnClick(Object sender, RoutedEventArgs e)
        {
            ChangeContentWindow(new ManageMealsView());
        }

        private void MealPlannerIngredientsList_OnClick(Object sender, RoutedEventArgs e)
        {
            ChangeContentWindow(new MealIngredientsList());
        }
    }

    public class EventArgs<T> : EventArgs where T : UserControl
    {
        public UserControl UserControl;
        EventArgs(T userControl)
        {
            UserControl = userControl;
        }
    }
}
