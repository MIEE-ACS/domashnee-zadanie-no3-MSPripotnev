using System;
using System.Windows;
using System.Windows.Controls;

namespace Homework3
{
    /// <summary>
    /// Дополнительный класс для работы программы. Представляет собой случайное аримфетическое выражение.
    /// </summary>
    class RandomTask
    {
        enum Operation
        {
            Sum,
            Substract
        }
        private string OperationToString(Operation oper) => (int)oper switch
        {
            0 => "+",
            1 => "-",
            _ => throw new Exception("Недопустимый код операции!")
        };
        private int OperationResult(Operation oper, int x, int y) => (int)oper switch
        {
            0 => x+y,
            1 => x-y,
            _ => throw new Exception("Недопустимый код операции!")
        };

        public int[] PossibleAnswers { get; private set; }
        readonly int X;
        readonly int Y;
        readonly int AnswersCount = 4;
        readonly Operation op;

        public RandomTask()
        {
            Random rnd = new Random();
            X = rnd.Next(1, 50);
            Y = rnd.Next(1, 50);
            op = (Operation)rnd.Next(0, 1);

            PossibleAnswers = AdditionalFunctions.AddFunc.GetRandomUniqueNumbers(AnswersCount, 1, X + Y);
            if (Array.FindIndex<int>(PossibleAnswers, p => p == OperationResult(op, X, Y)) == -1)
                PossibleAnswers[AnswersCount - 1] = OperationResult(op, X, Y);
        }
        public RandomTask(int answersCount)
        {
            Random rnd = new Random();
            X = rnd.Next(1, 50);
            Y = rnd.Next(1, 50);
            op = (Operation)rnd.Next(0, 1);
            AnswersCount = answersCount;

            PossibleAnswers = AdditionalFunctions.AddFunc.GetRandomUniqueNumbers(AnswersCount, 1, X + Y);
            if (Array.FindIndex<int>(PossibleAnswers, p => p == OperationResult(op, X, Y)) == -1)
                PossibleAnswers[AnswersCount - 1] = OperationResult(op, X, Y);
        }

        public override string ToString()
        {   
            return $"{X} {OperationToString(op)} {Y} = ?";
        }
        public bool CheckAnswer(int answer) => answer == OperationResult(op, X, Y);
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// /// Счётчик правильных/неправильных ответов, где RA - правильные, WA - неправильные.
        /// </summary>
        int raCounter, waCounter;
        /// <summary>
        /// Right Answers Counter. Счётчик правильных ответов.
        /// </summary>
        private int RACounter
        {
            get
            {
                return raCounter;
            }
            set
            {
                if (value - raCounter == 1)
                    MessageBox.Show(this, "Правильный ответ!");
                raCounter = value;
            }
        }
        /// <summary>
        /// Wrong Answers Counter. Счётчик неправильных ответов.
        /// </summary>
        private int WACounter
        {
            get
            {
                return waCounter;
            }
            set
            {
                if (value - waCounter == 1)
                    MessageBox.Show(this, "Неправильный ответ!");
                waCounter = value;
            }
        }
        RandomTask RT;
        public MainWindow()
        {
            InitializeComponent();
            InitializeNewTask();
        }

        private void CheckAnswer(object sender, RoutedEventArgs e)
        {
            bool is_any_checked = false;
            foreach(RadioButton rb in AnswersGrid.Children)
                if (rb.IsChecked.Value)
                {
                    int answer = int.Parse(rb.Content.ToString());
                    var c = RT.CheckAnswer(answer) ? RACounter++ : WACounter++;
                    is_any_checked = true;
                    break;
                }
            if (!is_any_checked)
                MessageBox.Show(this, "Пожалуйста, выберите один ответ!", "Необходимо выбрать один ответ!", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                InitializeNewTask();
        }
        private void InitializeNewTask()
        {
            RT = new RandomTask(AnswersGrid.Children.Count);

            RALabel.Content = "Правильных ответов: " + RACounter;
            WALabel.Content = "Неправильных ответов: " + WACounter;
            TaskLabel.Content = RT.ToString();

            //Создание массива рандомных индексов для случайного распределения возможных ответов
            int[] randomizer = AdditionalFunctions.AddFunc.GetRandomIndexes(AnswersGrid.Children.Count);
            for (int i = 0; i < AnswersGrid.Children.Count; i++)
            {
                (AnswersGrid.Children[randomizer[i]] as RadioButton).Content = RT.PossibleAnswers[i];
                (AnswersGrid.Children[randomizer[i]] as RadioButton).IsChecked = false;
            }                
        }
    }

    namespace AdditionalFunctions
    {
        public static class AddFunc
        {
            /// <summary>
            /// Создаёт набор индексов массива, разбросанных в случайном порядке.
            /// </summary>
            /// <param name="count">Количество элементов массива.</param>
            /// <returns>Массив случайных индексов массива.</returns>
            public static int[] GetRandomIndexes(int count) => GetRandomUniqueNumbers(count, 0, count);

            /// <summary>
            /// Создаёт набор случайных неповторяющихся чисел.
            /// </summary>
            /// <param name="count">Количество элементов.</param>
            /// <param name="min">Минимальное значение.</param>
            /// <param name="max">Максимальное значение.</param>
            /// <returns></returns>
            public static int[] GetRandomUniqueNumbers(int count, int min, int max)
            {
                int[] result = new int[count];
                Random rnd = new Random();
                for (int i = 0; i < count; i++)
                {
                    int c = rnd.Next(min, max);
                    for (int j = 0; j < i; j++)
                        while (c == result[j]) //while вместо одиночного if, т.к. рандом совершенно рандомно может выдать то же самое число n-раз подряд.
                        {
                            c = rnd.Next(min, max);
                            //Если вдруг найдётся два одинаковых элемента, снова рандомизируем один и пробегаем весь массив заново.
                            if (j != 0)
                                j = 0;
                        }
                        
                    result[i] = c;
                }
                return result;
            }
        }
    }
}
