using System.Windows.Input;
using MealPlanner.Model;

namespace MealPlanner.ViewModel
{
    public class MealViewModel : BaseViewModel
    {
        private Meal _meal;
        public Meal Meal
        {
            get => _meal;
            set
            {
                if (Equals(value, _meal)) return;
                _meal = value;
                OnPropertyChanged();
            }
        }

        public MealViewModel()
        {
            Meal = new Meal();
        }

        public MealViewModel(Meal meal)
        {
            Meal = meal;
        }

        public void AddIngredient(Ingredient ingredient)
        {
            Meal.Ingredients.Add(ingredient);
        }
        public void RemoveIngredient(Ingredient ingredient)
        {
            Meal.Ingredients.Remove(ingredient);
        }

        public void RemoveLastIngredient()
        {
            Meal.Ingredients.RemoveAt(Meal.Ingredients.Count-1);
        }
    }
}