using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizLib;

namespace QuizAsp.Pages
{
    public class IndexModel : PageModel
    {
        const string QuestionNumberKey = "QuestionNumber";
        const string AnswerCorrectKey = "AnswerCorrect";
        const string AnswerPreviousKey = "AnswerPreviousIndex";
        const string CurrentScoreKey = "CurrentScore";

        public string Question { get; set; }
        public string[] Answers { get; set; }
        public int CurrentQuestionNumber { get; set; }
        public int CurrentScore { get; set; }

        public string SuccessMessage;
        public int previousAnswer = -1;

        public void OnGet ()
        {
            if (!HttpContext.Session.TryGetInt(QuestionNumberKey, out var currentQuestion) ||
               (!HttpContext.Session.TryGetInt(CurrentScoreKey, out var currentScore)))
            {
                CurrentQuestionNumber = 0;
                CurrentScore = 0;
                HttpContext.Session.Set(QuestionNumberKey, BitConverter.GetBytes(CurrentQuestionNumber));
                HttpContext.Session.Set(CurrentScoreKey, BitConverter.GetBytes(CurrentScore));
            }
            else
            {
                CurrentQuestionNumber = currentQuestion;
                CurrentScore = currentScore;
            }

            var questions = QuizCache.LoadQuiz("Quizzes/data.txt");
            Question = questions[CurrentQuestionNumber].Question;
            Answers = questions[CurrentQuestionNumber].Answers;

            if (TempData.TryGetValue(AnswerCorrectKey, out var answerCorrect))
            {

                if ((bool)answerCorrect)
                {
                    SuccessMessage = "Correct Answer";
                    CurrentScore++;
                    HttpContext.Session.Set(CurrentScoreKey, BitConverter.GetBytes(CurrentScore));
                }
                else
                {
                    SuccessMessage = $"Incorrect Answer. The correct answer is: { questions[CurrentQuestionNumber].Answers[questions[CurrentQuestionNumber].CorrectAnswer]}.";
                }

                TempData.TryGetValue(AnswerPreviousKey, out var answerIndex);
                previousAnswer = (int)answerIndex;
            }
        }

        [ValidateAntiForgeryToken]
        public IActionResult OnPost (int answer, string answerbutton, string nextbutton)
        {
            if (!HttpContext.Session.TryGetInt(QuestionNumberKey, out var currentQuestion))
            {
                return new RedirectResult("/");
            }
            else
            {
                CurrentQuestionNumber = currentQuestion;
                if (nextbutton != null)
                {
                    CurrentQuestionNumber++;
                    HttpContext.Session.Set(QuestionNumberKey, BitConverter.GetBytes(CurrentQuestionNumber));
                    return new RedirectResult("/");
                }
            }

            if (answerbutton != null)
            {
                var questions = QuizCache.LoadQuiz("Quizzes/data.txt");

                TempData[AnswerPreviousKey] = answer;
                TempData[AnswerCorrectKey] = (answer == questions[CurrentQuestionNumber].CorrectAnswer);
            }           
            return new RedirectResult("/");
        }
    }

    public static class QuizCache
    {

        private static Dictionary<string, List<QuizQuestion>> _quizzes = new Dictionary<string, List<QuizQuestion>>();

        public static List<QuizQuestion> LoadQuiz (string fileName)
        {
            if (_quizzes.TryGetValue(fileName, out var quiz))
            {
                return quiz;
            }
            quiz = Quiz.LoadQuiz(fileName);
            _quizzes[fileName] = quiz;
            return quiz;
        }
    }

    public static class SessionExtensions
    {
        public static bool TryGetInt(this ISession session, string key, out int value)
        {
            value = 0;
            if (session.TryGetValue(key, out var qNumBytes))
            {
                value = BitConverter.ToInt32(qNumBytes);
                return true;
            }
            return false;
        }
    }
}