using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.CompassBlockKeys;

namespace PKHeX.WinForms;

/// <summary>
/// Compass toolkit editor for Pokemon Compass saves.
/// </summary>
public partial class SAV_CompassEditor : Form
{
    private readonly SaveFile Origin;
    private readonly SAV9SV SAV;
    private readonly SCBlockAccessor Blocks;

    private bool _warnedEdit;
    private bool _loading;

    public SAV_CompassEditor(SAV9SV sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV9SV)(Origin = sav).Clone();
        Blocks = SAV.Blocks;

        _loading = true;
        BuildOverview();
        BuildQuickActions();
        BuildCompassSettings();
        HookSummaryRefresh();
        RefreshSummary();
        LoadRawData();
        _loading = false;
    }

    private bool ConfirmEdit()
    {
        if (_warnedEdit)
            return true;

        var result = WinFormsUtil.Prompt(MessageBoxButtons.YesNo,
            "Modifications can potentially DAMAGE or CORRUPT your save.",
            "Are you sure you want to edit this value?");

        if (result != DialogResult.Yes)
            return false;

        _warnedEdit = true;
        return true;
    }

    private int _row;

    private void AddSectionHeader(string text)
    {
        var lbl = new Label
        {
            Text = text,
            Font = new Font(Font, FontStyle.Bold),
            AutoSize = false,
            Height = 28,
            TextAlign = ContentAlignment.BottomLeft,
            Dock = DockStyle.Fill,
            ForeColor = SystemColors.ControlText,
        };
        TLP_Settings.SetColumnSpan(lbl, 2);
        TLP_Settings.Controls.Add(lbl, 0, _row++);
    }

    private void AddSeparator()
    {
        var sep = new Label { Height = 6, Dock = DockStyle.Fill };
        TLP_Settings.SetColumnSpan(sep, 2);
        TLP_Settings.Controls.Add(sep, 0, _row++);
    }

    private Label AddInfoRow(string text)
    {
        var lbl = new Label
        {
            Text = text,
            AutoSize = false,
            Height = 52,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.TopLeft,
            ForeColor = SystemColors.ControlDarkDark,
            Padding = new Padding(0, 2, 0, 0),
        };
        TLP_Settings.SetColumnSpan(lbl, 2);
        TLP_Settings.Controls.Add(lbl, 0, _row++);
        return lbl;
    }

    private void AddControlRow(Control control)
    {
        TLP_Settings.SetColumnSpan(control, 2);
        TLP_Settings.Controls.Add(control, 0, _row++);
    }

    private Button CreateActionButton(string text, EventHandler onClick)
    {
        var btn = new Button
        {
            Text = text,
            Width = 150,
            Height = 28,
            Margin = new Padding(0, 0, 8, 0),
            AutoSize = false,
        };
        btn.Click += onClick;
        return btn;
    }

    private void AddActionButtonsRow(params Button[] buttons)
    {
        var flow = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Fill,
            WrapContents = true,
            Margin = new Padding(0, 2, 0, 2),
        };
        flow.Controls.AddRange(buttons);
        AddControlRow(flow);
    }

    private ComboBox AddComboRow(string label, string[] items, int selectedIndex)
    {
        var lbl = new Label { Text = label, AutoSize = false, Height = 24, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        var cb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 180, Dock = DockStyle.Left };
        cb.Items.AddRange(items);
        if (selectedIndex >= 0 && selectedIndex < items.Length)
            cb.SelectedIndex = selectedIndex;
        TLP_Settings.Controls.Add(lbl, 0, _row);
        TLP_Settings.Controls.Add(cb, 1, _row);
        _row++;
        return cb;
    }

    private void BuildOverview()
    {
        AddSectionHeader("Compass Toolkit");
        AddInfoRow("Edits in this window only affect Compass-specific modifiers. Vanilla in-game settings were intentionally removed and should be changed inside the game itself.");
        AddSeparator();
    }

    private ComboBox _cbShinyNotif = null!;
    private ComboBox _cbLevelCap = null!;
    private ComboBox _cbExpMulti = null!;
    private ComboBox _cbLetsGoEV = null!;
    private ComboBox _cbExpShare = null!;
    private ComboBox _cbPicnicExp = null!;
    private ComboBox _cbCaptureBonus = null!;
    private ComboBox _cbSpawnRate = null!;
    private ComboBox _cbAnimRate = null!;
    private ComboBox _cbPreset = null!;
    private Label _lSummary = null!;

    private enum CompassPreset
    {
        RecommendedQoL,
        FastGrind,
        ChallengeRun,
    }

    private void BuildQuickActions()
    {
        AddSectionHeader("Quick Actions");

        _cbPreset = AddComboRow("Preset", ["Recommended QoL", "Fast Grind", "Challenge Run"], 0);
        AddActionButtonsRow(
            CreateActionButton("Apply Preset", B_ApplyPreset_Click),
            CreateActionButton("Reset to Compass Defaults", B_ResetDefaults_Click),
            CreateActionButton("Open Capture Bonuses", B_OpenCaptureBonus_Click),
            CreateActionButton("Open Story Flags", B_OpenStoryFlags_Click)
        );

        _lSummary = AddInfoRow(string.Empty);
        AddSeparator();
    }

    private static int GetSByteBlockValue(SCBlockAccessor blocks, uint key, int defaultVal = 0)
    {
        if (blocks.TryGetBlock(key, out var block) && block.Type == SCTypeCode.SByte)
            return (sbyte)block.GetValue();
        return defaultVal;
    }

    private static void SetSByteBlockValue(SCBlockAccessor blocks, uint key, int value)
    {
        if (blocks.TryGetBlock(key, out var block) && block.Type == SCTypeCode.SByte)
            block.SetValue((sbyte)value);
    }

    private void BuildCompassSettings()
    {
        AddSectionHeader("Gameplay Modifiers");

        // Shiny Notification
        int shinyVal = GetSByteBlockValue(Blocks, KShinyNotification);
        _cbShinyNotif = AddComboRow("Shiny Notification",
            ["Spoiler-Free", "Full", "Off"],
            shinyVal switch { 1 => 1, 2 => 2, _ => 0 });

        // Level Cap: raw 0 = No Cap (Level 100), 1–99 = hard cap at that level
        var levelCapItems = new string[100];
        levelCapItems[0] = "No Cap (Level 100)";
        for (int i = 1; i <= 99; i++)
            levelCapItems[i] = $"Level {i}";
        int levelCapVal = Math.Clamp(GetSByteBlockValue(Blocks, KLevelcap), 0, 99);
        _cbLevelCap = AddComboRow("Level Cap", levelCapItems, levelCapVal);

        // Exp Multiplier: value 0–9 → 60%–150% (formula: value × 10 + 60%)
        string[] expMultiItems = new string[10];
        for (int i = 0; i <= 9; i++)
            expMultiItems[i] = $"{i * 10 + 60}%";
        int expMultiVal = Math.Clamp(GetSByteBlockValue(Blocks, KExpmulti), 0, 9);
        _cbExpMulti = AddComboRow("Exp. Multiplier", expMultiItems, expMultiVal);

        // Let's Go EV target: 0=Party, 1=Leader, 2=Disabled
        int letsGoEVVal = Math.Clamp(GetSByteBlockValue(Blocks, KLetsGoEV), 0, 2);
        _cbLetsGoEV = AddComboRow("Let's Go EV Target",
            ["Party", "Leader", "Disabled"],
            letsGoEVVal);

        // Exp Share: 0=On, 1=Off
        int expShareVal = Math.Clamp(GetSByteBlockValue(Blocks, KExpshare), 0, 1);
        _cbExpShare = AddComboRow("Exp. Share", ["On", "Off"], expShareVal);

        // Picnic Experience: 0=On, 1=Off
        int picnicExpVal = Math.Clamp(GetSByteBlockValue(Blocks, KPicnicExp), 0, 1);
        _cbPicnicExp = AddComboRow("Picnic Experience", ["On", "Off"], picnicExpVal);

        AddSeparator();
        AddSectionHeader("World Settings");

        // Capture Bonuses (KRNGSkew base): 0=On, 1=Off
        int captureBonusVal = Math.Clamp(GetSByteBlockValue(Blocks, KRNGSkew), 0, 1);
        _cbCaptureBonus = AddComboRow("Capture Bonuses", ["On", "Off"], captureBonusVal);

        // Max Pokémon Spawns: value 0–4 → 10–50 (value × 10 + 10)
        int spawnRateVal = Math.Clamp(GetSByteBlockValue(Blocks, KSpawnRate), 0, 4);
        _cbSpawnRate = AddComboRow("Max Pokémon Spawns",
            ["10 Pokémon", "20 Pokémon", "30 Pokémon", "40 Pokémon", "50 Pokémon"],
            spawnRateVal);

        // Animation Quality: 0=High, 1=Medium, 2=Low
        int animRateVal = Math.Clamp(GetSByteBlockValue(Blocks, KAnimRate), 0, 2);
        _cbAnimRate = AddComboRow("Animation Quality", ["High", "Medium", "Low"], animRateVal);
    }

    private void HookSummaryRefresh()
    {
        foreach (var cb in new[]
                 {
                     _cbShinyNotif, _cbLevelCap, _cbExpMulti, _cbLetsGoEV,
                     _cbExpShare, _cbPicnicExp, _cbCaptureBonus, _cbSpawnRate, _cbAnimRate,
                 })
            cb.SelectedIndexChanged += (_, _) => RefreshSummary();
    }

    private void RefreshSummary()
    {
        _lSummary.Text =
            $"Current profile:\n" +
            $"- Level Cap: {GetLevelCapLabel(_cbLevelCap.SelectedIndex)}\n" +
            $"- EXP Multiplier: {GetExpMultiLabel(_cbExpMulti.SelectedIndex)}\n" +
            $"- Exp Share / Picnic EXP: {GetExpShareLabel(_cbExpShare.SelectedIndex)} / {GetPicnicExpLabel(_cbPicnicExp.SelectedIndex)}\n" +
            $"- Capture Bonuses: {GetCaptureBonusLabel(_cbCaptureBonus.SelectedIndex)} | Spawns: {GetSpawnRateLabel(_cbSpawnRate.SelectedIndex)} | Animations: {GetAnimRateLabel(_cbAnimRate.SelectedIndex)}";
    }

    private void ApplyPreset(CompassPreset preset)
    {
        // Defaults are explicit so preset behavior remains stable across updates.
        switch (preset)
        {
            case CompassPreset.RecommendedQoL:
                _cbShinyNotif.SelectedIndex = 0;
                _cbLevelCap.SelectedIndex = 0;
                _cbExpMulti.SelectedIndex = 4; // 100%
                _cbLetsGoEV.SelectedIndex = 0; // Party
                _cbExpShare.SelectedIndex = 0; // On
                _cbPicnicExp.SelectedIndex = 0; // On
                _cbCaptureBonus.SelectedIndex = 0; // On
                _cbSpawnRate.SelectedIndex = 2; // 30 Pokemon
                _cbAnimRate.SelectedIndex = 1; // Medium
                break;
            case CompassPreset.FastGrind:
                _cbShinyNotif.SelectedIndex = 1; // Full
                _cbLevelCap.SelectedIndex = 0;
                _cbExpMulti.SelectedIndex = 9; // 150%
                _cbLetsGoEV.SelectedIndex = 0;
                _cbExpShare.SelectedIndex = 0;
                _cbPicnicExp.SelectedIndex = 0;
                _cbCaptureBonus.SelectedIndex = 0;
                _cbSpawnRate.SelectedIndex = 4; // 50 Pokemon
                _cbAnimRate.SelectedIndex = 2; // Low
                break;
            case CompassPreset.ChallengeRun:
                _cbShinyNotif.SelectedIndex = 0;
                _cbLevelCap.SelectedIndex = 55;
                _cbExpMulti.SelectedIndex = 0; // 60%
                _cbLetsGoEV.SelectedIndex = 1; // Leader
                _cbExpShare.SelectedIndex = 1; // Off
                _cbPicnicExp.SelectedIndex = 1; // Off
                _cbCaptureBonus.SelectedIndex = 1; // Off
                _cbSpawnRate.SelectedIndex = 1; // 20 Pokemon
                _cbAnimRate.SelectedIndex = 0; // High
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(preset), preset, null);
        }

        RefreshSummary();
    }

    private void SetCompassDefaults()
    {
        _cbShinyNotif.SelectedIndex = 0;
        _cbLevelCap.SelectedIndex = 0;
        _cbExpMulti.SelectedIndex = 0;
        _cbLetsGoEV.SelectedIndex = 0;
        _cbExpShare.SelectedIndex = 0;
        _cbPicnicExp.SelectedIndex = 0;
        _cbCaptureBonus.SelectedIndex = 0;
        _cbSpawnRate.SelectedIndex = 0;
        _cbAnimRate.SelectedIndex = 0;
        RefreshSummary();
    }

    private void B_ApplyPreset_Click(object? sender, EventArgs e)
    {
        if (!ConfirmEdit())
            return;

        _loading = true;
        ApplyPreset((CompassPreset)_cbPreset.SelectedIndex);
        _loading = false;
    }

    private void B_ResetDefaults_Click(object? sender, EventArgs e)
    {
        if (!ConfirmEdit())
            return;

        _loading = true;
        SetCompassDefaults();
        _loading = false;
    }

    private void B_OpenCaptureBonus_Click(object? sender, EventArgs e)
    {
        using var dlg = new SAV_CaptureBonus(SAV);
        dlg.ShowDialog(this);
    }

    private void B_OpenStoryFlags_Click(object? sender, EventArgs e)
    {
        using var dlg = new SAV_CompassStoryFlags(SAV);
        dlg.ShowDialog(this);
    }

    private void LoadRawData()
    {
        DGV_Raw.SuspendLayout();
        DGV_Raw.Rows.Clear();

        if (Blocks.TryGetBlock(KTeamSeedTable, out var seedBlock))
        {
            string hex = Convert.ToHexString(seedBlock.Data);
            int ri = DGV_Raw.Rows.Add("0xA9296A9C", "Object", hex, "Team Seed Table (257 bytes)");
            DGV_Raw.Rows[ri].Tag = KTeamSeedTable;
            DGV_Raw.Rows[ri].DefaultCellStyle.ForeColor = Color.DarkOrange;
        }
        foreach (var (key, desc) in new (uint, string)[]
        {
            (KTeamStar050A, "Team Star Rematch A? (32 bytes)"),
            (KTeamStar050B, "Team Star Rematch B? (32 bytes)"),
            (KTeamStar050C, "Team Star Rematch C? (32 bytes)"),
        })
        {
            if (!Blocks.TryGetBlock(key, out var block))
                continue;
            string hex = Convert.ToHexString(block.Data);
            int ri = DGV_Raw.Rows.Add($"0x{key:X8}", "Object", hex, desc);
            DGV_Raw.Rows[ri].Tag = key;
            DGV_Raw.Rows[ri].DefaultCellStyle.ForeColor = Color.DarkOrange;
        }

        DGV_Raw.ResumeLayout();
    }

    private void B_ShowRaw_Click(object? sender, EventArgs e)
    {
        bool show = !GB_Raw.Visible;
        GB_Raw.Visible = show;
        B_ShowRaw.Text = show ? "Hide Raw Data" : "Show Raw Data\u2026";

        if (show)
            ClientSize = new Size(ClientSize.Width, ClientSize.Height + GB_Raw.Height + 4);
        else
            ClientSize = new Size(ClientSize.Width, ClientSize.Height - GB_Raw.Height - 4);
    }

    private void DGV_Raw_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
    {
        if (_loading || e.ColumnIndex != 2) return;
        var row = DGV_Raw.Rows[e.RowIndex];
        if (row.Cells[2].ReadOnly) return;
        string type = row.Cells[1].Value?.ToString() ?? "";
        string newText = e.FormattedValue?.ToString() ?? "";

        if (type == "Object")
        {
            try { Convert.FromHexString(newText); }
            catch
            {
                e.Cancel = true;
                WinFormsUtil.Alert("Object value must be a valid hex string (e.g. 0A1B2C...).");
            }
        }
    }

    private void DGV_Raw_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (_loading || e.RowIndex < 0 || e.ColumnIndex != 2) return;
        if (!ConfirmEdit())
        {
            var row = DGV_Raw.Rows[e.RowIndex];
            if (row.Tag is uint key && Blocks.TryGetBlock(key, out var blk) && blk.Type == SCTypeCode.Object)
            {
                _loading = true;
                row.Cells[2].Value = Convert.ToHexString(blk.Data);
                _loading = false;
            }
            return;
        }

        var r = DGV_Raw.Rows[e.RowIndex];
        if (r.Tag is not uint bk) return;
        if (!Blocks.TryGetBlock(bk, out var block) || block.Type != SCTypeCode.Object) return;

        string val = r.Cells[2].Value?.ToString() ?? "";
        try
        {
            var bytes = Convert.FromHexString(val);
            if (bytes.Length == block.Data.Length)
                bytes.CopyTo(block.Data);
            else
                WinFormsUtil.Alert($"Hex length mismatch: expected {block.Data.Length * 2} hex chars, got {val.Length}.");
        }
        catch { /* validated in CellValidating */ }
    }

    private void B_Save_Click(object? sender, EventArgs e)
    {
        if (!ConfirmEdit())
            return;

        int shinyVal = _cbShinyNotif.SelectedIndex switch { 1 => 1, 2 => 2, _ => 0 };
        SetSByteBlockValue(Blocks, KShinyNotification, shinyVal);

        SetSByteBlockValue(Blocks, KLevelcap, _cbLevelCap.SelectedIndex);
        SetSByteBlockValue(Blocks, KExpmulti, _cbExpMulti.SelectedIndex);
        SetSByteBlockValue(Blocks, KLetsGoEV, _cbLetsGoEV.SelectedIndex);
        SetSByteBlockValue(Blocks, KExpshare, _cbExpShare.SelectedIndex);
        SetSByteBlockValue(Blocks, KPicnicExp, _cbPicnicExp.SelectedIndex);
        SetSByteBlockValue(Blocks, KRNGSkew, _cbCaptureBonus.SelectedIndex);
        SetSByteBlockValue(Blocks, KSpawnRate, _cbSpawnRate.SelectedIndex);
        SetSByteBlockValue(Blocks, KAnimRate, _cbAnimRate.SelectedIndex);

        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Cancel_Click(object? sender, EventArgs e) => Close();
}
