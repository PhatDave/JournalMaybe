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
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        static string[] files = { "todoList.txt", "myLittleDiary.txt" };
        static List<string> diaryList = new List<string>();
        static List<string> todoList = new List<string>();

        public void ReadFiles() {
            string todoInputString = System.IO.File.ReadAllText(files[0]);
            string diaryInputString = System.IO.File.ReadAllText(files[1]);
            todoList.Clear();
            diaryList.Clear();
            foreach (string element in todoInputString.Split("\n"))
                todoList.Add(element + "\n");
            foreach (string element in diaryInputString.Split("\n"))
                diaryList.Add(element);
            todoList.Sort((x, y) => {
                int intEndX = x.IndexOf(".");
                int intEndY = y.IndexOf(".");
                if (x == "\n" || y == "\n")
                    return -1;
                if (intEndX < 0 || intEndY < 0)
                    return 1;
                if (Int32.TryParse(x.Substring(0, intEndX), out int X) && Int32.TryParse(y.Substring(0, intEndY), out int Y)) {
                    if (X > Y)
                        return 1;
                    else
                        return -1;
                } else {
                    return -1;
                }
            });
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
        }

        public void TodoAdd(string entry) {
            int intEnd = todoList[todoList.Count - 1].IndexOf(".");
            try {
                bool yes = Int32.TryParse(todoList[todoList.Count - 1].Substring(0, intEnd), out int lastEntry);
                if (!yes)
                    return;
                string addEntry = (lastEntry + 1).ToString() + ". " + entry + "\n";
                todoList.Add(addEntry);
                this.UpdateTextFields();
            } catch(ArgumentOutOfRangeException) {
                return;
            }
        }

        public void TodoRemove(int entry) {
            try {
                todoList.RemoveAt(entry);
            } catch (ArgumentOutOfRangeException) {
                this.console.Text = "";
                return;
            }

            for (int i = entry; i < todoList.Count; i++) {
                string temp = todoList[i];
                string newString = i.ToString() + ".";
                int intEnd = todoList[todoList.Count - 1].IndexOf(".");
                if (Int32.TryParse(temp.Substring(0, intEnd), out _)) {
                    newString += temp.Substring(2);
                    todoList[i] = newString;
                }
            }
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
            });
            this.UpdateTextFields();
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
        }

        protected override void WndProc(ref Message m) {
            if (m.Msg == 0x0312) {
                int id = m.WParam.ToInt32();
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
                    this.AdjustTimer();
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

        public void AdjustTimer() {
            this.timer1.Interval = ((((DateTime.Now.Minute / 10 + 1) * 10) - DateTime.Now.Minute - 1) * 60 + (60 - DateTime.Now.Second)) * 1000;
        }

        private void TimerTick(object sender, EventArgs e) {
            using System.IO.StreamWriter sw = System.IO.File.AppendText("timings.txt");
                sw.WriteLine(DateTime.Now.ToString());
            if (!this.Visible) {
                // IntPtr handle = GetForegroundWindow();
                BeepThread.BeepPlease(1200, 500);
                this.currentEntry.Text = "";
                this.console.Text = "";
                ReadFiles();
                this.Show();
                this.ActiveControl = this.currentEntry;
                // SetForegroundWindow(handle);
            }
        }

        private void SubmitEntry(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && e.Shift == false) {
                BeepThread.BeepPlease(800, 500);
                this.Hide();
                this.AdjustTimer();
                string[] output = { "[" + DateTime.Now.ToString() + "]\n" + this.currentEntry.Text + "\n\n" };
                System.IO.File.AppendAllLines(files[1], output);
                this.currentEntry.Text = "";
            }
        }

        private void ConsoleSubmit(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                string entry = this.console.Text.Substring(4);
                if (this.console.Text.Substring(0, 3).Contains("add")) {
                    TodoAdd(this.console.Text.Substring(4));
                } else if (this.console.Text.Substring(0, 3).Contains("rem")) {
                    int remEntry = 0;
                    bool yesPlease = Int32.TryParse(entry, out remEntry);
                    if (!yesPlease) {
                        this.console.Text = "";
                        return;
                    } else {
                        TodoRemove(remEntry);
                    }
                }
                this.console.Text = "";
            }
        }
    }
}
