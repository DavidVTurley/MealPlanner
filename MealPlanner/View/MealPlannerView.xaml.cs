using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DatabaseConnection;
using MealPlanner.Model;
using MealPlanner.Resources;
using MealPlanner.ViewModel;

namespace MealPlanner.View
{
    /// <summary>
    /// Interaction logic for GenerateMealsListUserControl.xaml
    /// </summary>
    public partial class MealPlannerView : UserControl
    {
        private MealPlannerModel _viewModel;

        public MealPlannerModel ViewModel
        {
            get => _viewModel;
            set
            {
                _viewModel = value;
                SetDatacontext();
            }
        }

        public MealPlannerView()
        {
            InitializeComponent();
            DBConnection connection = new DBConnection();

            ViewModel = MealPlannerModel.GetMealPlanner();

            SetDatacontext();
        }

        private void SetDatacontext()
        {
            if (ViewModel == null) return;
            DataContext = ViewModel;

            if (ViewModel.Meals == null) return;
            ListViewMeals.DataContext = ViewModel.Meals;
            ListViewMeals.ItemsSource = ViewModel.Meals;
        }

        private void AddNewMeal_Click(Object sender, RoutedEventArgs e)
        {

            MealPlannerModel.AddNewMealToMealPlannerModel(ViewModel, ViewModel);
        }

        private void RefreshMealClick(Object sender, RoutedEventArgs e)
        {
            Int32 numberOfSelectedItems = ListViewMeals.SelectedItems.Count;
            List<MealPlannerMeal> mealPlannerMeal = new List<MealPlannerMeal>(ListViewMeals.SelectedItems.Cast<MealPlannerMeal>());

            foreach (MealPlannerMeal meal in mealPlannerMeal)
            {
                DBConnection connection = new DBConnection();

                connection.OpenConnection();
                connection.Delete($"DELETE FROM `mealplanner_meals` WHERE `mealPlanner_ID` = '{meal.MealPlannerId}' AND `meal_Id` = '{meal.MealId}'; ");
                connection.CloseConnection();

                ViewModel.Meals.Remove(meal);
                MealPlannerModel.AddNewMealToMealPlannerModel(numberOfSelectedItems, ViewModel.NumberOfPeopleInput, ViewModel.NumberOfVegetarianInput, ViewModel);
            }
        }
        private void GenerateNewMealList_Click(Object sender, RoutedEventArgs e)
        {

            MealPlannerModel oldVieModel = ViewModel;

            //Generates a new mealplanner id
            ViewModel = MealPlannerModel.CreateNewMealPlanner(new List<MealPlannerMeal>(), ViewModel.NumberOfPeopleInput);

            MealPlannerModel.AddNewMealToMealPlannerModel(oldVieModel, ViewModel);
        }
    }
}
