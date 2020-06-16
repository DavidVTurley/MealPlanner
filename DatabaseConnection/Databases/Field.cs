using System;

namespace DatabaseConnection.Databases
{
    public class Field
    {
        public String Name;
        public Int32 Size;

        public Field(String name, Int32 size)
        {
            Name = name;
            Size = size;
        }

        public override String ToString()
        {
            return Name;
        }
    }
}