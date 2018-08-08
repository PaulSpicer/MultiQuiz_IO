using QuizLib;
using System;
using System.Collections.Generic;
using System.IO;

namespace MultiQuizIO
{

    //A program to test file input
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
            else
            {
                string[] quizFiles = null;
                try
                {
                    quizFiles = Directory.GetFiles("quizzes", "*.txt", SearchOption.AllDirectories);
                    if (quizFiles.Length == 0)
                    {
                        Console.WriteLine("No quizzes found, please add some and try again");
                        Environment.Exit(3);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Environment.Exit(2);
                }

                Console.WriteLine("The following quizzes are available: \n");
                for (int i = 0; i < quizFiles.Length; i++)
                {
                    Console.WriteLine($"{i + 1}) {quizFiles[i]}");
                }
                QuizSelect:
                Console.Write("\nSelect which quiz to play: ");
                var input = Console.ReadLine();
                if (int.TryParse(input, out int choiceInt) && (choiceInt > 0 && choiceInt <= quizFiles.Length))
                {
                    filename = quizFiles[choiceInt - 1];
                }
                else
                {
                    Console.Write("Invalid input, try again.\n");
                    goto QuizSelect;
                }
            }

            Console.WriteLine($"Using {filename} for quiz file\n");

            var questions = Quiz.LoadQuiz(filename);

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
                if (int.TryParse(input, out int answerInt) && (answerInt > 0 && answerInt <= questions[currentQuestion].Answers.Length))
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
                    Console.WriteLine("\nThat is not a valid answer, try again. \n");
                    goto Answering;
                }
            }

            Console.WriteLine($"\n Your Total Score is: {score} out of {questions.Count}.");
            Console.Read();
        }
    }
}