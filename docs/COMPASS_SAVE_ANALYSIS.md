# Pokémon Compass Save File Analysis

Analysis of Pokémon Compass (ROM hack) vs "Vanilla" Scarlet/Violet save data.
Derived from SCBlock-level comparison using PKHeX.Core's SwishCrypto.
Cross-referenced with the [v2.1.x Primer](https://compass.seri.studio/2100-primer/) and [full documentation](https://compass.seri.studio/).

# Disclaimer

This analysis is an independent project. It is not affiliated with, endorsed by, or supported by the Pokémon Compass developers or the Project Pokémon (PKHeX) developers.

Please do not ask for support for this documentation, PKCompassHex or any other related tools within the official Inidar Academy Discord server or Project Pokémon forms. The Compass developers have stated they do not provide support for this tool or any other external documentation projects. Asking for support in these resources has a chance for permanent moderation. (I myself am banned from the Inidar Academy Discord server for asking for documentation, so make as you will with that)

Any questions/support should be reported to the GitHub repo. I will NOT respond to any other platform or repository about this project. 

---

## Table of Contents

1. [Overview](#overview)
2. [Block Key Cryptanalysis](#block-key-cryptanalysis)
3. [File-Level Differences](#file-level-differences)
4. [Block Count Summary](#block-count-summary)
5. [Compass-Only Blocks (483)](#compass-only-blocks-483)
   - [Object Blocks](#object-blocks)
   - [SByte Blocks (439)](#sbyte-blocks-439)
   - [UInt64 Blocks (27)](#uint64-blocks-27)
   - [Boolean Blocks (12)](#boolean-blocks-12)
   - [Int32 Blocks (1)](#int32-blocks-1)
6. [Vanilla-Only Blocks (2)](#vanilla-only-blocks-2)
7. [Structural Differences (~280 Bool Swaps)](#structural-differences-280-bool-swaps)
8. [Key Block Size Comparison](#key-block-size-comparison)
9. [PKHeX Compatibility Changes](#pkhex-compatibility-changes)
10. [Open Questions](#open-questions)
11. [Appendix: Full Compass-Only Block Listing](#appendix-full-compass-only-block-listing)
12. [Reproduction Guide](#reproduction-guide)
---

## Overview

Pokémon Compass is a ROM hack of Pokémon Scarlet/Violet. Version 2.1.0.0 introduced exeFS modifications alongside romFS changes, including new code systems (Capture Bonuses, shiny indicator, level cap, experience rate modifiers, spawn system overhaul) that added new save blocks. The current PKHeX project does not support these changes. Which is where this comes into play.

Scarlet & Violet (& Compass) saves use the **SwishCrypto** encryption system (SHA256 hash + static XOR pad + per-block XorShift32 cipher). Raw binary comparison is meaningless, so we must decrypt them first and compare at the **SCBlock** level. Both test saves decrypted successfully with valid SHA256 hashes.

### SCBlock System

Scarlet & Violet saves consist of a list of **SCBlocks**, each identified by a `uint32` key (an FNV-1a hash). Each block has:

- **Key**: `uint32` identifier
- **Type**: `SCTypeCode` enum - `Bool1` (false), `Bool2` (true), `SByte`, `Int32`, `UInt64`, `Object`, `Array`, etc.
- **Data**: byte span (length depends on type; booleans have no data bytes)

Boolean blocks encoded their value directly in the type field: `Bool1 = false`, `Bool2 = true`.

---

## Block Key Cryptanalysis

### FNV-1a Hash Function (PKHeX variant)

PKHeX uses a **custom FNV-1a-64** hash for SCBlock key generation, truncated to `uint32`.

| Parameter | Value |
|-----------|-------|
| Algorithm | FNV-1a-64 |
| Offset basis | `0xCBF29CE484222645` |
| Prime | `0x00000100000001B3` |
| Output | Lower 32 bits of 64-bit hash |

> **CRITICAL**: The offset basis differs from the standard FNV-1a-64 value (`0xCBF29CE484222325`). The difference is in the last two bytes: `2645` vs `2325`. PKHeX's implementation is in `PKHeX.Core/Saves/Encryption/SwishCrypto/FnvHash.cs`.

Verification: `hash("") = 0x84222645`, which equals `KSaveFormatVersion`. This is the hash of an empty string, confirming the offset value.

### Capture Bonus Keys: `Compass_RNGSkew_{NationalDexNumber}`

Each species capture bonus block is keyed by the hash of `Compass_RNGSkew_{N}`, where `N` is the **National Dex number** (no zero padding).

| String | FNV-1a Hash (uint32) | Species |
|--------|---------------------|---------|
| `Compass_RNGSkew_1` | `0x7E01F663` | Bulbasaur |
| `Compass_RNGSkew_25` | `0x1D58A379` | Pikachu |
| `Compass_RNGSkew_133` | `0xD8591659` | Eevee |
| `Compass_RNGSkew_150` | `0xD86A9A96` | Mewtwo |
| `Compass_RNGSkew_906` | `0xFF455525` | Sprigatito |
| `Compass_RNGSkew_1025` | `0xAF2B981C` | Pecharunt |

All 426 capture bonus keys follow this pattern. Of the 1025 national Pokédex numbers, 426 have capture bonus keys; the rest are not listed in the Compass data.

The Lua bytecode constructs these strings dynamically at runtime by concatenating `Compass_RNGSkew_` with a species number variable. Full strings do not appear as literals in the binary dump.

### Setting Keys: `Compass_{SettingName}`

| String | FNV-1a Hash | Purpose |
|--------|------------|---------|
| `Compass_shiny_spoiler` | `0x4D38ED1E` | Shiny notification mode (0=Spoiler-Free, 1=Full, 2=Off) |
| `Compass_TrainerSeed` | `0xA9296A9C` | 257-byte deterministic team seed table |
| `Compass_RNGSkew` | `0xB1366CBF` | Capture Bonuses toggle (0=On, 1=Off) - base setting, no species suffix |
| `Compass_PicnicExp` | `0x1C25C049` | Picnic Experience (0=On, 1=Off) |
| `Compass_BattleCam` | `0x34780809` | Battle camera setting (values unconfirmed) |
| `Compass_LetsGoEV` | `0x46E0EDCB` | Let's Go EV target (0=Party, 1=Leader, 2=Disabled) |
| `Compass_Levelcap` | `0x837B300E` | Level cap (0=No Cap/Level 100, 1–99=hard cap at that level) |
| `Compass_Expmulti` | `0x8B12B6B8` | Experience multiplier (value × 10 + 60 = %; range 0–9 → 60%–150%) |
| `Compass_ColorProfile` | `0x92569278` | Color profile setting (values unconfirmed) |
| `Compass_SpawnRate` | `0x96C58F45` | Max Pokémon spawns (value × 10 + 10; 0=10, 1=20, 2=30, 3=40, ...) |
| `Compass_Expshare` | `0xCC806ED6` | Exp. Share (0=On, 1=Off) |
| `Compass_CamDither` | `0xD2E638A3` | Camera dither setting (values unconfirmed) |
| `Compass_AnimRate` | `0xFB067137` | Animation quality (0=High, 1=Medium, 2=Low; more values may exist) |
| `Compass_PEPPERTALKALREADY` | `0xD73E5336` | Story flag (SByte, value=1) - purpose unknown |

#### Confirmed Setting Value Mappings (from in-game toggle testing)

| Block Key | Name | Values | Notes |
|-----------|------|--------|-------|
| `0x837B300E` | `Compass_Levelcap` | 0=No Cap, 1–99=Level 1–99 | 0 allows level 100 as normal |
| `0x8B12B6B8` | `Compass_Expmulti` | 0=60%, 1=70%, …, 9=150% | Formula: `value x 10 + 60`%; max is 150% |
| `0x46E0EDCB` | `Compass_LetsGoEV` | 0=Party, 1=Leader, 2=Disabled | Controls which Pokémon gain EVs in Let's Go |
| `0xCC806ED6` | `Compass_Expshare` | 0=On, 1=Off | |
| `0x1C25C049` | `Compass_PicnicExp` | 0=On, 1=Off | |
| `0xB1366CBF` | `Compass_RNGSkew` | 0=On, 1=Off | Global capture bonus enable/disable |
| `0x96C58F45` | `Compass_SpawnRate` | 1=20, 2=30, 3=40 (more possible) | Formula: `value x 10 + 10` spawns |
| `0xFB067137` | `Compass_AnimRate` | 0=High, 1=Medium, 2=Low | There are more values which I will add soon |

#### Blocks Co-changing with Level Cap (100→99 transition)

When changing Level Cap from 100 (no cap, value=0) to 99, the following additional blocks change:

| Block Key | Change | Notes |
|-----------|--------|-------|
| `0x837B300E` | 0 → 99 | Primary level cap value |
| `0x8B0D3ACE` | Bool1 → Bool2 | Shared block; purpose unconfirmed - may be a "cap is active" flag |
| `0x40EECC82` | 93 → 83 | Shared value block; purpose unconfirmed - changes with cap state |
| Zukan block(s) | Bool1 ↔ Bool2 | Two Pokédex-related booleans swap; likely incidental save-state difference |

> **Note**: `0x8B0D3ACE` and `0x40EECC82` were previously attributed in this document to the Shiny Notification setting. They may change with multiple settings. The confirmed observation here is that they change when the level cap is toggled between 100 (disabled) and 99.


### Bool Feature Keys

| String | FNV-1a Hash | Bool State |
|--------|------------|-----------|
| `compass_displayTip_CaptureBonuses` | `0x4197A769` | Bool2 (true) |
| `Compass_PERRINPARADOX` | `0x72958FC5` | Bool2 (true) |
| `nixskip_selfie` | `0x74043B2C` | Bool2 (true) |
| `compass_newTip_PhoePrices` | `0x78A49D0A` | Bool1 (false) |
| `compass_displayTip_Obedience` | `0x7FD7214C` | Bool2 (true) |
| `roo_flag` | `0x8F8F4A1E` | Bool1 (false) |
| `compass_newTip_NewMaps` | `0xA371E2C1` | Bool2 (true) |
| `compass_displayTip_NewMaps` | `0xAAC9AAD1` | Bool2 (true) |
| `compass_newTip_Obedience` | `0xB3745A5C` | Bool2 (true) |
| `yinskip_swift` | `0xBE24A8C0` | Bool2 (true) |
| `compass_displayTip_PhoePrices` | `0xD555F09A` | Bool1 (false) |
| `compass_newTip_CaptureBonuses` | `0xD93612B9` | Bool2 (true) |

### Clustering

The SByte block keys cluster into groups with the shared upper byte prefixes. Since the source strings are `Compass_RNGSkew_{N}` with sequential `N`, species with adjacent dex numbers should produce adjacent hash values.

| Prefix Range | Species Range |
|-------------|---------------|
| `0x1D58xxxx`–`0x1D6Cxxxx` | Gen 1 (#23–92) |
| `0x7E01xxxx`–`0x7E02xxxx` | Gen 1 (#1–9) |
| `0xAF22xxxx`–`0xAF2Bxxxx` | Gen 9 DLC (#1001–1025) |
| `0xD859xxxx`–`0xD87Axxxx` | Gen 1–2 (#102–199) |
| `0xDD95xxxx`–`0xDDC0xxxx` | Gen 2–3 (#228–299) |
| `0xE2D0xxxx`–`0xE2FCxxxx` | Gen 3–4 (#311–396) |
| `0xE6A6xxxx`–`0xE6C6xxxx` | Gen 4 (#403–499) |
| `0xEBE7xxxx`–`0xEC02xxxx` | Gen 5 (#500–596) |
| `0xF0E3xxxx`–`0xF101xxxx` | Gen 5–6 (#603–687) |
| `0xF61Fxxxx`–`0xF645xxxx` | Gen 7 (#700–789) |
| `0xFB5Bxxxx`–`0xFB87xxxx` | Gen 8 (#800–898) |
| `0xFF2Bxxxx`–`0xFF51xxxx` | Gen 8–9 (#901–999) |

---

## File-Level Differences

| Property | Compass | Vanilla | Delta |
|----------|---------|---------|-------|
| File size | 4,438,720 bytes (0x43BAC0) | 4,436,579 bytes (0x43B263) | +2,141 bytes |
| SHA256 hash valid | True | True | - |
| Total SCBlocks | 10,411 | 9,930 | +481 |

The +2,141 byte size difference accounts for the 483 additional Compass blocks (each SByte block adds ~6 bytes: 4-byte key + 1-byte type + 1-byte data).

---

## Block Count Summary

| Category | Count |
|----------|-------|
| Blocks only in Compass | 483 |
| Blocks only in Vanilla | 2 |
| Shared blocks (same key) | 9,928 |
| Shared with type mismatch (bool swaps) | ~280 |
| Shared with data differences | 2,828 |
| Shared with identical data | 6,800 |

---

## Compass-Only Blocks (483)

### Object Blocks

#### "ajito_aku_050" Blocks (3 instances, 32 bytes each)

| Key | Data |
|-----|------|
| `0x08C07101` | `"None"` + `"ajito_aku_050"` |
| `0x4DBFD14C` | `"None"` + `"ajito_aku_050"` |
| `0x9B19CB47` | `"None"` + `"ajito_aku_050"` |

**Structure**: Two 16-byte null-padded ASCII strings:
- Bytes 0–15: `"None"` (state field)
- Bytes 16–31: `"ajito_aku_050"` (identifier)

`ajito` (アジト) = "hideout/base" - possible naming convention for Team Star bases in SV. `aku` (悪) = "evil/dark" - refers to Giacomo's Dark-type base. `_050` is a numerical index for an encounter or team configuration.

Compass v2.1.0.0 adds multiple battle teams per Team Star leader with singles/doubles/random selection. The three copies with different keys probably correspond to three subsystems referencing the encounter data.

#### Deterministic Team Seed Table (257 bytes)

| Key | Type | Size |
|-----|------|------|
| `0xA9296A9C` (`Compass_TrainerSeed`) | Object | 257 bytes |

Contains base64-encoded binary data (~192 bytes decoded). This is the deterministic team selection seed table. Used for random teams.

> **INFO** - potentially can be used to produce the same experience across two Compass saves.

### SByte Blocks (439) {#sbyte-blocks-439}

439 SByte blocks split into four categories:

1. **426 Capture Bonus trackers** - species capture counts (0–25), managed by `Compass_RNGSkew_{NationalDexNumber}.`
2. **11 Compass gameplay settings** - mod configuration values keyed by `Compass_{SettingName}` 
3. **1 Shiny Notification setting** - `Compass_shiny_spoiler` (`0x4D38ED1E`)
4. **1 Story flag** - `Compass_PEPPERTALKALREADY` (`0xD73E5336`)

#### Capture Bonus Value Distribution

| Value | Count (approx) | Meaning |
|-------|-----------------|---------|
| 0x00 | ~10 | Not yet caught |
| 0x01 | ~280 | Caught once |
| 0x02 | ~80 | Caught twice |
| 0x03–0x04 | ~20 | 3–4 captures |
| 0x13 (19) | 1 | Sinistea (#854) |
| 0x19 (25) | 1 | Terapagos (#1024) - HA + shiny bonus unlocked |

At 25 capture,s a species gains a 25% Hidden Ability chance and an additional shiny roll.

### UInt64 Blocks (27)

| Key | Value | Key | Value |
|-----|-------|-----|-------|
| `0x02EEF680` | 0 | `0x94D0D569` | 0 |
| `0x0FB466E0` | 0 | `0x9A085043` | 0 |
| `0x1194092E` | 0 | `0x9A4CC475` | 0 |
| `0x11CBC209` | 0 | `0xA0B96EEB` | 0 |
| `0x54DB99E0` | **4** | `0xA4F26D47` | **4** |
| `0x552ACEFC` | 0 | `0xB1C035CB` | 0 |
| `0x584EC3C5` | 0 | `0xB1F0151A` | 0 |
| `0x59CE6487` | **1** | `0xC3CD694D` | 0 |
| `0x60E95C49` | 0 | `0xC4B67782` | **1** |
| `0x7D845059` | **4** | `0xD27D3F34` | 0 |
| `0x8DB69836` | 0 | `0xD455B377` | 0 |
| `0x8F4E359D` | **1** | `0xD64B10DF` | 0 |
| | | `0xEDBE4224` | 0 |
| | | `0xF0A3C0A7` | 0 |
| | | `0xFE3C16CC` | 0 |

Non-zero values are exclusively **1** or **4** in my test save. None of these keys match known PKHeX constants. No clue what these are (see [Open Questions](#open-questions)).

### Boolean Blocks (12)

| Key | Type | Hash Input |
|-----|------|-----------|
| `0x4197A769` | Bool2 (true) | `compass_displayTip_CaptureBonuses` |
| `0x72958FC5` | Bool2 (true) | `Compass_PERRINPARADOX` |
| `0x74043B2C` | Bool2 (true) | `nixskip_selfie` |
| `0x78A49D0A` | Bool1 (false) | `compass_newTip_PhoePrices` |
| `0x7FD7214C` | Bool2 (true) | `compass_displayTip_Obedience` |
| `0x8F8F4A1E` | Bool1 (false) | `roo_flag` |
| `0xA371E2C1` | Bool2 (true) | `compass_newTip_NewMaps` |
| `0xAAC9AAD1` | Bool2 (true) | `compass_displayTip_NewMaps` |
| `0xB3745A5C` | Bool2 (true) | `compass_newTip_Obedience` |
| `0xBE24A8C0` | Bool2 (true) | `yinskip_swift` |
| `0xD555F09A` | Bool1 (false) | `compass_displayTip_PhoePrices` |
| `0xD93612B9` | Bool2 (true) | `compass_newTip_CaptureBonuses` |

The `displayTip_*` and `newTip_*` pairs track whether a Tips screen has been shown and whether it should display as "new". The three `Bool1` (false) blocks are tips not yet triggered in this save.

### Int32 Blocks (1)

| Key | Value | Hash Input |
|-----|-------|-----------|
| `0x84222645` | 2 | `""` (empty string) |

`0x84222645` is the FNV-1a hash of an empty string - the PKHeX offset basis `0xCBF29CE484222645` truncated to `uint32`. This block is the **Compass save format version**; value `2` identifies a v2.x Compass save, I presume. Used by `CompassBlockKeys.IsCompassSave()` as the primary detection key.

---

## Vanilla-Only Blocks (2)

### WSYS_PARTNER_WALK_COUNT (`0x0CB2BF62`)

| Property | Value |
|----------|-------|
| Type | Int32 |
| Value | 0 |

The Let's Go partner walk counter. Absent from Compass because v2.1.0.0 reworks the Auto Battle (Let's Go) system with new EV gain tracking and an Options menu setting for Auto Battle EVs (Party/Leader/Disabled). The walk counter is superseded by this new system.

### Unnamed Large Object Block (`0xB00BFE35`)

| Property | Value |
|----------|-------|
| Type | Object |
| Size | 1,284 bytes (0x504) |
| Content | Mostly zeroed; bytes at offset 18–19 = `0xA9, 0x34` |

Present in the vanilla blank blocks template (`BlankBlocks9.cs`). Absent from Compass's near-zero content in vanilla suggests it is just an allocated space. Or it could've been a tracker for a vanilla feature replaced or removed by Compass's reworked trainer/raid systems.

---

## Structural Differences (~280 Bool Swaps)

Approximately 280 shared blocks have their boolean type swapped between `Bool1` (false) and `Bool2` (true) relative to vanilla. These fall into identifiable categories:

### Fly Points (Map Fast Travel) - 16+ blocks

All `Bool1` in Compass, `Bool2` in vanilla. Just a small difference between my two tests saves.

| Constant | Key |
|----------|-----|
| `FSYS_YMAP_FLY_05` | `0xEB59835C` |
| `FSYS_YMAP_FLY_06` | `0xEB598875` |
| `FSYS_YMAP_FLY_13` | `0xEB569967` |
| `FSYS_YMAP_FLY_16`–`30` | various |
| `FSYS_YMAP_FLY_35` | `0xEB507087` |
| `FSYS_YMAP_FLY_MAGATAMA` | `0x1530B53C` |
| `FSYS_YMAP_FLY_UTSUWA` | `0x1EB92B72` |
| `FSYS_YMAP_MAGATAMA` | `0x159DC19E` |
| `FSYS_YMAP_MOKKAN` | `0x22587DBC` |

### Multiplayer BGM Unlocks - 21 blocks

`KUnlockedMultiplayerBGM35`–`57` are `Bool2` (unlocked) in Compass and `Bool1` (locked) in vanilla. Compass pre-unlocks BGM tracks as a QOL change.

### Tutorial/Tips Flags - 20+ blocks

`FSYS_TIPS_NEW_*` / `FSYS_TIPS_DISP_*` flags differ in mixed directions, because of Compass's modified tutorial flow and early-game scene skips. 

### DLC Content (Teal Mask / Indigo Disk) - 20+ blocks

| Category | Notes |
|----------|-------|
| Ogre Clan Rewards (`KOgreClanReward1`–`7`) | Compass adds Ogre Clan as repeatable battles; reward flags reflect this |
| Terrarium Starters | Compass moves starters into the normal spawn set; unlock flags differ accordingly |
| Synchro Machine (`KUnlockedSynchroMachine`) | Compass provides early access prior to DLC2 |
| Ogre Oustin (`KOgreOustinClearedHard`, etc.) | Rewards overhauled; first reward replaced with Rare Candies |
| DLC Emotes (`KUnlockedDLCEmote03`–`05`) | Granted via scene skip item compensation |

### Game Progression / Story Events - 15+ blocks. These are likely just changes due to the two test save points in the game.

| Constant | Key |
|----------|-----|
| `KGameClearIndigoDisk` | `0x0DDBBAAF` |
| `KBattledStellarTerapagosOnce` | `0x04A230FA` |
| `KReceivedMasterRankRibbon` | `0x44CA754B` |
| `FEVT_CHAMP_HAVE_FAILED_INTERVIEW` | `0x10E3E483` |
| `FEVT_GYM_KUSA_TEST_BATTLE03_WIN` | `0x1245C70B` |
| `KCapturedExTitanTatsugiri` | `0x2D7B5238` |

### Treasure Stakes (Ruinous Quartet) - 4 blocks

| Constant | Key | Compass | Vanilla |
|----------|-----|---------|---------|
| `KRemovedStakeTingLu1` | `0x12AC859B` | Bool1 | Bool2 |
| `KRemovedStakeTingLu8` | `0x32E24C38` | Bool1 | Bool2 |
| `KRemovedStakeWoChien6` | `0x27CB724C` | Bool1 | Bool2 |
| `KRemovedStakeWoChien7` | `0x0470BDF7` | Bool1 | Bool2 |

### SUSHI_DAMMY Flags - 6 blocks

`SUSHI_DAMMY_03`, `_04`, `_07`, `_09`, `_17`, `_18` differ between saves. These may have been repurposed by Compass as event flags for new encounter states. 

### Recipe / Herba Mystica Unlocks - 3 blocks

`KUnlockedRecipesHerbaMystica2`, `4`, `5` - unlock states differ.

### Unidentified Boolean Flags - ~6 blocks

Keys `0x0599C3C6`, `0x1922C87D`, `0x1979234E`, `0x197926B4`, `0x20A6E59D`, `0x3FCD0B56` do not match any known named constants.

---

## Key Block Size Comparison

All major structural blocks have identical sizes between Compass and Vanilla:

| Block | Key | Size |
|-------|-----|------|
| KBox (Box Pokémon) | `0x0D66012C` | 340,560 bytes |
| KParty (Party) | `0x3AA1A9AD` | 2,068 bytes |
| KItem (Inventory) | `0x21C9BD44` | 48,000 bytes |
| KMyStatus (Trainer) | `0xE3E89BD1` | 104 bytes |
| KBoxLayout | `0x19722C89` | 1,122 bytes |
| KPlayTime | `0xEDAFF794` | 12 bytes |
| KZukan (Pokédex) | `0x0DEAAEBD` | 48,420 bytes |
| KBCATRaidEnemy | `0x0520A1B0` | 30,000 bytes |
| KTeraRaidPaldea | `0xCAAC8800` | 3,224 bytes |
| KTeraRaidDLC | `0x100B93DA` | 6,416 bytes |
| KFieldItems | `0x2482AD60` | 240,000 bytes |
| KOverworld | `0x173304D8` | 9,360 bytes |
| KSandwiches | `0x29B4AED2` | 1,816 bytes |
| KPictureProfile | `0x14C5A101` | 622,080 bytes |

Compass does not alter core block sizes. PKHeX editors for Box, Party, Items, Pokédex, Raids, and other vanilla systems work without modification. Shouldn't need any changes at all. 

---

## PKCompassHeX Development Recommendations

| System | Guidance |
|--------|----------|
| KBox, KParty, KItem, KMyStatus, KZukan | Safe to edit - vanilla sizes and formats preserved |
| Capture Bonuses | Use `CompassBlockKeys.GetSpeciesForKey()` to display per-species progress (0–25) |
| Settings display | All Compass SByte setting blocks now have confirmed value mappings. Use `CompassBlockKeys.Get*Label()` helpers or the SAV_CompassEditor UI |
| `KTeamSeedTable` (`0xA9296A9C`) | **Caution when modifying** - used for random trainer team selection. Haven't tested the effects of modification. |
| `KSaveFormatVersion` (`0x84222645`) | **Do not modify** - used for save type detection |
| ajito_aku_050 blocks | **Caution when modifying** - Probably Team Star rematch states |
| Save size | Must accept `0x43BAC0` as a known valid Compass save size |
| `0x8B0D3ACE`, `0x40EECC82` | Do not modify directly - co-change with Level Cap and possibly Shiny Notification; let the game manage these |
| Controls while Flying | Implemented at ConfigCamera9 bit 4 (inferred, not confirmed). Verify with save comparison before relying on it. |

---

## Open Questions

| Item | Status |
|------|--------|
| 27 UInt64 block hash inputs | Unknown  and not found in Lua bytecode |
| `0xB00BFE35` (1,284-byte vanilla-only block) | Purpose unclear |
| ~6 unidentified boolean flags | Keys `0x0599C3C6`, `0x1922C87D`, `0x1979234E`, `0x197926B4`, `0x20A6E59D`, `0x3FCD0B56` have no matching named constants |

---

## Appendix: Full Compass-Only Block Listing

### Object Blocks (4)

```
0x08C07101  Object   32 bytes    "None" + "ajito_aku_050"
0x4DBFD14C  Object   32 bytes    "None" + "ajito_aku_050"
0x9B19CB47  Object   32 bytes    "None" + "ajito_aku_050"
0xA9296A9C  Object  257 bytes    Base64-encoded data (Compass_TrainerSeed)
```

### Int32 Blocks (1)

```
0x84222645  Int32   value=2   (hash("") - Compass save format version)
```

### Bool Blocks (12)

```
0x4197A769  Bool2 (true)   compass_displayTip_CaptureBonuses
0x72958FC5  Bool2 (true)   Compass_PERRINPARADOX
0x74043B2C  Bool2 (true)   nixskip_selfie
0x78A49D0A  Bool1 (false)  compass_newTip_PhoePrices
0x7FD7214C  Bool2 (true)   compass_displayTip_Obedience
0x8F8F4A1E  Bool1 (false)  roo_flag
0xA371E2C1  Bool2 (true)   compass_newTip_NewMaps
0xAAC9AAD1  Bool2 (true)   compass_displayTip_NewMaps
0xB3745A5C  Bool2 (true)   compass_newTip_Obedience
0xBE24A8C0  Bool2 (true)   yinskip_swift
0xD555F09A  Bool1 (false)  compass_displayTip_PhoePrices
0xD93612B9  Bool2 (true)   compass_newTip_CaptureBonuses
```

### UInt64 Blocks (27)

See [UInt64 Blocks (27)](#uint64-blocks-27) table.

### SByte Blocks (439)

426 capture bonus keys follow `Compass_RNGSkew_{NationalDexNumber}`.
11 settings follow `Compass_{SettingName}`.
1 shiny notification: `Compass_shiny_spoiler` (`0x4D38ED1E`).
1 story flag: `Compass_PEPPERTALKALREADY` (`0xD73E5336`).

See `CompassBlockKeys.cs` for the complete annotated listing.

### Shiny Notification Block Detail

| Block Key | Type | Values | Description |
|-----------|------|--------|-------------|
| `0x4D38ED1E` | SByte (Compass-only) | 0=Spoiler-Free, 1=Full, 2=Off | Shiny notification display mode |
| `0x8B0D3ACE` | Bool (shared) | Bool1 / Bool2 | Related flag - also observed changing with Level Cap toggle (see above) |
| `0x40EECC82` | Value (shared) | Observed: 93 and 83 | Likely a bitfield encoding state - also observed changing with Level Cap toggle |

> **Caution**: The exact trigger condition for `0x8B0D3ACE` and `0x40EECC82` is not fully isolated. They co-change with both the Level Cap transition and may also co-change with Shiny Notification. Do not modify these blocks directly.

### Vanilla S/V Settings Not Yet Mapped

The following vanilla Scarlet/Violet game option is visible in the in-game Options menu but its save block bit position has not been confirmed via direct save comparison:

| Setting | Location | Status |
|---------|----------|--------|
| Controls while Flying | `KConfigCamera` (0x998844C9), bit 4 | **Inferred** - bit 4 of the ConfigCamera block follows the layout pattern. Regular=0, Inverted=1. Verify by toggling in-game and comparing. |


---

## Reproduction Guide

> All your save files should be in a `saves` folder at the root of the project, with a `Vanilla` & `Compass` for each respective save file.

### Prerequisites

- PKHeX source code (for `FnvHash.cs` - the custom offset basis)
- A decrypted Compass save file
- Compass mod files
- Python 3.x

### Step 1: Hash Function

```python
def fnv1a_64_pkhex(s: str) -> int:
    """PKHeX's custom FNV-1a-64 hash, truncated to uint32."""
    h = 0xCBF29CE484222645  # NOT standard 0xCBF29CE484222325
    for c in s:
        h ^= ord(c)
        h = (h * 0x100000001B3) & 0xFFFFFFFFFFFFFFFF
    return h & 0xFFFFFFFF

# Verify:
assert fnv1a_64_pkhex("") == 0x84222645
```

### Step 2: Extract Strings from Lua Bytecode

```bash
strings -n 6 <mod dir>/romfs/script/lua/bin/release/main/main.blua > lua_strings.txt
grep -i "compass\|RNGSkew" lua_strings.txt
```

Key strings present: `Compass_RNGSkew`, `Compass_shiny_spoiler`, `Compass_TrainerSeed`, and ~120 other `Compass_*` strings.

### Step 3: Hash Candidate Strings Against Known Keys

```python
known_keys = {0x7E01F663, 0x1D58A379, ...}  # from save comparison output

for s in candidate_strings:
    h = fnv1a_64_pkhex(s)
    if h in known_keys:
        print(f"MATCH: {s} -> 0x{h:08X}")
```

### Step 4: Discover the Numbered Suffix Pattern

```python
for n in range(1, 1026):
    s = f"Compass_RNGSkew_{n}"
    h = fnv1a_64_pkhex(s)
    if h in known_keys:
        print(f"Species #{n}: {s} -> 0x{h:08X}")
```

Produces 426 matches across 1025 national dex numbers.

### Step 5: Mathematical Verification

For strings sharing a common prefix and differing only in trailing characters, consecutive FNV-1a hashes differ by exact multiples of the FNV prime (`0x1B3`). This property can be used to verify the source string pattern:

```python
sorted_keys = sorted(capture_bonus_keys)
for i in range(len(sorted_keys) - 1):
    delta = sorted_keys[i+1] - sorted_keys[i]
    if delta < 0x10000:  # same cluster
        assert delta % 0x1B3 == 0
```

---

## Sources

- [v2.1.x Primer](https://compass.seri.studio/2100-primer/)
- [Full Compass Documentation](https://compass.seri.studio/)
- `PKHeX.Core/Saves/AccessControl/SaveBlockAccessor9SV.cs` - SCBlock key constants
- `PKHeX.Core/Saves/Encryption/SwishCrypto/` - Save encryption implementation
