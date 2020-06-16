using System;
using System.Collections.Generic;
using DatabaseConnection;
using DatabaseConnection.Model;

namespace MealPlanner.Model
{
    public class Ingredient : BaseModel
    {
        
        private Int32 _id = -1;
        private String _name;
        private Boolean _vegetarian;
        private List<String> _servingUnit;
        private Decimal _servingSize;
        private Int32 _selectedServingUnit;
        private Boolean _replaceWithVegetarian;


        public List<String> ServingUnit
        {
            get => _servingUnit;
            set
            {
                if (value == _servingUnit) return;
                _servingUnit = value;
                OnPropertyChanged();
            }
        }
        public Boolean Vegetarian
        {
            get => _vegetarian; 
            set
            {
                if (value == _vegetarian) return;
                _vegetarian = value;
                OnPropertyChanged();
            }
        }
        public Int32 Id
        {
            get => _id;
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged();
            }
        }
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
        public Decimal ServingSize
        {
            get => _servingSize;
            set
            {
                if (value == _servingSize) return;
                _servingSize = value;
                OnPropertyChanged();
            }
        }
        public Int32 SelectedServingUnit
        {
            get => _selectedServingUnit;
            set
            {
                if (value == _selectedServingUnit) return;
                _selectedServingUnit = value;
                OnPropertyChanged();
            }
        }
        public Boolean ReplaceIfVegetarian
        {
            get => _replaceWithVegetarian;
            set
            {
                if (value == _replaceWithVegetarian) return;
                _replaceWithVegetarian = value;
                OnPropertyChanged();
            }
        }



        public Ingredient()
        {
            ServingUnit = new List<String>(Enum.GetNames(typeof(IngredientServingSizeEnum)));
        }
        public Ingredient(String name, Int32 servingSize, Int32 selectedServingUnit = 0, Boolean vegetarian = false, Boolean replaceWithVegetarian = false, Int32 id = -1) : this()
        {
            _name = name;
            _servingSize = servingSize;
            _selectedServingUnit = selectedServingUnit;
            _vegetarian = vegetarian;
            _replaceWithVegetarian = replaceWithVegetarian;
            _id = id;
        }

        public static void AddIngredientToDb(Ingredient ingredient, Int32 mealId)
        {
            Int32 vegetarian = ingredient.Vegetarian ? 1 : 0;
            Int32 replaceIfVegitarian = ingredient.ReplaceIfVegetarian ? 1 : 0;

            Connection.DbConnection.OpenConnection();
            Connection.DbConnection.Insert($"INSERT INTO `ingredients`( `ingredientName`, `vegetarian`, `replaceIfVegetarian`, `servingUnit`, `servingSize`) VALUES ('{ingredient.Name}','{vegetarian}','{replaceIfVegitarian}','{ingredient.SelectedServingUnit + 1}','{ingredient.ServingSize}'); ");
            Connection.DbConnection.CloseConnection();

            Connection.DbConnection.OpenConnection();
            ingredient.Id = Connection.DbConnection.GetLastInsertedId();
            Connection.DbConnection.CloseConnection();
            LinkMealAndIngredients(mealId, ingredient);
        }
        public static void EditIngredientInDb(Ingredient ingredient)
        {
            Int32 vegetarian = ingredient.Vegetarian? 1 : 0;
            Int32 replaceIfVegitarian = ingredient.ReplaceIfVegetarian ? 1 : 0;

            Connection.DbConnection.OpenConnection();
            Connection.DbConnection.Update($"UPDATE `ingredients` SET `ingredientName`= '{ingredient.Name}',`vegetarian`='{vegetarian}',`replaceIfVegetarian`='{replaceIfVegitarian}',`servingUnit`='{ingredient.SelectedServingUnit +1}',`servingSize`='{ingredient.ServingSize}' WHERE `id` = '{ingredient.Id}';");
            Connection.DbConnection.CloseConnection();

        }
        public static void DeleteIngredientInDb(Ingredient ingredient)
        {
            Connection.DbConnection.OpenConnection();
            Connection.DbConnection.Delete($"DELETE FROM `ingredients` WHERE `id` = '{ingredient.Id}';");
            Connection.DbConnection.CloseConnection();
        }

        public static void LinkMealAndIngredients(Int32 mealId, Ingredient ingredient)
        {
            Connection.DbConnection.OpenConnection();
            Connection.DbConnection.Insert($"INSERT INTO `meal_ingredients`(`meal_id`, `ingredient_id`) VALUES ('{mealId}','{ingredient.Id}');");
            Connection.DbConnection.CloseConnection();
        }
        public static void LinkMealAndIngredients(Meal meal, Ingredient ingredient)
        {
            LinkMealAndIngredients(meal.MealId, ingredient);
        }

        public static List<Ingredient> GetListOfIngredientsFromMealId(Int32 mealId)
        {
            DBConnection connection = new DBConnection();

            connection.OpenConnection();
            connection.Select($"SELECT * FROM ingredients WHERE id IN (SELECT ingredient_id FROM meal_ingredients WHERE meal_id = {mealId}) ;");
            List<Ingredient> ingredients = new List<Ingredient>();
            while (connection.Read())
            {

                Enum.TryParse(connection.GetString("servingUnit"), out IngredientServingSizeEnum result);

                ingredients.Add(new Ingredient(
                    connection.GetString("ingredientName"),
                    connection.GetInt32("servingSize"),
                    (Int32)result,
                    connection.GetBoolean("vegetarian"),
                    connection.GetBoolean("replaceIfVegetarian"),
                    connection.GetInt32("id")
                    ));
            }
            connection.CloseConnection();

            return ingredients;
        }

    }
}