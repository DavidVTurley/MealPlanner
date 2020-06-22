using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DatabaseConnection;
using MealPlanner.Model;

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

            ViewModel = MealPlannerModel.GetMealPlanner();
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
            //Generates a new mealplanner id
            MealPlannerModel newModel = MealPlannerModel.GetNewMealPlannerModel(ViewModel.GenerateNewListAmountToGenerate, ViewModel.NumberOfPeopleInput, ViewModel.NumberOfVegetarianInput);
            
            foreach (MealPlannerMeal mealPlannerMeal in newModel.Meals)
            {
                MealPlannerModel.CalculateMealIngredients(mealPlannerMeal);
            }

            ViewModel = newModel;
        }
    }
}
