using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MealPlanner.Model;

namespace MealPlanner.View
{
    /// <summary>
    /// Interaction logic for MealIngredientsList.xaml
    /// </summary>
    public partial class MealIngredientsList : UserControl
    {
        public MealPlannerModel ViewModel;
        public List<Ingredient> Ingredients = new List<Ingredient>();

        public MealIngredientsList()
        {
            InitializeComponent();

            ViewModel = MealPlannerModel.GetMealPlanner();
            List<Ingredient> ìngredientsList = new List<Ingredient>();


            //Calculate the serving sizes in each of the meals
            foreach (MealPlannerMeal mealPlannerMeal in ViewModel.Meals)
            {
                MealPlannerModel.CalculateMealIngredients(mealPlannerMeal);
            }


            // Gets a list of meals
            foreach (MealPlannerMeal mealInViewModel in ViewModel.Meals)
            {
                //Gets a list of ingredients
                foreach (Ingredient ingredientInViewModel in mealInViewModel.Ingredients)
                {
                    Boolean foundInIngredients = false;
                    // Check to see if the ingredient is already in the list of ingredients
                    foreach (Ingredient ingredient in ìngredientsList)
                    {
                        //If the meal is already on the list
                        var s = ingredientInViewModel.Name.ToLower(CultureInfo.InvariantCulture) == 
                                ingredient.Name.ToLower(CultureInfo.InvariantCulture);
                        var q = ingredientInViewModel.Name.ToLower(CultureInfo.InvariantCulture);
                        var w = ingredient.Name.ToLower(CultureInfo.InvariantCulture);


                        if (ingredientInViewModel.Name.ToLower(CultureInfo.InvariantCulture) == ingredient.Name.ToLower(CultureInfo.InvariantCulture))
                        {
                            foundInIngredients = true;
                            // Ingredient is the same unit
                            if (ingredient.SelectedServingUnit == ingredientInViewModel.SelectedServingUnit)
                            {
                                ingredient.ServingSize += ingredientInViewModel.ServingSize;
                                break;
                            }
                            // ingredient is of a different serving unit
                            else
                            {
                                ìngredientsList.Add(ingredientInViewModel);
                                break;
                            }
                        }
                    }
                    //If the ingredietn was not found in the list add it

                    if (!foundInIngredients)
                    {
                        ìngredientsList.Add(ingredientInViewModel);
                    }
                }
            }

            Ingredients = ìngredientsList;

            DataContext = this;
            ListView.ItemsSource = Ingredients;
        }

        private void PrintOutIngredientsClick(Object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(ListView, "My First Print Job");
            }
        }
    }
}
