using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DatabaseConnection.Model
{
    public class Meal : BaseModel
    {
        private Int32 _id = -1;
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
        public Meal(String name, IEnumerable<Ingredient> ingredients = null)
        {
            _name = name;
            Ingredients = ingredients != null ? new ObservableCollection<Ingredient>(ingredients) : new ObservableCollection<Ingredient>(new List<Ingredient>());
        }
    }
}