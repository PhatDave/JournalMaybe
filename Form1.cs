using System;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace JournalMaybe {
    public class BeepThread {
        public static void BeepPlease(int freq, int dur) {
            Thread threadCur = new Thread(() => Console.Beep(freq, dur));
            threadCur.Start();
        }
    }

    public partial class Form1 : Form {
        private bool timerEnablePopup = true;
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        static string[] files = { "todoList.txt", "myLittleDiary.txt", "reminders.txt" };
        static List<string> diaryList = new List<string>();
        static List<string> todoList = new List<string>();
        static List<DateTime> reminderList = new List<DateTime>();
        static List<string> reminderComments = new List<string>();
        static DateTime currentAlarm;
        static bool alarmBeep = false;

        public void ReadFiles() {
            foreach (string file in files) {
                if (!System.IO.File.Exists(file))
                    System.IO.File.Create(file).Close();
            }

            string todoInputString = System.IO.File.ReadAllText(files[0]);
            string diaryInputString = System.IO.File.ReadAllText(files[1]);
            string reminderInputString = System.IO.File.ReadAllText(files[2]);
            todoList.Clear();
            diaryList.Clear();
            reminderList.Clear();
            reminderComments.Clear();
            foreach (string element in todoInputString.Split("\n"))
                todoList.Add(element + "\n");
            foreach (string element in diaryInputString.Split("\n"))
                diaryList.Add(element);
            foreach (string element in reminderInputString.Split("\n")) {
                if (!element.Equals("")) {
                    string[] chaos = element.Split(" ");
                    reminderComments.Add(chaos[4]);
                    DateTime date;
                    int month = 0;
                    switch (chaos[0]) {
                        case "Jan": month = 1; break;
                        case "Feb": month = 2; break;
                        case "Mar": month = 3; break;
                        case "Apr": month = 4; break;
                        case "May": month = 5; break;
                        case "Jun": month = 6; break;
                        case "Jul": month = 7; break;
                        case "Aug": month = 8; break;
                        case "Sep": month = 9; break;
                        case "Oct": month = 10; break;
                        case "Nov": month = 11; break;
                        case "Dec": month = 12; break;
                    }
                    string[] moreChaos = chaos[3].Split(":");
                    date = new DateTime(Int32.Parse(chaos[2]), month, Int32.Parse(chaos[1].Substring(0, chaos[1].IndexOf(","))), Int32.Parse(moreChaos[0]), Int32.Parse(moreChaos[1]), Int32.Parse(moreChaos[2]));
                    reminderList.Add(date);
                }
            }
            this.UpdateTextFields();
        }

        public void UpdateTextFields() {
            string[] temp = diaryList.ToArray();
            string update = "";
            for (int i = temp.Length - 1; i >= 0; i--) {
                if (temp[i].Contains("[") && temp[i].Contains(DateTime.Now.Year.ToString())) {
                    for (int j = i; j < temp.Length; j++) {
                        update += temp[j];
                        update += '\n';
                    }
                    break;
                }
            }
            this.lastEntry.Text = update;
            string updateText = "";
            foreach (string input in todoList) {
                if (input != "\n")
                    updateText += input;
            }
            this.todo.Text = updateText;
            System.IO.File.WriteAllText(files[0], updateText);

            SortReminders();
            string reminderText = "";
            for (int i = 0; i < reminderList.Count; i++) {
                reminderText += reminderList[i].ToString() + " " + reminderComments[i] + "\n";
            }
            System.IO.File.WriteAllText(files[2], reminderText);
            this.reminder.Text = reminderText;
        }

        public void SortReminders() {
            reminderList.Sort((x, y) => DateTime.Compare(x, y));
            try {
                while (reminderList[0].CompareTo(DateTime.Now) < 0)
                    reminderList.RemoveAt(0);
            } catch (ArgumentOutOfRangeException) { return; }
        }

        public void AlarmBeep() {
            alarmBeep = true;
            Thread alarm = new Thread(() => {
                while (alarmBeep) {
                    Console.Beep(2000, 300);
                    Thread.Sleep(150);
                }
            });
            alarm.Start();
        }

        public void FuckOffAlarm() {
            if (alarmBeep) {
                alarmBeep = false;
                currentAlarm = new DateTime();
                SortReminders();
            }
        }

        public void TodoSort() {
            while (todoList.Contains("\n"))
                todoList.Remove("\n");

            for (int i = 0; i < todoList.Count; i++) {
                string temp = todoList[i];
                string newString = (i + 1).ToString() + ".";

                int intEnd = todoList[i].IndexOf(".");
                bool yes = Int32.TryParse(todoList[i].Substring(0, intEnd), out _);
                if (yes) {
                    newString += temp.Substring(intEnd + 1);
                    todoList[i] = newString;
                }

                while (todoList[i].IndexOf(".") != todoList[i].LastIndexOf(".")) {
                    string newDotlessEntry = todoList[i].Substring(0, todoList[i].IndexOf(".")) + todoList[i].Substring(todoList[i].LastIndexOf("."));
                    todoList[i] = newDotlessEntry;
                }
            }

            UpdateTextFields();
        }

        public void TodoAdd(string entry) {
            while (todoList.Contains("\n"))
                todoList.Remove("\n");

            todoList.Add("0. " + entry + "\n");
            TodoSort();
        }

        public void TodoRemove(int entry) {
            while (todoList.Contains("\n"))
                todoList.Remove("\n");

            --entry;
            try {
                todoList.RemoveAt(entry);
            } catch (ArgumentOutOfRangeException) {
                this.console.Text = "";
                return;
            }/*
            todoList.Sort((x, y) => {
                int intEndX = x.IndexOf(".");
                int intEndY = y.IndexOf(".");
                if (x == "\n" || y == "\n")
                    return 1;
                if (intEndX < 0 || intEndY < 0)
                    return 1;
                if (Int32.TryParse(x.Substring(0, intEndX), out int X) && Int32.TryParse(y.Substring(0, intEndY), out int Y)) {
                    if (X > Y)
                        return 1;
                    else
                        return -1;
                } else {
                    return 1;
                }
            });*/
            TodoSort();
            UpdateTextFields();
        }

        public Form1() {

            RegisterHotKey(this.Handle, 1, 0x0001, (int)Keys.PageDown); // Manual Entry - Alt end
            RegisterHotKey(this.Handle, 2, 0x0004, (int)Keys.PageDown); // Manual Hide - Shift end
            RegisterHotKey(this.Handle, 3, 0x0002, (int)Keys.PageDown); // Manual Console - Ctrl end
            RegisterHotKey(this.Handle, 4, 0x0000, (int)Keys.PageUp); // Disable key for python
            RegisterHotKey(this.Handle, 4, 0x0001, (int)Keys.PageUp); // Disable key for python
            RegisterHotKey(this.Handle, 4, 0x0002, (int)Keys.PageUp); // Disable key for python
            RegisterHotKey(this.Handle, 4, 0x0004, (int)Keys.PageUp); // Disable key for python

            InitializeComponent();
            ReadFiles();
            TodoSort();
        }

        protected override void WndProc(ref Message m) {
            if (m.Msg == 0x0312) {
                int id = m.WParam.ToInt32();
                FuckOffAlarm();
                if (id == 1 && this.Visible == false) {
                    ReadFiles();
                    BeepThread.BeepPlease(1200, 500);
                    this.Show();
                    this.currentEntry.Text = "";
                    this.console.Text = "";
                    Application.OpenForms[this.Name].Activate();
                    this.ActiveControl = this.currentEntry;
                } else if (id == 2 && this.Visible == true) {
                    BeepThread.BeepPlease(800, 500);
                    this.Hide();
                } else if (id == 1 && this.Visible == true) {
                    Application.OpenForms[this.Name].Activate();
                    this.ActiveControl = this.currentEntry;
                } else if (id == 3) {
                    if (this.Visible == false) {
                        ReadFiles();
                        BeepThread.BeepPlease(1200, 500);
                        this.Show();
                        this.currentEntry.Text = "";
                        this.console.Text = "";
                        Application.OpenForms[this.Name].Activate();
                        this.ActiveControl = this.console;
                    } else {
                        Application.OpenForms[this.Name].Activate();
                        this.ActiveControl = this.console;
                    }
                }
            }

            base.WndProc(ref m);
        }

        private void TimerTick(object sender, EventArgs e) {
            DateTime now = DateTime.Now;
            if (now.Minute % 10 != 0 && !timerEnablePopup) { timerEnablePopup = true; }
            else if (now.Minute % 10 == 0 && timerEnablePopup) {
                BeepThread.BeepPlease(1200, 500);
                if (!this.Visible) {
                    this.currentEntry.Text = "";
                    this.console.Text = "";
                    ReadFiles();
                    this.Show();
                    this.ActiveControl = this.currentEntry;
                    timerEnablePopup = false;
                }
            }
            try { if (now.Date == reminderList[0].Date && now.Hour == reminderList[0].Hour && now.Minute == reminderList[0].Minute && !alarmBeep) { AlarmBeep(); }} catch (ArgumentOutOfRangeException) { return; }

        }

        private void SubmitEntry(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && e.Shift == false) {
                BeepThread.BeepPlease(800, 500);
                this.Hide();
                string[] output = { "[" + DateTime.Now.ToString() + "]\n" + this.currentEntry.Text + "\n\n" };
                System.IO.File.AppendAllLines(files[1], output);
                this.currentEntry.Text = "";
            }
        }

        private void ConsoleSubmit(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                if (this.console.Text.StartsWith("\n")) {
                    this.console.Text = this.console.Text.Split("\n")[1];
                }
                try {
                    string entry = this.console.Text.Substring(4);
                    if (this.console.Text.Substring(0, 3).Contains("add")) {
                        TodoAdd(this.console.Text.Substring(4));
                    } else if (this.console.Text.Substring(0, 3).Contains("rem")) {
                        int remEntry;
                        bool yesPlease = Int32.TryParse(entry, out remEntry);
                        if (!yesPlease) {
                            this.console.Text = "";
                            return;
                        } else {
                            TodoRemove(remEntry);
                        }
                        // TODO: How to remove reminders? - maybe do same as todo
                    } else if (this.console.Text.Substring(0, 3).Contains("rmd")) {
                        string[] dateInfo = this.console.Text.Substring(4).Split("/");
                        string comment;
                        try {
                            comment = dateInfo[dateInfo.Length - 1].Split(" ")[1];
                        } catch (IndexOutOfRangeException) {
                            comment = "";
                        }
                        dateInfo[dateInfo.Length - 1] = dateInfo[dateInfo.Length - 1].Split(" ")[0];
                        DateTime currentTime = DateTime.Now;
                        DateTime date;

                        switch (dateInfo.Length) {
                            case 2:
                                date = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, Int32.Parse(dateInfo[0]), Int32.Parse(dateInfo[1]), 0);
                                break;
                            case 3:
                                date = new DateTime(currentTime.Year, currentTime.Month, Int32.Parse(dateInfo[0]), Int32.Parse(dateInfo[1]), Int32.Parse(dateInfo[2]), 0);
                                break;
                            case 4:
                                date = new DateTime(currentTime.Year, Int32.Parse(dateInfo[0]), Int32.Parse(dateInfo[1]), Int32.Parse(dateInfo[2]), Int32.Parse(dateInfo[3]), 0);
                                break;
                            case 5:
                                date = new DateTime(Int32.Parse(dateInfo[0]), Int32.Parse(dateInfo[1]), Int32.Parse(dateInfo[2]), Int32.Parse(dateInfo[3]), Int32.Parse(dateInfo[4]), 0);
                                break;
                            default:
                                this.console.Text = "";
                                return;
                        }
                        if (date.CompareTo(currentTime) > 0) {
                            reminderList.Add(date);
                            reminderComments.Add(comment);
                            UpdateTextFields();
                        } else {
                            this.console.Text = "";
                            return;
                        }
                    }
                } catch (ArgumentOutOfRangeException) {
                    return;
                }
                this.console.Text = "";
            }
        }
    }
}
