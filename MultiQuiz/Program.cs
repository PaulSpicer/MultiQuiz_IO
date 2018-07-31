using System;
using System.Collections.Generic;
using System.IO;

namespace MultiQuizIO
{
    class Program
    {
        static void Main (string[] args)
        {
            int currentQuestion = 0, score = 0;
            var filename = "Quizzes/data.txt";

            if (args.Length > 0)
            {
                filename = args[0];
            }

            var questions = SetQuestions(filename);

            Console.WriteLine("Welcome to the Quiz");
            Console.WriteLine($"Please answer the {questions.Count} multiple choice questions below \n\n");

            for (; currentQuestion < questions.Count; currentQuestion++)
            {
                Console.WriteLine("QUESTION " + (currentQuestion + 1));
                Console.WriteLine(questions[currentQuestion].Question);
                for (int j = 0; j < questions[currentQuestion].Answers.Length; j++)
                {
                    Console.WriteLine($"{(j + 1)}) {questions[currentQuestion].Answers[j]}");
                }
                Answering:
                Console.Write("\nANSWER: ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out int answerInt))
                {
                    if (answerInt - 1 == questions[currentQuestion].CorrectAnswer)
                    {
                        score++;
                        Console.WriteLine("\nCORRECT! \n");
                    }
                    else
                    {
                        Console.WriteLine("\nWRONG! \n");
                    }
                }
                else
                {
                    Console.WriteLine("\nThat is not a valid answer, try again \n");
                    goto Answering;
                }
            }

            Console.WriteLine($"\n Your Total Score is: {score} out of {questions.Count}.");
            Console.Read();
        }

        private static List<QuizQuestion> SetQuestions (string fileName)
        {
            QuizQuestion currentQuestion = null;
            Random rand = new Random();
            List<QuizQuestion> questions = new List<QuizQuestion>();
            string[] quizFile = null;
            
            try
            {
                quizFile = File.ReadAllLines(fileName);
            }
            catch
            {
                Console.WriteLine("Error reading input file");
                Environment.Exit(1);
            }

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
    
    class QuizQuestion
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