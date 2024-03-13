using System;
using MySql.Data.MySqlClient;

namespace TreasureHunt
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int RoleId { get; set; }

        public User(int userId, string username, int roleId)
        {
            UserId = userId;
            Username = username;
            RoleId = roleId;
        }

        public override string ToString()
        {
            return Username;
        }


    }

    public class Requirement
    {
        public int RequirementId { get; set; }
        public int ProjectId { get; set; } = 1; 
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public Requirement(int requirementId, string title, string description, string status = "InActive")
        {
            RequirementId = requirementId;
            Title = title;
            Description = description;
            Status = status;
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

    public class Application
    {
        private DatabaseManager dbManager = new DatabaseManager();

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
            dbManager.AddUser(newUser);
        }

        private void ListUsers()
        {
            List<User> users = dbManager.ListUsers();
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

            Requirement newRequirement = new Requirement(0, title, description, "InActive"); // RequirementId由数据库自动生成，这里传0作为占位符
            dbManager.AddRequirement(newRequirement);
        }

        private void ListRequirements()
        {
            List<Requirement> requirements = dbManager.ListRequirements();


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
            
                Application app = new Application();
                app.Run();
            
        }

    }
}

