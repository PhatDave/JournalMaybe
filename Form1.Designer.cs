
using System.Windows.Forms;

namespace JournalMaybe {
    partial class Form1 {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.lastEntry = new System.Windows.Forms.RichTextBox();
            this.currentEntry = new System.Windows.Forms.RichTextBox();
            this.console = new System.Windows.Forms.RichTextBox();
            this.todo = new System.Windows.Forms.RichTextBox();
            this.reminder = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // lastEntry
            // 
            this.lastEntry.BackColor = System.Drawing.SystemColors.Window;
            this.lastEntry.Location = new System.Drawing.Point(0, 0);
            this.lastEntry.Margin = new System.Windows.Forms.Padding(2);
            this.lastEntry.Name = "lastEntry";
            this.lastEntry.Size = new System.Drawing.Size(900, 300);
            this.lastEntry.TabIndex = 0;
            this.lastEntry.Text = "";
            this.lastEntry.Name = "old";
            this.lastEntry.Font = new System.Drawing.Font(this.lastEntry.Font.Name, this.lastEntry.Font.Size + 2.0F);
            this.lastEntry.ReadOnly = true;
            // 
            // currentEntry
            // 
            this.currentEntry.Location = new System.Drawing.Point(0, this.lastEntry.Height);
            this.currentEntry.Margin = new System.Windows.Forms.Padding(2);
            this.currentEntry.Name = "currentEntry";
            this.currentEntry.Size = new System.Drawing.Size(this.lastEntry.Width, this.lastEntry.Height);
            this.currentEntry.TabIndex = 1;
            this.currentEntry.Text = "";
            this.currentEntry.Name = "new";
            this.currentEntry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SubmitEntry);
            this.currentEntry.Font = new System.Drawing.Font(this.currentEntry.Font.Name, this.currentEntry.Font.Size + 2.0F);
            // 
            // todo
            // 
            this.todo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.todo.Location = new System.Drawing.Point(this.lastEntry.Width, 0);
            this.todo.Margin = new System.Windows.Forms.Padding(2);
            this.todo.Name = "todo";
            this.todo.Size = new System.Drawing.Size(500, this.lastEntry.Height + this.currentEntry.Height);
            this.todo.TabIndex = 3;
            this.todo.Text = "";
            this.todo.Name = "todo";
            this.todo.Font = new System.Drawing.Font(this.todo.Font.Name, this.todo.Font.Size + 2.0F);
            this.todo.ReadOnly = true;
            // 
            // console
            // 
            this.console.Location = new System.Drawing.Point(0, this.lastEntry.Height + this.currentEntry.Height);
            this.console.Margin = new System.Windows.Forms.Padding(2);
            this.console.Name = "console";
            this.console.Size = new System.Drawing.Size(this.lastEntry.Width + this.todo.Width, 30);
            this.console.TabIndex = 2;
            this.console.Text = "";
            this.console.Name = "console";
            this.console.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ConsoleSubmit);
            this.console.Font = new System.Drawing.Font(this.console.Font.Name, this.console.Font.Size + 2.0F);
            //
            // Reminder Window
            //
            this.reminder.Location = new System.Drawing.Point(0, this.lastEntry.Height + this.currentEntry.Height + this.console.Height);
            this.reminder.Size = new System.Drawing.Size(this.console.Width, 200);
            this.reminder.Font = new System.Drawing.Font(this.reminder.Font.Name, this.reminder.Font.Size + 2.0F);
            this.reminder.ReadOnly = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.todo);
            this.Controls.Add(this.console);
            this.Controls.Add(this.currentEntry);
            this.Controls.Add(this.lastEntry);
            this.Controls.Add(this.reminder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            int heightSum = this.lastEntry.Height + this.currentEntry.Height + this.console.Height + this.reminder.Height;
            int widthSum = this.lastEntry.Width + this.todo.Width;

            this.MaximumSize = new System.Drawing.Size(widthSum, heightSum);
            this.MinimumSize = new System.Drawing.Size(widthSum, heightSum);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);

            try {
                this.timer1 = new System.Windows.Forms.Timer();
                this.timer1.Tick += new System.EventHandler(this.TimerTick);
                this.timer1.Enabled = true;
                this.AdjustTimer();

                this.alarmTimer = new System.Windows.Forms.Timer();
                this.alarmTimer.Tick += new System.EventHandler(this.AlarmBeep);
                this.alarmTimer.Enabled = false;
                this.UpdateAlarm();
            } catch (System.ArgumentNullException) {
            }
        }
        protected override bool ShowWithoutActivation {
            get { return true; }
        }



        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer alarmTimer;
        private System.Windows.Forms.RichTextBox lastEntry;
        private System.Windows.Forms.RichTextBox currentEntry;
        private System.Windows.Forms.RichTextBox console;
        private System.Windows.Forms.RichTextBox todo;
        private System.Windows.Forms.RichTextBox reminder;
    }
}