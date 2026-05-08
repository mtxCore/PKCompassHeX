namespace PKHeX.WinForms
{
    partial class SAV_CompassEditor
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
            P_Scroll = new System.Windows.Forms.Panel();
            TLP_Settings = new System.Windows.Forms.TableLayoutPanel();
            B_ShowRaw = new System.Windows.Forms.Button();
            GB_Raw = new System.Windows.Forms.GroupBox();
            DGV_Raw = new System.Windows.Forms.DataGridView();
            COL_RKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            COL_RType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            COL_RValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            COL_RLabel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            B_Cancel = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)DGV_Raw).BeginInit();
            GB_Raw.SuspendLayout();
            SuspendLayout();

            // P_Scroll - scrollable container for all settings controls
            P_Scroll.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            P_Scroll.AutoScroll = true;
            P_Scroll.Controls.Add(TLP_Settings);
            P_Scroll.Location = new System.Drawing.Point(8, 8);
            P_Scroll.Name = "P_Scroll";
            P_Scroll.Size = new System.Drawing.Size(618, 400);
            P_Scroll.TabIndex = 0;

            // TLP_Settings - two-column table: Label | Control
            TLP_Settings.AutoSize = true;
            TLP_Settings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            TLP_Settings.ColumnCount = 2;
            TLP_Settings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 220));
            TLP_Settings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100));
            TLP_Settings.Dock = System.Windows.Forms.DockStyle.Top;
            TLP_Settings.Location = new System.Drawing.Point(0, 0);
            TLP_Settings.Name = "TLP_Settings";
            TLP_Settings.Padding = new System.Windows.Forms.Padding(4);
            TLP_Settings.TabIndex = 0;

            // B_ShowRaw
            B_ShowRaw.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_ShowRaw.Location = new System.Drawing.Point(8, 412);
            B_ShowRaw.Name = "B_ShowRaw";
            B_ShowRaw.Size = new System.Drawing.Size(150, 27);
            B_ShowRaw.TabIndex = 1;
            B_ShowRaw.Text = "Show Raw Data\u2026";
            B_ShowRaw.UseVisualStyleBackColor = true;
            B_ShowRaw.Click += B_ShowRaw_Click;

            // GB_Raw
            GB_Raw.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            GB_Raw.Controls.Add(DGV_Raw);
            GB_Raw.Location = new System.Drawing.Point(8, 442);
            GB_Raw.Name = "GB_Raw";
            GB_Raw.Size = new System.Drawing.Size(618, 200);
            GB_Raw.TabIndex = 2;
            GB_Raw.TabStop = false;
            GB_Raw.Text = "Raw Block Data (Dangerous)";
            GB_Raw.Visible = false;

            // DGV_Raw
            DGV_Raw.AllowUserToAddRows = false;
            DGV_Raw.AllowUserToDeleteRows = false;
            DGV_Raw.AllowUserToResizeRows = false;
            DGV_Raw.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            DGV_Raw.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            DGV_Raw.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_Raw.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { COL_RKey, COL_RType, COL_RValue, COL_RLabel });
            DGV_Raw.Location = new System.Drawing.Point(6, 22);
            DGV_Raw.Name = "DGV_Raw";
            DGV_Raw.RowHeadersVisible = false;
            DGV_Raw.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            DGV_Raw.Size = new System.Drawing.Size(606, 172);
            DGV_Raw.TabIndex = 0;
            DGV_Raw.CellValueChanged += DGV_Raw_CellValueChanged;
            DGV_Raw.CellValidating += DGV_Raw_CellValidating;

            // COL_RKey
            COL_RKey.FillWeight = 20;
            COL_RKey.HeaderText = "Block Key";
            COL_RKey.Name = "COL_RKey";
            COL_RKey.ReadOnly = true;

            // COL_RType
            COL_RType.FillWeight = 10;
            COL_RType.HeaderText = "Type";
            COL_RType.Name = "COL_RType";
            COL_RType.ReadOnly = true;

            // COL_RValue
            COL_RValue.FillWeight = 35;
            COL_RValue.HeaderText = "Value";
            COL_RValue.Name = "COL_RValue";

            // COL_RLabel
            COL_RLabel.FillWeight = 35;
            COL_RLabel.HeaderText = "Description";
            COL_RLabel.Name = "COL_RLabel";
            COL_RLabel.ReadOnly = true;

            // B_Cancel
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(540, 412);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(88, 27);
            B_Cancel.TabIndex = 3;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;

            // B_Save
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(446, 412);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(88, 27);
            B_Save.TabIndex = 4;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;

            // SAV_CompassEditor
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(634, 448);
            Controls.Add(P_Scroll);
            Controls.Add(B_ShowRaw);
            Controls.Add(GB_Raw);
            Controls.Add(B_Cancel);
            Controls.Add(B_Save);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            Icon = Properties.Resources.Icon;
            MinimumSize = new System.Drawing.Size(650, 480);
            Name = "SAV_CompassEditor";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Compass Toolkit - Pokemon Compass";
            ((System.ComponentModel.ISupportInitialize)DGV_Raw).EndInit();
            GB_Raw.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel P_Scroll;
        private System.Windows.Forms.TableLayoutPanel TLP_Settings;
        private System.Windows.Forms.Button B_ShowRaw;
        private System.Windows.Forms.GroupBox GB_Raw;
        private System.Windows.Forms.DataGridView DGV_Raw;
        private System.Windows.Forms.DataGridViewTextBoxColumn COL_RKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn COL_RType;
        private System.Windows.Forms.DataGridViewTextBoxColumn COL_RValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn COL_RLabel;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
    }
}
