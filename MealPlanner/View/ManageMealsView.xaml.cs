using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DatabaseConnection.Model;
using MealPlanner.ViewModel;
using Ingredient = MealPlanner.Model.Ingredient;
using Meal = MealPlanner.Model.Meal;

namespace MealPlanner.View
{
    /// <summary>
    /// Interaction logic for ManageMealsView.xaml
    /// </summary>
    public partial class ManageMealsView : UserControl
    {
        public MealsViewModel ViewModel = new MealsViewModel(new List<Meal>());

        public ManageMealsView()
        {
            InitializeComponent();

            List<Meal> meals = new List<Meal>();

            Connection.DbConnection.OpenConnection();
            Connection.DbConnection.Select("SELECT * FROM `meal`");
            while (Connection.DbConnection.Read())
            {
                meals.Add(new Meal(Connection.DbConnection.GetString("mealName"), null, Connection.DbConnection.GetInt32("id")));
            }

            Connection.DbConnection.CloseConnection();
            foreach (Meal meal in meals)
            {
                Connection.DbConnection.OpenConnection();

                String sql = "SELECT * FROM ingredients WHERE id IN (SELECT ingredient_id FROM meal_ingredients WHERE meal_id = " + meal.MealId + ");";
                Connection.DbConnection.Select(sql);
                while (Connection.DbConnection.Read())
                {
                    Enum.TryParse<IngredientServingSizeEnum>(Connection.DbConnection.GetString("servingUnit"), out IngredientServingSizeEnum servingUnit);
                    var q = Connection.DbConnection.GetString("ingredientName");
                    var w = Connection.DbConnection.GetInt32("servingSize");
                    var e = (Int32)servingUnit;
                    var r = Connection.DbConnection.GetBoolean("vegetarian");
                    var t = Connection.DbConnection.GetBoolean("replaceIfVegetarian");
                    var y = Connection.DbConnection.GetInt32("id");




                    meal.Ingredients.Add(new Ingredient(
                        Connection.DbConnection.GetString("ingredientName"),
                        Connection.DbConnection.GetInt32("servingSize"),
                        (Int32)servingUnit,
                        Connection.DbConnection.GetBoolean("vegetarian"),
                        Connection.DbConnection.GetBoolean("replaceIfVegetarian"),
                        Connection.DbConnection.GetInt32("id")
                    ));
                }
                Connection.DbConnection.CloseConnection();
            }

            ViewModel.Meals = new ObservableCollection<Meal>(meals);
            DataContext = ViewModel.Meals;


            //ViewModel.Meals = new ObservableCollection<Meal>(new List<Meal>
            //{
            //    //todo Load all meals
            //    new Meal("Steak",
            //        new List<Ingredient>
            //        {
            //            new Ingredient("Something", 12, new List<String> {"Grams", "Liters"}, 0, false),
            //            new Ingredient("Something", 12, new List<String> {"Grams"}, 0, false),
            //            new Ingredient("Something", 12, new List<String> {"Grams"}, 0, false),
            //        }),
            //    new Meal("Steak",
            //        new List<Ingredient>
            //        {
            //            new Ingredient("Something", 12, new List<String> {"Grams"}, 0, false),
            //            new Ingredient("Something", 12, new List<String> {"Grams"}, 0, false),
            //            new Ingredient("Something", 12, new List<String> {"Grams"}, 0, false),
            //        }),
            //    new Meal("Steak",
            //        new List<Ingredient>
            //        {
            //            new Ingredient("Something", 12, new List<String> {"Grams"}, 0, false),
            //            new Ingredient("Something", 12, new List<String> {"Grams"}, 0, false),
            //            new Ingredient("Something", 12, new List<String> {"Grams"}, 0, false),
            //        }),
            //    new Meal("Steak",
            //        new List<Ingredient>
            //        {
            //            new Ingredient("Something", 12, new List<String> {"Grams"}, 0, false),
            //            new Ingredient("Something", 12, new List<String> {"Grams"}, 0, false),
            //            new Ingredient("Something", 12, new List<String> {"Grams"}, 0, false),
            //        }),
            //    new Meal("Steak",
            //        new List<Ingredient>
            //        {
            //            new Ingredient("Something", 12, new List<String> {"Grams"}, 0, false),
            //            new Ingredient("Something", 12, new List<String> {"Grams"}, 0, false),
            //            new Ingredient("Something", 12, new List<String> {"Grams"}, 0, false),
            //        })
            //});

        }

        public ManageMealsView(List<Meal> meals)
        {
            ViewModel.Meals = new ObservableCollection<Meal>(meals);
            DataContext = ViewModel.Meals;
        }


        private void AddClick(Object sender, RoutedEventArgs e)
        {
            MainWindow.MainWindowAccessor.ChangeContentWindow(new AddEditMealView(new Meal("", new List<Ingredient>())));
        }
        private void EditClick(Object sender, RoutedEventArgs e)
        {
            if (ListView.SelectedItems.Count != 1) return;
            Meal meal = (Meal)ListView.SelectedItems[0];
            MainWindow.MainWindowAccessor.ChangeContentWindow(new AddEditMealView(meal));
        }
        private void DeleteClick(Object sender, RoutedEventArgs e)
        {
            if (ListView.SelectedItems.Count <= 0) return;
            List<Meal> meals = new List<Meal>(ListView.SelectedItems.Cast<Meal>());
            foreach (Meal meal in meals)
            {
                Meal.DeleteMealInDatabase(meal);
                ViewModel.Meals.Remove(meal);
            }
        }

    }
}
