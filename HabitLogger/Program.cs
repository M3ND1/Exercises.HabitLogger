
using System.Configuration;
using System.Data.Entity.Core.Mapping;
using System.Data.SQLite;
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
            bool closeApp = false;
            while(closeApp == false)
            {
                //Console.Clear();
                Console.WriteLine("What do you wanna do here");
                Console.WriteLine("-------------------------\n");
                Console.WriteLine("0 - Exit App");
                Console.WriteLine("1 - Insert");
                Console.WriteLine("2 - See records");
                Console.WriteLine("3 - Delete record"); //repair waitime on get
                Console.WriteLine("4 - Update existing record");

                input = Convert.ToInt32(Console.ReadLine());
                switch(input)
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
                
                if(reader.HasRows)
                {
                    while(reader.Read())
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

                foreach(var gym in gymList)
                {
                    Console.WriteLine($"Person {gym.Id} on day {gym.Data.ToString("dd-MMM-yyyy")} you were {gym.Times} time/s at gym.");
                    //Console.WriteLine($"On day {gym.Data} you were {gym.Times} time/s at gym.");
                }
                System.Threading.Thread.Sleep(3000);
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
            using(var conn = new SQLiteConnection(connection_string))
            {
                conn.Open();
                var cmdTable = conn.CreateCommand();
                cmdTable.CommandText = $"DELETE FROM Gym WHERE Id = '{number_delete}'";
            
                int rowCount = cmdTable.ExecuteNonQuery();
                if(rowCount == 0)
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
            //Console.Clear();
            //readline which one (id)
            string id_name = GetId();
            string column_name = GetColumnName();
            string data_type = GetDataType();

            using (var conn = new SQLiteConnection(connection_string))
            {
                conn.Open();
                var tableCmd = conn.CreateCommand();
                tableCmd.CommandText = $"UPDATE Gym SET '{column_name}' '{data_type}' WHERE Id = {id_name}";
                tableCmd.ExecuteNonQuery();


                conn.Close();
            }

        }
        internal static string GetId()
        {
            Get();
            Console.WriteLine("What Id Do you want to Update");
            string id_type = Console.ReadLine();
            if (id_type == "0") GetUserInput();

            return id_type;
        }
        internal static string GetDateInput()
        {
            Console.WriteLine("Insert Date and when you were at gym. Date format (dd-mm-yy)");
            Console.WriteLine("To exit console type 0.");
            string date = Console.ReadLine();
            if (date == "0") GetUserInput();
            Console.Clear();
            return date;
        }
        internal static string GetColumnName()
        {
            Console.WriteLine("Type Select which column you want to update: ");
            string column = Console.ReadLine();
            if (column == "0") GetUserInput();

            return column;

        }
        internal static string GetDataType()
        {
            Console.WriteLine("Type new data in column you want to edit.");
            string data_type = Console.ReadLine();
            if (data_type == "0") GetUserInput();

            return data_type;
        }
        internal static int GetNumberInput()
        {
            Console.WriteLine("Insert Times, how many times you weere at gym that day. Numbers only");
            Console.WriteLine("To exit console type 0.");
            int times = Convert.ToInt32(Console.ReadLine());
            if(times == 0) GetUserInput();
            return times;
        }
        
    }

}