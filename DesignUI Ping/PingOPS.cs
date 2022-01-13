using DesignUI_Ping.Components;
using DesignUI_Ping.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DesignUI_Ping
{

    // Класс PingOPS основной формы приложения Ping   

    public partial class PingOPS : ShadowedForm
    {
        //Переменные

        readonly static string baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        readonly static string appStorageFolder = Path.Combine(baseFolder, "Start_EAS_Trans");

        //Конструктор формы PingOPS      

        public PingOPS()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            AutoCompleteStringCollection source = new AutoCompleteStringCollection() { };
            source.AddRange(ReadText(appStorageFolder + @"\data\path\Start_EAS_Trans\save\eas all ops.txt"));
            egoldsGoogleTextBox1.AutoCompleteCustomSource = source;
            egoldsGoogleTextBox1.AutoCompleteMode = AutoCompleteMode.Suggest;
            egoldsGoogleTextBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            Animator.Start();
        }

        // Метод для считывания текста из текстового документа по заданному пути

        public string[] ReadText(string path)
        {
            int count = File.ReadAllLines(path).Length;
            string[] array = new string[count];
            using (StreamReader fs = new StreamReader(path))
            {
                int counter = 0;
                while (true)
                {
                    counter++;
                    string temp = fs.ReadLine();
                    if (temp == null) break;
                    array[counter - 1] = temp;
                }
            }
            return array;
        }

        //Метод для запуска приложений по имени процесса приложения и пути,
        //используется для запуска приложений удаленного доступа, находящихся на моем локальном ПК

        private void StartRemoteAccess(string nameProcess, string path)
        {
            try
            {
                bool count = true;
                foreach (Process pr in Process.GetProcessesByName(nameProcess))
                {
                    count = false;
                    ShowNormal(pr);
                }
                if (count)
                    AsyncProcessStart(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex}");
            }
        }

        // Метод для запуска процесса по заданному пути

         private void AsyncProcessStart(string path)
        {
            try
            {
                Process.Start(path);               
            }
            catch (Exception)
            {
                MessageBox.Show($"Диск не найден");
            }
        }

        // Метода для запуска процесса, активирует и отображает окно,
        // если окно свернуто, Windows восстанавливает его в первоначальном размере и позиции

        public void ShowNormal(Process proc)
        {
            WinAPI.ShowWindow(proc.MainWindowHandle, WinAPI.Consts.SHOWWINDOW.SW_SHOWNORMAL);
        }

        /// <summary>
        /// Набор WinAPI функций, как есть (Или почти как есть)
        /// </summary>
        public static class WinAPI
        {

            /// <summary>
            ///  Устанавливает состояние показа определяемого окна.
            ///  Если функция завершилась успешно, возвращается значение
            ///  отличное от нуля. Если функция потерпела неудачу,
            ///  возвращаемое значение - ноль.
            /// </summary>
            /// <param name="hWnd">Дескриптор окна</param>
            /// <param name="nCmdShow">Определяет, как окно должно быть показано.</param>
            /// <returns>
            ///  Если функция завершилась успешно, возвращается значение
            ///  отличное от нуля. Если функция потерпела неудачу,
            ///  возвращаемое значение - ноль.
            ///  </returns>
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            /// <summary>
            ///  Устанавливает состояние показа определяемого окна.
            ///  Если функция завершилась успешно, возвращается значение
            ///  отличное от нуля. Если функция потерпела неудачу,
            ///  возвращаемое значение - ноль.
            /// </summary>
            /// <param name="hWnd">Дескриптор окна</param>
            /// <param name="nCmdShow">Определяет, как окно должно быть показано.</param>
            /// <returns>
            ///  Если функция завершилась успешно, возвращается значение
            ///  отличное от нуля. Если функция потерпела неудачу,
            ///  возвращаемое значение - ноль.
            ///  </returns>
            public static bool ShowWindow(IntPtr hWnd, Consts.SHOWWINDOW nCmdShow)
            {
                return ShowWindow(hWnd, (int)nCmdShow);
            }

            /// <summary>
            /// Установить окно на передний план
            /// </summary>
            /// <param name="hWnd">Handle окна</param>
            /// <returns>Удачность</returns>
            [DllImport("user32.dll")]
            public static extern bool SetForegroundWindow(IntPtr hWnd);


            /// <summary>
            /// Набор констант
            /// </summary>
            public static class Consts
            {

                /// <summary>
                /// Параметры к функции ShowWindow. 
                /// Внимание! Некоторые параметры имеют одинаковое значение
                /// (Почему? За ответом к дяде Биллу)
                /// </summary>
                public enum SHOWWINDOW : uint
                {
                    /// <summary>
                    /// Скрывает окно и активизирует другое окно
                    /// </summary>
                    SW_HIDE = 0,
                    /// <summary>
                    /// Активизирует и отображает окно.
                    /// Если окно свернуто или развернуто,
                    /// Windows восстанавливает его в 
                    /// первоначальном размере и позиции. 
                    /// Прикладная программа должна установить 
                    /// этот флажок при отображении окна впервые
                    /// </summary>
                    SW_SHOWNORMAL = 1,
                    SW_NORMAL = 1,
                    /// <summary>
                    /// Активизирует окно и отображает его как свернутое окно
                    /// </summary>
                    SW_SHOWMINIMIZED = 2,
                    /// <summary>
                    /// Активизирует окно и отображает его как развернутое окно
                    /// </summary>
                    SW_SHOWMAXIMIZED = 3,
                    /// <summary>
                    /// Развертывает определяемое окно
                    /// </summary>
                    SW_MAXIMIZE = 3,
                    /// <summary>
                    /// Отображает окно в его самом современном размере и позиции. 
                    /// Активное окно остается активным
                    /// </summary>
                    SW_SHOWNOACTIVATE = 4,
                    /// <summary>
                    /// Активизирует окно и отображает его текущие размеры и позицию
                    /// </summary>
                    SW_SHOW = 5,
                    /// <summary>
                    /// Свертывает определяемое окно и активизирует следующее окно 
                    /// верхнего уровня в Z-последовательности
                    /// </summary>
                    SW_MINIMIZE = 6,
                    /// <summary>
                    /// Отображает окно как свернутое окно. Активное окно остается активным
                    /// </summary>
                    SW_SHOWMINNOACTIVE = 7,
                    /// <summary>
                    /// Отображает окно в его текущем состоянии. Активное окно остается активным
                    /// </summary>
                    SW_SHOWNA = 8,
                    /// <summary>
                    /// Активизирует и отображает окно. 
                    /// Если окно свернуто или развернуто, 
                    /// Windows восстанавливает в его первоначальных 
                    /// размерах и позиции. Прикладная программа должна 
                    /// установить этот флажок при восстановлении свернутого окна
                    /// </summary>
                    SW_RESTORE = 9,
                    /// <summary>
                    /// Устанавливает состояние показа, основанное на флажке SW_
                    /// , определенном в структуре STARTUPINFO, 
                    /// переданной в функцию CreateProcess программой, 
                    /// которая запустила прикладную программу
                    /// </summary>
                    SW_SHOWDEFAULT = 10,
                    /// <summary>
                    /// Windows 2000/XP: Свертывает окно, даже если поток,
                    /// который владеет окном, зависает. Этот флажок должен 
                    /// быть использоваться только при свертывании окон 
                    /// другого потока
                    /// </summary>
                    SW_FORCEMINIMIZE = 11,
                    SW_MAX = 11,
                }
            }
        }

        // Событие, устанавливающее клавишу Enter, как клавишу по умолчанию для кнопки "Ping" (yt_Button1)

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) yt_Button1.PerformClick();
        }

        // Событие кнопки "Ping"

        private void yt_Button1_Click(object sender, EventArgs e)
        {
            if (egoldsGoogleTextBox1.Text.Length == 6)
            {
                Ping_Method("-n");
            }
            else
                MessageBox.Show("Некорректный ввод");
        }

        //Метод Ping, пингует по имени, проверяет на наличие неверного IP адреса.
        //Неверный IP адрес - если класс PingReply, свойство .Address возвращает IP адрес,
        //который занимает на одно место ближе к основному серверу сети, чем место,
        //которое в действительности ему принадлежит
        //(при трассировке маршрутов(IP адресов) неверный адрес может находится выше по списку к основному серверу сети,
        //в то время как реальный адрес не пингуется, ПК выключен или Интернет отключен от него
        //поэтому, для исключения таких ситуаций, этот адрес не проходит проверку и метод выводит диалоговое окно ("Пк выключен"))


        public void Ping_Method(string domain)
        {
            CheckForIllegalCrossThreadCalls = false;
            try
            {
                IPAddress ipAddress = Dns.GetHostEntry("r40-" + egoldsGoogleTextBox1.Text + domain).AddressList[0];
                Ping ping = new Ping();
                PingReply pingReply = ping.Send(ipAddress);
                if (pingReply.Address.ToString() == null)
                {
                    MessageBox.Show("Пк выключен");
                }
                else
                {
                    if (pingReply.Address.ToString() != "10.94.187.117"
                                    & pingReply.Address.ToString() != "10.94.209.149"
                                    & pingReply.Address.ToString() != "10.94.187.101"
                                    & pingReply.Address.ToString() != "10.94.185.21"
                                    & pingReply.Address.ToString() != "10.94.225.101"
                                    & pingReply.Address.ToString() != "10.94.205.197"
                                    & pingReply.Address.ToString() != "10.94.185.85"
                                    & pingReply.Address.ToString() != "10.94.206.69"
                                    & pingReply.Address.ToString() != "10.94.219.165"
                                    & pingReply.Address.ToString() != "10.94.185.117"
                                    & pingReply.Address.ToString() != "10.94.218.53"
                                    & pingReply.Address.ToString() != "10.94.207.245")
                    {
                        string result = pingReply.Address.ToString();
                        egoldsGoogleTextBox2.Text = result;
                        egoldsGoogleTextBox2.TextPreview = result;
                        egoldsGoogleTextBox2.TextInput = result;
                        egoldsGoogleTextBox3.Text = "\\\\" + result + @"\c$";
                        egoldsGoogleTextBox3.TextPreview = "\\\\" + result + @"\c$";
                        egoldsGoogleTextBox3.TextInput = "\\\\" + result + @"\c$";
                        Clipboard.SetText(egoldsGoogleTextBox2.Text);
                    }
                    else
                    {
                        MessageBox.Show("Пк выключен");
                    }
                }
            }
            catch (PingException ex)
            {
                MessageBox.Show($"\nКоманда пинг - не проходит.\n{ex.Message}");
            }
            catch (SocketException)
            {
                MessageBox.Show("\nКоманда пинг - не проходит.\nCould not resolve host name.");
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("\nКоманда пинг - не проходит.\nPlease enter the host name or IP address to ping.");
            }
            catch (System.Net.NetworkInformation.NetworkInformationException)
            {
                MessageBox.Show($"\nКоманда пинг - не проходит.\nПк ОПС {egoldsGoogleTextBox1.Text} - выключен или без интернета");
            }
            catch (NullReferenceException)
            {
                MessageBox.Show($"\nКоманда пинг - не проходит.\nПк ОПС {egoldsGoogleTextBox1.Text} - выключен или без интернета");
            }
            egoldsGoogleTextBox1.Focus();
        }

        // Событие текстового поля "IP Адрес" 

        private void egoldsGoogleTextBox2_Click(object sender, EventArgs e)
        {
            this.egoldsGoogleTextBox2.FontTextPreview = new System.Drawing.Font("Segoe UI Black", 7.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.egoldsGoogleTextBox2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        }

        // Событие текстового поля "Path to disk C"  

        private void egoldsGoogleTextBox3_Click(object sender, EventArgs e)
        {
            this.egoldsGoogleTextBox3.TextPreview = "Path to disk C";
            this.egoldsGoogleTextBox3.FontTextPreview = new System.Drawing.Font("Segoe UI Black", 7.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.egoldsGoogleTextBox3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        }

        // Событие кнопки "Window"

        private void yt_Button2_Click(object sender, EventArgs e)
        {
            if (egoldsGoogleTextBox1.Text.Length > 6)
            {
                Ping_Method("");
            }
            else
                MessageBox.Show("Некорректный ввод");
        }

        // Событие кнопки "С"

        private void yt_Button3_Click(object sender, EventArgs e)
        {
            AsyncProcessStart(@"\\" + egoldsGoogleTextBox2.Text + @"\c$");
        }

        // Событие кнопки "D"

        private void yt_Button6_Click(object sender, EventArgs e)
        {
            AsyncProcessStart(@"\\" + egoldsGoogleTextBox2.Text + @"\d$");
        }

        // Событие кнопки "E"

        private void yt_Button7_Click(object sender, EventArgs e)
        {
            AsyncProcessStart(@"\\" + egoldsGoogleTextBox2.Text + @"\e$");
        }

        // Событие кнопки "Copy" напротив поля "IP Адрес"

        private void yt_Button4_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(egoldsGoogleTextBox2.Text);
        }

        // Событие кнопки "Copy" напротив поля "Path to disk C"

        private void yt_Button5_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(egoldsGoogleTextBox3.Text);
        }

        // Событие кнопки "RDP"

        private void yt_Button8_Click(object sender, EventArgs e)
        {
            StartRemoteAccess("mstsc", @"C:\WINDOWS\system32\mstsc.exe");
        }

        // Событие кнопки "DWare"

        private void yt_Button9_Click(object sender, EventArgs e)
        {
            StartRemoteAccess("DNTU", @"D:\DWare\DNTU.exe");
        }

        // Событие кнопки "Sccm"

        private void yt_Button10_Click(object sender, EventArgs e)
        {
            StartRemoteAccess("CmRcViewer", @"D:\2020 год папка загрузки на С\RC SCCM\CmRcViewer.exe");
        }
    }
}
