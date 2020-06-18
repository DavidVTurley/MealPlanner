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
            MealPlannerMeal meal = MealPlannerMeal.GetRandomMeals(1, ViewModel.Id, ViewModel.NumberOfPeopleInput, ViewModel.NumberOfVegetarianInput)[0];
            MealPlannerMeal.AddMealPlannerMealToDatabase(meal, ViewModel.Id);
            MealPlannerModel.CalculateMealIngredients(meal);
            ViewModel.Meals.Add(meal);
        }

        private void RefreshMealClick(Object sender, RoutedEventArgs e)
        {
            //Gets the list of selected items
            Int32 numberOfSelectedItems = ListViewMeals.SelectedItems.Count;
            List<MealPlannerMeal> mealPlannerMeal = new List<MealPlannerMeal>(ListViewMeals.SelectedItems.Cast<MealPlannerMeal>());
            
            // Remove the link to the meal
            foreach (MealPlannerMeal mealToRemove in mealPlannerMeal)
            {
                // Remove value from database
                MealPlannerMeal.RemoveMealPlannerMealFromDatabase(mealToRemove.MealId, mealToRemove.MealPlannerId);
                // Get new mealPlannerMeal
                MealPlannerMeal meal = MealPlannerMeal.ConvertMealsIntoMealPlannerMeals(Meal.GetRandomMeals(1), mealToRemove.MealPlannerId, mealToRemove.NumberOfPeople, mealToRemove.NumberOfVegetarians)[0];
                // Adds mealPlanner Meal to the database
                MealPlannerMeal.AddMealPlannerMealToDatabase(meal, meal.MealPlannerId);
                // Add the mealplanner to the list of meals
                MealPlannerModel.CalculateMealIngredients(meal);
                ViewModel.Meals[ViewModel.Meals.IndexOf(mealToRemove)] = meal;
            }
        }
        private void GenerateNewMealList_Click(Object sender, RoutedEventArgs e)
        {

            MealPlannerModel oldVieModel = ViewModel;

            //Generates a new mealplanner id
            ViewModel = MealPlannerModel.GetNewMealPlannerModel(ViewModel.GenerateNewListAmountToGenerate, ViewModel.NumberOfPeopleInput, ViewModel.NumberOfVegetarianInput);
            foreach (MealPlannerMeal mealPlannerMeal in ViewModel.Meals)
            {
                MealPlannerModel.CalculateMealIngredients(mealPlannerMeal);
            }
        }
    }
}
