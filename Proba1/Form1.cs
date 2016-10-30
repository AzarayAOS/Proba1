using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proba1
{
    public partial class Form1 : Form
    {
        public bool[] x;       // значения входов
        public int[][] w;     // значения весов нейронов и каждой его связи
        public bool[] s;       // значения выходов
        public bool[] y;       // значения порога каждого нейрона
        public int[][] wx;     // значение входов непосредвственно на скрытый слой
        public byte input;      // вход на нейрон
        public byte output;     // выход от нейрона
        public Encoding enc8 = Encoding.UTF8;   // для кодирования и декодирования

        // работа нейронной сети
        private void DoNeiron(byte sm1)
        {
            // создаем массив для битовых значений
            int[] bitsm1 = new int[8];

            // заполняем битовые массивы
            for (int i = 0; i < 8; i++)
            {
                // сощдаем переменную-множитель
                int mnog = (int)Math.Pow(2, i);
                // выдиляем нужный бит из первого символа
                int rez = sm1 & mnog;

                if (rez == 0)
                    bitsm1[i] = 0;
                else
                    bitsm1[i] = 1;
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                    wx[i][j] = w[i][j] * bitsm1[j];

                double xa = 0;
                for (int j = 0; j < 8; j++)
                    xa = xa + wx[i][j];

                y[i] = fun(xa, 128);
            }

            // перевод из булевского массива в число
            output = 0;
            for (int i = 0; i < 8; i++)
            {
                int a = 0;
                // сощдаем переменную-множитель
                int mnog = (int)Math.Pow(2, i);

                if (y[i])
                    a = 1;
                else
                    a = 0;

                output = Convert.ToByte(output + (a * mnog));
            }
        }

        // тут идет смена весов нужных нейронов

        private void ChengW(byte sm1, byte sm2)
        {
            // создаем массивы для битовых значений
            bool[] bitsm1 = new bool[8];
            bool[] bitsm2 = new bool[8];

            // заполняем битовые массивы
            for (int i = 0; i < 8; i++)
            {
                // сощдаем переменную-множитель
                int mnog = (int)Math.Pow(2, i);
                // выдиляем нужный бит из первого символа
                int rez = sm1 & mnog;

                if (rez == 0)
                    bitsm1[i] = false;
                else
                    bitsm1[i] = true;

                // выдиляем нужный бит из второго символа
                rez = sm2 & mnog;

                if (rez == 0)
                    bitsm2[i] = false;
                else
                    bitsm2[i] = true;
            }

            // делаем проверку
            for (int q = 0; q < 8; q++)
                if (bitsm1[q] != bitsm2[q])    // какие из битов не совпадает
                {
                    //т.к. биты уже не одинаковые, то
                    // если один бит равен 1, то другой равен 0
                    // и если первый равен еденыце, то
                    //веса нейрона надо понизить, чтоб он стремился к 0

                    if (bitsm1[q])
                        for (int i = 0; i < 8; i++)
                            w[q][i]--;
                    else
                        for (int i = 0; i < 8; i++)
                            w[q][i]++;
                }
        }

        /*  private void ChengW(char sm1, char sm2)
          {
              // переводим символы в наборы битов
              BitArray bits1 = new BitArray(sm1);
              BitArray bits2 = new BitArray(sm2);

              // проходим по всем битам
              for (int q = 0; q < 8; q++)
                  if (bits1.Get(q) != bits2.Get(q))       // какие из битов не совпадает

                      //т.к. биты уже не одинаковые, то
                      // если один бит равен 1, то другой равен 0
                      // и если первый равен еденыце, то
                      //веса нейрона надо понизить, чтоб он стремился к 0

                      if (bits1.Get(q))
                          for (int i = 0; i < 16; i++)
                              w[q][i]--;

                      // иначе веса увеличивать, чтоб он стремился к 1
                      else
                          for (int i = 0; i < 16; i++)
                              w[q][i]++;
          }*/

        // функция обучения нейронной сети
        private void LearnNeiron()
        {
            // создаем случайнве веса для нейронов

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    w[i][j] = 0;

            ////FileStream fil = File.Open(openFileDialog1.FileName, FileMode.Open);
            //StreamReader fil = new StreamReader(openFileDialog1.FileName);
            byte[] newtext = File.ReadAllBytes(openFileDialog1.FileName);
            char[] chartext = System.Text.Encoding.GetEncoding(1251).GetChars(newtext);
            //char x1='1';
            //char x2='1';
            int kol = 0;

            while (kol < chartext.Length)
            {
                //x1 = chartext[kol];
                //x2 = chartext[kol + 1];

                input = newtext[kol];

                DoNeiron(input);
                if (input != newtext[kol + 1])
                    ChengW(input, newtext[kol + 1]);
                kol = kol + 2;
            }
        }

        //  реализует пороговую функцию включения
        private bool fun(double x, int a)
        {
            return a > x ? true : false;
        }

        // представить массив логических значений как строку нулей и едениц
        // младшие биты вначале
        private string BooleanToString(Boolean[] ba)
        {
            string res = "";
            for (int i = 0; i < ba.Length; i++)
            {
                if (ba[i])
                    res += "1";
                else
                    res += "0";
            }

            return res;
        }

        //Конвертирование битовых занчение в число типа byte
        private byte BitArrayToByte(BitArray ba)
        {
            byte[] b = new byte[1];
            ba.CopyTo(b, 0);
            return b[0];
        }

        public Form1()
        {
            x = new bool[8];
            w = new int[8][];
            for (int j = 0; j < 8; j++)
                w[j] = new int[8];
            wx = new int[8][];
            for (int j = 0; j < 8; j++)
                wx[j] = new int[8];
            s = new bool[8];
            y = new bool[8];

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // получим биты из сторки

            byte[] stroka = enc8.GetBytes(this.textBox1.Text);
            byte[] shstroka = new byte[stroka.Length];

            // запускаем  в работу нейронную сеть
            for (int i = 0; i < stroka.Length; i++)
            {
                input = stroka[i];
                DoNeiron(input);
                shstroka[i] = output;
            }

            this.textBox2.Text = enc8.GetString(shstroka);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                LearnNeiron();
        }

        // Сохранение значений нейронной сети
        private void button3_Click(object sender, EventArgs e)
        {
        }
    }
}