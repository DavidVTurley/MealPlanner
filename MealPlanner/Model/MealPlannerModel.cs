using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using DatabaseConnection;

namespace MealPlanner.Model
{
    public class MealPlannerModel : BaseModel
    {
        private Int32 _id;
        private ObservableCollection<MealPlannerMeal> _meals;
        private Int32 _numberOfPeopleInput = 5;
        private Int32 _numberOfVegetarianInput = 1;

        private Int32 _generateNewListAmountToGenerate = 7;
        public Int32 GenerateNewListAmountToGenerate
        {
            get => _generateNewListAmountToGenerate;
            set
            {
                _generateNewListAmountToGenerate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Meals));
                OnPropertyChanged(nameof(Id));

                MealPlannerModel newMealPlanner = GetNewMealPlannerModel(value, NumberOfPeopleInput, NumberOfVegetarianInput);

                Id = newMealPlanner.Id;
                Meals.Clear();
                foreach (MealPlannerMeal mealPlannerMeal in newMealPlanner.Meals)
                {
                    Meals.Add(mealPlannerMeal);
                }
            }
        }

        public Int32 Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<MealPlannerMeal> Meals
        {
            get { return _meals; }
            set
            {
                _meals = value;
                OnPropertyChanged();
            }
        }
        public Int32 NumberOfPeopleInput
        {
            get => _numberOfPeopleInput;
            set
            {
                _numberOfPeopleInput = value;
                UpdateNumPeople(value, "numberOfPeople");
                OnPropertyChanged();
            }
        }
        public Int32 NumberOfVegetarianInput
        {
            get { return _numberOfVegetarianInput; }
            set
            {
                _numberOfVegetarianInput = value;
                UpdateNumPeople(value, "numberOfVegetarians");
                OnPropertyChanged();
            }
        }

        public MealPlannerModel()
        {
            Meals = new ObservableCollection<MealPlannerMeal>();
        }
        public MealPlannerModel(Int32 id, List<MealPlannerMeal> meals)
        {
            _id = id;
            Meals = new ObservableCollection<MealPlannerMeal>(meals);
        }

        public static MealPlannerModel GetMealPlanner()
        {
            DBConnection connection = new DBConnection();

            MealPlannerModel mealPlanner = new MealPlannerModel();

            connection.OpenConnection();
            connection.Select("SELECT * FROM `mealplanner` ORDER BY id DESC LIMIT 1");

            if (connection.HasRos())
            {
                //Load the last meal planner
                connection.Read();
                mealPlanner.Id = connection.GetInt32("id");
                connection.CloseConnection();
            }
            else
            {
                connection.CloseConnection();

                mealPlanner.Meals = new ObservableCollection<MealPlannerMeal>();
            }

            mealPlanner.Meals = new ObservableCollection<MealPlannerMeal>(GetMealPlannerMeals(mealPlanner.Id));

            return mealPlanner;
        }
        public static List<MealPlannerMeal> GetMealPlannerMeals(Int32 mealPlannerId)
        {
            DBConnection connection = new DBConnection();

            List<MealPlannerMeal> meals = new List<MealPlannerMeal>();

            //Load the meals associated to the meal planner
            connection.OpenConnection();
            connection.Select($"SELECT mealplanner_meals.meal_Id, mealplanner_meals.numberOfPeople, mealplanner_meals.numberOfVegetarians, meal.mealName FROM mealplanner_meals INNER JOIN meal ON mealplanner_meals.meal_Id = meal.id WHERE mealplanner_meals.mealPlanner_ID = {mealPlannerId} ;");
            if (connection.HasRos())
            {
                while (connection.Read())
                {
                    meals.Add(new MealPlannerMeal(mealPlannerId, connection.GetInt32("meal_id"), connection.GetString("mealName"), connection.GetInt32("numberOfPeople"), connection.GetInt32("numberOfVegetarians")));
                }
                connection.CloseConnection();

                //Add ingredients to the meal
                foreach (MealPlannerMeal meal in meals)
                {
                    //Get all ingredeints in meal
                    meal.Ingredients = new ObservableCollection<Ingredient>(Ingredient.GetListOfIngredientsFromMealId(meal.MealId));

                    //calculate the number of ingredients needed
                    CalculateMealIngredients(meal);
                }
            }
            else
            {
                connection.CloseConnection();

                meals = new List<MealPlannerMeal>();
            }

            return meals;
        }

        /// <summary>
        /// Gets a new MealPlannerModel and connects a number of new meals with it
        /// </summary>
        /// <param name="numberOfNewMeals"></param>
        /// <returns></returns>
        public static MealPlannerModel GetNewMealPlannerModel(Int32 numberOfNewMeals, Int32 numOfPpl, Int32 numOfVege)
        {
            // Insert new MealPlanner into database
            DBConnection connection = new DBConnection();
            connection.OpenConnection();
            connection.Insert("INSERT INTO `mealplanner`(`id`) VALUES ('');");
            Int32 newMealPlannerId = connection.GetLastInsertedId();
            connection.CloseConnection();

            List<MealPlannerMeal> meals = MealPlannerMeal.ConvertMealsIntoMealPlannerMeals(Meal.GetRandomMeals(numberOfNewMeals), newMealPlannerId, numOfPpl, numOfVege);

            foreach (MealPlannerMeal mealPlannerMeal in meals)
            {
                MealPlannerMeal.AddMealPlannerMealToDatabase(mealPlannerMeal, mealPlannerMeal.MealPlannerId);
            }



            return new MealPlannerModel(newMealPlannerId, meals);
        }

        public void UpdateNumPeople(Int32 newValue, String dbField)
        {
            DBConnection connection = new DBConnection();
            connection.OpenConnection();
            connection.Update($"UPDATE `mealplanner_meals` SET `{dbField}` = '{newValue}' WHERE `mealPlanner_ID` = '{Id}'");
            connection.CloseConnection();

            foreach (MealPlannerMeal mealPlannerMeal in Meals)
            {
                switch (dbField)
                {
                    case "numberOfPeople":
                        mealPlannerMeal.NumberOfPeople = newValue;
                        break;
                    case "numberOfVegetarians":
                        mealPlannerMeal.NumberOfVegetarians = newValue;
                        break;
                }

                CalculateMealIngredients(mealPlannerMeal);
            }

        }
        public static void CalculateMealIngredients(MealPlannerMeal meal)
        {
            Int32 meatEaters = meal.NumberOfPeople - meal.NumberOfVegetarians;
            Int32 vegetarians = meal.NumberOfVegetarians;

            List<Ingredient> baseIngredients = Ingredient.GetListOfIngredientsFromMealId(meal.MealId);

            foreach (Ingredient ingredient in meal.Ingredients)
            {
                Ingredient baseIngredient = baseIngredients.Where(x => x.Id == ingredient.Id).ToList()[0];

                if (ingredient.Vegetarian)
                {
                    ingredient.ServingSize = baseIngredient.ServingSize * vegetarians;
                }
                else if (ingredient.ReplaceIfVegetarian)
                {
                    ingredient.ServingSize = baseIngredient.ServingSize * meatEaters;
                }
                else
                {
                    ingredient.ServingSize = baseIngredient.ServingSize * (meatEaters + vegetarians);
                }
            }
        }
    }

    public class CreateNewMealsEventArgs : EventArgs
    {
        public Int32 NewAmountOfMeals;

        public CreateNewMealsEventArgs(Int32 newAmountOfMeals)
        {
            NewAmountOfMeals = newAmountOfMeals;
        }
    }
}