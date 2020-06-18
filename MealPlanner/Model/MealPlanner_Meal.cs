using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DatabaseConnection;
using MealPlanner.Resources;

namespace MealPlanner.Model
{
    public class MealPlannerMeal : Meal
    {
        private Int32 _mealPlannerId;

        public Int32 MealPlannerId
        {
            get => _mealPlannerId;
            set
            {
                _mealPlannerId = value;
                OnPropertyChanged();
            }
        }

        private Int32 _numberOfPeople;
        private Int32 _numberOfVegetarians;

        public Int32 NumberOfPeople
        {
            get => _numberOfPeople;
            set
            {
                _numberOfPeople = value;
                OnPropertyChanged();
            }
        }

        public Int32 NumberOfVegetarians
        {
            get => _numberOfVegetarians;
            set
            {
                _numberOfVegetarians = value;
                OnPropertyChanged();
            }
        }

        public MealPlannerMeal(Meal meal, Int32 mealPlannerId, Int32 numberOfPeople, Int32 numberOfVegetarians)
        {
            NumberOfPeople = numberOfPeople;
            NumberOfVegetarians = numberOfVegetarians;
            MealPlannerId = mealPlannerId;
            MealId = meal.MealId;
            Name = meal.Name;
            Ingredients = meal.Ingredients;
        }
        public MealPlannerMeal(Int32 mealPlannerId, Int32 mealMealId = -1, String name = "", Int32 numberOfPeople = 5, Int32 numberOfVegtarians = 1, IEnumerable<Ingredient> ingredients = null)
        {
            MealPlannerId = mealPlannerId;
            MealId = mealMealId;
            Name = name;
            NumberOfPeople = numberOfPeople;
            NumberOfVegetarians = numberOfVegtarians;
        }

        public static void AddMealPlannerMealToDatabase(MealPlannerMeal meal, Int32 mealPlannerId)
        {
            DBConnection connection = new DBConnection();
            connection.OpenConnection();
            connection.Insert(
                $"INSERT INTO `mealplanner_meals`(`mealPlanner_ID`, `meal_Id`, `numberOfPeople`, `numberOfVegetarians`) VALUES ('{mealPlannerId}', '{meal.MealId}', '{meal.NumberOfPeople}', '{meal.NumberOfVegetarians}')");
            connection.CloseConnection();
        }

        public static void RemoveMealPlannerMealFromDatabase(Int32 mealId, Int32 mealPlannerId)
        {
            DBConnection connection = new DBConnection();

            connection.OpenConnection();
            connection.Delete(
                $"DELETE FROM `mealplanner_meals` WHERE `mealPlanner_ID` = '{mealPlannerId}' AND `meal_Id` = '{mealId}';");
        }

        /// <summary>
        /// Converts a meal
        /// </summary>
        public static List<MealPlannerMeal> ConvertMealsIntoMealPlannerMeals(List<Meal> meals, Int32 mealPlannerId,
            Int32 numberOfPeople, Int32 numberOfVegetarians)
        {
            return meals.Select(meal => new MealPlannerMeal(meal, mealPlannerId, 5, 1)).ToList();
        }

        public static List<MealPlannerMeal> GetRandomMeals(Int32 numberOfMeals, Int32 mealPlannerId, Int32 numberOfPeople, Int32 numberOfVegtarians)
        {
            List<Meal> meals = Meal.GetRandomMeals(numberOfMeals);
            return ConvertMealsIntoMealPlannerMeals(meals, mealPlannerId, numberOfPeople, numberOfVegtarians);
        }
    }
}
