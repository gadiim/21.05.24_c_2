using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace question_class
{
    internal class Question
    {
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public int CorrectOption { get; set; }
        public string Category { get; set; }

        public Question(string text, List<string> options, int correctOption, string category)
        {
            Text = text;
            Options = options;
            CorrectOption = correctOption;
            Category = category;
        }

        public bool IsCorrect(int answer)
        {
            return answer == CorrectOption;
        }
    }
}
