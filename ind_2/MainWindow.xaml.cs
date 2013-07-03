using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MathParser;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.IO;
using System.IO.Log;
using System.Windows.Controls.DataVisualization.Charting;
using System.Collections.ObjectModel;
using Gif.Components;

namespace ind_4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region private vars
        string func, variable;
        string flag = "textbox";
        string a1, b1, n1;
        double a, b, S, h;
        int n;
        int curr_step;
        double prev_x, prev_y;
        double curr_x, curr_y;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FileMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HelpClick(object sender, RoutedEventArgs e)
        {

        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            string input;
            Parser p = new Parser();
            Parser pp = new Parser();
            a1 = t_a.Text;
            b1 = t_b.Text;
            n1 = t_accurate.Text;
            variable = t_variable.Text;
            func = t_function.Text;
            input = func;

            myChart.Series.Clear();

            if ((func != "") || (a1 != "") || (b1 != "") || (n1 != ""))
            {
                string sPattern = @"(^(\+|\-){0,1}\d+$)|(^(\+|\-){0,1}\d+(\.|\,){1}\d+(\*10\^{0,1}\({0,1}(\+|\-){0,1}\d*\){0,1}){0,1}$)|(^\({0,1}(\+|\-){0,1}\d+\/{1}\d+\){0,1}(\*10\^{0,1}\({0,1}(\+|\-){0,1}\d*\){0,1}){0,1}$)|(^(\+|\-){0,1}\d+(\*10\^{0,1}\({0,1}(\+|\-){0,1}\d*\){0,1}){0,1}$)|(^(10\^{0,1}\({0,1}(\+|\-){0,1}\d*\){0,1}){1}$)";
                if (Regex.IsMatch(a1, sPattern) && Regex.IsMatch(b1, sPattern) && Regex.IsMatch(n1, sPattern))
                {
                    string p1 = @"\.";
                    if (Regex.IsMatch(a1, p1))
                    {
                        a1 = Regex.Replace(a1, p1, ",");
                    }
                    if (Regex.IsMatch(b1, p1))
                    {
                        b1 = Regex.Replace(b1, p1, ",");
                    }
                    if (Regex.IsMatch(n1, p1))
                    {
                        n1 = Regex.Replace(n1, p1, ",");
                    }
                    string p2 = @"abs(.*)|acos(.*)|asin(.*)|atan(.*)|cos(.*)|cosh(.*)|floor(.*)|ln(.*)|log(.*)|sign(.*)|sin(.*)|sinh(.*)|qrt(.*)|tan(.*)|tanh(.*)";
                    if (Regex.IsMatch(variable, p2))
                    {
                        System.Windows.MessageBox.Show("Недопустимое имя переменной", "Ошибка!");
                    }
                    else
                    {
                        if (p.Evaluate(a1))
                        {
                            a = p.Result;
                        }
                        if (p.Evaluate(b1))
                        {
                            b = p.Result;
                        }
                        if (p.Evaluate(n1))
                        {
                            n = (int)p.Result;
                        }
                        h = (b - a) / n;
                        prev_x = a;
                        if (p.Evaluate(Regex.Replace(func, variable, "(" + a.ToString() + ")")))
                            prev_y = p.Result;
                        curr_x = prev_x;
                        curr_y = prev_y;
                        S = 0;
                        curr_step = 1;
                        myChart.Visibility = System.Windows.Visibility.Visible;
                        textBoxResult.Visibility = System.Windows.Visibility.Visible;
                        X.Minimum = a;
                        X.Maximum = b;
                        draw_func();
                        dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                        dispatcherTimer.Tick += new EventHandler(integral_calc);
                        dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                        dispatcherTimer.Start();

                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Данные введены некорректно (неизвестный формат)", "Ошибка!");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Введите данные", "Ошибка!");
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void m_file(object sender, RoutedEventArgs e)
        {
            flag = "file";
            get_data();

        }

        private void m_TextBox(object sender, RoutedEventArgs e)
        {
            flag = "textbox";
            get_data();
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            if (radioButton_file.IsChecked == true)
            {
                flag = "file";
            }
            if (radioButton_TextBox.IsChecked == true)
            {
                flag = "textbox";
            }
            get_data();
        }


        public void draw_func()
        {
            Parser p = new Parser();
            List<KeyValuePair<double, double>> valueList = new List<KeyValuePair<double, double>>();
            double h = (b - a) / 100;
            LineSeries s = new LineSeries();
            double x;
            for (int i = 0; i <= 100; i++)
            {
                x = a + i * h;
                p.Evaluate(Regex.Replace(func, variable, "(" + x.ToString() + ")"));
                valueList.Add(new KeyValuePair<double, double>(x, p.Result));
            }

            Style style = new Style(typeof(LineDataPoint));
            style.Setters.Add(new Setter(LineDataPoint.TemplateProperty, null));
            style.Setters.Add(new Setter(LineDataPoint.BackgroundProperty, new SolidColorBrush(Color.FromRgb(0, 0, 0))));
            s.Title = "Function";
            s.DataPointStyle = style;
            s.DependentValuePath = "Value";
            s.IndependentValuePath = "Key";
            s.ItemsSource = valueList;
            myChart.Series.Add(s);

        }

        public void integral_calc(object sender, EventArgs e)
        {
            Parser p = new Parser();
            List<KeyValuePair<double, double>> valueList = new List<KeyValuePair<double, double>>();
            AreaSeries s;

            if (curr_step <= n)
            {
                curr_x += h;
                if (p.Evaluate(Regex.Replace(func, variable, "(" + curr_x.ToString() + ")")))
                    curr_y = p.Result;
                S += (prev_y + curr_y) * h / 2;
                textBoxResult.Text = "Integral = " + S.ToString();
                valueList.Add(new KeyValuePair<double, double>(prev_x, prev_y));
                valueList.Add(new KeyValuePair<double, double>(curr_x, curr_y));
                s = new AreaSeries();
                s.Title = curr_step.ToString();
                s.DependentValuePath = "Value";
                s.IndependentValuePath = "Key";
                s.ItemsSource = valueList;
                myChart.Series.Add(s);
                prev_y = curr_y;
                prev_x = curr_x;
                curr_step++;
            }
            else
            {
                dispatcherTimer.Stop();
                MessageBox.Show(S.ToString());
            }


            ////вывод на график

            if (s_picture.IsChecked == true)
            {
                gif();
            }
                         //        System.Windows.MessageBox.Show(x.ToString(), "Ошибка!");
                         //    }
                         //    DriveInfo drv = new DriveInfo(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                         //    if (drv.AvailableFreeSpace > 1048576)
                         //    {
                         //        if (s_picture.IsChecked == true)
                         //        {
                         //            if (Charts.Series[0] == null)
                         //            {
                         //                MessageBox.Show("there is nothing to export");
                         //            }
                         //            else
                         //            {
                         //                Rect bounds = VisualTreeHelper.GetDescendantBounds(Charts);

                         //                RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, 96, 96, PixelFormats.Pbgra32);

                         //                DrawingVisual isolatedVisual = new DrawingVisual();
                         //                using (DrawingContext drawing = isolatedVisual.RenderOpen())
                         //                {
                         //                    drawing.DrawRectangle(Brushes.White, null, new Rect(new Point(), bounds.Size)); // Optional Background
                         //                    drawing.DrawRectangle(new VisualBrush(Charts), null, new Rect(new Point(), bounds.Size));
                         //                }

                         //                renderBitmap.Render(isolatedVisual);

                         //                Microsoft.Win32.SaveFileDialog uloz_obr = new Microsoft.Win32.SaveFileDialog();
                         //                uloz_obr.FileName = "picture\\Graf" + step.ToString() + ".png";
                         //                uloz_obr.DefaultExt = "png";
                         //                string obr_cesta = uloz_obr.FileName;
                         //                using (FileStream outStream = new FileStream(obr_cesta, FileMode.Create))
                         //                {
                         //                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                         //                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                         //                    encoder.Save(outStream);
                         //                }
                         //            }
                         //        }
                         //    }
                         //    else
                         //    {
                         //        l_s.Visibility = Visibility.Visible;
                         //        l_s.Content = "Сохранение изображений не возможно";
                         //    }
                         //}
        }

        public void get_data()
        {
            if (flag == "file")
            {
                try
                {
                    string filename = "";
                    OpenFileDialog openFileDialog1 = new OpenFileDialog() { Filter = "Текстовые файлы(*.txt)|*.txt" };
                    if (openFileDialog1.ShowDialog() != null)
                    {
                        filename = openFileDialog1.FileName;
                        FileStream stream = new FileStream(filename, FileMode.Open);
                        StreamReader reader = new StreamReader(stream);
                        t_a.Text = reader.ReadLine();
                        t_b.Text = reader.ReadLine();
                        t_accurate.Text = reader.ReadLine();
                        t_variable.Text = reader.ReadLine();
                        t_function.Text = reader.ReadLine();
                        t_function.ToolTip = "Уравнение, считаннное из файла";
                        stream.Close();
                        t_function.Visibility = Visibility.Visible;
                        canvas1.Visibility = Visibility.Visible;
                        start.Visibility = Visibility.Visible;
                        radioButton_file.Visibility = Visibility.Collapsed;
                        radioButton_TextBox.Visibility = Visibility.Collapsed;
                        next.Visibility = Visibility.Collapsed;
                        label_2.Visibility = Visibility.Visible;
                        s_word.Visibility = Visibility.Visible;
                        s_picture.Visibility = Visibility.Visible;
                        s_txt.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Файл не выбран", "Ошибка!");
                    }
                }
                catch
                {
                }
            }
            else
            {
                t_function.Visibility = Visibility.Visible;
                canvas1.Visibility = Visibility.Visible;
                start.Visibility = Visibility.Visible;
                radioButton_file.Visibility = Visibility.Collapsed;
                radioButton_TextBox.Visibility = Visibility.Collapsed;
                next.Visibility = Visibility.Collapsed;
                label_1.Visibility = Visibility.Visible;
                s_word.Visibility = Visibility.Visible;
                s_picture.Visibility = Visibility.Visible;
                s_txt.Visibility = Visibility.Visible;
            }
        }


        private void reset_Click(object sender, RoutedEventArgs e)
        {
            //Charts.Series.Clear();
            //dispatcherTimer.Stop();
            t_function.Visibility = Visibility.Collapsed;
            canvas1.Visibility = Visibility.Collapsed;
            start.Visibility = Visibility.Collapsed;
            radioButton_file.Visibility = Visibility.Visible;
            radioButton_TextBox.Visibility = Visibility.Visible;
            next.Visibility = Visibility.Visible;
            label_1.Visibility = Visibility.Collapsed;
            label_2.Visibility = Visibility.Collapsed;
            myChart.Visibility = Visibility.Collapsed;
            //progressBar1.Visibility = Visibility.Collapsed;
            s_word.Visibility = Visibility.Collapsed;
            s_picture.Visibility = Visibility.Collapsed;
            s_txt.Visibility = Visibility.Collapsed;
            textBoxResult.Visibility = System.Windows.Visibility.Collapsed;
            //lresult.Visibility = Visibility.Collapsed;
            t_function.Text = "";
            t_a.Text = "";
            t_b.Text = "";
            t_accurate.Text = "";
            t_variable.Text = "";
            func = "";
            variable = "";
            a1 = "";
            b1 = "";
            n1 = "";
            string path = "picture";
            if (Directory.Exists(path))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    file.Delete();
                }
            }
        }

        public void gif()
        {
            string path = "picture";
            String outputFilePath = "test1.gif";
            AnimatedGifEncoder e = new AnimatedGifEncoder();
            e.Start(outputFilePath);
            e.SetDelay(500);
            e.SetRepeat(0);
            if (Directory.Exists(path))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    e.AddFrame(System.Drawing.Image.FromFile(file.FullName));
                    //file.Delete();
                }
            }
            e.Finish();
        }
    }
}
