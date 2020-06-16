using System;

namespace DatabaseConnection.Databases
{
    public class BaseDatabaseModel
    {
        public static String Db { get; set; }
        public static String TableName { get; set; }

        public static Field Id { get; set; }

        public static String[] FieldList 
        {
            get
            {
                return new String[]
                {
                    Db,
                    TableName,
                    Id.Name
                };
            }
        }
    }
}