namespace Sockets
{
    partial class frmMain
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.rtbMessages = new System.Windows.Forms.RichTextBox();
            this.userList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // rtbMessages
            // 
            this.rtbMessages.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.rtbMessages.Location = new System.Drawing.Point(0, 0);
            this.rtbMessages.Name = "rtbMessages";
            this.rtbMessages.ReadOnly = true;
            this.rtbMessages.Size = new System.Drawing.Size(389, 205);
            this.rtbMessages.TabIndex = 0;
            this.rtbMessages.Text = "";
            // 
            // userList
            // 
            this.userList.FormattingEnabled = true;
            this.userList.Location = new System.Drawing.Point(396, 7);
            this.userList.Name = "userList";
            this.userList.Size = new System.Drawing.Size(139, 186);
            this.userList.TabIndex = 1;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 205);
            this.Controls.Add(this.userList);
            this.Controls.Add(this.rtbMessages);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Сервер";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbMessages;
        private System.Windows.Forms.ListBox userList;
    }
}

