PKCompassHeX
![License](https://img.shields.io/badge/License-GPLv3-blue.svg)
=====
> **A fork of PKHeX with Compass and enhanced save editing support.**
>
> **Do not ask for support from the Pokémon Compass or PKHeX developers with this project**
>
> **Not affiliated with PKHeX or Compass developers. For the original projects, visit:**
> [PKHeX](https://github.com/kwsch/PKHeX)**/**[Compass](https://www.nexusmods.com/pokemonscarletandviolet/mods/21)

<div>
  <span>English</span> / <a href=".github/README-es.md">Español</a> / <a href=".github/README-fr.md">Français</a> / <a href=".github/README-de.md">Deutsch</a> / <a href=".github/README-it.md">Italiano</a> / <a href=".github/README-ko.md">한국어</a> / <a href=".github/README-zh-Hant.md">繁體中文</a> / <a href=".github/README-zh-Hans.md">简体中文</a>
</div>
---

## About

**PKCompassHeX** is a save editor for Pokémon core series games, with special support for the Compass project and additional features.
It is based on [PKHeX](https://github.com/kwsch/PKHeX) and programmed in [C#](https://en.wikipedia.org/wiki/C_Sharp_%28programming_language%29).

**Key Features:**
- Edit save files for the Pokémon Compass Romhack.
- Enhanced flag, event, and unlock editing for Scarlet/Violet and Compass.
- Dedicated Compass event/progression flag support.
- Import/export Pokémon, Mystery Gifts, and more.
- Pokémon Showdown set and QR code import/export.
- Cross-generation transfer and conversion.
- **Shiny icons** support (credit: [bdsp-shiny-icons](https://github.com/BlupBlurp/bdsp-shiny-icons), implementation inspired by [PKLumiHex](https://github.com/TalonSabre/PKLumiHex)).
- Additional bugfixes, UI improvements, and Compass-specific tweaks.

---

## Supported Files

- Save files (`main`, `*.sav`, `*.dsv`, `*.dat`, `*.gci`, `*.bin`)
- GameCube Memory Card files (`*.raw`, `*.bin`) with Pokémon saves
- Individual Pokémon entity files (`.pk*`, `*.ck3`, `*.xk3`, `*.pb7`, `*.sk2`, `*.bk4`, `*.rk4`)
- Mystery Gift files (`*.pgt`, `*.pcd`, `*.pgf`, `.wc*`) with conversion to `.pk*`
- GO Park entities (`*.gp1`) with conversion to `.pb7`
- Teams from decrypted 3DS Battle Videos
- Cross-generation transfer and conversion

---

## Usage

- Data is displayed in an editable view.
- The interface can be translated with resource/external text files.
- Pokémon Showdown sets and QR codes can be imported/exported.

> **Note:** PKCompassHeX expects decrypted save files. Use a save manager like [Checkpoint](https://github.com/FlagBrew/Checkpoint), save_manager, [JKSM](https://github.com/J-D-K/JKSM), or SaveDataFiler to extract/import saves.



## Building

PKCompassHeX is a Windows Forms application requiring [.NET 10](https://dotnet.microsoft.com/download/dotnet/10.0).
Build with any compiler supporting C# 14.

### Build Configurations

Use Debug or Release configurations. No platform-specific code.

---

## Dependencies

- QR code generation: [QRCoder](https://github.com/codebude/QRCoder) ([MIT License](https://github.com/codebude/QRCoder/blob/master/LICENSE.txt))
- Shiny sprite collection: [pokesprite](https://github.com/msikma/pokesprite) ([MIT License](https://github.com/msikma/pokesprite/blob/master/LICENSE))
- Legends: Arceus sprites: [National Pokédex - Icon Dex](https://www.deviantart.com/pikafan2000/art/National-Pokedex-Version-Delta-Icon-Dex-824897934)
- **BDSP shiny icons:** [bdsp-shiny-icons](https://github.com/BlupBlurp/bdsp-shiny-icons) 

---

## IDE

Open with [Visual Studio](https://visualstudio.microsoft.com/downloads/) or any IDE supporting .sln/.csproj files.
