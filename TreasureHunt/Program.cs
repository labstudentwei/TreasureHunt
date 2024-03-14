using System;
using MySql.Data.MySqlClient;

namespace TreasureHunt
{
    public class User
    {

        private int userId;
        private string username;
        private int roleId;


        public User(int userId, string username, int roleId)
        {
            this.userId = userId;
            this.username = username;
            this.roleId = roleId;
        }


        public int UserId
        {
            get { return userId; }

        }

        public string Username
        {
            get { return username; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Username cannot be null or whitespace.");//error: no throw exception
                }
                username = value;
            }
        }

        public int RoleId
        {
            get { return roleId; }
            set { roleId = value; }
        }

        public override string ToString()
        {
            return Username;
        }
    }


    public class Requirement
    {
        private int requirementId;
        private int projectId = 1;
        private string title;
        private string description;
        private string status;

        public int RequirementId
        {
            get { return requirementId; }
        }

        public int ProjectId
        {
            get { return projectId; }
        }

        public string Title
        {
            get { return title; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Title cannot be null or whitespace.");//error: no throw exception
                }
                title = value;
            }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        public Requirement(int requirementId, string title, string description, string status = "InActive")
        {
            this.requirementId = requirementId;
            this.title = title;
            this.description = description;
            this.status = status;
        }

        public override string ToString()
        {
            return Title;
        }
    }






    public class DatabaseManager
    {
        private string connectionString = "server=localhost;port=3306;database=aaopweiway;user=root;password=";

        public void AddUser(User user)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    var query = "INSERT INTO users (username, roleid) VALUES (@username, @roleid)";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", user.Username);
                        cmd.Parameters.AddWithValue("@roleid", user.RoleId);
                        cmd.ExecuteNonQuery();
                    }

                    Console.WriteLine("User successfully added.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }
        public List<User> ListUsers()
        {
            List<User> users = new List<User>();
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    var query = "SELECT UserId, Username, RoleId FROM users";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new User(
                                    reader.GetInt32("UserId"),
                                    reader.GetString("Username"),
                                    reader.GetInt32("RoleId")));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            return users;
        }
        public void AddRequirement(Requirement requirement)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    var query = "INSERT INTO requirements (projectid, title, description, status) VALUES (@projectid, @title, @description, @status)";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@projectid", requirement.ProjectId);
                        cmd.Parameters.AddWithValue("@title", requirement.Title);
                        cmd.Parameters.AddWithValue("@description", requirement.Description);
                        cmd.Parameters.AddWithValue("@status", requirement.Status);

                        cmd.ExecuteNonQuery();
                    }

                    Console.WriteLine("Requirement successfully added.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        public List<Requirement> ListRequirements()
        {
            List<Requirement> requirements = new List<Requirement>();
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    var query = "SELECT requirementid, projectid, title, description, status FROM requirements";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                requirements.Add(new Requirement(
                                    reader.GetInt32("requirementid"),
                                    reader.GetString("title"),
                                    reader.GetString("description"),
                                    reader.GetString("status")));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            return requirements;
        }



    }

    public class ApplicationService
    {
        private DatabaseManager dbManager;

        public ApplicationService(DatabaseManager manager)
        {
            dbManager = manager;
        }

        public bool AddUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                Console.WriteLine("Username cannot be null or whitespace.");
                return false;
            }

            var existingUsers = dbManager.ListUsers();
            var userExists = existingUsers.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase));

            if (userExists)
            {
                Console.WriteLine("Username already exists.");
                return false;
            }
            else
            {
                dbManager.AddUser(user);
                return true;
            }
        }



        public List<User> ListUsers()
        {
            return dbManager.ListUsers();
        }

        public bool AddRequirement(Requirement requirement)
        {
            if (string.IsNullOrWhiteSpace(requirement.Title))
            {
                Console.WriteLine("Title cannot be null or whitespace.");
                return false;
            }

            var existingRequirements = dbManager.ListRequirements();
            var requirementExists = existingRequirements.Any(r => r.Title.Equals(requirement.Title, StringComparison.OrdinalIgnoreCase) && r.ProjectId == requirement.ProjectId);

            if (requirementExists)
            {
                Console.WriteLine("Requirement with the same title already exists.");
                return false;
            }
            else
            {
                dbManager.AddRequirement(requirement);
                return true;
            }
        }



        public List<Requirement> ListRequirements()
        {
            return dbManager.ListRequirements();
        }
    }

    public class UI
    {
        private ApplicationService appService;

        public UI()
        {
            DatabaseManager dbManager = new DatabaseManager();
            appService = new ApplicationService(dbManager);
        }

        public void Run()
        {
            bool exitApp = false;

            while (!exitApp)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Add User");
                Console.WriteLine("2. List Users");
                Console.WriteLine("3. Add Requirement");
                Console.WriteLine("4. List Requirements");
                Console.WriteLine("0. Exit");
                Console.Write("Enter your choice (0-4): ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddUser();
                        break;
                    case "2":
                        ListUsers();
                        break;
                    case "3":
                        AddRequirement();
                        break;
                    case "4":
                        ListRequirements();
                        break;
                    case "0":
                        exitApp = true;
                        Console.WriteLine("Exiting application...");
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please enter 0-4.");
                        break;
                }
            }
        }

        private void AddUser()
        {
            Console.WriteLine("Enter username:");
            string username = Console.ReadLine();

            Console.WriteLine("Enter role ID (1 for manager, 2 for staff):");
            int roleId = Convert.ToInt32(Console.ReadLine());

            User newUser = new User(0, username, roleId);
            bool result = appService.AddUser(newUser);

           
        }

        private void ListUsers()
        {
            List<User> users = appService.ListUsers();
            foreach (User user in users)
            {
                Console.WriteLine(user.ToString());
            }
        }
        private void AddRequirement()
        {
            Console.WriteLine("Enter requirement title:");
            string title = Console.ReadLine();

            Console.WriteLine("Enter requirement description:");
            string description = Console.ReadLine();

            Requirement newRequirement = new Requirement(0, title, description, "InActive"); 
            bool result = appService.AddRequirement(newRequirement);

            if (result)
            {
                Console.WriteLine("Requirement successfully added.");
            }
        }

        private void ListRequirements()
        {
            List<Requirement> requirements = appService.ListRequirements();


            foreach (Requirement requirement in requirements)
            {
                Console.WriteLine(requirement.ToString());
            }
        }




    }


    class Program
    {
        static void Main(string[] args)
        {

            UI app = new UI();
            app.Run();

        }

    }
}

