using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace QuizLib
{
    public class Quiz
    {
        public static List<QuizQuestion> LoadQuiz (string fileName)
        {
            return LoadQuiz(fileName, out var discard, out var discard2);   
        }

        public static List<QuizQuestion> LoadQuiz (string fileName, out string desc, out string title)
        {
            desc = "";
            title = "";

            QuizQuestion currentQuestion = null;
            Random rand = new Random();
            List<QuizQuestion> questions = new List<QuizQuestion>();
            string[] quizFile = null;

            quizFile = File.ReadAllLines(fileName);

            foreach (var line in quizFile)
            {
                if (line.StartsWith("$Q"))
                {
                    currentQuestion = new QuizQuestion();
                    currentQuestion.Question = line.Substring(3);
                }
                else if (line.StartsWith("$A"))
                {
                    currentQuestion.Answers = line.Substring(3).Split(',');

                    for (int i = 1; i < currentQuestion.Answers.Length; i++)
                    {
                        currentQuestion.Answers[i] = currentQuestion.Answers[i].Substring(1);
                    }

                    currentQuestion.CorrectAnswer = rand.Next(0, currentQuestion.Answers.Length - 1);

                    if (currentQuestion.CorrectAnswer != 0)
                    {
                        var tempAnswer = currentQuestion.Answers[currentQuestion.CorrectAnswer];
                        currentQuestion.Answers[currentQuestion.CorrectAnswer] = currentQuestion.Answers[0];
                        currentQuestion.Answers[0] = tempAnswer;
                    }
                    questions.Add(currentQuestion);
                }
                else if (line.StartsWith("$DESC"))
                {
                    desc = line.Substring(5);
                }
                else if (line.StartsWith("$TITLE"))
                {
                    title = line.Substring(6);
                }
            }
            return questions;
        }
    }

    public class QuizQuestion
    {
        public QuizQuestion ()
        {
            
        }

        public QuizQuestion (string question, string[] answers, int correctAnswer)
        {
            Question = question;
            Answers = answers;
            CorrectAnswer = correctAnswer;
        }

        public string Question { get; set; }
        public string[] Answers { get; set; }
        public int CorrectAnswer { get; set; }
    }

    public class CompleteQuiz
    {
        public string ID { get; set; }
        public string FilePath { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public List<QuizQuestion> Questions;

        public CompleteQuiz (string path)
        {
            FilePath = path;
            Questions = Quiz.LoadQuiz(path, out var desc, out var title);
            Description = desc;
            Title = title;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    ID = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty).ToLowerInvariant();
                }              
            }
        }
    }
}
