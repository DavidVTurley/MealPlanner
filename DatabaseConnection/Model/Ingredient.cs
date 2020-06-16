using System;
using System.Collections.Generic;

namespace DatabaseConnection.Model
{
    public class Ingredient : BaseModel
    {
        
        private Int32 _id = -1;
        private String _name;
        private Boolean _vegetarian;
        private List<String> _servingUnit;
        private Int32 _servingSize;
        private Int32 _selectedServingUnit;

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
        public Int32 ServingSize
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

        public Ingredient()
        { 
            ServingUnit = new List<String>
            {
                "g" ,
                "KG",
                "Liter"
            };
            SelectedServingUnit = 0;
        }

        public Ingredient(String name, Int32 servingSize, List<String> servingUnit, Int32 selectedServingUnit = 0, Boolean vegetarian = false) :base()
        {
            _name = name;
            _servingSize = servingSize;
            _servingUnit = servingUnit;
            _selectedServingUnit = selectedServingUnit;
            _vegetarian = vegetarian;
        }





    }
}