using System.Collections.Generic;
using QuizLib;
using System.Security.Cryptography;
using System.IO;
using System;

namespace QuizAsp.Pages
{
    public static class QuizCache
    {
        private static Dictionary<string, List<QuizQuestion>> _quizzesCache = new Dictionary<string, List<QuizQuestion>>();
        private static Dictionary<string, CompleteQuiz> _quizzes = new Dictionary<string, CompleteQuiz>();


        public static bool TryLoadQuiz (string id, out List<QuizQuestion> questions)
        {
            questions = null;
            if (_quizzes.TryGetValue(id, out var complete))
            {
                questions = complete.Questions;
                return true;
            }
            return false;
        }

        public static void LoadAllQuizzes ()
        {
            var fileList = Directory.GetFiles("Quizzes/", "*.quiz", SearchOption.AllDirectories);
            if (_quizzes.Count == fileList.Length)
            {
                return;
            }

            for (var i = 0; i < fileList.Length; i++)
            {
                CompleteQuiz tempQuiz = new CompleteQuiz(fileList[i]);
                _quizzes[tempQuiz.ID] = tempQuiz;
            }
        }

        public static Dictionary<string, CompleteQuiz> ShowAllQuizzes ()
        {
            return _quizzes;
        }
    }
}