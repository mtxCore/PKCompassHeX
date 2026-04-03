using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.CompassBlockKeys;

namespace PKHeX.WinForms;

/// <summary>
/// Game Settings editor for Pokémon Compass saves.
/// </summary>
public partial class SAV_CompassEditor : Form
{
    private readonly SaveFile Origin;
    private readonly SAV9SV SAV;
    private readonly SCBlockAccessor Blocks;
    private readonly ConfigSave9 Config;
    private readonly ConfigCamera9 Camera;

    private bool _warnedEdit;
    private bool _loading;

    public SAV_CompassEditor(SAV9SV sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV9SV)(Origin = sav).Clone();
        Blocks = SAV.Blocks;
        Config = SAV.Config;
        Camera = SAV.Blocks.ConfigCamera;

        _loading = true;
        BuildVanillaSettings();
        BuildCompassSettings();
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

    private CheckBox AddCheckRow(string label, bool isChecked)
    {
        var lbl = new Label { Text = label, AutoSize = false, Height = 24, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        var chk = new CheckBox { Checked = isChecked, Dock = DockStyle.Left, AutoSize = true };
        TLP_Settings.Controls.Add(lbl, 0, _row);
        TLP_Settings.Controls.Add(chk, 1, _row);
        _row++;
        return chk;
    }

    private NumericUpDown AddNumericRow(string label, int value, int min, int max)
    {
        var lbl = new Label { Text = label, AutoSize = false, Height = 24, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        var nud = new NumericUpDown { Minimum = min, Maximum = max, Value = Math.Clamp(value, min, max), Width = 80, Dock = DockStyle.Left };
        TLP_Settings.Controls.Add(lbl, 0, _row);
        TLP_Settings.Controls.Add(nud, 1, _row);
        _row++;
        return nud;
    }

    // ConfigSave9 uses inverted logic: On=0, Off=1.
    private static bool IsOn(ConfigOption9 opt) => opt == ConfigOption9.On;
    private static ConfigOption9 ToOpt(bool on) => on ? ConfigOption9.On : ConfigOption9.Off;

    private ComboBox _cbTextSpeed = null!;
    private ComboBox _cbSendToBoxes = null!;
    private CheckBox _chkSkipMoveLearning = null!, _chkGiveNicknames = null!;
    private ComboBox _cbVerticalCamera = null!, _cbHorizontalCamera = null!;
    private CheckBox _chkAutoSave = null!, _chkShowNicknames = null!, _chkSkipCutscenes = null!;
    private NumericUpDown _nudBGM = null!, _nudSE = null!, _nudCry = null!;
    private CheckBox _chkRumble = null!, _chkHelp = null!;
    private ComboBox _cbCameraSupport = null!, _cbCameraInterp = null!, _cbCameraDist = null!, _cbControlsWhileFlying = null!;

    private void BuildVanillaSettings()
    {
        AddSectionHeader("Vanilla S/V Settings");

        _cbTextSpeed = AddComboRow("Text Speed", ["Slow", "Normal", "Fast"], Config.TalkingSpeed);
        _chkSkipMoveLearning = AddCheckRow("Skip Move Learning", IsOn(Config.SkipMoveLearning));
        // PromptSendToBox: Manual = ConfigOption9.On = 0, Auto = ConfigOption9.Off = 1
        _cbSendToBoxes = AddComboRow("Send to Boxes", ["Manual", "Auto"], (int)Config.PromptSendToBox);
        _chkGiveNicknames = AddCheckRow("Give Nicknames", IsOn(Config.PromptGiveNickname));
        _cbVerticalCamera = AddComboRow("Vertical Camera Controls", ["Regular", "Inverted"], (int)Config.InvertCameraVertical);
        _cbHorizontalCamera = AddComboRow("Horizontal Camera Controls", ["Regular", "Inverted"], (int)Config.InvertCameraHorizontal);
        _cbCameraSupport = AddComboRow("Camera Support", ["On", "Off"], (int)Camera.CameraSupport);
        _cbCameraInterp = AddComboRow("Camera Interpolation", ["Slow", "Normal"], (int)Camera.CameraInterpolation);
        _cbCameraDist = AddComboRow("Camera Distance", ["Close", "Normal", "Far"], (int)Camera.CameraDistance);
        _chkAutoSave = AddCheckRow("Autosave", IsOn(Config.EnableAutoSave));
        // ShowNicknames: Show = ConfigOption9.On = 0, Don't show = ConfigOption9.Off = 1
        _chkShowNicknames = AddCheckRow("Show Nicknames", IsOn(Config.ShowNicknames));
        _chkSkipCutscenes = AddCheckRow("Skip Cutscenes", IsOn(Config.SkipCutscenes));
        _nudBGM = AddNumericRow("Background Music Volume", Config.VolumeBGM, 0, 10);
        _nudSE = AddNumericRow("Sound Effects Volume", Config.VolumeSE, 0, 10);
        _nudCry = AddNumericRow("Pokémon Cries Volume", Config.VolumeCry, 0, 10);
        _chkRumble = AddCheckRow("Controller Rumble", IsOn(Config.EnableRumble));
        _chkHelp = AddCheckRow("Helping Functions", IsOn(Config.EnableHelp));
        // Controls while Flying: bit 4 of KConfigCamera (Regular = 0, Inverted = 1)
        // NOTE: bit position inferred from layout - verify against save comparison if issues arise.
        _cbControlsWhileFlying = AddComboRow("Controls while Flying", ["Regular", "Inverted"], (int)Camera.ControlsWhileFlying);

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
    private readonly List<(uint Key, NumericUpDown Nud)> _compassUInt64s = [];

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
        AddSectionHeader("Compass Settings");

        // Shiny Notification
        int shinyVal = GetSByteBlockValue(Blocks, KShinyNotification);
        _cbShinyNotif = AddComboRow("Shiny Notification",
            ["Spoiler-Free", "Full", "Off"],
            shinyVal switch { 1 => 1, 2 => 2, _ => 0 });

        AddSeparator();
        AddSectionHeader("Gameplay Modifiers");

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

        // UInt64 blocks - purpose unconfirmed; show in raw data panel only
        foreach (var (key, label) in UInt64OptionLabels)
        {
            if (!Blocks.TryGetBlock(key, out var block) || block.Type != SCTypeCode.UInt64)
                continue;
            ulong val = (ulong)block.GetValue();
            int clamped = val > int.MaxValue ? int.MaxValue : (int)val;
            var nud = AddNumericRow(label, clamped, 0, int.MaxValue);
            _compassUInt64s.Add((key, nud));
        }
    }

    private void LoadRawData()
    {
        DGV_Raw.SuspendLayout();
        DGV_Raw.Rows.Clear();

        if (Blocks.TryGetBlock(KSaveFormatVersion, out var versionBlock))
        {
            int ri = DGV_Raw.Rows.Add("0x84222645", "Int32", $"{(int)versionBlock.GetValue()}", "Save format version");
            DGV_Raw.Rows[ri].Cells[2].ReadOnly = true;
            DGV_Raw.Rows[ri].DefaultCellStyle.ForeColor = Color.Gray;
        }

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

        Config.TalkingSpeed = _cbTextSpeed.SelectedIndex;
        Config.SkipMoveLearning = ToOpt(_chkSkipMoveLearning.Checked);
        Config.PromptSendToBox = (ConfigOption9)_cbSendToBoxes.SelectedIndex;
        Config.PromptGiveNickname = ToOpt(_chkGiveNicknames.Checked);
        Config.InvertCameraVertical = (ConfigOption9)_cbVerticalCamera.SelectedIndex;
        Config.InvertCameraHorizontal = (ConfigOption9)_cbHorizontalCamera.SelectedIndex;
        Config.EnableAutoSave = ToOpt(_chkAutoSave.Checked);
        Config.ShowNicknames = ToOpt(_chkShowNicknames.Checked);
        Config.SkipCutscenes = ToOpt(_chkSkipCutscenes.Checked);
        Config.VolumeBGM = (int)_nudBGM.Value;
        Config.VolumeSE = (int)_nudSE.Value;
        Config.VolumeCry = (int)_nudCry.Value;
        Config.EnableRumble = ToOpt(_chkRumble.Checked);
        Config.EnableHelp = ToOpt(_chkHelp.Checked);

        Camera.CameraSupport = (ConfigOption9)_cbCameraSupport.SelectedIndex;
        Camera.CameraInterpolation = (CameraInterpolation9)_cbCameraInterp.SelectedIndex;
        Camera.CameraDistance = (CameraDistance9)_cbCameraDist.SelectedIndex;
        Camera.ControlsWhileFlying = (ConfigOption9)_cbControlsWhileFlying.SelectedIndex;

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

        foreach (var (key, nud) in _compassUInt64s)
        {
            if (Blocks.TryGetBlock(key, out var block) && block.Type == SCTypeCode.UInt64)
                block.SetValue((ulong)nud.Value);
        }

        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Cancel_Click(object? sender, EventArgs e) => Close();
}
