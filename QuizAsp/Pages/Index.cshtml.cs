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
        public Dictionary<string, CompleteQuiz> quizzes;

        public void OnGet ()
        {
            QuizCache.LoadAllQuizzes();
            quizzes = QuizCache.ShowAllQuizzes();
            //expose ID, description, #ofquestions/, title
        }        
    }
}