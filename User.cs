using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace user_class
{
    internal class User
    {
        private string login { get; set; }
        private string password { get; set; }
        private DateTime birthDate { get; set; }

        public User()
        {
        }
        public User(string login, string password, DateTime birthDate)
        {
            this.login = login;
            this.password = password;
            this.birthDate = birthDate;
        }

        public string Login { get { return login; } set { login = value; } }
        public string Password { get { return password; } set { password = value; } }
        public DateTime BirthDate { get { return birthDate; } set { birthDate = value; } }

        public void SaveToFile(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine($"{Login},{Password},{BirthDate:yyyy-MM-dd}");
            }
        }

        public void LoadFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(',');
                        string login = parts[0];
                        string password = parts[1];
                        DateTime birthDate = DateTime.ParseExact(parts[2], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    Console.WriteLine("File not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while loading data from file: {ex.Message}");
            }
        }

        public void ChangePassword(string newPassword)
        {
            Password = newPassword;
        }

        public void ChangeBirthDate(DateTime newBirthDate)
        {
            BirthDate = newBirthDate;
        }

        public string[] GetLineFromFile(string searchElement, string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                int lineNumber = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains(searchElement))
                    {
                        string[] lineArray = line.Split(',');
                        if (lineArray[0] == searchElement)
                        {
                            return lineArray;
                        }
                    }
                    lineNumber++;
                }
                return new string[0];
            }
        }

        public string CombineArrayToString(string[] array)
        {
            return string.Join(",", array);
        }

        public User LogIn(string filePath)
        {
            ClearScreen();
            Console.WriteLine("-= LOGIN MENU =-");
            Console.Write("Enter login: ");
            string login = Console.ReadLine();

            string searchElement = login;
            string[] line = GetLineFromFile(searchElement, filePath);

            if (line.Length == 0)
            {
                Console.WriteLine($"Login \"{searchElement}\" not found");
                ClickClearScreen();
                return null;
            }
            else
            {
                Console.Write("Enter password: ");
                string password = Console.ReadLine();
                if (line[1].Trim() == password)
                {
                    User currentUser = new User
                    {
                        Login = line[0],
                        Password = line[1]
                    };

                    Console.WriteLine("Correct password");
                    ClickClearScreen();
                    return currentUser;
                }
                else
                {
                    Console.WriteLine("Incorrect password");
                    ClickClearScreen();
                    return null;
                }
            }
        }

        public void Register(string filePath)
        {
            ClearScreen();
            Console.WriteLine("-= REGISTRATION MENU =-");
            Console.Write("Enter new login: ");
            string login = Console.ReadLine();

            string searchElement = login;
            string[] line = GetLineFromFile(searchElement, filePath);

            if (line.Length != 0)
            {
                Console.WriteLine($"Login \"{searchElement}\" already exists");
                ClickClearScreen();
                return;
            }

            Console.Write("Enter new password: ");
            string password = Console.ReadLine();
            Console.Write("Enter date of birth (yyyy-MM-dd): ");
            DateTime birthDate;
            while (!DateTime.TryParse(Console.ReadLine(), out birthDate))
            {
                Console.WriteLine("Invalid date format. Try again.");
                Console.Write("Enter date of birth (yyyy-MM-dd): ");
            }

            User newUser = new User
            {
                Login = login,
                Password = password,
                BirthDate = birthDate
            };

            newUser.SaveToFile(filePath);
            ClickClearScreen();
        }

        public void EditProfile()
        {
            Console.WriteLine("Editing user profile:");
            Console.WriteLine("1. Change password");
            Console.WriteLine("2. Change date of birth");
            Console.WriteLine("3. Return to the main menu");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter new password: ");
                    string newPassword = Console.ReadLine();
                    ChangePassword(newPassword);
                    Console.Write("Do you want to save changes? (Y/N): ");
                    string saveNewPassword = Console.ReadLine().ToUpper();

                    if (saveNewPassword == "Y")
                    {
                        try
                        {
                            string filePath = "profile.txt";
                            string[] userLine = GetLineFromFile(Login, filePath);

                            if (userLine.Length > 0)
                            {
                                userLine[1] = newPassword;
                            }
                            else
                            {
                                Console.WriteLine("User not found in the file.");
                            }
                            string user = CombineArrayToString(userLine);
                            File.WriteAllText(filePath, user);
                            Console.WriteLine("Changes saved successfully.");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("An error occurred while processing the file:");
                            Console.WriteLine(e.Message);
                        }
                    }
                    break;

                case "2":
                    Console.Write("Enter new date of birth (yyyy-MM-dd): ");
                    DateTime newBirthDate;
                    while (!DateTime.TryParse(Console.ReadLine(), out newBirthDate))
                    {
                        Console.WriteLine("Invalid date format. Try again.");
                        Console.Write("Enter new date of birth (yyyy-MM-dd): ");
                    }
                    ChangeBirthDate(newBirthDate);

                    Console.Write("Do you want to save changes? (Y/N): ");
                    string saveNewBirthDate = Console.ReadLine().ToUpper();

                    if (saveNewBirthDate == "Y")
                    {
                        try
                        {
                            string filePath = "profile.txt";
                            string[] userLine = GetLineFromFile(Login, filePath);

                            if (userLine.Length > 0)
                            {
                                userLine[2] = newBirthDate.ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                Console.WriteLine("User not found in the file.");
                            }
                            string user = CombineArrayToString(userLine);
                            File.WriteAllText(filePath, user);
                            Console.WriteLine("Changes saved successfully.");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("An error occurred while processing the file:");
                            Console.WriteLine(e.Message);
                        }
                    }
                    break;

                case "3":
                    ClickClearScreen();
                    return;

                default:
                    Console.WriteLine("Invalid selection. Please try again.");
                    break;
            }
        }

        public void ClearScreen()
        {
            Console.Clear();
        }

        public void ClickClearScreen()
        {
            Console.WriteLine("\nNext . . .");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
