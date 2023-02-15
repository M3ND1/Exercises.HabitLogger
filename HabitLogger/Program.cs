using System.Data.SQLite;
using System.Globalization;
using System.Reflection;

namespace HabitLogger
{
    class Program
    {

        static string connection_string = "Data Source=HabitDatabase.db;Version=3;";

        static void Main(string[] args)
        {

            using (var conn = new SQLiteConnection(connection_string))
            {
                conn.Open();
                var tableCmd = conn.CreateCommand();
                tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS Gym ( " +
                    "Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                    "DATA TEXT, " +
                    "TIMES INTEGER NOT NULL)";
                tableCmd.ExecuteNonQuery();
                conn.Close();
            }
            GetUserInput();
            //SQLConnection conn = new SQLConnection(); //open database

        }
        static void GetUserInput()
        {
            int input;
            Console.Clear();
            Console.WriteLine("Welcome to Habit Logger, what do You want to do here?");
            Console.WriteLine("-------------------------\n");
            bool closeApp = false;
            while (closeApp == false)
            {
                Console.WriteLine("Choose Your Activity");
                Console.WriteLine("0 - Exit App");
                Console.WriteLine("1 - Insert");
                Console.WriteLine("2 - See records");
                Console.WriteLine("3 - Delete record"); //repair waitime on get
                Console.WriteLine("4 - Update existing record");

                input = Convert.ToInt32(Console.ReadLine());
                switch (input)
                {
                    case 0:
                        closeApp = true;
                        break;
                    case 1:
                        Insert();
                        break;
                    case 2:
                        Get();
                        break;
                    case 3:
                        Delete();
                        break;
                    case 4:
                        Update();
                        break;
                    case 5:
                        Create();
                        break;
                    case 6:
                        checkEmptyDb();
                        break;
                    default:
                        Console.WriteLine("Type valid command.");
                        break;
                }
            }
        }
        private static void Insert()
        {
            string date = GetDateInput();
            int times = GetNumberInput();
            using (SQLiteConnection conn = new SQLiteConnection(connection_string))
            {
                conn.Open();
                var tableCmd = conn.CreateCommand();
                tableCmd.CommandText = $"INSERT INTO Gym(data, times) VALUES ('{date}', {times})";
                tableCmd.ExecuteNonQuery();

                conn.Close();
            }
        }
        private static void Get()
        {
            Console.Clear();
            using (var conn = new SQLiteConnection(connection_string))
            {
                conn.Open();
                var tableCmd = conn.CreateCommand();
                tableCmd.CommandText = $"SELECT * FROM Gym";

                List<Gym> gymList = new();

                SQLiteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        gymList.Add(
                        new Gym
                        {
                            Id = reader.GetInt32(0),
                            Data = DateTime.Parse(reader.GetString(1)),
                            Times = reader.GetInt32(2)
                        });
                    }
                }
                else { Console.WriteLine("No rows in this table"); }


                conn.Close();

                foreach (var gym in gymList)
                {
                    Console.WriteLine($"Person {gym.Id} on day {gym.Data.ToString("dd-MMM-yyyy")} you were {gym.Times} time/s at gym.");
                    //Console.WriteLine($"On day {gym.Data} you were {gym.Times} time/s at gym.");
                }
                //System.Threading.Thread.Sleep(3000);
                Console.WriteLine("-----\n");
                //Console.Clear();
            }
        }
        private static void Delete()
        {
            Console.Clear();
            Console.WriteLine("Provide Id number to delete or type 0 to exit to menu.");
            System.Threading.Thread.Sleep(2000);
            Get();
            int number_delete = Convert.ToInt32(Console.ReadLine());
            if (number_delete == 0) GetUserInput();
            using (var conn = new SQLiteConnection(connection_string))
            {
                conn.Open();
                var cmdTable = conn.CreateCommand();
                cmdTable.CommandText = $"DELETE FROM Gym WHERE Id = '{number_delete}'";

                int rowCount = cmdTable.ExecuteNonQuery();
                if (rowCount == 0)
                {
                    Console.WriteLine("There are no rows to delete.");
                    Delete();
                }
            }
            Console.Write($"Record {number_delete} was deleted properly.");
            System.Threading.Thread.Sleep(2000);
            Console.Clear();
            GetUserInput();
        }
        private static void Update()
        {
            //readline which one (id)

            if (checkEmptyDb() == true)
            {
                Console.WriteLine("Returning back to main menu...");
                System.Threading.Thread.Sleep(2000);
                GetUserInput(); //if Db empty return to menu 
            }
            string id_name = GetId(); //check if there are rows in table
            showDataType(id_name);
            string column_name = GetColumnName();
            string data_type = GetDateInput();
            using (var conn = new SQLiteConnection(connection_string))
            {
                conn.Open();
                var tableCmd = conn.CreateCommand();
                tableCmd.CommandText = $"UPDATE Gym SET {column_name} = '{data_type}' WHERE Id = {id_name}";
                tableCmd.ExecuteNonQuery();

                conn.Close();
            }

        }
        private static void Create()
        {
            Console.Clear();
            Console.WriteLine("What will be the name of your habit that you wanna add?");
            string habit = Console.ReadLine();
            Console.Clear();

            System.Threading.Thread.Sleep(2000);
            Console.Clear();
            Console.WriteLine("Choose data type of your habit: ");
            Console.WriteLine("TEXT");
            Console.WriteLine("INT");
            Console.WriteLine("NUMERIC");
            Console.WriteLine("REAL");
            string data_type = Console.ReadLine().ToUpper();
            var propertyInfo = typeof(Gym).GetProperty(habit);
            if (propertyInfo == null)
            {
                var propertyType = Type.GetType($"System.{data_type}");
                propertyInfo = typeof(Gym).GetProperty(habit, propertyType);
                if (propertyInfo == null)
                {
                    propertyInfo = typeof(Gym).GetProperty(habit, propertyType ?? typeof(string));
                }
                Console.WriteLine("Habit added succesfully");
            }
            else
            {
                Console.WriteLine("Habit exists.");
                Console.WriteLine("Returning to Menu");
                GetUserInput();
            }
            using (var conn = new SQLiteConnection(connection_string))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = $"ALTER TABLE Gym ADD COLUMN {habit} {data_type}";
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        internal static string GetId()
        {
            Get();
            Console.WriteLine("Which Person Id do you want to Update \t To exit type: 0");
            string id_type = Console.ReadLine();
            if (id_type == "0") GetUserInput();

            return id_type;
        }
        internal static string GetDateInput()
        {
            Console.Write("Insert Date when you were at Gym.\n Remember! Example of Date: 01-January-2000 To exit type: 0\t");
            string date = Console.ReadLine(); //validation
            if (date == "0") GetUserInput();
            DateTime date_time;
            while(!DateTime.TryParseExact(date, "dd-MMMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date_time))
            {
                Console.WriteLine("Provide proper date. Example: 23-MARCH-2000");
                date = Console.ReadLine();
                Console.Clear();
            }
            Console.Clear();
            return date;
        }
        internal static string GetColumnName()
        {
            Console.WriteLine("Type which column you want to update: ");
            Type gym = typeof(Gym);
            PropertyInfo[] prop = gym.GetProperties(); //array of properies in Gym.cs
            foreach (PropertyInfo p in prop)
            {
                if (p.Name == "Id") continue; //dont change Id
                Console.WriteLine(p.Name);
            }
            Console.WriteLine("=============");
            string column = Console.ReadLine();
            if (column == "0") GetUserInput();
            Console.Clear();
            return column;

        }
        internal static int GetNumberInput()
        {
            int times=0;
            bool validInput = false;
            while(validInput == false)
            {
                Console.WriteLine("Insert how many times you were at gym that day. To exit type 0");
                string input = Console.ReadLine();
                validInput = int.TryParse(input, out times);

                if (!validInput) Console.WriteLine("Provide number."); //sleep
                if (input == "0") GetUserInput();
                Console.Clear();
            }
            return times;
        }
        internal static void showDataType(string data_type)
        {
            using (var conn = new SQLiteConnection(connection_string))
            {
                conn.Open();
                var tableCmd = conn.CreateCommand();
                tableCmd.CommandText = $"SELECT * FROM Gym where Id = '{data_type}'";
                tableCmd.ExecuteNonQuery();

                conn.Close();

            }
        } // change name of this method later
        private static bool checkEmptyDb()
        {
            bool sol;
            using(var conn = new SQLiteConnection(connection_string))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT COUNT(*) FROM Gym";
                int is_Empty = Convert.ToInt32(cmd.ExecuteScalar());
                if (is_Empty == 0)
                {
                    Console.WriteLine("The Table is empty.");
                    sol = true;
                }
                else
                {
                    Console.WriteLine("The Table is not empty");
                    sol = false;
                }
                System.Threading.Thread.Sleep(2000);
                conn.Close();
            }
            Console.Clear();
            return sol;
        }
    }

}