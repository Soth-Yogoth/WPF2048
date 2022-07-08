using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF2048
{
    public partial class MainWindow : Window
    {
        int boardSize;
        int[,] slots;
        Button[,] buttons;
        public MainWindow()
        {
            InitializeComponent();
        }
        void Start(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(size.Text, out boardSize) && boardSize > 2 && boardSize < 9) { }
            else
            {
                MessageBox.Show("Введите целое положительное число в указанном диапазоне");
                return;
            }

            gameBoard.Children.Clear();          
            gameBoard.Width = gameBoard.Height = boardSize * 60;

            buttons = new Button[boardSize, boardSize];
            slots = new int[boardSize, boardSize];

            for (int i = 0; i < boardSize; i++)
                for (int j = 0; j < boardSize; j++)
                {
                    slots[i, j] = 0;
                    buttons[i, j] = new Button();
                    buttons[i, j].Focusable = false;
                    buttons[i, j].FontSize = 20;
                    buttons[i, j].Background = Brushes.Wheat;
                    gameBoard.Children.Add(buttons[i, j]);
                }
            AddNum();
            AddNum();
        }
        void Move(object sender, KeyEventArgs e)
        {
            int[,] originalSlots = new int[boardSize,boardSize];
            for (int i = 0; i < slots.GetLength(0); i++)
                for (int j = 0; j < slots.GetLength(1); j++)
                    originalSlots[i, j] = slots[i, j];

            switch (e.Key)
            {
                case Key.Up:
                    CleanColumns(0, boardSize);
                    CombineVerticalPairs(0, boardSize);
                    CleanColumns(0, boardSize);
                    break;
                case Key.Left:
                    CleanRows(0, boardSize);
                    CombineHorizontalPairs(0, boardSize);
                    CleanRows(0, boardSize);
                    break;
                case Key.Down:
                    CleanColumns(boardSize - 1, -1);
                    CombineVerticalPairs(boardSize - 1, -1);
                    CleanColumns(boardSize - 1, -1);
                    break;
                case Key.Right:
                    CleanRows(boardSize - 1, -1);
                    CombineHorizontalPairs(boardSize - 1, -1);
                    CleanRows(boardSize - 1, -1);
                    break;
            }
            Redraw();
            if (Win() || GameOver() || !IsChanged(originalSlots, slots))
                return;         
            AddNum();
        }
        void Redraw()
        {
            for (int i = 0; i < boardSize; i++)
                for (int j = 0; j < boardSize; j++)
                {
                    buttons[i, j].Background = Brushes.Wheat;
                    if (slots[i, j] != 0)
                    {
                        buttons[i, j].Content = slots[i, j];
                        buttons[i, j].Background = Brushes.Salmon;
                    }
                    else
                        buttons[i, j].Content = null;                   
                }
        }
        void CleanColumns(int start, int end)
        {
            int x = start < end ? 1 : -1;

            for (int j = 0; j < boardSize; j ++)
            {
                int emptyCells = 0;
                for (int i = start; i != (end - x*emptyCells); i+=x)
                    if (slots[i, j] == 0)
                    {
                        for (int k = i; k != end - x; k+=x)
                        {
                            slots[k, j] = slots[k + x, j];
                        }
                        slots[end - x, j] = 0;
                        emptyCells++;
                        i-=x;
                    }
            }
        }
        void CleanRows(int start, int end)
        {
            int x = start < end ? 1 : -1;

            for (int j = 0; j < boardSize; j++)
            {
                int emptyCells = 0;
                for (int i = start; i != (end - x * emptyCells); i += x)
                    if (slots[j, i] == 0)
                    {
                        for (int k = i; k != end - x; k += x)
                        {
                            slots[j, k] = slots[j, k + x];
                        }
                        slots[j, end - x] = 0;
                        emptyCells++;
                        i -= x;
                    }
            }
        }
        void CombineVerticalPairs(int start, int end)
        {
            int x = start < end ? 1 : -1;

            for (int j = 0; j < boardSize; j++)
                for (int i = start; i != end - x; i+=x)
                    if (slots[i, j] == slots[i + x, j])
                    {
                        slots[i, j] *= 2;
                        slots[i + x, j] = 0;
                    }
        }
        void CombineHorizontalPairs(int start, int end)
        {
            int x = start < end ? 1 : -1;

            for (int j = 0; j < boardSize; j++)
                for (int i = start; i != end - x; i += x)
                    if (slots[j, i] == slots[j, i + x])
                    {
                        slots[j, i] *= 2;
                        slots[j, i + x] = 0;
                    }
        }
        void AddNum()
        {
            while (true)
            {
                Random random = new Random();                
                int a = random.Next(0, boardSize);
                int b = random.Next(0, boardSize);
                int num = random.Next(0, 100) % 5 == 0 ? (4) : (2);
                if (slots[a, b] == 0)
                {
                    buttons[a, b].Content = slots[a, b] = num;
                    buttons[a, b].Background = Brushes.LightSalmon;
                    break;
                }
            }
        }
        bool IsChanged(int[,] originalArr, int[,] newArr)
        {
            for (int i = 0; i < newArr.GetLength(0); i++)
                for (int j = 0; j < newArr.GetLength(1); j++)
                    if(originalArr[i, j] != newArr[i, j])
                        return true;
            return false;
        }
        bool GameOver()
        {
            foreach(int slot in slots)
                if(slot == 0)
                    return false;
            MessageBox.Show("Нет доступных ходов в этом направлении. " +
                "\nЕсли у Вас не осталось ходов, начните новую игру. Удачи!");
            return true;
        }
        bool Win()
        {
            foreach (int slot in slots)
                if (slot == 2048)
                {
                    MessageBox.Show("Вы победили! Поздравляем!");
                    return true;
                }
            return false;
        }
    }
}
