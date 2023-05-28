
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Xml.Linq;
using System.Threading.Tasks;

class Program
{
    class SQLiteHandler
    {
        string? _database_name;
        string _connection_string;
        SQLiteConnection _connection; //kräver using System.Data.SQLite;
        SQLiteCommand _command;

        private void Open()
        {
            if (_connection != null)
            {
                _connection.Open();
            }
        }

        private void Close()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
        }

        public SQLiteHandler(string databaseName)
        {
            _database_name = databaseName;
            _connection_string = "URI=file:" + databaseName;
            _connection = new SQLiteConnection(_connection_string);

            CheckSQLiteVersion();
            GetCurrentTime();
        }

        private void CheckSQLiteVersion()
        {
            Open();
            string stm = "SELECT SQLITE_VERSION();";
            _command = new SQLiteCommand(stm, _connection);
            string? version = _command.ExecuteScalar().ToString();

            Console.WriteLine($"SQLite version: {version}");
            Close();
        }

        private void GetCurrentTime()
        {
            Open();
            string stm = "SELECT(datetime('now', 'localtime'));";
            _command = new SQLiteCommand(stm, _connection);
            string? date = _command.ExecuteScalar().ToString();
            Console.WriteLine($"SQLite dateNow: {date} \n");
            Close();
        }

        public void CreateNewGuestsTable()
        {
            RemoveGuestsTable();

            Open();
            _command.CommandText = "CREATE TABLE Guests(id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT, lastname TEXT, age INT);";
            _command.ExecuteNonQuery();

            Console.WriteLine("Table 'Guests' created\n");
            Close();
        }

        public void AddSomeGuests()
        {
            Open();
            _command.CommandText = "INSERT INTO Guests(name, lastname, age) VALUES('Lisa','Andersson',56);";
            _command.ExecuteNonQuery();

            _command.CommandText = "INSERT INTO Guests(name, lastname, age) VALUES('Jonas','Ströberg',32);";
            _command.ExecuteNonQuery();

            Console.WriteLine("Table 'Guests' filled with data\n");
            Close();
        }

        public void AddGuestToGuestsTable(string name, string lastname, int age)
        {
            Open();
            _command.CommandText = "INSERT INTO Guests(name, lastname, age) VALUES (@name, @lastname, @age);";

            SQLiteParameter nameParam = new SQLiteParameter("@name", System.Data.DbType.String);
            SQLiteParameter lastnameParam = new SQLiteParameter("@lastname", System.Data.DbType.String);
            SQLiteParameter ageParam = new SQLiteParameter("@age", System.Data.DbType.Int32);

            nameParam.Value = name;
            lastnameParam.Value = lastname;
            ageParam.Value = age;

            _command.Parameters.Add(nameParam);
            _command.Parameters.Add(lastnameParam);
            _command.Parameters.Add(ageParam);

            _command.Prepare();
            _command.ExecuteNonQuery();

            Console.WriteLine($"{name} was added to 'Guests'\n");
            Close();
        }

        public void DumpGuestsTableToConsole()
        {
            Open();
            _command.CommandText = "SELECT * FROM Guests;";
            using SQLiteDataReader rdr = _command.ExecuteReader();

            Console.WriteLine("---Guests---");
            Console.WriteLine($"{rdr.GetName(0)} {rdr.GetName(1)} {rdr.GetName(2)} {rdr.GetName(3)}");
            Console.WriteLine("--------------");
            while (rdr.Read())
            {
                Console.WriteLine($"{rdr.GetInt32(0)} {rdr.GetString(1)} {rdr.GetString(2)} {rdr.GetInt32(3)}");
            }
            Console.WriteLine("--------------\n");
            Close();
        }

        public void UpdateGuestDatByID(int id, string name, string lastname, int age)
        {
            Open();
            _command.CommandText = "UPDATE Guests SET name = @name, lastname = @lastname, age = @age WHERE id = @id;";

            SQLiteParameter idParam = new SQLiteParameter("@id", System.Data.DbType.Int32);
            SQLiteParameter nameParam = new SQLiteParameter("@name", System.Data.DbType.String);
            SQLiteParameter lastnameParam = new SQLiteParameter("@lastname", System.Data.DbType.String);
            SQLiteParameter ageParam = new SQLiteParameter("@age", System.Data.DbType.Int32);

            idParam.Value = id;
            nameParam.Value = name;
            lastnameParam.Value = lastname; 
            ageParam.Value = age;

            _command.Parameters.Add(idParam);
            _command.Parameters.Add(nameParam);
            _command.Parameters.Add(lastnameParam);
            _command.Parameters.Add(ageParam);

            _command.Prepare();
            _command.ExecuteNonQuery();

            Console.WriteLine($"Guest with id {id} was updated\n");
            Close();
        }

        public void DeleteDataByID(int id)
        {
            Open();
            _command.CommandText = "DELETE FROM Guests WHERE id = @id;";

            SQLiteParameter idParam = new SQLiteParameter("@id", System.Data.DbType.Int32);

            idParam.Value = id;

            _command.Parameters.Add(idParam);

            _command.Prepare();
            _command.ExecuteNonQuery();

            Console.WriteLine($"Guest with id {id} was removed\n");

            Close();
        }

        public void RemoveGuestsTable()
        {
            Open();
            _command.CommandText = "DROP TABLE IF EXISTS Guests";
            _command.ExecuteNonQuery();
            Console.WriteLine("Table 'Guests' was removed\n");
            Close();
        }

        public void DeleteAllGuests()
        {
            Open();
            _command.CommandText = "DELETE FROM Guests;";

            _command.Prepare();
            _command.ExecuteNonQuery();

            Close();
        }

        public void AddRandomNamesToGuestTable()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var stringCharsName = new char[8];
            var randomChar = new Random();

            for (int i = 0; i < stringCharsName.Length; i++)
            {
                stringCharsName[i] = chars[randomChar.Next(chars.Length)];
            }
            string stringName = new string(stringCharsName);


            var stringCharsLastname = new char[8];

            for (int i = 0; i < stringCharsLastname.Length; i++)
            {
                stringCharsLastname[i] = chars[randomChar.Next(chars.Length)];
            }
            string stringLastname = new string(stringCharsLastname);


            Random rnd = new Random();
            int age = rnd.Next(0, 100);



            Open();
            _command.CommandText = "INSERT INTO Guests(name, lastname, age) VALUES (@name, @lastname, @age);";

            SQLiteParameter nameParam = new SQLiteParameter("@name", System.Data.DbType.String);
            SQLiteParameter lastnameParam = new SQLiteParameter("@lastname", System.Data.DbType.String);
            SQLiteParameter ageParam = new SQLiteParameter("@age", System.Data.DbType.Int32);

            nameParam.Value = stringName;
            lastnameParam.Value = stringLastname;
            ageParam.Value = age;

            _command.Parameters.Add(nameParam);
            _command.Parameters.Add(lastnameParam);
            _command.Parameters.Add(ageParam);

            _command.Prepare();
            _command.ExecuteNonQuery();

            Close();

        }

        public List<string> DumpGuestsNameInOrder()
        {
            List<string> unsortedlist = new List<string>();

            
            Open();
            _command.CommandText = "SELECT * FROM Guests;";
            using SQLiteDataReader rdr = _command.ExecuteReader();

            while (rdr.Read())
            {
                unsortedlist.Add(rdr.GetString(2));
            }
            Close();

            List<string> list1 = new List<string>();
            List<string> list2 = new List<string>();


            list1 = unsortedlist.GetRange(0, 100); //Delar upp gärstlistan i två listor
            list2 = unsortedlist.GetRange(100, unsortedlist.Count - 100);

            Task<List<string>>[] tasks = new Task<List<string>>[2]; //Startar upp 2 trådar för att sortera list1 och list2
            tasks[0] = Task.Run(() => SortList(list1));
            tasks[1] = Task.Run(() => SortList(list2));

            Task.WaitAll(tasks);

            list1 = tasks[0].Result; //List1 är nu sorterad
            list2 = tasks[1].Result; //List2 är också nu sorterad

            List<string> combinedList = new List<string>();
            int temp1 = 0;
            int temp2 = 0;

            for (int i = 0; i < (list1.Count + list2.Count); i++)
            {
                if (temp1 >= list1.Count)
                {
                    combinedList.Add(list2[temp2]);
                    temp2++;
                }
                else if (temp2 >= list2.Count)
                {
                    combinedList.Add(list1[temp1]);
                    temp1++;
                }
                else if (list1[temp1].CompareTo(list2[temp2]) < 0)
                {
                    combinedList.Add(list1[temp1]);
                    temp1++;
                }
                else
                {
                    combinedList.Add(list2[temp2]);
                    temp2++;
                }
            }

            for (int i = 0; i <combinedList.Count; i++)
            {
                Console.Write((i+1) + ": ");
                Console.WriteLine(combinedList[i]);
            }

            return combinedList;
        }

        public List<string> SortList(List<string> Guestlist)
        {
            return Guestlist.OrderBy(x => x).ToList();
        }

    }

    static void Main(string[] args)
    {
        SQLiteHandler db = new SQLiteHandler("URI-studentfest.db");

        db.CreateNewGuestsTable();
        db.AddSomeGuests();


        for (int i = 0; i < 200; i++)
        {
            db.AddRandomNamesToGuestTable();
        }


        //Menysystem
        bool continue_program = true;
        while (continue_program)
        {
            Console.WriteLine("1. Hantera gäster");
            Console.WriteLine("2. Avsluta programmet");

            int number1 = int.Parse(Console.ReadLine());

            if (number1 == 1)
            {
                bool continue_students = true;
                while (continue_students)
                {
                    Console.WriteLine("Gästregister:");

                    Console.WriteLine("1. Visa gästregister");
                    Console.WriteLine("2. Visa gäster i bokstavsordning");
                    Console.WriteLine("3. Lägg till gäst");
                    Console.WriteLine("4. Ta bort gäst, med ID");
                    Console.WriteLine("5. Ändra gäst, med ID");
                    Console.WriteLine("6. Töm gästregistret");
                    Console.WriteLine("7. Gå tillbaka");

                    int number = int.Parse(Console.ReadLine());

                    if (number == 1)
                    {
                        db.DumpGuestsTableToConsole();
                    }

                    else if (number == 2)
                    {
                        db.DumpGuestsNameInOrder();
                    }

                    else if (number == 3)
                    {
                        Console.WriteLine("Skriv in gästens förnamn: ");
                        string name = Console.ReadLine();
                        Console.WriteLine("Skriv in gästans efternamn: ");
                        string lastname = Console.ReadLine();
                        Console.WriteLine("Skriv in gästens ålder: ");
                        int age = int.Parse(Console.ReadLine());

                        db.AddGuestToGuestsTable(name, lastname, age);
                    }

                    else if (number == 4)
                    {
                        Console.WriteLine("Skriv in gästans id som du vill ta bort: ");
                        int id = int.Parse(Console.ReadLine());

                        db.DeleteDataByID(id);
                    }

                    else if (number == 5)
                    {
                        Console.WriteLine("Skriv in gästens id som du vill ändra: ");
                        int id = int.Parse(Console.ReadLine());
                        Console.WriteLine("Skriv in det nya namnet på gästen: ");
                        string name = Console.ReadLine();
                        Console.WriteLine("Skriv in det nya efternamnet på gästen: ");
                        string lastname = Console.ReadLine();
                        Console.WriteLine("Skriv in den nya åldern på gästen: ");
                        int age = int.Parse(Console.ReadLine());

                        db.UpdateGuestDatByID(id, name, lastname, age);
                    }

                    else if (number == 6)
                    {
                        db.DeleteAllGuests();
                    }

                    else
                    {
                        continue_students = false;
                    }
                }
            }
            else
            {
                continue_program = false;
            }
        }

    }
}
