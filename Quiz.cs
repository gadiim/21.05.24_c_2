using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using question_class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using user_class;

namespace quiz_class
{
    internal class Quiz
    {
        public Dictionary<string, List<Question>> QuestionsByCategory { get; private set; }
        public int Score { get; private set; }
        private int currentQuestionIndex;
        private List<Question> currentQuestions;

        public Quiz()
        {
            QuestionsByCategory = new Dictionary<string, List<Question>>();
            Score = 0;
            currentQuestionIndex = 0;
        }

        public void AddQuestion(Question question)
        {
            if (!QuestionsByCategory.ContainsKey(question.Category))
            {
                QuestionsByCategory[question.Category] = new List<Question>();
            }
            QuestionsByCategory[question.Category].Add(question);
        }

        public void Start(User currentUser)
        {
            Console.WriteLine("*** GAME MENU ***");
            Console.WriteLine("Available categories:");
            foreach (var category in QuestionsByCategory.Keys)
            {
                Console.WriteLine($"- {category}");
            }

            Console.WriteLine("- Mixed");

            Console.Write("Choose a category: ");
            string chosenCategory = Console.ReadLine().Trim();

            if (QuestionsByCategory.ContainsKey(chosenCategory))
            {
                currentQuestions = QuestionsByCategory[chosenCategory];
                Console.WriteLine($"\nStarting quiz on {chosenCategory}\n");
                PlayQuiz(currentUser, chosenCategory);
            }
            else if (chosenCategory.ToLower() == "mixed")
            {
                StartMixedQuiz(currentUser);
            }
            else
            {
                Console.WriteLine("Invalid category.");
            }
        }

        public void StartMixedQuiz(User currentUser)
        {
            List<Question> allQuestions = new List<Question>();
            int numbQuestions = 5;

            foreach (var categoryQuestions in QuestionsByCategory.Values)
            {
                allQuestions.AddRange(categoryQuestions);
            }

            Random rng = new Random();
            allQuestions = allQuestions.OrderBy(q => rng.Next()).ToList();

            if (allQuestions.Count > numbQuestions)
            {
                allQuestions = allQuestions.Take(numbQuestions).ToList();
            }

            currentQuestions = allQuestions;
            Console.WriteLine("\nStarting mixed quiz\n");
            ClearScreen();
            PlayQuiz(currentUser, "Mixed");
        }

        private void PlayQuiz(User currentUser, string categoryName)
        {
            while (currentQuestionIndex < currentQuestions.Count)
            {
                DisplayQuestion();
                int userAnswer = GetUserAnswer();
                if (currentQuestions[currentQuestionIndex].IsCorrect(userAnswer))
                {
                    Console.WriteLine("Correct!\n");
                    Score++;
                }
                else
                {
                    Console.WriteLine($"Incorrect! The correct answer was {currentQuestions[currentQuestionIndex].Options[currentQuestions[currentQuestionIndex].CorrectOption]}\n");
                }

                ClickClearScreen();
                currentQuestionIndex++;
            }

            SaveQuizResultsToFile(currentUser, categoryName);
            Console.WriteLine($"Quiz finished! Your score is {Score} out of {currentQuestions.Count}");
        }

        private void DisplayQuestion()
        {
            var question = currentQuestions[currentQuestionIndex];
            Console.WriteLine($"Question {currentQuestionIndex + 1}: {question.Text}");
            for (int i = 0; i < question.Options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {question.Options[i]}");
            }
        }

        private int GetUserAnswer()
        {
            int answer;
            while (true)
            {
                Console.Write("Your answer: ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out answer) && answer > 0 && answer <= currentQuestions[currentQuestionIndex].Options.Count)
                {
                    return answer - 1; // !!! convert to zero-based index !!!
                }
                Console.WriteLine("Invalid answer. Please enter a number corresponding to the options.");
            }
        }

        public void SaveToFile(string filePath)
        {
            var json = JsonConvert.SerializeObject(QuestionsByCategory, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public void LoadFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                QuestionsByCategory = JsonConvert.DeserializeObject<Dictionary<string, List<Question>>>(json);
            }
            else
            {
                Console.WriteLine("File not found.");
            }
        }

        public void SaveQuizResultsToFile(User currentUser, string categoryName)
        {
            string filePath = "score.txt";
            string scoreData = $"{currentUser.Login},{categoryName},{Score},{DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            List<string> lines = new List<string>();

            if (File.Exists(filePath))
            {
                lines = File.ReadAllLines(filePath).ToList();
            }

            string currentKey = $"{currentUser.Login},{categoryName}";
            string previousScoreLine = null;

            foreach (var line in lines)
            {
                if (line.StartsWith(currentKey))
                {
                    previousScoreLine = line;
                    break;
                }
            }

            bool shouldSaveNewScore = true;
            if (previousScoreLine != null)
            {
                var parts = previousScoreLine.Split(',');
                if (parts.Length >= 3 && int.TryParse(parts[2], out int previousScore))
                {
                    shouldSaveNewScore = Score > previousScore;
                }

                if (shouldSaveNewScore)
                {
                    lines.Remove(previousScoreLine);
                }
            }

            if (shouldSaveNewScore)
            {
                try
                {
                    lines.Add(scoreData);
                    File.WriteAllLines(filePath, lines);
                    Console.WriteLine("Quiz results saved successfully.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred while saving the quiz results to file:");
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine("Current score is not higher than the previous score. Results not saved.");
            }
        }

        public void CreateQuiz(Quiz quiz)
        {
            Console.WriteLine("Creating a new quiz...");

            Console.Write("Enter quiz name: ");
            string quizName = Console.ReadLine();

            Console.Write("Enter quiz category: ");
            string category = Console.ReadLine();

            List<Question> newQuizQuestions = new List<Question>();

            Console.Write("Enter number of questions: ");
            int numQuestions;
            while (!int.TryParse(Console.ReadLine(), out numQuestions) || numQuestions <= 0)
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                Console.Write("Enter number of questions: ");
            }

            for (int i = 0; i < numQuestions; i++)
            {
                Console.WriteLine($"\nEnter details for question {i + 1}:");
                Console.Write("Enter question text: ");
                string questionText = Console.ReadLine();

                List<string> options = new List<string>();
                Console.WriteLine("Enter options (one per line, enter 'done' when finished):");
                string option;
                do
                {
                    option = Console.ReadLine();
                    if (option.ToLower() != "done")
                    {
                        options.Add(option);
                    }
                } while (option.ToLower() != "done");

                int correctOption;
                do
                {
                    Console.Write("Enter the correct option (1 - " + options.Count + "): ");
                } while (!int.TryParse(Console.ReadLine(), out correctOption) || correctOption < 1 || correctOption > options.Count);

                Question newQuestion = new Question(questionText, options, correctOption - 1, category);

                newQuizQuestions.Add(newQuestion);
            }

            Quiz newQuiz = new Quiz();
            foreach (var question in newQuizQuestions)
            {
                newQuiz.AddQuestion(question);
            }

            quiz.QuestionsByCategory.Add(category, newQuizQuestions);

            SaveQuizToFile(quiz);

            Console.WriteLine("\nQuiz created successfully!");
        }


        private void SaveQuizToFile(Quiz quiz)
        {
            string filePath = "quiz_questions.txt";
            Dictionary<string, List<Question>> existingQuestionsByCategory = new Dictionary<string, List<Question>>();


            if (File.Exists(filePath))
            {
                try
                {
                    string existingJson = File.ReadAllText(filePath);
                    existingQuestionsByCategory = JsonConvert.DeserializeObject<Dictionary<string, List<Question>>>(existingJson);
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred while reading the existing quiz data:");
                    Console.WriteLine(e.Message);
                    return;
                }
            }


            foreach (var category in quiz.QuestionsByCategory)
            {
                if (existingQuestionsByCategory.ContainsKey(category.Key))
                {
                    existingQuestionsByCategory[category.Key].AddRange(category.Value);
                }
                else
                {
                    existingQuestionsByCategory[category.Key] = new List<Question>(category.Value);
                }
            }

            try
            {
                var json = JsonConvert.SerializeObject(existingQuestionsByCategory, Formatting.Indented);
                File.WriteAllText(filePath, json);
                Console.WriteLine("Quiz saved successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred while saving the quiz to file:");
                Console.WriteLine(e.Message);
            }
        }

        public void EditQuiz()
        {
            Console.WriteLine("Editing a quiz...");
            Console.WriteLine("Available quizzes:");

            List<string> availableQuizzes = LoadAvailableQuizzes();

            for (int i = 0; i < availableQuizzes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableQuizzes[i]}");
            }

            Console.Write("Choose a quiz to delete (enter number): ");
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > availableQuizzes.Count)
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                Console.Write("Choose a quiz to delete (enter number): ");
            }

            string quizToDelete = availableQuizzes[choice - 1];
            if (DeleteQuiz(quizToDelete))
            {
                Console.WriteLine($"Quiz '{quizToDelete}' has been successfully deleted.");
            }
            else
            {
                Console.WriteLine($"Failed to delete quiz '{quizToDelete}'.");
            }
        }

        private List<string> LoadAvailableQuizzes()
        {
            List<string> availableQuizzes = new List<string>();

            string filePath = "quiz_questions.txt";

            try
            {
                string json = File.ReadAllText(filePath);

                JObject quizData = JObject.Parse(json);

                foreach (var category in quizData)
                {
                    availableQuizzes.Add(category.Key);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }

            return availableQuizzes;
        }


        private bool DeleteQuiz(string quizName)
        {
            string filePath = "quiz_questions.txt";

            try
            {
                string json = File.ReadAllText(filePath);

                JObject quizData = JObject.Parse(json);

                if (quizData.ContainsKey(quizName))
                {
                    quizData.Remove(quizName);

                    File.WriteAllText(filePath, quizData.ToString());

                    return true;
                }
                else
                {
                    Console.WriteLine($"Quiz '{quizName}' not found.");
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
                return false;
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
