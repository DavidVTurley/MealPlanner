using System.Collections.Generic;
using System.Collections.ObjectModel;
using MealPlanner.Model;

namespace MealPlanner.ViewModel
{
    public class MealsViewModel : BaseViewModel
    {
        public ObservableCollection<Meal> Meals;

        public MealsViewModel(IEnumerable<Meal> meals)
        {
            Meals = new ObservableCollection<Meal>(meals);
        }
    }
}