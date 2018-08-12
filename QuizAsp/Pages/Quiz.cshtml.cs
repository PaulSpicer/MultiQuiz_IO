using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QuizAsp.Pages
{
    public class QuizModel : PageModel
    {
        const string QuestionNumberKey = "QuestionNumber";
        const string AnswerCorrectKey = "AnswerCorrect";
        const string AnswerPreviousKey = "AnswerPreviousIndex";
        const string CurrentScoreKey = "CurrentScore";

        public string Question { get; set; }
        public string[] Answers { get; set; }
        public int CurrentQuestionNumber { get; set; }
        public int CurrentScore { get; set; }
        public int TotalQuestions { get; set; }
        public string Id { get; set; }

        public string SuccessMessage;
        public int previousAnswer = -1;

        public void OnGet (string id)
        {
            Id = id;
            if (!HttpContext.Session.TryGetInt(QuestionNumberKey, out var currentQuestion) ||
                !HttpContext.Session.TryGetInt(CurrentScoreKey, out var currentScore))
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
            if (!QuizCache.TryLoadQuiz(id, out var questions))
            {
                //
            }

            TotalQuestions = questions.Count;

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
        public IActionResult OnPost (int answer, string answerbutton, string nextbutton, string id)
        {
            if (!HttpContext.Session.TryGetInt(QuestionNumberKey, out var currentQuestion) ||
                !HttpContext.Session.TryGetInt(CurrentScoreKey, out var currentScore))
            {
                HttpContext.Session.Clear();
                return Redirect("/");
            }

            CurrentQuestionNumber = currentQuestion;

            if (!QuizCache.TryLoadQuiz(id, out var questions))
            {
                //TODO: QUIZ NOT FOUND - FALLBACK BEHAVIOUR
            }

            if (nextbutton != null)
            {
                CurrentQuestionNumber++;

                if (CurrentQuestionNumber >= questions.Count || CurrentQuestionNumber < 0)
                {
                    HttpContext.Session.Clear();
                    return Redirect("/");
                }
                HttpContext.Session.Set(QuestionNumberKey, BitConverter.GetBytes(CurrentQuestionNumber));
            }

            if (answerbutton != null)
            {
                TempData[AnswerPreviousKey] = answer;
                TempData[AnswerCorrectKey] = (answer == questions[CurrentQuestionNumber].CorrectAnswer);
            } 
           
            return Redirect($"{HttpContext.Request.Path}/?id={id}");
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