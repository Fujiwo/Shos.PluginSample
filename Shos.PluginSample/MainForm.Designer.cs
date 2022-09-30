namespace Shos.PluginSample
{
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
        private void InitializeComponent()
        {
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.pluginsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllPluginsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.codeTextBox = new System.Windows.Forms.TextBox();
            this.messageTextBox = new System.Windows.Forms.TextBox();
            this.addPluginsButton = new System.Windows.Forms.Button();
            this.mainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.ImageScalingSize = new System.Drawing.Size(48, 48);
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pluginsMenuItem,
            this.toolsMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(3119, 56);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "mainMenu";
            // 
            // pluginsMenuItem
            // 
            this.pluginsMenuItem.Name = "pluginsMenuItem";
            this.pluginsMenuItem.Size = new System.Drawing.Size(162, 52);
            this.pluginsMenuItem.Text = "&Plugins";
            // 
            // toolsMenuItem
            // 
            this.toolsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeAllPluginsMenuItem});
            this.toolsMenuItem.Name = "toolsMenuItem";
            this.toolsMenuItem.Size = new System.Drawing.Size(129, 52);
            this.toolsMenuItem.Text = "&Tools";
            // 
            // removeAllPluginsMenuItem
            // 
            this.removeAllPluginsMenuItem.Name = "removeAllPluginsMenuItem";
            this.removeAllPluginsMenuItem.Size = new System.Drawing.Size(522, 66);
            this.removeAllPluginsMenuItem.Text = "&Remove All Plugins";
            this.removeAllPluginsMenuItem.Click += new System.EventHandler(this.removeAllPluginsMenuItem_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 56);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.codeTextBox);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.messageTextBox);
            this.splitContainer.Panel2.Controls.Add(this.addPluginsButton);
            this.splitContainer.Size = new System.Drawing.Size(3119, 1539);
            this.splitContainer.SplitterDistance = 1582;
            this.splitContainer.TabIndex = 1;
            // 
            // codeTextBox
            // 
            this.codeTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.codeTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeTextBox.Location = new System.Drawing.Point(0, 0);
            this.codeTextBox.Multiline = true;
            this.codeTextBox.Name = "codeTextBox";
            this.codeTextBox.Size = new System.Drawing.Size(1582, 1539);
            this.codeTextBox.TabIndex = 0;
            // 
            // messageTextBox
            // 
            this.messageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageTextBox.BackColor = System.Drawing.Color.White;
            this.messageTextBox.ForeColor = System.Drawing.Color.Black;
            this.messageTextBox.Location = new System.Drawing.Point(2, 78);
            this.messageTextBox.Multiline = true;
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.Size = new System.Drawing.Size(1528, 1461);
            this.messageTextBox.TabIndex = 1;
            // 
            // addPluginsButton
            // 
            this.addPluginsButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addPluginsButton.Location = new System.Drawing.Point(3, 3);
            this.addPluginsButton.Name = "addPluginsButton";
            this.addPluginsButton.Size = new System.Drawing.Size(1530, 69);
            this.addPluginsButton.TabIndex = 0;
            this.addPluginsButton.Text = "Add Plugins";
            this.addPluginsButton.UseVisualStyleBackColor = true;
            this.addPluginsButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(20F, 48F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(3119, 1595);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.Name = "MainForm";
            this.Text = "PluginSample";
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private MenuStrip mainMenu;
        private ToolStripMenuItem pluginsMenuItem;
        private ToolStripMenuItem toolsMenuItem;
        private ToolStripMenuItem removeAllPluginsMenuItem;
        private SplitContainer splitContainer;
        private TextBox codeTextBox;
        private Button addPluginsButton;
        private TextBox messageTextBox;
    }
}