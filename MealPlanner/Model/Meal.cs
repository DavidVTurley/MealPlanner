using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DatabaseConnection;

namespace MealPlanner.Model
{
    public class Meal : BaseModel
    {
        private Int32 _mealId = -1;
        public Int32 MealId
        {
            get => _mealId;
            set
            {
                if (value == _mealId) return;
                _mealId = value;
                OnPropertyChanged();
            }
        }


        private String _name;
        private ObservableCollection<Ingredient> _ingredients = new ObservableCollection<Ingredient>();

        public String Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Ingredient> Ingredients
        {
            get => _ingredients;
            set
            {
                if (Equals(value, _ingredients)) return;
                _ingredients = value;
                OnPropertyChanged();
            }
        }


        public Meal()
        {
            Name = "";
            Ingredients = new ObservableCollection<Ingredient>(new List<Ingredient>());
        }
        public Meal(String name, IEnumerable<Ingredient> ingredients = null, Int32 mealId = -1)
        {
            _name = name;
            _mealId = mealId;
            if (ingredients != null)
                Ingredients = new ObservableCollection<Ingredient>(ingredients);
            else
                Ingredients = new ObservableCollection<Ingredient>(new List<Ingredient>());
        }

        public static void AddMealToDataBase(Meal meal)
        {
            Connection.DbConnection.OpenConnection();
            Connection.DbConnection.Insert($"INSERT INTO `meal`(`mealName`) VALUES ('{meal.Name}')");
            Connection.DbConnection.CloseConnection();
            Connection.DbConnection.OpenConnection();
            meal.MealId = Connection.DbConnection.GetLastInsertedId();
            Connection.DbConnection.CloseConnection();

            foreach (Ingredient mealIngredient in meal.Ingredients)
            {
                Ingredient.AddIngredientToDb(mealIngredient, meal.MealId);
            }
        }
        public static void EditMealInDatabase(Meal meal)
        {
            Connection.DbConnection.OpenConnection();
            Connection.DbConnection.Insert($"UPDATE `meal` SET `mealName`= '{meal.Name}' WHERE `id` = '{meal.MealId}';");
            Connection.DbConnection.CloseConnection();

            foreach (Ingredient mealIngredient in meal.Ingredients)
            {
                if (mealIngredient.Id >= 0)
                {
                    Ingredient.EditIngredientInDb(mealIngredient);
                }
                else
                {
                    Ingredient.AddIngredientToDb(mealIngredient, meal.MealId);
                }
            }
        }
        public static void DeleteMealInDatabase(Meal meal)
        {
            Connection.DbConnection.OpenConnection();
            Connection.DbConnection.Delete($"DELETE FROM `meal` WHERE  mealId = {meal.MealId}");
            Connection.DbConnection.CloseConnection();

            foreach (Ingredient mealIngredient in meal.Ingredients)
            {
                Ingredient.DeleteIngredientInDb(mealIngredient);
            }
        }


        public static Meal GetMealFromMealConnection(DBConnection connection)
        {
            return new 
                Meal(connection.GetString("mealName"),
                Ingredient.GetListOfIngredientsFromMealId(connection.GetInt32("id")),
                connection.GetInt32("id")
            );
        }
        public static Meal GetMealFromMealId(Int32 id)
        {
            DBConnection connection = new DBConnection();
            connection.OpenConnection();
            connection.Select($"SELECT * FROM `meal` WHERE `mealId` = '{id}';");
            connection.CloseConnection();

            return new Meal(
                connection.GetString("name"),
                Ingredient.GetListOfIngredientsFromMealId(connection.GetInt32("mealId")),
                connection.GetInt32("mealId")
                );
        }

        /// <summary>
        /// Get list of random meals. This list is not linked to the meal planner. You will have to do this manually
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static List<Meal> GetRandomMeals(Int32 amount)
        {
            if (amount < 1) return new List<Meal>();
            DBConnection connection = new DBConnection();
            List<Meal> meals = new List<Meal>();

            //get the meals
            connection.OpenConnection();
            connection.Select($"SELECT * FROM meal ORDER BY RAND() LIMIT {amount}");
            while (connection.Read())
            {
                meals.Add(Meal.GetMealFromMealConnection(connection));
            }
            connection.CloseConnection();

            return meals;
        }
    }
}