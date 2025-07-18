using System;

namespace Exam_System
{

    //Question

    #region Base Question
    public abstract class Question //Cannot be used as it's, but in child classes
    {
        public string Header { get; set; }
        public string Body { get; set; }
        public int Marks { get; set; }
        public Answer[] Answers { get; set; }

        //Constructor
        public Question(string header, string body, int marks)
        {
            Header = header;
            Body = body;
            Marks = marks;
        }

        //Methods of Base
        public abstract void Display();
        public abstract Answer GetCorrectAnswer();
    }
    #endregion

    #region True/False Question
    public class TrueFalseQuestion : Question
    {
        public TrueFalseQuestion(string header, string body, int marks)
            : base(header, body, marks)
        {
            Answers = new Answer[2]
            {
            new Answer("True", false),
            new Answer("False", false)
            };
        }

        public void SetCorrectAnswer(bool isTrue)
        {
            Answers[0].IsCorrect = isTrue;
            Answers[1].IsCorrect = !isTrue;
        }

        public override void Display()
        {
            Console.WriteLine($"{Header}\n{Body} (Marks: {Marks})");
            Console.WriteLine("1. True\n2. False");
        }

        public override Answer GetCorrectAnswer()
        {
            foreach (var ans in Answers)
                if (ans.IsCorrect) return ans;

            return null;
        }
    }
    #endregion

    #region Choose One Question
    public class ChooseOneQuestion : Question
    {
        public ChooseOneQuestion(string header, string body, int marks, Answer[] answers)
            : base(header, body, marks)
        {
            Answers = answers;
        }

        public override void Display()
        {
            Console.WriteLine($"{Header}\n{Body} (Marks: {Marks})");
            for (int i = 0; i < Answers.Length; i++)
                Console.WriteLine($"{i + 1}. {Answers[i].Text}");
        }

        public override Answer GetCorrectAnswer()
        {
            foreach (var ans in Answers)
                if (ans.IsCorrect) return ans;

            return null;
        }
    }
    #endregion

    #region Choose All Question
    public class ChooseAllQuestion : Question
    {
        public ChooseAllQuestion(string header, string body, int marks, Answer[] answers)
            : base(header, body, marks)
        {
            Answers = answers;
        }

        public override void Display()
        {
            Console.WriteLine($"{Header}\n{Body} (Marks: {Marks})");
            for (int i = 0; i < Answers.Length; i++)
                Console.WriteLine($"{i + 1}. {Answers[i].Text}");
        }

        public override Answer GetCorrectAnswer()
        {
            // Not applicable for Choose All; return null or override with multiple logic if needed
            return null;
        }

        public Answer[] GetCorrectAnswers()
        {
            int count = 0;
            foreach (var ans in Answers)
                if (ans.IsCorrect) count++;

            Answer[] correctAnswers = new Answer[count];
            int index = 0;
            foreach (var ans in Answers)
                if (ans.IsCorrect)
                    correctAnswers[index++] = ans;

            return correctAnswers;
        }
    }
    #endregion


    //Exam

    #region Base Exam 
    public abstract class Exam
    {
        public int TimeInMinutes { get; set; }
        public Question[] Questions { get; set; }
        public Subject Subject { get; set; }

        public Exam(int time, Subject subject, Question[] questions)
        {
            TimeInMinutes = time;
            Subject = subject;
            Questions = questions;
        }

        public abstract void ShowExam();
    }
    #endregion

    #region Practice Exam
    public class PracticeExam : Exam
    {
        public PracticeExam(int time, Subject subject, Question[] questions)
            : base(time, subject, questions) { }

        public override void ShowExam()
        {
            Console.WriteLine($"Practice Exam - Subject: {Subject.Name}, Time: {TimeInMinutes} mins\n");
            foreach (var q in Questions)
            {
                q.Display();
                Console.WriteLine($"Correct Answer: {q.GetCorrectAnswer()?.Text ?? "Multiple"}\n");
            }
        }
    }
    #endregion
    
    #region Final Exam
    public class FinalExam : Exam
    {
        public FinalExam(int time, Subject subject, Question[] questions)
            : base(time, subject, questions) { }

        public override void ShowExam()
        {
            Console.WriteLine($"Final Exam - Subject: {Subject.Name}, Time: {TimeInMinutes} mins\n");
            int totalMarks = 0;
            foreach (var q in Questions)
            {
                q.Display();
                totalMarks += q.Marks;
                Console.WriteLine();
            }
            Console.WriteLine($"Total Marks: {totalMarks}");
        }
    }

    #endregion




    //Subject

    #region Base Subject
    public class Subject
    {
        public string Name { get; set; }

        public Subject(string name)
        {
            Name = name;
        }
    }
    #endregion



    //Answer

    #region Base Answer
    public class Answer
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }

        public Answer(string text, bool isCorrect)
        {
            Text = text;
            IsCorrect = isCorrect;
        }
    }
    #endregion



    // Main

    #region Main
    public class Program
    {
        public static void Main()
        {
            Subject subject = new Subject("Mathematics");

            var q1 = new TrueFalseQuestion("Q1", "2+2=4?", 2);
            q1.SetCorrectAnswer(true);

            var q2 = new ChooseOneQuestion("Q2", "What's the capital of France?", 3,
                new Answer[]
                {
                new Answer("Berlin", false),
                new Answer("Paris", true),
                new Answer("Madrid", false)
                });

            var q3 = new ChooseAllQuestion("Q3", "Select prime numbers:", 5,
                new Answer[]
                {
                new Answer("2", true),
                new Answer("3", true),
                new Answer("4", false),
                new Answer("5", true)
                });

            Question[] questions = new Question[] { q1, q2, q3 };

            Exam practiceExam = new PracticeExam(30, subject, questions);
            practiceExam.ShowExam();

            Console.WriteLine("\n===========================\n");

            Exam finalExam = new FinalExam(30, subject, questions);
            finalExam.ShowExam();
        }

    }
    #endregion

}
