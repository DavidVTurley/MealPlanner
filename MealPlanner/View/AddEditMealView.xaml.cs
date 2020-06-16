using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DatabaseConnection;
using MealPlanner.Model;
using MealPlanner.ViewModel;

namespace MealPlanner.View
{
    /// <summary>
    /// Interaction logic for AddMealsView.xaml
    /// </summary>
    public partial class AddEditMealView : UserControl
    {
        public MealViewModel ViewModel;
        private List<Int32> _removedIngredientIds = new List<Int32>();

        public AddEditMealView()
        {
            InitializeComponent();
            ViewModel = new MealViewModel();
            DataContext = ViewModel.Meal;
        }

        public AddEditMealView(Meal meal) : this()
        {
            ViewModel.Meal = meal;
            DataContext = ViewModel.Meal;
        }

        private void AddIngredientClick(Object sender, RoutedEventArgs e)
        {
            ViewModel.AddIngredient(new Ingredient());
        }

        private void RemoveIngredientClick(Object sender, RoutedEventArgs e)
        {
            if (ListView.SelectedItems.Count <= 0) return;
            List<Ingredient> ingredients = new List<Ingredient>(ListView.SelectedItems.OfType<Ingredient>());
            foreach (Ingredient ingredient in ingredients)
            {
                if (ingredient.Id >= 0)
                {
                    _removedIngredientIds.Add(ingredient.Id);
                }

                ViewModel.RemoveIngredient(ingredient);
            }
        }

        private void OnSaveClick(Object sender, RoutedEventArgs e)
        {
            if (ViewModel.Meal.MealId >= 0)
            {
                // edit meal
                Meal.EditMealInDatabase(ViewModel.Meal);
            }
            else
            {
                // add new meal
                Meal.AddMealToDataBase(ViewModel.Meal);
            }

            //TODO Remove Removed ingredients
        }
    }
}
