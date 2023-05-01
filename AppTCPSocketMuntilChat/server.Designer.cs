namespace AppTCPSocketMuntilChat
{
    partial class server
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSend = new System.Windows.Forms.Button();
            this.txtMessager = new System.Windows.Forms.TextBox();
            this.lsvMesseger = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(657, 444);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(105, 46);
            this.btnSend.TabIndex = 11;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click_1);
            // 
            // txtMessager
            // 
            this.txtMessager.Location = new System.Drawing.Point(12, 444);
            this.txtMessager.Multiline = true;
            this.txtMessager.Name = "txtMessager";
            this.txtMessager.Size = new System.Drawing.Size(639, 46);
            this.txtMessager.TabIndex = 10;
            // 
            // lsvMesseger
            // 
            this.lsvMesseger.HideSelection = false;
            this.lsvMesseger.Location = new System.Drawing.Point(12, 12);
            this.lsvMesseger.Name = "lsvMesseger";
            this.lsvMesseger.Size = new System.Drawing.Size(750, 426);
            this.lsvMesseger.TabIndex = 9;
            this.lsvMesseger.UseCompatibleStateImageBehavior = false;
            this.lsvMesseger.View = System.Windows.Forms.View.List;
            // 
            // server
            // 
            this.AcceptButton = this.btnSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(777, 502);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtMessager);
            this.Controls.Add(this.lsvMesseger);
            this.Name = "server";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Server";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.server_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtMessager;
        private System.Windows.Forms.ListView lsvMesseger;
    }
}

