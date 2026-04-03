using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.CompassBlockKeys;

namespace PKHeX.WinForms;

/// <summary>
/// Flags editor for Pokémon Compass and Vanilla S/V.
/// </summary>
public partial class SAV_CompassStoryFlags : Form
{
  private readonly SaveFile Origin;
  private readonly SAV9SV SAV;
  private readonly SCBlockAccessor Blocks;
  private readonly Dictionary<CheckedListBox, List<FlagEntry>> _entriesByList = [];

  private bool _warnedEdit;

  private readonly record struct FlagEntry(uint Key, bool IsPepperTalk);

  public SAV_CompassStoryFlags(SAV9SV sav)
  {
    InitializeComponent();
    WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
    SAV = (SAV9SV)(Origin = sav).Clone();
    Blocks = SAV.Blocks;

    _entriesByList[CLB_Story] = [];
    _entriesByList[CLB_Travel] = [];
    _entriesByList[CLB_Unlocks] = [];
    _entriesByList[CLB_Compass] = [];

    BuildFlags();
  }

  private bool ConfirmEdit()
  {
    if (_warnedEdit)
      return true;

    var result = WinFormsUtil.Prompt(MessageBoxButtons.YesNo,
        "Modifications can potentially DAMAGE or CORRUPT your save.",
        "Are you sure you want to edit these flags?");

    if (result != DialogResult.Yes)
      return false;

    _warnedEdit = true;
    return true;
  }

  private readonly HashSet<uint> _seenFlagKeys = [];

  private static readonly (string Name, string Label)[] VanillaCoreFlagLabels =
  [
    ("FSYS_GAME_CLEAR", "Main Story Complete"),
    ("FSYS_YMAP_SCENARIO_GYM_CLEAR_MUSHI", "Gym Badge: Cortondo (Bug)"),
    ("FSYS_YMAP_SCENARIO_GYM_CLEAR_KUSA", "Gym Badge: Artazon (Grass)"),
    ("FSYS_YMAP_SCENARIO_GYM_CLEAR_DENKI", "Gym Badge: Levincia (Electric)"),
    ("FSYS_YMAP_SCENARIO_GYM_CLEAR_MIZU", "Gym Badge: Cascarrafa (Water)"),
    ("FSYS_YMAP_SCENARIO_GYM_CLEAR_NORMAL", "Gym Badge: Medali (Normal)"),
    ("FSYS_YMAP_SCENARIO_GYM_CLEAR_GHOST", "Gym Badge: Montenevera (Ghost)"),
    ("FSYS_YMAP_SCENARIO_GYM_CLEAR_ESPER", "Gym Badge: Alfornada (Psychic)"),
    ("FSYS_YMAP_SCENARIO_GYM_CLEAR_KOORI", "Gym Badge: Glaseado (Ice)"),
    ("FSYS_RIDE_DASH_ENABLE", "Koraidon/Miraidon Dash"),
    ("FSYS_RIDE_SWIM_ENABLE", "Koraidon/Miraidon Swim"),
    ("FSYS_RIDE_HIJUMP_ENABLE", "Koraidon/Miraidon High Jump"),
    ("FSYS_RIDE_GLIDE_ENABLE", "Koraidon/Miraidon Glide"),
    ("FSYS_RIDE_CLIMB_ENABLE", "Koraidon/Miraidon Climb"),
    ("FSYS_RIDE_FLIGHT_ENABLE", "Koraidon/Miraidon Flight"),
    ("FSYS_YMAP_SU1MAP_CHANGE", "DLC1 Map Travel Unlocked"),
    ("FSYS_YMAP_S2_MAPCHANGE_ENABLE", "DLC2 Map Travel Unlocked"),
  ];

  private static readonly (string Name, string Label)[] VanillaScenarioFlagLabels =
  [
    ("FSYS_YMAP_SCENARIO_DAN_AKU", "Team Star Dark Base"),
    ("FSYS_YMAP_SCENARIO_DAN_DOKU", "Team Star Poison Base"),
    ("FSYS_YMAP_SCENARIO_DAN_FAIRY", "Team Star Fairy Base"),
    ("FSYS_YMAP_SCENARIO_DAN_FINAL", "Team Star Final"),
    ("FSYS_YMAP_SCENARIO_DAN_FINAL_02", "Team Star Final (Phase 2)"),
    ("FSYS_YMAP_SCENARIO_DAN_FINAL_03", "Team Star Final (Phase 3)"),
    ("FSYS_YMAP_SCENARIO_DAN_HONOO", "Team Star Fire Base"),
    ("FSYS_YMAP_SCENARIO_DAN_KAKUTOU", "Team Star Fighting Base"),

    ("FSYS_YMAP_SCENARIO_GYM_DENKI", "Gym Electric"),
    ("FSYS_YMAP_SCENARIO_GYM_DENKI_02", "Gym Electric (Phase 2)"),
    ("FSYS_YMAP_SCENARIO_GYM_DENKI_03", "Gym Electric (Phase 3)"),
    ("FSYS_YMAP_SCENARIO_GYM_CLEAR_DENKI", "Gym Electric Cleared"),
    ("FSYS_YMAP_SCENARIO_GYM_ESPER", "Gym Psychic"),
    ("FSYS_YMAP_SCENARIO_GYM_ESPER_02", "Gym Psychic (Phase 2)"),
    ("FSYS_YMAP_SCENARIO_GYM_ESPER_03", "Gym Psychic (Phase 3)"),
    ("FSYS_YMAP_SCENARIO_GYM_CLEAR_ESPER", "Gym Psychic Cleared"),
    ("FSYS_YMAP_SCENARIO_GYM_FINAL", "Gym Final"),
    ("FSYS_YMAP_SCENARIO_GYM_FINAL_02", "Gym Final (Phase 2)"),
    ("FSYS_YMAP_SCENARIO_GYM_GHOST", "Gym Ghost"),
    ("FSYS_YMAP_SCENARIO_GYM_GHOST_02", "Gym Ghost (Phase 2)"),
    ("FSYS_YMAP_SCENARIO_GYM_GHOST_03", "Gym Ghost (Phase 3)"),
    ("FSYS_YMAP_SCENARIO_GYM_CLEAR_GHOST", "Gym Ghost Cleared"),
    ("FSYS_YMAP_SCENARIO_GYM_KOORI", "Gym Ice"),
    ("FSYS_YMAP_SCENARIO_GYM_KOORI_02", "Gym Ice (Phase 2)"),
    ("FSYS_YMAP_SCENARIO_GYM_KOORI_03", "Gym Ice (Phase 3)"),
    ("FSYS_YMAP_SCENARIO_GYM_CLEAR_KOORI", "Gym Ice Cleared"),
    ("FSYS_YMAP_SCENARIO_GYM_KUSA", "Gym Grass"),
    ("FSYS_YMAP_SCENARIO_GYM_KUSA_02", "Gym Grass (Phase 2)"),
    ("FSYS_YMAP_SCENARIO_GYM_KUSA_03", "Gym Grass (Phase 3)"),
    ("FSYS_YMAP_SCENARIO_GYM_CLEAR_KUSA", "Gym Grass Cleared"),
    ("FSYS_YMAP_SCENARIO_GYM_MIZU", "Gym Water"),
    ("FSYS_YMAP_SCENARIO_GYM_MIZU_02", "Gym Water (Phase 2)"),
    ("FSYS_YMAP_SCENARIO_GYM_MIZU_03", "Gym Water (Phase 3)"),
    ("FSYS_YMAP_SCENARIO_GYM_CLEAR_MIZU", "Gym Water Cleared"),
    ("FSYS_YMAP_SCENARIO_GYM_MUSI", "Gym Bug"),
    ("FSYS_YMAP_SCENARIO_GYM_MUSI_02", "Gym Bug (Phase 2)"),
    ("FSYS_YMAP_SCENARIO_GYM_MUSI_03", "Gym Bug (Phase 3)"),
    ("FSYS_YMAP_SCENARIO_GYM_CLEAR_MUSHI", "Gym Bug Cleared"),
    ("FSYS_YMAP_SCENARIO_GYM_NORMAL", "Gym Normal"),
    ("FSYS_YMAP_SCENARIO_GYM_NORMAL_02", "Gym Normal (Phase 2)"),
    ("FSYS_YMAP_SCENARIO_GYM_NORMAL_03", "Gym Normal (Phase 3)"),
    ("FSYS_YMAP_SCENARIO_GYM_CLEAR_NORMAL", "Gym Normal Cleared"),

    ("FSYS_YMAP_SCENARIO_NUSI_DRAGON", "Titan Dragon"),
    ("FSYS_YMAP_SCENARIO_NUSI_DRAGON_02", "Titan Dragon (Phase 2)"),
    ("FSYS_YMAP_SCENARIO_NUSHI_FINAL", "Titan Final"),
    ("FSYS_YMAP_SCENARIO_NUSHI_FINAL_02", "Titan Final (Phase 2)"),
    ("FSYS_YMAP_SCENARIO_NUSI_HAGANE", "Titan Steel"),
    ("FSYS_YMAP_SCENARIO_NUSI_HAGANE_02", "Titan Steel (Phase 2)"),
    ("FSYS_YMAP_SCENARIO_NUSI_HIKOU", "Titan Flying"),
    ("FSYS_YMAP_SCENARIO_NUSI_IWA", "Titan Rock"),
    ("FSYS_YMAP_SCENARIO_NUSI_IWA_02", "Titan Rock (Phase 2)"),
    ("FSYS_YMAP_SCENARIO_NUSI_JIMEN", "Titan Ground"),
    ("FSYS_YMAP_SCENARIO_NUSI_JIMEN_02", "Titan Ground (Phase 2)"),

    ("FSYS_YMAP_SCENARIO_00", "Scenario 00"),
    ("FSYS_YMAP_SCENARIO_01", "Scenario 01"),
    ("FSYS_YMAP_SCENARIO_02", "Scenario 02"),
    ("FSYS_YMAP_SCENARIO_03", "Scenario 03"),
    ("FSYS_YMAP_SCENARIO_04", "Scenario 04"),
    ("FSYS_YMAP_SCENARIO_05", "Scenario 05"),
    ("FSYS_YMAP_SCENARIO_06", "Scenario 06"),
    ("FSYS_YMAP_SCENARIO_07", "Scenario 07"),
    ("FSYS_YMAP_SCENARIO_08", "Scenario 08"),

    ("FSYS_YMAP_SCENARIO_COMMON_0060", "Scenario Common 0060"),
    ("FSYS_YMAP_SCENARIO_COMMON_0090", "Scenario Common 0090"),
    ("FSYS_YMAP_SCENARIO_COMMON_0095", "Scenario Common 0095"),
    ("FSYS_YMAP_SCENARIO_COMMON_0100", "Scenario Common 0100"),
    ("FSYS_YMAP_SCENARIO_COMMON_0130", "Scenario Common 0130"),
    ("FSYS_YMAP_SCENARIO_COMMON_0170", "Scenario Common 0170"),
    ("FSYS_YMAP_SCENARIO_COMMON_0185", "Scenario Common 0185"),
    ("FSYS_YMAP_SCENARIO_COMMON_0190", "Scenario Common 0190"),
    ("FSYS_YMAP_SCENARIO_COMMON_0210", "Scenario Common 0210"),
    ("FSYS_YMAP_SCENARIO_COMMON_0220", "Scenario Common 0220"),
    ("FSYS_YMAP_SCENARIO_COMMON_0225", "Scenario Common 0225"),
    ("FSYS_YMAP_SCENARIO_COMMON_0990", "Scenario Common 0990"),
    ("FSYS_YMAP_SCENARIO_COMMON_1010", "Scenario Common 1010"),
    ("FSYS_YMAP_SCENARIO_COMMON_2000", "Scenario Common 2000"),
    ("FSYS_YMAP_SCENARIO_COMMON_2030", "Scenario Common 2030"),
    ("FSYS_YMAP_SCENARIO_COMMON_2070", "Scenario Common 2070"),
    ("FSYS_YMAP_SCENARIO_COMMON_2080", "Scenario Common 2080"),
  ];

  private static readonly string[] VanillaCoachFlags =
  [
    "FSYS_CLUB_HUD_COACH_BOTAN",
    "FSYS_CLUB_HUD_COACH_CHAMP_HAGANE",
    "FSYS_CLUB_HUD_COACH_CHAMP_JIMEN",
    "FSYS_CLUB_HUD_COACH_CHAMP_TOP",
    "FSYS_CLUB_HUD_COACH_FRIEND",
    "FSYS_CLUB_HUD_COACH_RIVAL",
    "FSYS_CLUB_HUD_COACH_TEACHER_ART",
    "FSYS_CLUB_HUD_COACH_TEACHER_ATHLETIC",
    "FSYS_CLUB_HUD_COACH_TEACHER_BIOLOGY",
    "FSYS_CLUB_HUD_COACH_TEACHER_HEAD",
    "FSYS_CLUB_HUD_COACH_TEACHER_HEALTH",
    "FSYS_CLUB_HUD_COACH_TEACHER_HISTORY",
    "FSYS_CLUB_HUD_COACH_TEACHER_HOME",
    "FSYS_CLUB_HUD_COACH_TEACHER_LANGUAGE",
    "FSYS_CLUB_HUD_COACH_TEACHER_MATH",
  ];

  private void AddFlagEntry(CheckedListBox list, string label, bool isChecked, FlagEntry entry)
  {
    list.Items.Add(label, isChecked);
    _entriesByList[list].Add(entry);
  }

  private void TryAddBooleanFlag(CheckedListBox list, string name, string? label = null)
  {
    if (!Blocks.TryGetBlock(name, out var block))
      return;
    if (block.Type is not (SCTypeCode.Bool1 or SCTypeCode.Bool2))
      return;
    if (!_seenFlagKeys.Add(block.Key))
      return;

    AddFlagEntry(list, label ?? name, block.Type == SCTypeCode.Bool2, new(block.Key, false));
  }

  private void TryAddBooleanFlag(CheckedListBox list, uint key, string? label = null)
  {
    if (!Blocks.TryGetBlock(key, out var block))
      return;
    if (block.Type is not (SCTypeCode.Bool1 or SCTypeCode.Bool2))
      return;
    if (!_seenFlagKeys.Add(block.Key))
      return;

    AddFlagEntry(list, label ?? $"0x{key:X8}", block.Type == SCTypeCode.Bool2, new(block.Key, false));
  }

  private void BuildFlags()
  {
    CLB_Story.BeginUpdate();
    CLB_Travel.BeginUpdate();
    CLB_Unlocks.BeginUpdate();
    CLB_Compass.BeginUpdate();

    BuildStoryFlags();
    BuildTravelFlags();
    BuildUnlockFlags();
    BuildCompassFlags();

    CLB_Story.EndUpdate();
    CLB_Travel.EndUpdate();
    CLB_Unlocks.EndUpdate();
    CLB_Compass.EndUpdate();
  }

  private void BuildStoryFlags()
  {
    foreach (var (name, label) in VanillaCoreFlagLabels)
      TryAddBooleanFlag(CLB_Story, name, label);

    foreach (var (name, label) in VanillaScenarioFlagLabels)
      TryAddBooleanFlag(CLB_Story, name, label);
  }

  private void BuildTravelFlags()
  {
    for (int i = 1; i <= 35; i++)
      TryAddBooleanFlag(CLB_Travel, $"FSYS_YMAP_FLY_{i:00}", $"Fly Point {i:00}");
    for (int i = 2; i <= 35; i++)
      TryAddBooleanFlag(CLB_Travel, $"FSYS_YMAP_POKECEN_{i:00}", $"Pokecenter {i:00}");

    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_FLY_MAGATAMA", "Fly: Ruin Shrine Magatama");
    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_FLY_MOKKAN", "Fly: Ruin Shrine Mokkan");
    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_FLY_TSURUGI", "Fly: Ruin Shrine Tsurugi");
    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_FLY_UTSUWA", "Fly: Ruin Shrine Utsuwa");
    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_MAGATAMA", "Map Marker: Ruin Shrine Magatama");
    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_MOKKAN", "Map Marker: Ruin Shrine Mokkan");
    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_TSURUGI", "Map Marker: Ruin Shrine Tsurugi");
    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_UTSUWA", "Map Marker: Ruin Shrine Utsuwa");

    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_SU1MAP_CHANGE", "DLC1 Map Travel Unlocked");
    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_FLY_SU1_AREA10", "DLC1 Fly Area10");
    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_FLY_SU1_BUSSTOP", "DLC1 Fly Bus Stop");
    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_FLY_SU1_CENTER01", "DLC1 Fly Center01");
    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_FLY_SU1_PLAZA", "DLC1 Fly Plaza");
    for (int i = 1; i <= 6; i++)
      TryAddBooleanFlag(CLB_Travel, $"FSYS_YMAP_FLY_SU1_SPOT{i:00}", $"DLC1 Fly Spot {i:00}");

    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_S2_MAPCHANGE_ENABLE", "DLC2 Map Travel Unlocked");
    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_FLY_SU2_DRAGON", "DLC2 Fly Dragon");
    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_FLY_SU2_ENTRANCE", "DLC2 Fly Entrance");
    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_FLY_SU2_FAIRY", "DLC2 Fly Fairy");
    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_FLY_SU2_HAGANE", "DLC2 Fly Steel");
    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_FLY_SU2_HONOO", "DLC2 Fly Fire");
    for (int i = 1; i <= 11; i++)
      TryAddBooleanFlag(CLB_Travel, $"FSYS_YMAP_FLY_SU2_SPOT{i:00}", $"DLC2 Fly Spot {i:00}");
    TryAddBooleanFlag(CLB_Travel, "FSYS_YMAP_POKECEN_SU02", "DLC2 Pokecenter");
  }

  private void BuildUnlockFlags()
  {
    foreach (var name in VanillaCoachFlags)
      TryAddBooleanFlag(CLB_Unlocks, name);

    for (int i = 1; i <= 3; i++)
      TryAddBooleanFlag(CLB_Unlocks, $"FSYS_CLUB_ROOM_BALL_THROW_FORM_0{i}", $"Throw Style {i:00} Unlocked");

    for (int i = 1; i <= 229; i++)
      TryAddBooleanFlag(CLB_Unlocks, $"FSYS_UI_WAZA_MACHINE_RELEASE_{i:000}", $"TM Recipe {i:000} Unlocked");

    for (int i = 13; i <= 37; i++)
      TryAddBooleanFlag(CLB_Unlocks, $"WEVT_S2_SUB_{i:000}_STATE", $"Snacksworth Legendary {i:000}");

    for (int i = 14; i <= 17; i++)
    {
      for (int f = 1; f <= 8; f++)
        TryAddBooleanFlag(CLB_Unlocks, $"FEVT_SUB_{i:000}_KUI_{f:00}_RELEASE", $"Ruin Stake {i:000}-{f:00}");
    }

    for (int i = 1; i <= 57; i++)
      TryAddBooleanFlag(CLB_Unlocks, $"FSYS_BGM_VS_SELECT_{i:00}", $"Union Circle BGM {i:00}");
  }

  private void BuildCompassFlags()
  {
    bool pepperTalk = false;
    if (Blocks.TryGetBlock(KPepperTalkAlready, out var pepperBlock) && pepperBlock.Type == SCTypeCode.SByte)
      pepperTalk = (sbyte)pepperBlock.GetValue() != 0;
    AddFlagEntry(CLB_Compass, "Compass_PEPPERTALKALREADY", pepperTalk, new(KPepperTalkAlready, true));

    foreach (var (key, label) in BoolFeatureLabels)
      TryAddBooleanFlag(CLB_Compass, key, label);
  }

  private void B_Save_Click(object? sender, EventArgs e)
  {
    if (!ConfirmEdit())
      return;

    foreach (var (list, entries) in _entriesByList)
    {
      for (int i = 0; i < entries.Count; i++)
      {
        var entry = entries[i];
        bool checkedState = list.GetItemChecked(i);

        if (entry.IsPepperTalk)
        {
          if (Blocks.TryGetBlock(entry.Key, out var pepperBlock) && pepperBlock.Type == SCTypeCode.SByte)
            pepperBlock.SetValue((sbyte)(checkedState ? 1 : 0));
          continue;
        }

        if (Blocks.TryGetBlock(entry.Key, out var block))
          block.ChangeBooleanType(checkedState ? SCTypeCode.Bool2 : SCTypeCode.Bool1);
      }
    }

    Origin.CopyChangesFrom(SAV);
    Close();
  }

  private void B_Cancel_Click(object? sender, EventArgs e) => Close();
}
