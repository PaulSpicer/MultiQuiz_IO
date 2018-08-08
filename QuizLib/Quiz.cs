using System;
using System.Collections.Generic;
using System.IO;

namespace QuizLib
{
    public class Quiz
    {
        public static List<QuizQuestion> LoadQuiz (string fileName)
        {
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

}
