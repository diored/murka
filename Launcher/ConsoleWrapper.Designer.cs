namespace DioRed.Murka.Launcher
{
    partial class ConsoleWrapper
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.startButton = new System.Windows.Forms.ToolStripButton();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.clearButton = new System.Windows.Forms.ToolStripButton();
            this.killButton = new System.Windows.Forms.ToolStripButton();
            this.titleLabel = new System.Windows.Forms.ToolStripLabel();
            this.logList = new System.Windows.Forms.ListBox();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startButton,
            this.stopButton,
            this.clearButton,
            this.killButton,
            this.titleLabel});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(314, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // startButton
            // 
            this.startButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.startButton.Image = global::DioRed.Murka.Launcher.Properties.Resources.Run_16x;
            this.startButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(23, 22);
            this.startButton.Text = "Start";
            // 
            // stopButton
            // 
            this.stopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopButton.Enabled = false;
            this.stopButton.Image = global::DioRed.Murka.Launcher.Properties.Resources.Stop_16x;
            this.stopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(23, 22);
            this.stopButton.Text = "Stop";
            // 
            // clearButton
            // 
            this.clearButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearButton.Image = global::DioRed.Murka.Launcher.Properties.Resources.CleanData_16x;
            this.clearButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(23, 22);
            this.clearButton.Text = "Clear log";
            // 
            // killButton
            // 
            this.killButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.killButton.Image = global::DioRed.Murka.Launcher.Properties.Resources.Close_red_16x;
            this.killButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.killButton.Name = "killButton";
            this.killButton.Size = new System.Drawing.Size(23, 22);
            this.killButton.Text = "Kill";
            // 
            // titleLabel
            // 
            this.titleLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(0, 22);
            // 
            // logList
            // 
            this.logList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logList.FormattingEnabled = true;
            this.logList.HorizontalScrollbar = true;
            this.logList.ItemHeight = 15;
            this.logList.Location = new System.Drawing.Point(0, 25);
            this.logList.Name = "logList";
            this.logList.Size = new System.Drawing.Size(314, 170);
            this.logList.TabIndex = 2;
            // 
            // ConsoleWrapper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.logList);
            this.Controls.Add(this.toolStrip);
            this.Name = "ConsoleWrapper";
            this.Size = new System.Drawing.Size(314, 195);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ToolStrip toolStrip;
        private ToolStripButton startButton;
        private ToolStripButton stopButton;
        private ToolStripButton clearButton;
        private ToolStripButton killButton;
        private ListBox logList;
        private ToolStripLabel titleLabel;
    }
}
