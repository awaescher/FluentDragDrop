namespace FluentDragDropNuGetExample
{
    partial class TestForm
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
			this.buttonDrag = new System.Windows.Forms.Button();
			this.pnlDrop = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// buttonDrag
			// 
			this.buttonDrag.Location = new System.Drawing.Point(75, 79);
			this.buttonDrag.Name = "buttonDrag";
			this.buttonDrag.Size = new System.Drawing.Size(180, 100);
			this.buttonDrag.TabIndex = 0;
			this.buttonDrag.Text = "Drag me";
			this.buttonDrag.UseVisualStyleBackColor = true;
			this.buttonDrag.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonDrag_MouseDown);
			// 
			// pnlDrop
			// 
			this.pnlDrop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlDrop.Location = new System.Drawing.Point(530, 79);
			this.pnlDrop.Name = "pnlDrop";
			this.pnlDrop.Size = new System.Drawing.Size(180, 100);
			this.pnlDrop.TabIndex = 1;
			// 
			// TestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 251);
			this.Controls.Add(this.pnlDrop);
			this.Controls.Add(this.buttonDrag);
			this.Name = "TestForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "FluentDrag&Drop NuGet Test";
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonDrag;
        private System.Windows.Forms.Panel pnlDrop;
    }
}