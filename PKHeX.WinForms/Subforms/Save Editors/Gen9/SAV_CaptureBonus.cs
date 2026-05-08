using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.CompassBlockKeys;

namespace PKHeX.WinForms;

/// <summary>
/// Standalone Capture Bonus viewer/editor for Pokémon Compass saves.
/// </summary>
public partial class SAV_CaptureBonus : Form
{
  private readonly SaveFile Origin;
  private readonly SAV9SV SAV;
  private readonly SCBlockAccessor Blocks;

  private bool _warnedEdit;
  private bool _loading;

  public SAV_CaptureBonus(SAV9SV sav)
  {
    InitializeComponent();
    WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
    SAV = (SAV9SV)(Origin = sav).Clone();
    Blocks = SAV.Blocks;

    _loading = true;
    LoadCaptureBonus();
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

  private void LoadCaptureBonus()
  {
    DGV_CaptureBonus.SuspendLayout();
    DGV_CaptureBonus.Rows.Clear();

    int lang = (int)GameLanguage.GetLanguage(Main.CurrentLanguage);
    var rows = new List<CaptureBonusRow>();

    int totalCaptures = 0, maxedCount = 0, totalBlocks = 0;

    foreach (uint key in CaptureBonusKeys)
    {
      int count = GetCaptureBonus(Blocks, key);
      totalBlocks++;
      totalCaptures += count;
      if (count >= 25) maxedCount++;

      string bar = BuildBar(count, 25, 20);
      string status = GetCaptureStatus(count);

      ushort species = GetSpeciesForKey(key);
      string name = species != 0
          ? $"{SpeciesName.GetSpeciesNameGeneration(species, lang, 9)}"
          : $"0x{key:X8}";

      var (group, dex) = GetRegionalDexIndex(species);
      rows.Add(new CaptureBonusRow(key, species, group, dex, name, count, bar, status));
    }

    rows.Sort(static (a, b) =>
    {
      int cmpGroup = a.DexGroup.CompareTo(b.DexGroup);
      if (cmpGroup != 0)
        return cmpGroup;

      int cmpDex = a.DexIndex.CompareTo(b.DexIndex);
      if (cmpDex != 0)
        return cmpDex;

      return a.Species.CompareTo(b.Species);
    });

    foreach (var row in rows)
    {
      int rowIdx = DGV_CaptureBonus.Rows.Add(
          row.Name,
          row.Count,
          $"{row.Bar} {row.Count,2}/25",
          row.Status
      );

      DGV_CaptureBonus.Rows[rowIdx].Tag = row.Key;
    }

    DGV_CaptureBonus.ResumeLayout();
    UpdateCBStats(totalBlocks, totalCaptures, maxedCount);
  }

  private (byte DexGroup, ushort DexIndex) GetRegionalDexIndex(ushort species)
  {
    if (species == 0)
      return (byte.MaxValue, ushort.MaxValue);

    var pi = SAV.Personal.GetFormEntry(species, 0);
    if (pi is not PersonalInfo9SV p9)
      return (byte.MaxValue, ushort.MaxValue);

    if (p9.DexPaldea != 0)
      return (1, p9.DexPaldea);
    if (p9.DexKitakami != 0)
      return (2, p9.DexKitakami);
    if (p9.DexBlueberry != 0)
      return (3, p9.DexBlueberry);

    return (byte.MaxValue, species);
  }

  private readonly record struct CaptureBonusRow(
      uint Key,
      ushort Species,
      byte DexGroup,
      ushort DexIndex,
      string Name,
      int Count,
      string Bar,
      string Status);

  private void UpdateCBStats(int totalBlocks, int totalCaptures, int maxedCount)
  {
    L_CBStats.Text = $"Total blocks: {totalBlocks}  |  Total captures: {totalCaptures}  |  Maxed (25, HA+Shiny): {maxedCount}";
  }

  private void RecalcCBStats()
  {
    int total = 0, maxed = 0, blocks = DGV_CaptureBonus.Rows.Count;
    foreach (DataGridViewRow row in DGV_CaptureBonus.Rows)
    {
      if (row.Cells[1].Value is int c)
      {
        total += c;
        if (c >= 25) maxed++;
      }
    }
    UpdateCBStats(blocks, total, maxed);
  }

  private void DGV_CaptureBonus_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
  {
    if (_loading || e.ColumnIndex != 1) return;

    if (!int.TryParse(e.FormattedValue?.ToString(), out var val) || val < 0 || val > 25)
    {
      e.Cancel = true;
      WinFormsUtil.Alert("Capture bonus must be an integer between 0 and 25.");
    }
  }

  private void DGV_CaptureBonus_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
  {
    if (_loading || e.RowIndex < 0 || e.ColumnIndex != 1) return;

    if (!ConfirmEdit())
    {
      var row = DGV_CaptureBonus.Rows[e.RowIndex];
      if (row.Tag is uint key)
      {
        _loading = true;
        row.Cells[1].Value = GetCaptureBonus(Blocks, key);
        _loading = false;
      }
      return;
    }

    var r = DGV_CaptureBonus.Rows[e.RowIndex];
    if (r.Tag is not uint blockKey) return;
    if (r.Cells[1].Value is not int newCount) return;

    if (Blocks.TryGetBlock(blockKey, out var block) && block.Type == SCTypeCode.SByte)
      block.SetValue((sbyte)Math.Clamp(newCount, 0, 25));

    string bar = BuildBar(newCount, 25, 20);
    r.Cells[2].Value = $"{bar} {newCount,2}/25";
    r.Cells[3].Value = GetCaptureStatus(newCount);

    RecalcCBStats();
  }

  private void B_MaxAll_Click(object? sender, EventArgs e)
  {
    if (!ConfirmEdit()) return;
    SetAllCaptureBonus(25);
  }

  private void B_SetSelectedMax_Click(object? sender, EventArgs e)
  {
    if (!ConfirmEdit()) return;
    SetSelectedCaptureBonus(25);
  }

  private void B_SetSelectedZero_Click(object? sender, EventArgs e)
  {
    if (!ConfirmEdit()) return;
    SetSelectedCaptureBonus(0);
  }

  private void B_ResetAll_Click(object? sender, EventArgs e)
  {
    if (!ConfirmEdit()) return;
    var result = WinFormsUtil.Prompt(MessageBoxButtons.YesNo,
        "Reset ALL capture bonuses to 0?",
        "This cannot be undone (until you cancel without saving).");
    if (result != DialogResult.Yes) return;
    SetAllCaptureBonus(0);
  }

  private void SetAllCaptureBonus(int value)
  {
    _loading = true;
    foreach (DataGridViewRow row in DGV_CaptureBonus.Rows)
    {
      if (row.Tag is not uint key) continue;
      if (Blocks.TryGetBlock(key, out var block) && block.Type == SCTypeCode.SByte)
        block.SetValue((sbyte)value);

      row.Cells[1].Value = value;
      row.Cells[2].Value = $"{BuildBar(value, 25, 20)} {value,2}/25";
      row.Cells[3].Value = GetCaptureStatus(value);
    }
    _loading = false;
    RecalcCBStats();
  }

  private static string GetCaptureStatus(int count) => count switch
  {
    0 => "No captures",
    < 5 => "Starting",
    < 10 => "Building",
    < 20 => "Active",
    < 25 => "Almost maxed!",
    _ => "\u2605 MAXED (HA+Shiny)",
  };

  private static string BuildBar(int value, int max, int width)
  {
    int filled = max == 0 ? 0 : Math.Clamp(value * width / max, 0, width);
    return new string('\u2588', filled) + new string('\u2591', width - filled);
  }

  private void SetSelectedCaptureBonus(int value)
  {
    var selectedRows = DGV_CaptureBonus.SelectedRows.Cast<DataGridViewRow>().ToArray();
    if (selectedRows.Length == 0)
    {
      if (DGV_CaptureBonus.CurrentCell is null)
      {
        WinFormsUtil.Alert("Select at least one row first.");
        return;
      }

      selectedRows = [DGV_CaptureBonus.Rows[DGV_CaptureBonus.CurrentCell.RowIndex]];
    }

    _loading = true;
    foreach (var row in selectedRows)
    {
      if (row.Tag is not uint key)
        continue;
      if (Blocks.TryGetBlock(key, out var block) && block.Type == SCTypeCode.SByte)
        block.SetValue((sbyte)value);

      row.Cells[1].Value = value;
      row.Cells[2].Value = $"{BuildBar(value, 25, 20)} {value,2}/25";
      row.Cells[3].Value = GetCaptureStatus(value);
    }
    _loading = false;
    RecalcCBStats();
  }

  private void TB_Filter_TextChanged(object? sender, EventArgs e) => ApplyFilter();

  private void B_ClearFilter_Click(object? sender, EventArgs e)
  {
    TB_Filter.Text = string.Empty;
    TB_Filter.Focus();
  }

  private void ApplyFilter()
  {
    string query = TB_Filter.Text.Trim();
    foreach (DataGridViewRow row in DGV_CaptureBonus.Rows)
    {
      string name = row.Cells[0].Value?.ToString() ?? string.Empty;
      row.Visible = query.Length == 0 || name.Contains(query, StringComparison.OrdinalIgnoreCase);
    }
  }

  private void B_Save_Click(object? sender, EventArgs e)
  {
    Origin.CopyChangesFrom(SAV);
    Close();
  }

  private void B_Cancel_Click(object? sender, EventArgs e) => Close();
}
