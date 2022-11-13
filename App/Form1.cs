namespace App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Thread groupChoiceThread = new Thread(new ThreadStart(GroupChoice_Location));
            groupChoiceThread.Start();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.GroupChoice = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ChangePosition = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar1.Location = new System.Drawing.Point(1886, 0);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(16, 1033);
            this.vScrollBar1.TabIndex = 1;
            this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // GroupChoice
            // 
            this.GroupChoice.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.GroupChoice.FormattingEnabled = true;
            this.GroupChoice.Items.AddRange(new object[] {
            "525-â",
            "525-á",
            "525"});
            this.GroupChoice.Location = new System.Drawing.Point(470, 0);
            this.GroupChoice.Name = "GroupChoice";
            this.GroupChoice.Size = new System.Drawing.Size(450, 30);
            this.GroupChoice.TabIndex = 2;
            this.GroupChoice.SelectedIndexChanged += new System.EventHandler(this.GroupChoice_SelectedIndexChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.ChangePosition, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(470, 85);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.61111F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.38889F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(467, 144);
            this.tableLayoutPanel1.TabIndex = 3;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint_1);
            // 
            // ChangePosition
            // 
            this.ChangePosition.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ChangePosition.Location = new System.Drawing.Point(70, 3);
            this.ChangePosition.Name = "ChangePosition";
            this.ChangePosition.Size = new System.Drawing.Size(92, 64);
            this.ChangePosition.TabIndex = 0;
            this.ChangePosition.Text = "Change";
            this.ChangePosition.UseVisualStyleBackColor = true;
            this.ChangePosition.Click += new System.EventHandler(this.ChangePosition_Click);
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(1902, 1033);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.GroupChoice);
            this.Controls.Add(this.vScrollBar1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void ChangePosition_Click(object sender, EventArgs e)
        {
            this.tableLayoutPanel1.Controls.Add(this.ChangePosition, 1, 0);
        }

        private void GroupChoice_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void GroupChoice_Location()
        {
            GroupChoice.Size = new System.Drawing.Size(this.ClientSize.Width * 1/4, 30);
            GroupChoice.Location = new((this.ClientSize.Width - GroupChoice.Size.Width - vScrollBar1.Size.Width) / 2 , 0);
        }

    }
}