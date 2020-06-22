using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using MealPlanner.View;
using UserControl = System.Windows.Controls.UserControl;

namespace MealPlanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow MainWindowAccessor;
        //public Process Xammp;

        public MainWindow()
        {
            InitializeComponent();
            MainWindowAccessor = this;
            //Xammp = new Process();

            //Xammp.StartInfo.FileName = @"Misc install\Xampp\xampp-control.exe";
            //Xammp.EnableRaisingEvents = true;

            //Xammp.Start();

            
        }
        

        public void ChangeContentWindow(UserControl control)
        {
            ContentGrid.Children.Clear();
            ContentGrid.Children.Add(control);
        }

        private void MealsNavButton_OnClick(Object sender, RoutedEventArgs e)
        {
            ChangeContentWindow(new MealPlannerView());
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
