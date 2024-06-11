using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using user_class;
using question_class;
using quiz_class;

namespace main
{
    class Program
    {
        static void Main()
        {
            RunMainMenu();
        }

        static void RunMainMenu()
        {
            User currentUser = null;

            while (true)
            {
                Console.WriteLine("-= MAIN MENU =-");
                Console.WriteLine("1. Log in");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Top 20");
                Console.WriteLine("4. Admin panel");
                Console.WriteLine("5. Exit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ClearScreen();
                        if (currentUser == null)
                        {
                            User user = new User();
                            currentUser = user.LogIn("profile.txt");
                            if (currentUser != null)
                            {
                                RunUserMenu(currentUser);
                            }
                        }
                        else
                        {
                            Console.WriteLine("You are already logged in.");
                        }

                        break;

                    case "2":
                        ClearScreen();
                        if (currentUser == null)
                        {
                            currentUser = new User();
                            currentUser.Register("profile.txt");
                        }
                        else
                        {
                            Console.WriteLine("You are already logged in.");
                        }

                        break;

                    case "3":
                        ClearScreen();
                        ViewTopPlayers();
                        break;


                    case "4":
                        ClearScreen();
                        RunAdminMenu();
                        break;

                    case "5":
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void RunUserMenu(User currentUser)
        {
            ClearScreen();
            while (true)
            {
                Console.WriteLine("-= PLAYER MENU =-");
                Console.WriteLine("1. Start a new quiz");
                Console.WriteLine("2. Results");
                Console.WriteLine("3. Edit profile");
                Console.WriteLine("4. Log out");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ClickClearScreen();
                        StartNewQuiz(currentUser);
                        break;

                    case "2":
                        ClickClearScreen();
                        ViewQuizResults(currentUser);
                        break;

                    case "3":
                        ClickClearScreen();
                        currentUser.EditProfile();
                        break;

                    case "4":
                        ClickClearScreen();
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void RunAdminMenu()
        {
            User currentUser = null;

            bool exit = false;

            while (!exit)
            {
                ClearScreen();
                Console.WriteLine("** ADMIN MENU **");
                Console.WriteLine("1. Log in");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Exit");

                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        if (currentUser == null)
                        {
                            User user = new User();
                            currentUser = user.LogIn("admin_profile.txt");
                            if (currentUser != null)
                            {
                                RunAdminPanel(currentUser);
                            }
                        }
                        else
                        {
                            Console.WriteLine("You are already logged in.");
                        }
                        break;

                    case "2":
                        if (currentUser == null)
                        {
                            User user = new User();
                            user.Register("admin_profile.txt");
                        }
                        else
                        {
                            Console.WriteLine("You are already logged in.");
                        }
                        break;

                    case "3":
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please choose a valid option.");
                        break;
                }
            }
        }

        static void RunAdminPanel(User currentUser)
        {
            ClearScreen();
            while (true)
            {
                Console.WriteLine("-= ** ADMIN PANEL ** =-");
                Console.WriteLine("1. Create a new quiz");
                Console.WriteLine("2. Edit a quiz");
                Console.WriteLine("3. All users");
                Console.WriteLine("4. Log out");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ClickClearScreen();
                        Quiz newQuiz = new Quiz();
                        newQuiz.CreateQuiz(newQuiz);
                        break;

                    case "2":
                        ClickClearScreen();
                        Quiz quiz = new Quiz();
                        quiz.EditQuiz();
                        break;

                    case "3":
                        ClickClearScreen();
                        DisplayAllUsers("profile.txt");
                        break;

                    case "4":
                        ClickClearScreen();
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void StartNewQuiz(User currentUser)
        {
            Quiz quiz = new Quiz();
            quiz.LoadFromFile("quiz_questions.txt");
            quiz.Start(currentUser);
        }

        static void ViewQuizResults(User currentUser)
        {
            string filePath = "score.txt";
            string[] lines = File.ReadAllLines(filePath);

            Console.WriteLine("-= QUIZ RESULTS =-");
            Console.WriteLine($"player {currentUser.Login}");

            foreach (var line in lines)
            {
                string[] parts = line.Split(',');

                if (parts[0] == currentUser.Login)
                {
                    string category = parts[1];
                    int score = int.Parse(parts[2]);
                    Console.WriteLine($"{category}: {score} points");
                }
            }

            // any results???
            if (!lines.Any(line => line.StartsWith(currentUser.Login)))
            {
                Console.WriteLine("No quiz results available.");
            }
            ClickClearScreen();
        }

        static void ViewTopPlayers()
        {
            string filePath = "score.txt";
            string[] lines = File.ReadAllLines(filePath);

            Dictionary<string, int> userScores = new Dictionary<string, int>();

            foreach (var line in lines)
            {
                string[] parts = line.Split(',');
                string username = parts[0];
                int score = int.Parse(parts[2]);

                if (userScores.ContainsKey(username))
                {
                    userScores[username] += score;
                }
                else
                {
                    userScores[username] = score;
                }
            }

            var sortedScores = userScores.OrderByDescending(x => x.Value);

            Console.WriteLine("-= TOP PLAYERS =-");
            int rank = 1;

            foreach (var userScore in sortedScores)
            {
                if (rank > 20)
                    break;
                Console.WriteLine($"{rank}.\t{userScore.Key}\t{userScore.Value}\tpoints");
                rank++;
            }
            ClickClearScreen();
        }

        static void DisplayAllUsers(string filePath)
        {
            Console.WriteLine("** ALL PLAYERS **\n");
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] fields = line.Split(',');

                if (fields.Length == 3)
                {
                    string name = fields[0];
                    string password = fields[1];
                    string birthDate = fields[2];

                    Console.WriteLine($"log: {name}, pass: {password}, birth: {birthDate}");
                }
                else
                {
                    Console.WriteLine($"Invalid data format: {line}");
                }
            }

            ClickClearScreen();
        }


        static void ClearScreen()
        {
            Console.Clear();
        }

        static void ClickClearScreen()
        {
            Console.WriteLine("\nNext . . .");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
