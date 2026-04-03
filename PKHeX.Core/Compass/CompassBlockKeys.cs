using System;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Block key constants and helpers specific to Pokémon Compass v2.1.x saves.
/// These blocks are present in Compass saves but absent from vanilla SV saves.
/// </summary>
/// <remarks>
/// <para>
/// Block keys are FNV-1a-64 hashes (truncated to uint32) of internal Lua string names.
/// The hash uses PKHeX's custom offset basis <c>0xCBF29CE484222645</c>
/// (NOT the standard FNV-1a-64 basis <c>0xCBF29CE484222325</c>).
/// </para>
/// <para>
/// Capture bonus keys use <c>Compass_RNGSkew_{NationalDexNumber}</c>.
/// Compass settings use <c>Compass_{SettingName}</c>.
/// </para>
/// </remarks>
public static class CompassBlockKeys
{


  /// <summary>Int32 = 2. Compass save-format version. Primary detection key.
  /// Hash input: <c>""</c> (empty string).</summary>
  public const uint KSaveFormatVersion = 0x84222645;

  /// <summary>Object 257 bytes. Deterministic team seed table.
  /// Hash input: <c>Compass_TrainerSeed</c>.</summary>
  public const uint KTeamSeedTable = 0xA9296A9C;

  /// <summary>Object 32 bytes. ajito_aku_050 Team Star rematch data (slot A).</summary>
  public const uint KTeamStar050A = 0x08C07101;

  /// <summary>Object 32 bytes. ajito_aku_050 Team Star rematch data (slot B).</summary>
  public const uint KTeamStar050B = 0x4DBFD14C;

  /// <summary>Object 32 bytes. ajito_aku_050 Team Star rematch data (slot C).</summary>
  public const uint KTeamStar050C = 0x9B19CB47;


  // Confirmed by in-game toggling test (Compass v2.1.x).

  /// <summary>
  /// SByte. Compass-only. Shiny Notification display mode.
  /// 0 = Spoiler-Free, 1 = Full, 2 = Off.
  /// Hash input: <c>Compass_shiny_spoiler</c>.
  /// </summary>
  public const uint KShinyNotification = 0x4D38ED1E;

  /// <summary>Shiny Notification value → display label.</summary>
  public static string GetShinyNotificationLabel(int value) => value switch
  {
    0 => "Spoiler-Free",
    1 => "Full",
    2 => "Off",
    _ => $"Unknown ({value})",
  };

  /// <summary>SByte. Hash input: <c>Compass_PicnicExp</c>.</summary>
  public const uint KPicnicExp = 0x1C25C049;

  /// <summary>SByte. Hash input: <c>Compass_BattleCam</c>.</summary>
  public const uint KBattleCam = 0x34780809;

  /// <summary>SByte. Hash input: <c>Compass_LetsGoEV</c>.</summary>
  public const uint KLetsGoEV = 0x46E0EDCB;

  /// <summary>SByte. Hash input: <c>Compass_Levelcap</c>.</summary>
  public const uint KLevelcap = 0x837B300E;

  /// <summary>SByte. Hash input: <c>Compass_Expmulti</c>.</summary>
  public const uint KExpmulti = 0x8B12B6B8;

  /// <summary>SByte. Hash input: <c>Compass_ColorProfile</c>.</summary>
  public const uint KColorProfile = 0x92569278;

  /// <summary>SByte. Hash input: <c>Compass_SpawnRate</c>.</summary>
  public const uint KSpawnRate = 0x96C58F45;

  /// <summary>SByte. Hash input: <c>Compass_RNGSkew</c> (base setting, no species suffix).</summary>
  public const uint KRNGSkew = 0xB1366CBF;

  /// <summary>SByte. Hash input: <c>Compass_Expshare</c>.</summary>
  public const uint KExpshare = 0xCC806ED6;

  /// <summary>SByte. Hash input: <c>Compass_CamDither</c>.</summary>
  public const uint KCamDither = 0xD2E638A3;

  /// <summary>SByte. Hash input: <c>Compass_AnimRate</c>.</summary>
  public const uint KAnimRate = 0xFB067137;



  /// <summary>
  /// <c>KLevelcap</c> value → display label.
  /// 0 = No Cap (Level 100 allowed); 1–99 = hard cap at that level.
  /// </summary>
  public static string GetLevelCapLabel(int value) => value switch
  {
    0 => "No Cap (Level 100)",
    >= 1 and <= 99 => $"Level {value}",
    _ => $"Unknown ({value})",
  };

  /// <summary>
  /// <c>KExpmulti</c> value → display label.
  /// Formula: percentage = value × 10 + 60. Range 0–9 → 60%–150%.
  /// 150% is the confirmed maximum.
  /// </summary>
  public static string GetExpMultiLabel(int value) => value switch
  {
    >= 0 and <= 9 => $"{value * 10 + 60}%",
    _ => $"Unknown ({value})",
  };

  /// <summary>
  /// <c>KLetsGoEV</c> value → display label.
  /// 0 = Party, 1 = Leader, 2 = Disabled.
  /// </summary>
  public static string GetLetsGoEVLabel(int value) => value switch
  {
    0 => "Party",
    1 => "Leader",
    2 => "Disabled",
    _ => $"Unknown ({value})",
  };

  /// <summary>
  /// <c>KSpawnRate</c> value → display label.
  /// Formula: spawns = value × 10 + 10. Range 0–4 → 10–50 max spawns.
  /// Observed values in-game: 1 (20), 2 (30), 3 (40).
  /// </summary>
  public static string GetSpawnRateLabel(int value) => value switch
  {
    0 => "10 Pokémon",
    1 => "20 Pokémon",
    2 => "30 Pokémon",
    3 => "40 Pokémon",
    4 => "50 Pokémon",
    _ => $"Unknown ({value})",
  };

  /// <summary>
  /// <c>KAnimRate</c> value → display label.
  /// 0 = High, 1 = Medium, 2 = Low. Additional values may exist.
  /// </summary>
  public static string GetAnimRateLabel(int value) => value switch
  {
    0 => "High",
    1 => "Medium",
    2 => "Low",
    _ => $"Unknown ({value})",
  };

  /// <summary>
  /// <c>KExpshare</c> value → display label.
  /// 0 = On, 1 = Off.
  /// </summary>
  public static string GetExpShareLabel(int value) => value switch
  {
    0 => "On",
    1 => "Off",
    _ => $"Unknown ({value})",
  };

  /// <summary>
  /// <c>KPicnicExp</c> value → display label.
  /// 0 = On, 1 = Off.
  /// </summary>
  public static string GetPicnicExpLabel(int value) => value switch
  {
    0 => "On",
    1 => "Off",
    _ => $"Unknown ({value})",
  };

  /// <summary>
  /// <c>KRNGSkew</c> (base setting) value → display label.
  /// This is the global Capture Bonuses toggle, not a per-species tracker.
  /// 0 = On, 1 = Off.
  /// </summary>
  public static string GetCaptureBonusLabel(int value) => value switch
  {
    0 => "On",
    1 => "Off",
    _ => $"Unknown ({value})",
  };

  /// <summary>
  /// SByte. Compass flag indicating whether the player has already talked to "Pepper".
  /// Hash input: <c>Compass_PEPPERTALKALREADY</c>. Observed value = 1.
  /// </summary>
  public const uint KPepperTalkAlready = 0xD73E5336;

  /// <summary>
  /// SByte block keys for Compass gameplay settings (not capture bonuses).
  /// These are mixed into the same SByte type as capture bonuses in the save,
  /// but represent global mod configuration, not per-species trackers.
  /// </summary>
  public static ReadOnlySpan<uint> CompassSettingKeys =>
  [
      0x1C25C049, // Compass_PicnicExp
        0x34780809, // Compass_BattleCam
        0x46E0EDCB, // Compass_LetsGoEV
        0x837B300E, // Compass_Levelcap
        0x8B12B6B8, // Compass_Expmulti
        0x92569278, // Compass_ColorProfile
        0x96C58F45, // Compass_SpawnRate
        0xB1366CBF, // Compass_RNGSkew (base)
        0xCC806ED6, // Compass_Expshare
        0xD2E638A3, // Compass_CamDither
        0xFB067137, // Compass_AnimRate
    ];

  // Exact mapping is unconfirmed; values noted from a mid-to-late game Compass save.

  /// <summary>UInt64 = 4. Likely raid tier or experience-rate index.</summary>
  public const uint KOptions_54DB99E0 = 0x54DB99E0;

  /// <summary>UInt64 = 4. Likely raid tier or experience-rate index.</summary>
  public const uint KOptions_7D845059 = 0x7D845059;

  /// <summary>UInt64 = 4. Likely raid tier or experience-rate index.</summary>
  public const uint KOptions_A4F26D47 = 0xA4F26D47;

  /// <summary>UInt64 = 1. Boolean-like setting (enabled/level 1).</summary>
  public const uint KOptions_59CE6487 = 0x59CE6487;

  /// <summary>UInt64 = 1. Boolean-like setting (enabled/level 1).</summary>
  public const uint KOptions_8F4E359D = 0x8F4E359D;

  /// <summary>UInt64 = 1. Boolean-like setting (enabled/level 1).</summary>
  public const uint KOptions_C4B67782 = 0xC4B67782;

  // Additional UInt64 blocks (value = 0 in observed save - reserved/counters)
  public const uint KOptions_02EEF680 = 0x02EEF680;
  public const uint KOptions_0FB466E0 = 0x0FB466E0;
  public const uint KOptions_1194092E = 0x1194092E;
  public const uint KOptions_11CBC209 = 0x11CBC209;
  public const uint KOptions_552ACEFC = 0x552ACEFC;
  public const uint KOptions_584EC3C5 = 0x584EC3C5;
  public const uint KOptions_60E95C49 = 0x60E95C49;
  public const uint KOptions_8DB69836 = 0x8DB69836;
  public const uint KOptions_94D0D569 = 0x94D0D569;
  public const uint KOptions_9A085043 = 0x9A085043;
  public const uint KOptions_9A4CC475 = 0x9A4CC475;
  public const uint KOptions_A0B96EEB = 0xA0B96EEB;
  public const uint KOptions_B1C035CB = 0xB1C035CB;
  public const uint KOptions_B1F0151A = 0xB1F0151A;
  public const uint KOptions_C3CD694D = 0xC3CD694D;
  public const uint KOptions_D27D3F34 = 0xD27D3F34;
  public const uint KOptions_D455B377 = 0xD455B377;
  public const uint KOptions_D64B10DF = 0xD64B10DF;
  public const uint KOptions_EDBE4224 = 0xEDBE4224;
  public const uint KOptions_F0A3C0A7 = 0xF0A3C0A7;
  public const uint KOptions_FE3C16CC = 0xFE3C16CC;


  // Hash inputs confirmed via Lua bytecode string extraction.

  /// <summary>Bool2 (true). Hash input: <c>compass_displayTip_CaptureBonuses</c>.</summary>
  public const uint KFeature_4197A769 = 0x4197A769;

  /// <summary>Bool2 (true). Hash input: <c>Compass_PERRINPARADOX</c>.</summary>
  public const uint KFeature_72958FC5 = 0x72958FC5;

  /// <summary>Bool2 (true). Hash input: <c>nixskip_selfie</c>.</summary>
  public const uint KFeature_74043B2C = 0x74043B2C;

  /// <summary>Bool2 (true). Hash input: <c>compass_displayTip_Obedience</c>.</summary>
  public const uint KFeature_7FD7214C = 0x7FD7214C;

  /// <summary>Bool2 (true). Hash input: <c>compass_newTip_NewMaps</c>.</summary>
  public const uint KFeature_A371E2C1 = 0xA371E2C1;

  /// <summary>Bool2 (true). Hash input: <c>compass_displayTip_NewMaps</c>.</summary>
  public const uint KFeature_AAC9AAD1 = 0xAAC9AAD1;

  /// <summary>Bool2 (true). Hash input: <c>compass_newTip_Obedience</c>.</summary>
  public const uint KFeature_B3745A5C = 0xB3745A5C;

  /// <summary>Bool2 (true). Hash input: <c>yinskip_swift</c>.</summary>
  public const uint KFeature_BE24A8C0 = 0xBE24A8C0;

  /// <summary>Bool2 (true). Hash input: <c>compass_newTip_CaptureBonuses</c>.</summary>
  public const uint KFeature_D93612B9 = 0xD93612B9;


  /// <summary>Bool1 (false). Hash input: <c>compass_newTip_PhoePrices</c>.</summary>
  public const uint KFeature_78A49D0A = 0x78A49D0A;

  /// <summary>Bool1 (false). Hash input: <c>roo_flag</c>.</summary>
  public const uint KFeature_8F8F4A1E = 0x8F8F4A1E;

  /// <summary>Bool1 (false). Hash input: <c>compass_displayTip_PhoePrices</c>.</summary>
  public const uint KFeature_D555F09A = 0xD555F09A;



  /// <summary>Human-readable labels for the UInt64 option blocks.</summary>
  public static readonly (uint Key, string Label)[] UInt64OptionLabels =
  [

      (KOptions_54DB99E0, "Unknown UInt64 (observed: 4) - 0x54DB99E0"),
        (KOptions_7D845059, "Unknown UInt64 (observed: 4) - 0x7D845059"),
        (KOptions_A4F26D47, "Unknown UInt64 (observed: 4) - 0xA4F26D47"),
        (KOptions_59CE6487, "Unknown UInt64 (observed: 1) - 0x59CE6487"),
        (KOptions_8F4E359D, "Unknown UInt64 (observed: 1) - 0x8F4E359D"),
        (KOptions_C4B67782, "Unknown UInt64 (observed: 1) - 0xC4B67782"),


        (KOptions_02EEF680, "Reserved / unknown (0x02EEF680)"),
        (KOptions_0FB466E0, "Reserved / unknown (0x0FB466E0)"),
        (KOptions_1194092E, "Reserved / unknown (0x1194092E)"),
        (KOptions_11CBC209, "Reserved / unknown (0x11CBC209)"),
        (KOptions_552ACEFC, "Reserved / unknown (0x552ACEFC)"),
        (KOptions_584EC3C5, "Reserved / unknown (0x584EC3C5)"),
        (KOptions_60E95C49, "Reserved / unknown (0x60E95C49)"),
        (KOptions_8DB69836, "Reserved / unknown (0x8DB69836)"),
        (KOptions_94D0D569, "Reserved / unknown (0x94D0D569)"),
        (KOptions_9A085043, "Reserved / unknown (0x9A085043)"),
        (KOptions_9A4CC475, "Reserved / unknown (0x9A4CC475)"),
        (KOptions_A0B96EEB, "Reserved / unknown (0xA0B96EEB)"),
        (KOptions_B1C035CB, "Reserved / unknown (0xB1C035CB)"),
        (KOptions_B1F0151A, "Reserved / unknown (0xB1F0151A)"),
        (KOptions_C3CD694D, "Reserved / unknown (0xC3CD694D)"),
        (KOptions_D27D3F34, "Reserved / unknown (0xD27D3F34)"),
        (KOptions_D455B377, "Reserved / unknown (0xD455B377)"),
        (KOptions_D64B10DF, "Reserved / unknown (0xD64B10DF)"),
        (KOptions_EDBE4224, "Reserved / unknown (0xEDBE4224)"),
        (KOptions_F0A3C0A7, "Reserved / unknown (0xF0A3C0A7)"),
        (KOptions_FE3C16CC, "Reserved / unknown (0xFE3C16CC)"),
    ];

  /// <summary>Human-readable labels for the Bool feature toggle blocks.</summary>
  public static readonly (uint Key, string Label)[] BoolFeatureLabels =
  [
      (KFeature_4197A769, "compass_displayTip_CaptureBonuses"),
        (KFeature_72958FC5, "Compass_PERRINPARADOX"),
        (KFeature_74043B2C, "nixskip_selfie"),
        (KFeature_7FD7214C, "compass_displayTip_Obedience"),
        (KFeature_A371E2C1, "compass_newTip_NewMaps"),
        (KFeature_AAC9AAD1, "compass_displayTip_NewMaps"),
        (KFeature_B3745A5C, "compass_newTip_Obedience"),
        (KFeature_BE24A8C0, "yinskip_swift"),
        (KFeature_D93612B9, "compass_newTip_CaptureBonuses"),

        (KFeature_78A49D0A, "compass_newTip_PhoePrices"),
        (KFeature_8F8F4A1E, "roo_flag"),
        (KFeature_D555F09A, "compass_displayTip_PhoePrices"),
    ];

  /// <summary>
  /// All Compass-only SByte block keys that track per-species Capture Bonus counts (0–25).
  /// At 25 captures a species gains a Hidden Ability chance (25%) and an extra shiny roll.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Each key is the FNV-1a hash of <c>Compass_RNGSkew_{NationalDexNumber}</c>.
  /// All 426 keys are mapped to species via <see cref="GetSpeciesForKey"/>.
  /// </para>
  /// <para>
  /// <b>Important:</b> 11 additional SByte blocks exist in the save that are Compass settings,
  /// NOT capture bonuses. These are in <see cref="CompassSettingKeys"/>.
  /// <see cref="KShinyNotification"/> is also SByte but tracked separately.
  /// </para>
  /// </remarks>
  public static ReadOnlySpan<uint> CaptureBonusKeys =>
  [
      // Species sorted by key value (ascending).
      // Use GetSpeciesForKey() to look up the national dex number.
      0x1D589E60, 0x1D58A013, 0x1D58A379, 0x1D58A6DF, 0x1D58B62A, //  26,  27,  25,  23,  28
        0x1D5BB2D0, 0x1D5BCA9A, 0x1D5BCC4D, 0x1D5DF48C, 0x1D5DF63F, //  39,  37,  36,  48,  49
        0x1D5DFB58, 0x1D5DFD0B, 0x1D5E073D, 0x1D610C62, 0x1D610E15, //  44,  45,  43,  59,  58
        0x1D61117B, 0x1D6114E1, 0x1D6119FA, 0x1D611BAD, 0x1D63FED6, //  56,  54,  51,  50,  60
        0x1D640E21, 0x1D670FE0, 0x1D6714F9, 0x1D671A12, 0x1D671BC5, //  69,  75,  76,  73,  72
        0x1D672444, 0x1D6A2450, 0x1D6A2603, 0x1D6A31E8, 0x1D6A339B, //  79,  88,  89,  80,  81
        0x1D6A38B4, 0x1D6A3A67, 0x1D6A3C1A, 0x1D6A3DCD, 0x1D6C6E8B, //  84,  85,  86,  87,  92
        0x7E01F663, 0x7E01F816, 0x7E01FB7C, 0x7E020095, 0x7E020248, //   1,   2,   4,   7,   8
        0x7E0203FB, 0xAF226F30, 0xAF2270E3, 0xAF227E7B, 0xAF228394, //   9,1018,1019,1011,1014
        0xAF228547, 0xAF2286FA, 0xAF2288AD, 0xAF259138, 0xAF25949E, //1015,1016,1017,1003,1001
        0xAF259804, 0xAF2599B7, 0xAF2B9150, 0xAF2B9303, 0xAF2B9669, //1007,1006,1021,1020,1022
        0xAF2B981C, 0xAF2B99CF, 0xD85912F3, 0xD85914A6, //1025,1024, 131, 132
        0xD8591659, 0xD8591D25, 0xD85C25B0, 0xD85C2E2F, 0xD85C2FE2, // 133, 137, 123, 126, 125
        0xD85C36AE, 0xD85C3861, 0xD85F3A20, 0xD85F3BD3, 0xD85F40EC, // 129, 128, 116, 117, 112
        0xD85F429F, 0xD85F4605, 0xD8624E90, 0xD8625F8E, 0xD8626141, // 113, 111, 109, 103, 102
        0xD8649F97, 0xD864A14A, 0xD867AB88, 0xD867B0A1, 0xD867B254, // 171, 172, 167, 164, 163
        0xD867B407, 0xD867B5BA, 0xD86A9730, 0xD86A98E3, 0xD86A9A96, // 162, 161, 152, 153, 150
        0xD86A9C49, 0xD86AA162, 0xD86AA315, 0xD86AA82E, 0xD86AA9E1, // 151, 154, 155, 158, 159
        0xD86DABA0, 0xD86DAD53, 0xD86DB0B9, 0xD87619A2, 0xD8761B55, // 145, 144, 146, 198, 199
        0xD876206E, 0xD8762587, 0xD876273A, 0xD8790DC9, 0xD8790F7C, // 194, 193, 190, 182, 185
        0xDD9503A3, 0xDD950556, 0xDD981660, 0xDD981B79, 0xDD982245, // 261, 262, 273, 270, 274
        0xDD982911, 0xDD9B2E36, 0xDD9B2FE9, 0xDD9B319C, 0xDD9B334F, // 278, 244, 245, 242, 243
        0xDD9B3502, 0xDD9B444D, 0xDD9D65C0, 0xDD9D6773, 0xDD9D750B, // 240, 249, 259, 258, 250
        0xDD9D7871, 0xDD9D7A24, 0xDD9D7F3D, 0xDDA080FC, 0xDDA082AF, // 252, 255, 256, 228, 229
        0xDDA398D2, 0xDDA39C38, 0xDDA39DEB, 0xDDA39F9E, 0xDDA3A151, // 239, 237, 236, 235, 234
        0xDDA3A4B7, 0xDDA3A81D, 0xDDA68993, 0xDDA68B46, 0xDDA68EAC, // 232, 230, 203, 200, 206
        0xDDA6905F, 0xDDA693C5, 0xDDA69A91, 0xDDA99E03, 0xDDA9A169, // 207, 205, 209, 214, 216
        0xDDA9A31C, 0xDDA9A4CF, 0xDDA9B267, 0xDDBD6096, 0xDDBD6249, // 211, 210, 218, 288, 289
        0xDDBD6C7B, 0xDDBD6E2E, 0xDDBD7347, 0xDDBD74FA, 0xDDC0786C, // 283, 280, 287, 284, 299
        0xE2D0F2A0, 0xE2D0F7B9, 0xE2D0FCD2, 0xE2D40710, 0xE2D408C3, // 390, 393, 396, 383, 382
        0xE2D40A76, 0xE2D40C29, 0xE2D40DDC, 0xE2D40F8F, 0xE2D41142, // 381, 380, 387, 386, 385
        0xE2D412F5, 0xE2D419C1, 0xE2E7D73B, 0xE2E7DAA1, 0xE2E7DC54, // 384, 388, 311, 313, 314
        0xE2EDFCB5, 0xE2EDFE68, 0xE2EE0381, 0xE2EE06E7, 0xE2EE089A, // 339, 336, 335, 333, 330
        0xE2F0ED76, 0xE2F0EF29, 0xE2F0F0DC, 0xE2F0F28F, 0xE2F0F7A8, // 323, 322, 325, 324, 329
        0xE2F0F95B, 0xE2F32500, 0xE2F326B3, 0xE2F330E5, 0xE2F33964, // 328, 354, 355, 353, 358
        0xE2F643A2, 0xE2F6513A, 0xE2F95146, 0xE2F952F9, 0xE2F95EDE, // 341, 349, 378, 379, 370
        0xE2F963F7, 0xE2F9675D, 0xE2FC76B4, 0xE6A6CDDC, 0xE6A6D4A8, // 377, 375, 361, 488, 484
        0xE6A6D65B, 0xE6A6D80E, 0xE6A6D9C1, 0xE6A6DB74, 0xE6A6DD27, // 485, 486, 487, 480, 481
        0xE6A6DEDA, 0xE6A6E08D, 0xE6A90C32, 0xE6A90DE5, 0xE6A9114B, // 482, 483, 499, 498, 496
        0xE6A912FE, 0xE6A91664, 0xE6A91817, 0xE6A919CA, 0xE6A91B7D, // 495, 493, 492, 491, 490
        0xE6B231B8, 0xE6B2351E, 0xE6B236D1, 0xE6B23BEA, 0xE6B23D9D, // 440, 442, 443, 446, 447
        0xE6B53BF6, 0xE6B54CF4, 0xE6B54EA7, 0xE6B7847E, 0xE6B787E4, // 459, 457, 456, 464, 462
        0xE6B78B4A, 0xE6BA6CC0, 0xE6BA7A58, 0xE6BD8130, 0xE6BD8D15, // 460, 471, 479, 404, 403
        0xE6C095A0, 0xE6C09906, 0xE6C09FD2, 0xE6C5ED7F, 0xE6C5F7B1, // 417, 415, 411, 438, 436
        0xE6C5FCCA, 0xEBE7EF56, 0xEBE7F2BC, 0xEBE7F622, 0xEBEB0060, // 433, 590, 596, 594, 585
        0xEBEB0579, 0xEBEB08DF, 0xEBEE2268, 0xEBEE25CE, 0xEBEE2934, // 586, 580, 570, 572, 574
        0xEBEE2AE7, 0xEBEE2C9A, 0xEBEE2E4D, 0xEBF37894, 0xEBF37A47, // 575, 576, 577, 552, 553
        0xEBF37DAD, 0xEBF65D70, 0xEBF65F23, 0xEBF66289, 0xEBF66955, // 551, 541, 540, 542, 546
        0xEBF66CBB, 0xEBF978AC, 0xEBF97C12, 0xEBFC8D1C, 0xEBFC8ECF, // 548, 530, 532, 523, 522
        0xEBFC9E1A, 0xEC01EA14, 0xEC01EBC7, 0xF0E3F126, 0xF0E3F48C, // 529, 501, 500, 622, 624
        0xF0E3F63F, 0xF0E3F9A5, 0xF0E3FB58, 0xF0E3FD0B, 0xF0E70749, // 625, 627, 628, 629, 630
        0xF0E70AAF, 0xF0E7132E, 0xF0E714E1, 0xF0EA1853, 0xF0EA1A06, // 636, 639, 638, 607, 604
        0xF0EA1F1F, 0xF0ED2B10, 0xF0ED3C0E, 0xF0ED4127, 0xF0EF6CCC, // 603, 619, 613, 614, 668
        0xF0EF7398, 0xF0EF78B1, 0xF0EF7C17, 0xF0EF7DCA, 0xF0EF7F7D, // 664, 667, 661, 662, 663
        0xF0F28655, 0xF0F28808, 0xF0F29087, 0xF0F573B0, 0xF0F57563, // 678, 677, 672, 642, 643
        0xF0F57716, 0xF0F578C9, 0xF0F57A7C, 0xF0F57C2F, 0xF0F57DE2, // 640, 641, 646, 647, 644
        0xF0F57F95, 0xF0F584AE, 0xF0F889D3, 0xF0F88D39, 0xF0F8909F, // 645, 648, 654, 656, 650
        0xF0F89252, 0xF0F89E37, 0xF100F988, 0xF100FB3B, 0xF61FE023, // 653, 658, 686, 687, 751
        0xF61FE1D6, 0xF61FE389, 0xF622F493, 0xF622F646, 0xF622F7F9, // 752, 753, 742, 741, 740
        0xF622FD12, 0xF622FEC5, 0xF62303DE, 0xF6260AB6, 0xF6260C69, // 745, 744, 749, 774, 775
        0xF6261F1A, 0xF62620CD, 0xF6284240, 0xF6284FD8, 0xF6285857, // 778, 779, 769, 761, 764
        0xF62B5F2F, 0xF62B6448, 0xF62B702D, 0xF62E7705, 0xF62E7C1E, // 719, 714, 713, 708, 705
        0xF62E7DD1, 0xF62E849D, 0xF6316460, 0xF6316613, 0xF6316979, // 704, 700, 732, 733, 731
        0xF6316B2C, 0xF6316CDF, 0xF6317711, 0xF63478D0, 0xF6347F9C, // 736, 737, 739, 725, 721
        0xF634814F, 0xF63484B5, 0xF6348D34, 0xF6348EE7, 0xF6452DBF, // 720, 722, 729, 728, 782
        0xF6453D0A, 0xFB5BD5EC, 0xFB5BD79F, 0xFB5BDCB8, 0xFB5BDE6B, // 789, 884, 885, 888, 889
        0xFB5EE390, 0xFB5EE543, 0xFB5EE6F6, 0xFB5EE8A9, 0xFB5EEA5C, // 893, 892, 891, 890, 897
        0xFB5EEC0F, 0xFB5EEDC2, 0xFB5EEF75, 0xFB5EF641, 0xFB72B208, // 896, 895, 894, 898, 800
        0xFB72B3BB, 0xFB75BC46, 0xFB75BDF9, 0xFB75C678, 0xFB75CB91, // 801, 819, 818, 813, 810
        0xFB75CD44, 0xFB75CEF7, 0xFB78E1B4, 0xFB78E51A, 0xFB7BC9F6, // 817, 816, 822, 820, 833
        0xFB7BCF0F, 0xFB7BD5DB, 0xFB7E0333, 0xFB7E04E6, 0xFB7E084C, // 834, 838, 845, 846, 840
        0xFB7E15E4, 0xFB7E1797, 0xFB8117A3, 0xFB811B09, 0xFB812DBA, // 848, 849, 856, 854, 859
        0xFB812F6D, 0xFB842DC6, 0xFB842F79, 0xFB8439AB, 0xFB874E1B, // 858, 868, 869, 863, 874
        0xFB874FCE, 0xFB875181, 0xFB87569A, 0xFF2B7AB0, 0xFF2B7C63, // 877, 876, 873, 996, 997
        0xFF2B7E16, 0xFF2B7FC9, 0xFF2B817C, 0xFF2B832F, 0xFF2B84E2, // 994, 995, 992, 993, 990
        0xFF2B8695, 0xFF2B927A, 0xFF2B942D, 0xFF2E8F20, 0xFF2E90D3, // 991, 998, 999, 989, 988
        0xFF2E9CB8, 0xFF2E9E6B, 0xFF2EA01E, 0xFF2EA1D1, 0xFF2EA384, // 981, 980, 983, 982, 985
        0xFF2EA537, 0xFF2EA6EA, 0xFF2EA89D, 0xFF3D0E38, 0xFF3D0FEB, // 984, 987, 986, 930, 931
        0xFF3D1504, 0xFF3D16B7, 0xFF3D1A1D, 0xFF401A29, 0xFF4022A8, // 934, 935, 937, 928, 923
        0xFF40245B, 0xFF40260E, 0xFF402CDA, 0xFF425A32, 0xFF4262B1, // 922, 921, 925, 918, 915
        0xFF426464, 0xFF426617, 0xFF4267CA, 0xFF454940, 0xFF454CA6, // 912, 913, 910, 901, 903
        0xFF454E59, 0xFF45500C, 0xFF455525, 0xFF4556D8, 0xFF486116, // 902, 905, 906, 909, 976
        0xFF487214, 0xFF4873C7, 0xFF4B73D3, 0xFF4B78EC, 0xFF4B7E05, // 978, 979, 966, 963, 960
        0xFF4B8B9D, 0xFF4E8BA9, 0xFF4E95DB, 0xFF4E9941, 0xFF4E9E5A, // 968, 959, 953, 951, 954
        0xFF4EA00D, 0xFF50C84C, 0xFF50CF18, 0xFF50D0CB, 0xFF50D27E, // 955, 949, 945, 944, 947
        0xFF50D797, 0xFF50DAFD,                                       // 940, 942
    ];


  /// <summary>
  /// Returns the national Pokédex number for a capture bonus block key,
  /// or 0 if the key is not a known capture bonus.
  /// </summary>
  public static ushort GetSpeciesForKey(uint key) => key switch
  {
    0x7E01F663 => 1, // Bulbasaur
    0x7E01F816 => 2, // Ivysaur
    0x7E01FB7C => 4, // Charmander
    0x7E020095 => 7, // Squirtle
    0x7E020248 => 8, // Wartortle
    0x7E0203FB => 9, // Blastoise
    0x1D58A6DF => 23, // Ekans
    0x1D58A379 => 25, // Pikachu
    0x1D589E60 => 26, // Raichu
    0x1D58A013 => 27, // Sandshrew
    0x1D58B62A => 28, // Sandslash
    0x1D5BCC4D => 36, // Clefable
    0x1D5BCA9A => 37, // Vulpix
    0x1D5BB2D0 => 39, // Jigglypuff
    0x1D5E073D => 43, // Oddish
    0x1D5DFB58 => 44, // Gloom
    0x1D5DFD0B => 45, // Vileplume
    0x1D5DF48C => 48, // Venonat
    0x1D5DF63F => 49, // Venomoth
    0x1D611BAD => 50, // Diglett
    0x1D6119FA => 51, // Dugtrio
    0x1D6114E1 => 54, // Psyduck
    0x1D61117B => 56, // Mankey
    0x1D610E15 => 58, // Growlithe
    0x1D610C62 => 59, // Arcanine
    0x1D63FED6 => 60, // Poliwag
    0x1D640E21 => 69, // Bellsprout
    0x1D671BC5 => 72, // Tentacool
    0x1D671A12 => 73, // Tentacruel
    0x1D670FE0 => 75, // Graveler
    0x1D6714F9 => 76, // Golem
    0x1D672444 => 79, // Slowpoke
    0x1D6A31E8 => 80, // Slowbro
    0x1D6A339B => 81, // Magnemite
    0x1D6A38B4 => 84, // Doduo
    0x1D6A3A67 => 85, // Dodrio
    0x1D6A3C1A => 86, // Seel
    0x1D6A3DCD => 87, // Dewgong
    0x1D6A2450 => 88, // Grimer
    0x1D6A2603 => 89, // Muk
    0x1D6C6E8B => 92, // Gastly
    0xD8626141 => 102, // Exeggcute
    0xD8625F8E => 103, // Exeggutor
    0xD8624E90 => 109, // Koffing
    0xD85F4605 => 111, // Rhyhorn
    0xD85F40EC => 112, // Rhydon
    0xD85F429F => 113, // Chansey
    0xD85F3A20 => 116, // Horsea
    0xD85F3BD3 => 117, // Seadra
    0xD85C25B0 => 123, // Scyther
    0xD85C2FE2 => 125, // Electabuzz
    0xD85C2E2F => 126, // Magmar
    0xD85C3861 => 128, // Tauros
    0xD85C36AE => 129, // Magikarp
    0xD85912F3 => 131, // Lapras
    0xD85914A6 => 132, // Ditto
    0xD8591659 => 133, // Eevee
    0xD8591D25 => 137, // Porygon
    0xD86DAD53 => 144, // Articuno
    0xD86DABA0 => 145, // Zapdos
    0xD86DB0B9 => 146, // Moltres
    0xD86A9A96 => 150, // Mewtwo
    0xD86A9C49 => 151, // Mew
    0xD86A9730 => 152, // Chikorita
    0xD86A98E3 => 153, // Bayleef
    0xD86AA162 => 154, // Meganium
    0xD86AA315 => 155, // Cyndaquil
    0xD86AA82E => 158, // Totodile
    0xD86AA9E1 => 159, // Croconaw
    0xD867B5BA => 161, // Sentret
    0xD867B407 => 162, // Furret
    0xD867B254 => 163, // Hoothoot
    0xD867B0A1 => 164, // Noctowl
    0xD867AB88 => 167, // Spinarak
    0xD8649F97 => 171, // Lanturn
    0xD864A14A => 172, // Pichu
    0xD8790DC9 => 182, // Bellossom
    0xD8790F7C => 185, // Sudowoodo
    0xD876273A => 190, // Aipom
    0xD8762587 => 193, // Yanma
    0xD876206E => 194, // Wooper
    0xD87619A2 => 198, // Murkrow
    0xD8761B55 => 199, // Slowking
    0xDDA68B46 => 200, // Misdreavus
    0xDDA68993 => 203, // Girafarig
    0xDDA693C5 => 205, // Forretress
    0xDDA68EAC => 206, // Dunsparce
    0xDDA6905F => 207, // Gligar
    0xDDA69A91 => 209, // Snubbull
    0xDDA9A4CF => 210, // Granbull
    0xDDA9A31C => 211, // Qwilfish
    0xDDA99E03 => 214, // Heracross
    0xDDA9A169 => 216, // Teddiursa
    0xDDA9B267 => 218, // Slugma
    0xDDA080FC => 228, // Houndour
    0xDDA082AF => 229, // Houndoom
    0xDDA3A81D => 230, // Kingdra
    0xDDA3A4B7 => 232, // Donphan
    0xDDA3A151 => 234, // Stantler
    0xDDA39F9E => 235, // Smeargle
    0xDDA39DEB => 236, // Tyrogue
    0xDDA39C38 => 237, // Hitmontop
    0xDDA398D2 => 239, // Elekid
    0xDD9B3502 => 240, // Magby
    0xDD9B319C => 242, // Blissey
    0xDD9B334F => 243, // Raikou
    0xDD9B2E36 => 244, // Entei
    0xDD9B2FE9 => 245, // Suicune
    0xDD9B444D => 249, // Lugia
    0xDD9D750B => 250, // Ho-Oh
    0xDD9D7871 => 252, // Treecko
    0xDD9D7A24 => 255, // Torchic
    0xDD9D7F3D => 256, // Combusken
    0xDD9D6773 => 258, // Mudkip
    0xDD9D65C0 => 259, // Marshtomp
    0xDD9503A3 => 261, // Poochyena
    0xDD950556 => 262, // Mightyena
    0xDD981B79 => 270, // Lotad
    0xDD981660 => 273, // Seedot
    0xDD982245 => 274, // Nuzleaf
    0xDD982911 => 278, // Wingull
    0xDDBD6E2E => 280, // Ralts
    0xDDBD6C7B => 283, // Surskit
    0xDDBD74FA => 284, // Masquerain
    0xDDBD7347 => 287, // Slakoth
    0xDDBD6096 => 288, // Vigoroth
    0xDDBD6249 => 289, // Slaking
    0xDDC0786C => 299, // Nosepass
    0xE2E7D73B => 311, // Plusle
    0xE2E7DAA1 => 313, // Volbeat
    0xE2E7DC54 => 314, // Illumise
    0xE2F0EF29 => 322, // Numel
    0xE2F0ED76 => 323, // Camerupt
    0xE2F0F28F => 324, // Torkoal
    0xE2F0F0DC => 325, // Spoink
    0xE2F0F95B => 328, // Trapinch
    0xE2F0F7A8 => 329, // Vibrava
    0xE2EE089A => 330, // Flygon
    0xE2EE06E7 => 333, // Swablu
    0xE2EE0381 => 335, // Zangoose
    0xE2EDFE68 => 336, // Seviper
    0xE2EDFCB5 => 339, // Barboach
    0xE2F643A2 => 341, // Corphish
    0xE2F6513A => 349, // Feebas
    0xE2F330E5 => 353, // Shuppet
    0xE2F32500 => 354, // Banette
    0xE2F326B3 => 355, // Duskull
    0xE2F33964 => 358, // Chimecho
    0xE2FC76B4 => 361, // Snorunt
    0xE2F95EDE => 370, // Luvdisc
    0xE2F9675D => 375, // Metang
    0xE2F963F7 => 377, // Regirock
    0xE2F95146 => 378, // Regice
    0xE2F952F9 => 379, // Registeel
    0xE2D40C29 => 380, // Latias
    0xE2D40A76 => 381, // Latios
    0xE2D408C3 => 382, // Kyogre
    0xE2D40710 => 383, // Groudon
    0xE2D412F5 => 384, // Rayquaza
    0xE2D41142 => 385, // Jirachi
    0xE2D40F8F => 386, // Deoxys
    0xE2D40DDC => 387, // Turtwig
    0xE2D419C1 => 388, // Grotle
    0xE2D0F2A0 => 390, // Chimchar
    0xE2D0F7B9 => 393, // Piplup
    0xE2D0FCD2 => 396, // Starly
    0xE6BD8D15 => 403, // Shinx
    0xE6BD8130 => 404, // Luxio
    0xE6C09FD2 => 411, // Bastiodon
    0xE6C09906 => 415, // Combee
    0xE6C095A0 => 417, // Pachirisu
    0xE6C5FCCA => 433, // Chingling
    0xE6C5F7B1 => 436, // Bronzor
    0xE6C5ED7F => 438, // Bonsly
    0xE6B231B8 => 440, // Happiny
    0xE6B2351E => 442, // Spiritomb
    0xE6B236D1 => 443, // Gible
    0xE6B23BEA => 446, // Munchlax
    0xE6B23D9D => 447, // Riolu
    0xE6B54EA7 => 456, // Finneon
    0xE6B54CF4 => 457, // Lumineon
    0xE6B53BF6 => 459, // Snover
    0xE6B78B4A => 460, // Abomasnow
    0xE6B787E4 => 462, // Magnezone
    0xE6B7847E => 464, // Rhyperior
    0xE6BA6CC0 => 471, // Glaceon
    0xE6BA7A58 => 479, // Rotom
    0xE6A6DB74 => 480, // Uxie
    0xE6A6DD27 => 481, // Mesprit
    0xE6A6DEDA => 482, // Azelf
    0xE6A6E08D => 483, // Dialga
    0xE6A6D4A8 => 484, // Palkia
    0xE6A6D65B => 485, // Heatran
    0xE6A6D80E => 486, // Regigigas
    0xE6A6D9C1 => 487, // Giratina
    0xE6A6CDDC => 488, // Cresselia
    0xE6A91B7D => 490, // Manaphy
    0xE6A919CA => 491, // Darkrai
    0xE6A91817 => 492, // Shaymin
    0xE6A91664 => 493, // Arceus
    0xE6A912FE => 495, // Snivy
    0xE6A9114B => 496, // Servine
    0xE6A90DE5 => 498, // Tepig
    0xE6A90C32 => 499, // Pignite
    0xEC01EBC7 => 500, // Emboar
    0xEC01EA14 => 501, // Oshawott
    0xEBFC8ECF => 522, // Blitzle
    0xEBFC8D1C => 523, // Zebstrika
    0xEBFC9E1A => 529, // Drilbur
    0xEBF978AC => 530, // Excadrill
    0xEBF97C12 => 532, // Timburr
    0xEBF65F23 => 540, // Sewaddle
    0xEBF65D70 => 541, // Swadloon
    0xEBF66289 => 542, // Leavanny
    0xEBF66955 => 546, // Cottonee
    0xEBF66CBB => 548, // Petilil
    0xEBF37DAD => 551, // Sandile
    0xEBF37894 => 552, // Krokorok
    0xEBF37A47 => 553, // Krookodile
    0xEBEE2268 => 570, // Zorua
    0xEBEE25CE => 572, // Minccino
    0xEBEE2934 => 574, // Gothita
    0xEBEE2AE7 => 575, // Gothorita
    0xEBEE2C9A => 576, // Gothitelle
    0xEBEE2E4D => 577, // Solosis
    0xEBEB08DF => 580, // Ducklett
    0xEBEB0060 => 585, // Deerling
    0xEBEB0579 => 586, // Sawsbuck
    0xEBE7EF56 => 590, // Foongus
    0xEBE7F622 => 594, // Alomomola
    0xEBE7F2BC => 596, // Galvantula
    0xF0EA1F1F => 603, // Eelektrik
    0xF0EA1A06 => 604, // Eelektross
    0xF0EA1853 => 607, // Litwick
    0xF0ED3C0E => 613, // Cubchoo
    0xF0ED4127 => 614, // Beartic
    0xF0ED2B10 => 619, // Mienfoo
    0xF0E3F126 => 622, // Golett
    0xF0E3F48C => 624, // Pawniard
    0xF0E3F63F => 625, // Bisharp
    0xF0E3F9A5 => 627, // Rufflet
    0xF0E3FB58 => 628, // Braviary
    0xF0E3FD0B => 629, // Vullaby
    0xF0E70749 => 630, // Mandibuzz
    0xF0E70AAF => 636, // Larvesta
    0xF0E714E1 => 638, // Cobalion
    0xF0E7132E => 639, // Terrakion
    0xF0F57716 => 640, // Virizion
    0xF0F578C9 => 641, // Tornadus
    0xF0F573B0 => 642, // Thundurus
    0xF0F57563 => 643, // Reshiram
    0xF0F57DE2 => 644, // Zekrom
    0xF0F57F95 => 645, // Landorus
    0xF0F57A7C => 646, // Kyurem
    0xF0F57C2F => 647, // Keldeo
    0xF0F584AE => 648, // Meloetta
    0xF0F8909F => 650, // Chespin
    0xF0F89252 => 653, // Fennekin
    0xF0F889D3 => 654, // Braixen
    0xF0F88D39 => 656, // Froakie
    0xF0F89E37 => 658, // Greninja
    0xF0EF7C17 => 661, // Fletchling
    0xF0EF7DCA => 662, // Fletchinder
    0xF0EF7F7D => 663, // Talonflame
    0xF0EF7398 => 664, // Scatterbug
    0xF0EF78B1 => 667, // Litleo
    0xF0EF6CCC => 668, // Pyroar
    0xF0F29087 => 672, // Skiddo
    0xF0F28808 => 677, // Espurr
    0xF0F28655 => 678, // Meowstic
    0xF100F988 => 686, // Inkay
    0xF100FB3B => 687, // Malamar
    0xF62E849D => 700, // Sylveon
    0xF62E7DD1 => 704, // Goomy
    0xF62E7C1E => 705, // Sliggoo
    0xF62E7705 => 708, // Phantump
    0xF62B702D => 713, // Avalugg
    0xF62B6448 => 714, // Noibat
    0xF62B5F2F => 719, // Diancie
    0xF634814F => 720, // Hoopa
    0xF6347F9C => 721, // Volcanion
    0xF63484B5 => 722, // Rowlet
    0xF63478D0 => 725, // Litten
    0xF6348EE7 => 728, // Popplio
    0xF6348D34 => 729, // Brionne
    0xF6316979 => 731, // Pikipek
    0xF6316460 => 732, // Trumbeak
    0xF6316613 => 733, // Toucannon
    0xF6316B2C => 736, // Grubbin
    0xF6316CDF => 737, // Charjabug
    0xF6317711 => 739, // Crabrawler
    0xF622F7F9 => 740, // Crabominable
    0xF622F646 => 741, // Oricorio
    0xF622F493 => 742, // Cutiefly
    0xF622FEC5 => 744, // Rockruff
    0xF622FD12 => 745, // Lycanroc
    0xF62303DE => 749, // Mudbray
    0xF61FE023 => 751, // Dewpider
    0xF61FE1D6 => 752, // Araquanid
    0xF61FE389 => 753, // Fomantis
    0xF6284FD8 => 761, // Bounsweet
    0xF6285857 => 764, // Comfey
    0xF6284240 => 769, // Sandygast
    0xF6260AB6 => 774, // Minior
    0xF6260C69 => 775, // Komala
    0xF6261F1A => 778, // Mimikyu
    0xF62620CD => 779, // Bruxish
    0xF6452DBF => 782, // Jangmo-o
    0xF6453D0A => 789, // Cosmog
    0xFB72B208 => 800, // Necrozma
    0xFB72B3BB => 801, // Magearna
    0xFB75CB91 => 810, // Grookey
    0xFB75C678 => 813, // Scorbunny
    0xFB75CEF7 => 816, // Sobble
    0xFB75CD44 => 817, // Drizzile
    0xFB75BDF9 => 818, // Inteleon
    0xFB75BC46 => 819, // Skwovet
    0xFB78E51A => 820, // Greedent
    0xFB78E1B4 => 822, // Corvisquire
    0xFB7BC9F6 => 833, // Chewtle
    0xFB7BCF0F => 834, // Drednaw
    0xFB7BD5DB => 838, // Carkol
    0xFB7E084C => 840, // Applin
    0xFB7E0333 => 845, // Cramorant
    0xFB7E04E6 => 846, // Arrokuda
    0xFB7E15E4 => 848, // Toxel
    0xFB7E1797 => 849, // Toxtricity
    0xFB811B09 => 854, // Sinistea
    0xFB8117A3 => 856, // Hatenna
    0xFB812F6D => 858, // Hatterene
    0xFB812DBA => 859, // Impidimp
    0xFB8439AB => 863, // Perrserker
    0xFB842DC6 => 868, // Milcery
    0xFB842F79 => 869, // Alcremie
    0xFB87569A => 873, // Frosmoth
    0xFB874E1B => 874, // Stonjourner
    0xFB875181 => 876, // Indeedee
    0xFB874FCE => 877, // Morpeko
    0xFB5BD5EC => 884, // Duraludon
    0xFB5BD79F => 885, // Dreepy
    0xFB5BDCB8 => 888, // Zacian
    0xFB5BDE6B => 889, // Zamazenta
    0xFB5EE8A9 => 890, // Eternatus
    0xFB5EE6F6 => 891, // Kubfu
    0xFB5EE543 => 892, // Urshifu
    0xFB5EE390 => 893, // Zarude
    0xFB5EEF75 => 894, // Regieleki
    0xFB5EEDC2 => 895, // Regidrago
    0xFB5EEC0F => 896, // Glastrier
    0xFB5EEA5C => 897, // Spectrier
    0xFB5EF641 => 898, // Calyrex
    0xFF454940 => 901, // Ursaluna
    0xFF454E59 => 902, // Basculegion
    0xFF454CA6 => 903, // Sneasler
    0xFF45500C => 905, // Enamorus
    0xFF455525 => 906, // Sprigatito
    0xFF4556D8 => 909, // Fuecoco
    0xFF4267CA => 910, // Crocalor
    0xFF426464 => 912, // Quaxly
    0xFF426617 => 913, // Quaxwell
    0xFF4262B1 => 915, // Lechonk
    0xFF425A32 => 918, // Spidops
    0xFF40260E => 921, // Pawmi
    0xFF40245B => 922, // Pawmo
    0xFF4022A8 => 923, // Pawmot
    0xFF402CDA => 925, // Maushold
    0xFF401A29 => 928, // Smoliv
    0xFF3D0E38 => 930, // Arboliva
    0xFF3D0FEB => 931, // Squawkabilly
    0xFF3D1504 => 934, // Garganacl
    0xFF3D16B7 => 935, // Charcadet
    0xFF3D1A1D => 937, // Ceruledge
    0xFF50D797 => 940, // Wattrel
    0xFF50DAFD => 942, // Maschiff
    0xFF50D0CB => 944, // Shroodle
    0xFF50CF18 => 945, // Grafaiai
    0xFF50D27E => 947, // Brambleghast
    0xFF50C84C => 949, // Toedscruel
    0xFF4E9941 => 951, // Capsakid
    0xFF4E95DB => 953, // Rellor
    0xFF4E9E5A => 954, // Rabsca
    0xFF4EA00D => 955, // Flittle
    0xFF4E8BA9 => 959, // Tinkaton
    0xFF4B7E05 => 960, // Wiglett
    0xFF4B78EC => 963, // Finizen
    0xFF4B73D3 => 966, // Revavroom
    0xFF4B8B9D => 968, // Orthworm
    0xFF486116 => 976, // Veluza
    0xFF487214 => 978, // Tatsugiri
    0xFF4873C7 => 979, // Annihilape
    0xFF2E9E6B => 980, // Clodsire
    0xFF2E9CB8 => 981, // Farigiraf
    0xFF2EA1D1 => 982, // Dudunsparce
    0xFF2EA01E => 983, // Kingambit
    0xFF2EA537 => 984, // Great Tusk
    0xFF2EA384 => 985, // Scream Tail
    0xFF2EA89D => 986, // Brute Bonnet
    0xFF2EA6EA => 987, // Flutter Mane
    0xFF2E90D3 => 988, // Slither Wing
    0xFF2E8F20 => 989, // Sandy Shocks
    0xFF2B84E2 => 990, // Iron Treads
    0xFF2B8695 => 991, // Iron Bundle
    0xFF2B817C => 992, // Iron Hands
    0xFF2B832F => 993, // Iron Jugulis
    0xFF2B7E16 => 994, // Iron Moth
    0xFF2B7FC9 => 995, // Iron Thorns
    0xFF2B7AB0 => 996, // Frigibax
    0xFF2B7C63 => 997, // Arctibax
    0xFF2B927A => 998, // Baxcalibur
    0xFF2B942D => 999, // Gimmighoul
    0xAF25949E => 1001, // Wo-Chien
    0xAF259138 => 1003, // Ting-Lu
    0xAF2599B7 => 1006, // Iron Valiant
    0xAF259804 => 1007, // Koraidon
    0xAF227E7B => 1011, // Dipplin
    0xAF228394 => 1014, // Okidogi
    0xAF228547 => 1015, // Munkidori
    0xAF2286FA => 1016, // Fezandipiti
    0xAF2288AD => 1017, // Ogerpon
    0xAF226F30 => 1018, // Archaludon
    0xAF2270E3 => 1019, // Hydrapple
    0xAF2B9303 => 1020, // Gouging Fire
    0xAF2B9150 => 1021, // Raging Bolt
    0xAF2B9669 => 1022, // Iron Boulder
    0xAF2B99CF => 1024, // Terapagos
    0xAF2B981C => 1025, // Pecharunt
    _ => 0,
  };

  /// <summary>
  /// Returns the capture bonus block key for a national Pokédex species number,
  /// or 0 if the species has no capture bonus tracker.
  /// </summary>
  public static uint GetKeyForSpecies(ushort species) => species switch
  {
    1 => 0x7E01F663,
    2 => 0x7E01F816,
    4 => 0x7E01FB7C,
    7 => 0x7E020095,
    8 => 0x7E020248,
    9 => 0x7E0203FB,
    23 => 0x1D58A6DF,
    25 => 0x1D58A379,
    26 => 0x1D589E60,
    27 => 0x1D58A013,
    28 => 0x1D58B62A,
    36 => 0x1D5BCC4D,
    37 => 0x1D5BCA9A,
    39 => 0x1D5BB2D0,
    43 => 0x1D5E073D,
    44 => 0x1D5DFB58,
    45 => 0x1D5DFD0B,
    48 => 0x1D5DF48C,
    49 => 0x1D5DF63F,
    50 => 0x1D611BAD,
    51 => 0x1D6119FA,
    54 => 0x1D6114E1,
    56 => 0x1D61117B,
    58 => 0x1D610E15,
    59 => 0x1D610C62,
    60 => 0x1D63FED6,
    69 => 0x1D640E21,
    72 => 0x1D671BC5,
    73 => 0x1D671A12,
    75 => 0x1D670FE0,
    76 => 0x1D6714F9,
    79 => 0x1D672444,
    80 => 0x1D6A31E8,
    81 => 0x1D6A339B,
    84 => 0x1D6A38B4,
    85 => 0x1D6A3A67,
    86 => 0x1D6A3C1A,
    87 => 0x1D6A3DCD,
    88 => 0x1D6A2450,
    89 => 0x1D6A2603,
    92 => 0x1D6C6E8B,
    102 => 0xD8626141,
    103 => 0xD8625F8E,
    109 => 0xD8624E90,
    111 => 0xD85F4605,
    112 => 0xD85F40EC,
    113 => 0xD85F429F,
    116 => 0xD85F3A20,
    117 => 0xD85F3BD3,
    123 => 0xD85C25B0,
    125 => 0xD85C2FE2,
    126 => 0xD85C2E2F,
    128 => 0xD85C3861,
    129 => 0xD85C36AE,
    131 => 0xD85912F3,
    132 => 0xD85914A6,
    133 => 0xD8591659,
    137 => 0xD8591D25,
    144 => 0xD86DAD53,
    145 => 0xD86DABA0,
    146 => 0xD86DB0B9,
    150 => 0xD86A9A96,
    151 => 0xD86A9C49,
    152 => 0xD86A9730,
    153 => 0xD86A98E3,
    154 => 0xD86AA162,
    155 => 0xD86AA315,
    158 => 0xD86AA82E,
    159 => 0xD86AA9E1,
    161 => 0xD867B5BA,
    162 => 0xD867B407,
    163 => 0xD867B254,
    164 => 0xD867B0A1,
    167 => 0xD867AB88,
    171 => 0xD8649F97,
    172 => 0xD864A14A,
    182 => 0xD8790DC9,
    185 => 0xD8790F7C,
    190 => 0xD876273A,
    193 => 0xD8762587,
    194 => 0xD876206E,
    198 => 0xD87619A2,
    199 => 0xD8761B55,
    200 => 0xDDA68B46,
    203 => 0xDDA68993,
    205 => 0xDDA693C5,
    206 => 0xDDA68EAC,
    207 => 0xDDA6905F,
    209 => 0xDDA69A91,
    210 => 0xDDA9A4CF,
    211 => 0xDDA9A31C,
    214 => 0xDDA99E03,
    216 => 0xDDA9A169,
    218 => 0xDDA9B267,
    228 => 0xDDA080FC,
    229 => 0xDDA082AF,
    230 => 0xDDA3A81D,
    232 => 0xDDA3A4B7,
    234 => 0xDDA3A151,
    235 => 0xDDA39F9E,
    236 => 0xDDA39DEB,
    237 => 0xDDA39C38,
    239 => 0xDDA398D2,
    240 => 0xDD9B3502,
    242 => 0xDD9B319C,
    243 => 0xDD9B334F,
    244 => 0xDD9B2E36,
    245 => 0xDD9B2FE9,
    249 => 0xDD9B444D,
    250 => 0xDD9D750B,
    252 => 0xDD9D7871,
    255 => 0xDD9D7A24,
    256 => 0xDD9D7F3D,
    258 => 0xDD9D6773,
    259 => 0xDD9D65C0,
    261 => 0xDD9503A3,
    262 => 0xDD950556,
    270 => 0xDD981B79,
    273 => 0xDD981660,
    274 => 0xDD982245,
    278 => 0xDD982911,
    280 => 0xDDBD6E2E,
    283 => 0xDDBD6C7B,
    284 => 0xDDBD74FA,
    287 => 0xDDBD7347,
    288 => 0xDDBD6096,
    289 => 0xDDBD6249,
    299 => 0xDDC0786C,
    311 => 0xE2E7D73B,
    313 => 0xE2E7DAA1,
    314 => 0xE2E7DC54,
    322 => 0xE2F0EF29,
    323 => 0xE2F0ED76,
    324 => 0xE2F0F28F,
    325 => 0xE2F0F0DC,
    328 => 0xE2F0F95B,
    329 => 0xE2F0F7A8,
    330 => 0xE2EE089A,
    333 => 0xE2EE06E7,
    335 => 0xE2EE0381,
    336 => 0xE2EDFE68,
    339 => 0xE2EDFCB5,
    341 => 0xE2F643A2,
    349 => 0xE2F6513A,
    353 => 0xE2F330E5,
    354 => 0xE2F32500,
    355 => 0xE2F326B3,
    358 => 0xE2F33964,
    361 => 0xE2FC76B4,
    370 => 0xE2F95EDE,
    375 => 0xE2F9675D,
    377 => 0xE2F963F7,
    378 => 0xE2F95146,
    379 => 0xE2F952F9,
    380 => 0xE2D40C29,
    381 => 0xE2D40A76,
    382 => 0xE2D408C3,
    383 => 0xE2D40710,
    384 => 0xE2D412F5,
    385 => 0xE2D41142,
    386 => 0xE2D40F8F,
    387 => 0xE2D40DDC,
    388 => 0xE2D419C1,
    390 => 0xE2D0F2A0,
    393 => 0xE2D0F7B9,
    396 => 0xE2D0FCD2,
    403 => 0xE6BD8D15,
    404 => 0xE6BD8130,
    411 => 0xE6C09FD2,
    415 => 0xE6C09906,
    417 => 0xE6C095A0,
    433 => 0xE6C5FCCA,
    436 => 0xE6C5F7B1,
    438 => 0xE6C5ED7F,
    440 => 0xE6B231B8,
    442 => 0xE6B2351E,
    443 => 0xE6B236D1,
    446 => 0xE6B23BEA,
    447 => 0xE6B23D9D,
    456 => 0xE6B54EA7,
    457 => 0xE6B54CF4,
    459 => 0xE6B53BF6,
    460 => 0xE6B78B4A,
    462 => 0xE6B787E4,
    464 => 0xE6B7847E,
    471 => 0xE6BA6CC0,
    479 => 0xE6BA7A58,
    480 => 0xE6A6DB74,
    481 => 0xE6A6DD27,
    482 => 0xE6A6DEDA,
    483 => 0xE6A6E08D,
    484 => 0xE6A6D4A8,
    485 => 0xE6A6D65B,
    486 => 0xE6A6D80E,
    487 => 0xE6A6D9C1,
    488 => 0xE6A6CDDC,
    490 => 0xE6A91B7D,
    491 => 0xE6A919CA,
    492 => 0xE6A91817,
    493 => 0xE6A91664,
    495 => 0xE6A912FE,
    496 => 0xE6A9114B,
    498 => 0xE6A90DE5,
    499 => 0xE6A90C32,
    500 => 0xEC01EBC7,
    501 => 0xEC01EA14,
    522 => 0xEBFC8ECF,
    523 => 0xEBFC8D1C,
    529 => 0xEBFC9E1A,
    530 => 0xEBF978AC,
    532 => 0xEBF97C12,
    540 => 0xEBF65F23,
    541 => 0xEBF65D70,
    542 => 0xEBF66289,
    546 => 0xEBF66955,
    548 => 0xEBF66CBB,
    551 => 0xEBF37DAD,
    552 => 0xEBF37894,
    553 => 0xEBF37A47,
    570 => 0xEBEE2268,
    572 => 0xEBEE25CE,
    574 => 0xEBEE2934,
    575 => 0xEBEE2AE7,
    576 => 0xEBEE2C9A,
    577 => 0xEBEE2E4D,
    580 => 0xEBEB08DF,
    585 => 0xEBEB0060,
    586 => 0xEBEB0579,
    590 => 0xEBE7EF56,
    594 => 0xEBE7F622,
    596 => 0xEBE7F2BC,
    603 => 0xF0EA1F1F,
    604 => 0xF0EA1A06,
    607 => 0xF0EA1853,
    613 => 0xF0ED3C0E,
    614 => 0xF0ED4127,
    619 => 0xF0ED2B10,
    622 => 0xF0E3F126,
    624 => 0xF0E3F48C,
    625 => 0xF0E3F63F,
    627 => 0xF0E3F9A5,
    628 => 0xF0E3FB58,
    629 => 0xF0E3FD0B,
    630 => 0xF0E70749,
    636 => 0xF0E70AAF,
    638 => 0xF0E714E1,
    639 => 0xF0E7132E,
    640 => 0xF0F57716,
    641 => 0xF0F578C9,
    642 => 0xF0F573B0,
    643 => 0xF0F57563,
    644 => 0xF0F57DE2,
    645 => 0xF0F57F95,
    646 => 0xF0F57A7C,
    647 => 0xF0F57C2F,
    648 => 0xF0F584AE,
    650 => 0xF0F8909F,
    653 => 0xF0F89252,
    654 => 0xF0F889D3,
    656 => 0xF0F88D39,
    658 => 0xF0F89E37,
    661 => 0xF0EF7C17,
    662 => 0xF0EF7DCA,
    663 => 0xF0EF7F7D,
    664 => 0xF0EF7398,
    667 => 0xF0EF78B1,
    668 => 0xF0EF6CCC,
    672 => 0xF0F29087,
    677 => 0xF0F28808,
    678 => 0xF0F28655,
    686 => 0xF100F988,
    687 => 0xF100FB3B,
    700 => 0xF62E849D,
    704 => 0xF62E7DD1,
    705 => 0xF62E7C1E,
    708 => 0xF62E7705,
    713 => 0xF62B702D,
    714 => 0xF62B6448,
    719 => 0xF62B5F2F,
    720 => 0xF634814F,
    721 => 0xF6347F9C,
    722 => 0xF63484B5,
    725 => 0xF63478D0,
    728 => 0xF6348EE7,
    729 => 0xF6348D34,
    731 => 0xF6316979,
    732 => 0xF6316460,
    733 => 0xF6316613,
    736 => 0xF6316B2C,
    737 => 0xF6316CDF,
    739 => 0xF6317711,
    740 => 0xF622F7F9,
    741 => 0xF622F646,
    742 => 0xF622F493,
    744 => 0xF622FEC5,
    745 => 0xF622FD12,
    749 => 0xF62303DE,
    751 => 0xF61FE023,
    752 => 0xF61FE1D6,
    753 => 0xF61FE389,
    761 => 0xF6284FD8,
    764 => 0xF6285857,
    769 => 0xF6284240,
    774 => 0xF6260AB6,
    775 => 0xF6260C69,
    778 => 0xF6261F1A,
    779 => 0xF62620CD,
    782 => 0xF6452DBF,
    789 => 0xF6453D0A,
    800 => 0xFB72B208,
    801 => 0xFB72B3BB,
    810 => 0xFB75CB91,
    813 => 0xFB75C678,
    816 => 0xFB75CEF7,
    817 => 0xFB75CD44,
    818 => 0xFB75BDF9,
    819 => 0xFB75BC46,
    820 => 0xFB78E51A,
    822 => 0xFB78E1B4,
    833 => 0xFB7BC9F6,
    834 => 0xFB7BCF0F,
    838 => 0xFB7BD5DB,
    840 => 0xFB7E084C,
    845 => 0xFB7E0333,
    846 => 0xFB7E04E6,
    848 => 0xFB7E15E4,
    849 => 0xFB7E1797,
    854 => 0xFB811B09,
    856 => 0xFB8117A3,
    858 => 0xFB812F6D,
    859 => 0xFB812DBA,
    863 => 0xFB8439AB,
    868 => 0xFB842DC6,
    869 => 0xFB842F79,
    873 => 0xFB87569A,
    874 => 0xFB874E1B,
    876 => 0xFB875181,
    877 => 0xFB874FCE,
    884 => 0xFB5BD5EC,
    885 => 0xFB5BD79F,
    888 => 0xFB5BDCB8,
    889 => 0xFB5BDE6B,
    890 => 0xFB5EE8A9,
    891 => 0xFB5EE6F6,
    892 => 0xFB5EE543,
    893 => 0xFB5EE390,
    894 => 0xFB5EEF75,
    895 => 0xFB5EEDC2,
    896 => 0xFB5EEC0F,
    897 => 0xFB5EEA5C,
    898 => 0xFB5EF641,
    901 => 0xFF454940,
    902 => 0xFF454E59,
    903 => 0xFF454CA6,
    905 => 0xFF45500C,
    906 => 0xFF455525,
    909 => 0xFF4556D8,
    910 => 0xFF4267CA,
    912 => 0xFF426464,
    913 => 0xFF426617,
    915 => 0xFF4262B1,
    918 => 0xFF425A32,
    921 => 0xFF40260E,
    922 => 0xFF40245B,
    923 => 0xFF4022A8,
    925 => 0xFF402CDA,
    928 => 0xFF401A29,
    930 => 0xFF3D0E38,
    931 => 0xFF3D0FEB,
    934 => 0xFF3D1504,
    935 => 0xFF3D16B7,
    937 => 0xFF3D1A1D,
    940 => 0xFF50D797,
    942 => 0xFF50DAFD,
    944 => 0xFF50D0CB,
    945 => 0xFF50CF18,
    947 => 0xFF50D27E,
    949 => 0xFF50C84C,
    951 => 0xFF4E9941,
    953 => 0xFF4E95DB,
    954 => 0xFF4E9E5A,
    955 => 0xFF4EA00D,
    959 => 0xFF4E8BA9,
    960 => 0xFF4B7E05,
    963 => 0xFF4B78EC,
    966 => 0xFF4B73D3,
    968 => 0xFF4B8B9D,
    976 => 0xFF486116,
    978 => 0xFF487214,
    979 => 0xFF4873C7,
    980 => 0xFF2E9E6B,
    981 => 0xFF2E9CB8,
    982 => 0xFF2EA1D1,
    983 => 0xFF2EA01E,
    984 => 0xFF2EA537,
    985 => 0xFF2EA384,
    986 => 0xFF2EA89D,
    987 => 0xFF2EA6EA,
    988 => 0xFF2E90D3,
    989 => 0xFF2E8F20,
    990 => 0xFF2B84E2,
    991 => 0xFF2B8695,
    992 => 0xFF2B817C,
    993 => 0xFF2B832F,
    994 => 0xFF2B7E16,
    995 => 0xFF2B7FC9,
    996 => 0xFF2B7AB0,
    997 => 0xFF2B7C63,
    998 => 0xFF2B927A,
    999 => 0xFF2B942D,
    1001 => 0xAF25949E,
    1003 => 0xAF259138,
    1006 => 0xAF2599B7,
    1007 => 0xAF259804,
    1011 => 0xAF227E7B,
    1014 => 0xAF228394,
    1015 => 0xAF228547,
    1016 => 0xAF2286FA,
    1017 => 0xAF2288AD,
    1018 => 0xAF226F30,
    1019 => 0xAF2270E3,
    1020 => 0xAF2B9303,
    1021 => 0xAF2B9150,
    1022 => 0xAF2B9669,
    1024 => 0xAF2B99CF,
    1025 => 0xAF2B981C,
    _ => 0,
  };

  /// <summary>
  /// Returns <see langword="true"/> if <paramref name="sav"/> is a Pokémon Compass save.
  /// Detection priority: unique save-format-version block first, then save size.
  /// </summary>
  public static bool IsCompassSave(SAV9SV sav) =>
      sav.Blocks.HasBlock(KSaveFormatVersion) ||
      sav.Data.Length == SaveUtil.SIZE_G9_COMPASS_210;


  /// <summary>
  /// Reads the Capture Bonus count for a single block key. Returns 0 if the block is absent.
  /// </summary>
  public static int GetCaptureBonus(SCBlockAccessor blocks, uint key)
  {
    if (!blocks.TryGetBlock(key, out var block))
      return 0;
    if (block.Type != SCTypeCode.SByte)
      return 0;
    return (sbyte)block.GetValue();
  }

  /// <summary>
  /// Returns the number of capture-bonus blocks that have reached the maximum.
  /// </summary>
  public static int CountMaxedBonuses(SCBlockAccessor blocks)
  {
    int count = 0;
    foreach (var key in CaptureBonusKeys)
    {
      if (GetCaptureBonus(blocks, key) >= 25)
        count++;
    }
    return count;
  }

  /// <summary>
  /// Returns the total captures recorded across all Capture Bonus blocks.
  /// </summary>
  public static int TotalCaptures(SCBlockAccessor blocks)
  {
    int total = 0;
    foreach (var key in CaptureBonusKeys)
      total += GetCaptureBonus(blocks, key);
    return total;
  }
}
