namespace PKHeX.WinForms
{
  partial class SAV_CompassStoryFlags
  {
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
        components.Dispose();
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
      TC_Flags = new System.Windows.Forms.TabControl();
      TAB_Story = new System.Windows.Forms.TabPage();
      CLB_Story = new System.Windows.Forms.CheckedListBox();
      TAB_Travel = new System.Windows.Forms.TabPage();
      CLB_Travel = new System.Windows.Forms.CheckedListBox();
      TAB_Unlocks = new System.Windows.Forms.TabPage();
      CLB_Unlocks = new System.Windows.Forms.CheckedListBox();
      TAB_Compass = new System.Windows.Forms.TabPage();
      CLB_Compass = new System.Windows.Forms.CheckedListBox();
      TC_Flags.SuspendLayout();
      TAB_Story.SuspendLayout();
      TAB_Travel.SuspendLayout();
      TAB_Unlocks.SuspendLayout();
      TAB_Compass.SuspendLayout();
      B_Cancel = new System.Windows.Forms.Button();
      B_Save = new System.Windows.Forms.Button();
      SuspendLayout();

      // TC_Flags
      TC_Flags.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
      TC_Flags.Controls.Add(TAB_Story);
      TC_Flags.Controls.Add(TAB_Travel);
      TC_Flags.Controls.Add(TAB_Unlocks);
      TC_Flags.Controls.Add(TAB_Compass);
      TC_Flags.Location = new System.Drawing.Point(8, 8);
      TC_Flags.Name = "TC_Flags";
      TC_Flags.SelectedIndex = 0;
      TC_Flags.Size = new System.Drawing.Size(720, 496);
      TC_Flags.TabIndex = 0;

      // TAB_Story
      TAB_Story.Controls.Add(CLB_Story);
      TAB_Story.Location = new System.Drawing.Point(4, 24);
      TAB_Story.Name = "TAB_Story";
      TAB_Story.Padding = new System.Windows.Forms.Padding(3);
      TAB_Story.Size = new System.Drawing.Size(712, 468);
      TAB_Story.TabIndex = 0;
      TAB_Story.Text = "Story";
      TAB_Story.UseVisualStyleBackColor = true;

      // CLB_Story
      CLB_Story.CheckOnClick = true;
      CLB_Story.Dock = System.Windows.Forms.DockStyle.Fill;
      CLB_Story.FormattingEnabled = true;
      CLB_Story.HorizontalScrollbar = true;
      CLB_Story.Location = new System.Drawing.Point(3, 3);
      CLB_Story.Name = "CLB_Story";
      CLB_Story.Size = new System.Drawing.Size(706, 462);
      CLB_Story.TabIndex = 0;

      // TAB_Travel
      TAB_Travel.Controls.Add(CLB_Travel);
      TAB_Travel.Location = new System.Drawing.Point(4, 24);
      TAB_Travel.Name = "TAB_Travel";
      TAB_Travel.Padding = new System.Windows.Forms.Padding(3);
      TAB_Travel.Size = new System.Drawing.Size(712, 468);
      TAB_Travel.TabIndex = 1;
      TAB_Travel.Text = "Travel";
      TAB_Travel.UseVisualStyleBackColor = true;

      // CLB_Travel
      CLB_Travel.CheckOnClick = true;
      CLB_Travel.Dock = System.Windows.Forms.DockStyle.Fill;
      CLB_Travel.FormattingEnabled = true;
      CLB_Travel.HorizontalScrollbar = true;
      CLB_Travel.Location = new System.Drawing.Point(3, 3);
      CLB_Travel.Name = "CLB_Travel";
      CLB_Travel.Size = new System.Drawing.Size(706, 462);
      CLB_Travel.TabIndex = 0;

      // TAB_Unlocks
      TAB_Unlocks.Controls.Add(CLB_Unlocks);
      TAB_Unlocks.Location = new System.Drawing.Point(4, 24);
      TAB_Unlocks.Name = "TAB_Unlocks";
      TAB_Unlocks.Padding = new System.Windows.Forms.Padding(3);
      TAB_Unlocks.Size = new System.Drawing.Size(712, 468);
      TAB_Unlocks.TabIndex = 2;
      TAB_Unlocks.Text = "Unlocks";
      TAB_Unlocks.UseVisualStyleBackColor = true;

      // CLB_Unlocks
      CLB_Unlocks.CheckOnClick = true;
      CLB_Unlocks.Dock = System.Windows.Forms.DockStyle.Fill;
      CLB_Unlocks.FormattingEnabled = true;
      CLB_Unlocks.HorizontalScrollbar = true;
      CLB_Unlocks.Location = new System.Drawing.Point(3, 3);
      CLB_Unlocks.Name = "CLB_Unlocks";
      CLB_Unlocks.Size = new System.Drawing.Size(706, 462);
      CLB_Unlocks.TabIndex = 0;

      // TAB_Compass
      TAB_Compass.Controls.Add(CLB_Compass);
      TAB_Compass.Location = new System.Drawing.Point(4, 24);
      TAB_Compass.Name = "TAB_Compass";
      TAB_Compass.Padding = new System.Windows.Forms.Padding(3);
      TAB_Compass.Size = new System.Drawing.Size(712, 468);
      TAB_Compass.TabIndex = 3;
      TAB_Compass.Text = "Compass";
      TAB_Compass.UseVisualStyleBackColor = true;

      // CLB_Compass
      CLB_Compass.CheckOnClick = true;
      CLB_Compass.Dock = System.Windows.Forms.DockStyle.Fill;
      CLB_Compass.FormattingEnabled = true;
      CLB_Compass.HorizontalScrollbar = true;
      CLB_Compass.Location = new System.Drawing.Point(3, 3);
      CLB_Compass.Name = "CLB_Compass";
      CLB_Compass.Size = new System.Drawing.Size(706, 462);
      CLB_Compass.TabIndex = 0;

      // B_Cancel
      B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
      B_Cancel.Location = new System.Drawing.Point(646, 510);
      B_Cancel.Name = "B_Cancel";
      B_Cancel.Size = new System.Drawing.Size(82, 27);
      B_Cancel.TabIndex = 1;
      B_Cancel.Text = "Cancel";
      B_Cancel.UseVisualStyleBackColor = true;
      B_Cancel.Click += B_Cancel_Click;

      // B_Save
      B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
      B_Save.Location = new System.Drawing.Point(558, 510);
      B_Save.Name = "B_Save";
      B_Save.Size = new System.Drawing.Size(82, 27);
      B_Save.TabIndex = 2;
      B_Save.Text = "Save";
      B_Save.UseVisualStyleBackColor = true;
      B_Save.Click += B_Save_Click;

      // SAV_CompassStoryFlags
      AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      ClientSize = new System.Drawing.Size(736, 545);
      Controls.Add(B_Save);
      Controls.Add(B_Cancel);
      Controls.Add(TC_Flags);
      FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      MaximizeBox = false;
      MinimizeBox = false;
      Name = "SAV_CompassStoryFlags";
      StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      Text = "Flags";
      TC_Flags.ResumeLayout(false);
      TAB_Story.ResumeLayout(false);
      TAB_Travel.ResumeLayout(false);
      TAB_Unlocks.ResumeLayout(false);
      TAB_Compass.ResumeLayout(false);
      ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.TabControl TC_Flags;
    private System.Windows.Forms.TabPage TAB_Story;
    private System.Windows.Forms.CheckedListBox CLB_Story;
    private System.Windows.Forms.TabPage TAB_Travel;
    private System.Windows.Forms.CheckedListBox CLB_Travel;
    private System.Windows.Forms.TabPage TAB_Unlocks;
    private System.Windows.Forms.CheckedListBox CLB_Unlocks;
    private System.Windows.Forms.TabPage TAB_Compass;
    private System.Windows.Forms.CheckedListBox CLB_Compass;
    private System.Windows.Forms.Button B_Cancel;
    private System.Windows.Forms.Button B_Save;
  }
}
