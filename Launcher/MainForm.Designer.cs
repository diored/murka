namespace DioRed.Murka.Launcher;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.serverWrapper = new DioRed.Murka.Launcher.ConsoleWrapper();
            this.botWrapper = new DioRed.Murka.Launcher.ConsoleWrapper();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.serverWrapper);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.botWrapper);
            this.splitContainer1.Size = new System.Drawing.Size(584, 411);
            this.splitContainer1.SplitterDistance = 292;
            this.splitContainer1.TabIndex = 1;
            // 
            // serverWrapper
            // 
            this.serverWrapper.AppArguments = "";
            this.serverWrapper.AppPath = "";
            this.serverWrapper.AppProcessName = null;
            this.serverWrapper.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serverWrapper.Font = new System.Drawing.Font("PT Mono", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.serverWrapper.Location = new System.Drawing.Point(0, 0);
            this.serverWrapper.Name = "serverWrapper";
            this.serverWrapper.Size = new System.Drawing.Size(292, 411);
            this.serverWrapper.TabIndex = 0;
            this.serverWrapper.Text = "Server";
            // 
            // botWrapper
            // 
            this.botWrapper.AppArguments = "";
            this.botWrapper.AppPath = "";
            this.botWrapper.AppProcessName = null;
            this.botWrapper.Dock = System.Windows.Forms.DockStyle.Fill;
            this.botWrapper.Font = new System.Drawing.Font("PT Mono", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.botWrapper.Location = new System.Drawing.Point(0, 0);
            this.botWrapper.Name = "botWrapper";
            this.botWrapper.Size = new System.Drawing.Size(288, 411);
            this.botWrapper.TabIndex = 0;
            this.botWrapper.Text = "Bot";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 411);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainForm";
            this.Text = "Murka bot Launcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion
    private SplitContainer splitContainer1;
    private ConsoleWrapper serverWrapper;
    private ConsoleWrapper botWrapper;
}
