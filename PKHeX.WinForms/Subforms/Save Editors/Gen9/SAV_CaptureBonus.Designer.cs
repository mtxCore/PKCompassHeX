namespace PKHeX.WinForms
{
  partial class SAV_CaptureBonus
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
      L_CBExplain = new System.Windows.Forms.Label();
      DGV_CaptureBonus = new System.Windows.Forms.DataGridView();
      COL_Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
      COL_Count = new System.Windows.Forms.DataGridViewTextBoxColumn();
      COL_Bar = new System.Windows.Forms.DataGridViewTextBoxColumn();
      COL_Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
      L_Filter = new System.Windows.Forms.Label();
      TB_Filter = new System.Windows.Forms.TextBox();
      B_ClearFilter = new System.Windows.Forms.Button();
      L_CBStats = new System.Windows.Forms.Label();
      FLP_CBButtons = new System.Windows.Forms.FlowLayoutPanel();
      B_SetSelectedMax = new System.Windows.Forms.Button();
      B_SetSelectedZero = new System.Windows.Forms.Button();
      B_MaxAll = new System.Windows.Forms.Button();
      B_ResetAll = new System.Windows.Forms.Button();
      B_Cancel = new System.Windows.Forms.Button();
      B_Save = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)DGV_CaptureBonus).BeginInit();
      FLP_CBButtons.SuspendLayout();
      SuspendLayout();

      // L_CBExplain
      L_CBExplain.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
      L_CBExplain.BackColor = System.Drawing.SystemColors.Control;
      L_CBExplain.BorderStyle = System.Windows.Forms.BorderStyle.None;
      L_CBExplain.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      L_CBExplain.Location = new System.Drawing.Point(8, 8);
      L_CBExplain.Name = "L_CBExplain";
      L_CBExplain.Padding = new System.Windows.Forms.Padding(4);
      L_CBExplain.Size = new System.Drawing.Size(618, 48);
      L_CBExplain.TabIndex = 0;
      L_CBExplain.Text = "Each tracked species has a Capture Bonus count (0\u201325). At 25 captures, the species gains a 25% Hidden Ability chance and an extra shiny roll. Edit the Count column directly. Double-click a cell to modify.";

      // L_Filter
      L_Filter.Location = new System.Drawing.Point(8, 62);
      L_Filter.Name = "L_Filter";
      L_Filter.Size = new System.Drawing.Size(40, 22);
      L_Filter.TabIndex = 1;
      L_Filter.Text = "Filter:";
      L_Filter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

      // TB_Filter
      TB_Filter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
      TB_Filter.Location = new System.Drawing.Point(48, 61);
      TB_Filter.Name = "TB_Filter";
      TB_Filter.PlaceholderText = "Species name contains...";
      TB_Filter.Size = new System.Drawing.Size(488, 23);
      TB_Filter.TabIndex = 2;
      TB_Filter.TextChanged += TB_Filter_TextChanged;

      // B_ClearFilter
      B_ClearFilter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
      B_ClearFilter.Location = new System.Drawing.Point(542, 60);
      B_ClearFilter.Name = "B_ClearFilter";
      B_ClearFilter.Size = new System.Drawing.Size(84, 24);
      B_ClearFilter.TabIndex = 3;
      B_ClearFilter.Text = "Clear";
      B_ClearFilter.UseVisualStyleBackColor = true;
      B_ClearFilter.Click += B_ClearFilter_Click;

      // DGV_CaptureBonus
      DGV_CaptureBonus.AllowUserToAddRows = false;
      DGV_CaptureBonus.AllowUserToDeleteRows = false;
      DGV_CaptureBonus.AllowUserToResizeRows = false;
      DGV_CaptureBonus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
      DGV_CaptureBonus.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      DGV_CaptureBonus.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      DGV_CaptureBonus.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { COL_Key, COL_Count, COL_Bar, COL_Status });
      DGV_CaptureBonus.Location = new System.Drawing.Point(8, 90);
      DGV_CaptureBonus.Name = "DGV_CaptureBonus";
      DGV_CaptureBonus.RowHeadersVisible = false;
      DGV_CaptureBonus.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      DGV_CaptureBonus.Size = new System.Drawing.Size(618, 320);
      DGV_CaptureBonus.TabIndex = 4;
      DGV_CaptureBonus.CellValueChanged += DGV_CaptureBonus_CellValueChanged;
      DGV_CaptureBonus.CellValidating += DGV_CaptureBonus_CellValidating;

      // COL_Key
      COL_Key.FillWeight = 25;
      COL_Key.HeaderText = "Species";
      COL_Key.Name = "COL_Key";
      COL_Key.ReadOnly = true;

      // COL_Count
      COL_Count.FillWeight = 10;
      COL_Count.HeaderText = "Count";
      COL_Count.Name = "COL_Count";

      // COL_Bar
      COL_Bar.FillWeight = 40;
      COL_Bar.HeaderText = "Progress (0\u201325)";
      COL_Bar.Name = "COL_Bar";
      COL_Bar.ReadOnly = true;

      // COL_Status
      COL_Status.FillWeight = 25;
      COL_Status.HeaderText = "Status";
      COL_Status.Name = "COL_Status";
      COL_Status.ReadOnly = true;

      // L_CBStats
      L_CBStats.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
      L_CBStats.Location = new System.Drawing.Point(8, 414);
      L_CBStats.Name = "L_CBStats";
      L_CBStats.Size = new System.Drawing.Size(618, 18);
      L_CBStats.TabIndex = 5;
      L_CBStats.Text = "Total blocks: 0  |  Total captures: 0  |  Maxed (25): 0";

      // FLP_CBButtons
      FLP_CBButtons.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
      FLP_CBButtons.Controls.Add(B_SetSelectedMax);
      FLP_CBButtons.Controls.Add(B_SetSelectedZero);
      FLP_CBButtons.Controls.Add(B_MaxAll);
      FLP_CBButtons.Controls.Add(B_ResetAll);
      FLP_CBButtons.Location = new System.Drawing.Point(8, 434);
      FLP_CBButtons.Name = "FLP_CBButtons";
      FLP_CBButtons.Size = new System.Drawing.Size(460, 30);
      FLP_CBButtons.TabIndex = 6;

      // B_SetSelectedMax
      B_SetSelectedMax.Size = new System.Drawing.Size(130, 26);
      B_SetSelectedMax.Name = "B_SetSelectedMax";
      B_SetSelectedMax.Text = "Set Selected (25)";
      B_SetSelectedMax.UseVisualStyleBackColor = true;
      B_SetSelectedMax.Click += B_SetSelectedMax_Click;

      // B_SetSelectedZero
      B_SetSelectedZero.Size = new System.Drawing.Size(130, 26);
      B_SetSelectedZero.Name = "B_SetSelectedZero";
      B_SetSelectedZero.Text = "Set Selected (0)";
      B_SetSelectedZero.UseVisualStyleBackColor = true;
      B_SetSelectedZero.Click += B_SetSelectedZero_Click;

      // B_MaxAll
      B_MaxAll.Size = new System.Drawing.Size(110, 26);
      B_MaxAll.Name = "B_MaxAll";
      B_MaxAll.Text = "Max All (25)";
      B_MaxAll.UseVisualStyleBackColor = true;
      B_MaxAll.Click += B_MaxAll_Click;

      // B_ResetAll
      B_ResetAll.Size = new System.Drawing.Size(110, 26);
      B_ResetAll.Name = "B_ResetAll";
      B_ResetAll.Text = "Reset All (0)";
      B_ResetAll.UseVisualStyleBackColor = true;
      B_ResetAll.Click += B_ResetAll_Click;

      // B_Cancel
      B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
      B_Cancel.Location = new System.Drawing.Point(540, 434);
      B_Cancel.Name = "B_Cancel";
      B_Cancel.Size = new System.Drawing.Size(88, 27);
      B_Cancel.TabIndex = 7;
      B_Cancel.Text = "Cancel";
      B_Cancel.UseVisualStyleBackColor = true;
      B_Cancel.Click += B_Cancel_Click;

      // B_Save
      B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
      B_Save.Location = new System.Drawing.Point(446, 434);
      B_Save.Name = "B_Save";
      B_Save.Size = new System.Drawing.Size(88, 27);
      B_Save.TabIndex = 8;
      B_Save.Text = "Save";
      B_Save.UseVisualStyleBackColor = true;
      B_Save.Click += B_Save_Click;

      // SAV_CaptureBonus
      AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      ClientSize = new System.Drawing.Size(634, 470);
      Controls.Add(L_CBExplain);
      Controls.Add(L_Filter);
      Controls.Add(TB_Filter);
      Controls.Add(B_ClearFilter);
      Controls.Add(DGV_CaptureBonus);
      Controls.Add(L_CBStats);
      Controls.Add(FLP_CBButtons);
      Controls.Add(B_Cancel);
      Controls.Add(B_Save);
      FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
      Icon = Properties.Resources.Icon;
      MinimumSize = new System.Drawing.Size(650, 500);
      Name = "SAV_CaptureBonus";
      StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      Text = "Capture Bonuses \u2014 Pok\u00e9mon Compass";
      ((System.ComponentModel.ISupportInitialize)DGV_CaptureBonus).EndInit();
      FLP_CBButtons.ResumeLayout(false);
      ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Label L_CBExplain;
    private System.Windows.Forms.DataGridView DGV_CaptureBonus;
    private System.Windows.Forms.DataGridViewTextBoxColumn COL_Key;
    private System.Windows.Forms.DataGridViewTextBoxColumn COL_Count;
    private System.Windows.Forms.DataGridViewTextBoxColumn COL_Bar;
    private System.Windows.Forms.DataGridViewTextBoxColumn COL_Status;
    private System.Windows.Forms.Label L_Filter;
    private System.Windows.Forms.TextBox TB_Filter;
    private System.Windows.Forms.Button B_ClearFilter;
    private System.Windows.Forms.Label L_CBStats;
    private System.Windows.Forms.FlowLayoutPanel FLP_CBButtons;
    private System.Windows.Forms.Button B_SetSelectedMax;
    private System.Windows.Forms.Button B_SetSelectedZero;
    private System.Windows.Forms.Button B_MaxAll;
    private System.Windows.Forms.Button B_ResetAll;
    private System.Windows.Forms.Button B_Cancel;
    private System.Windows.Forms.Button B_Save;
  }
}
