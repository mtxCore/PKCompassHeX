#!/usr/bin/env python3
"""
Brute-force unknown Compass block keys by trying candidate string patterns.

Tries format patterns against unidentified keys in CompassBlockKeys.cs
(those named KOptions_XXXXXXXX or KFeature_XXXXXXXX).

Pattern sets (combinable with --patterns):
  capture     Compass_RNGSkew_{N} for N in 1..1200
  setting     Compass_{word} for a broad vocabulary of setting-style names
  prefix      {prefix}_{suffix} for all defined prefixes and numeric/named suffixes
  lua         Every printable string from a .blua file (requires --blua)

Usage:
    python3 tools/brute_force.py --targets 0x54DB99E0 0x7D845059
    python3 tools/brute_force.py --unknown
    python3 tools/brute_force.py --unknown --patterns capture setting prefix
    python3 tools/brute_force.py --blua <path/to/main.blua> --unknown
"""
import re
import sys
import string
import itertools
from pathlib import Path

OFFSET_BASIS = 0xCBF29CE484222645
FNV_PRIME = 0x100000001B3
SCRIPT_DIR = Path(__file__).resolve().parent
REPO_ROOT = SCRIPT_DIR.parent
KEYS_FILE = REPO_ROOT / "PKHeX.Core" / "Compass" / "CompassBlockKeys.cs"

# Patterns for the "setting" and "prefix" generators
COMPASS_PREFIXES = [
    "Compass", "compass", "nix", "nixskip", "yin", "yinskip", "roo",
]
SETTING_NOUNS = [
    "AnimRate", "BattleCam", "CamDither", "ColorProfile", "Expshare",
    "Expmulti", "LetsGoEV", "Levelcap", "PicnicExp", "SpawnRate",
    "TrainerSeed", "RaidTier", "RaidLevel", "MaxSpawns", "ExpRate",
    "XPRate", "PikachuMode", "ObedienceLevel", "GoldBattle", "NewMap",
    "displayTip", "newTip", "PERRINPARADOX", "PEPPERTALKALREADY",
    "CaptureBonuses", "Obedience", "PhoePrices", "NewMaps",
    "selfie", "swift", "flag",
]
NUMERIC_SUFFIXES = list(range(0, 50))

PRINTABLE = set(string.printable) - set('\x0b\x0c')
MIN_LEN = 4


def fnv_key(s: str) -> int:
    h = OFFSET_BASIS
    for c in s:
        h ^= ord(c)
        h = (h * FNV_PRIME) & 0xFFFFFFFFFFFFFFFF
    return h & 0xFFFFFFFF


def resolve_input_path(path_arg: str) -> Path:
    """Resolve a path relative to cwd, repo root, or tools/ directory."""
    candidate = Path(path_arg)
    if candidate.is_file():
        return candidate

    repo_candidate = REPO_ROOT / path_arg
    if repo_candidate.is_file():
        return repo_candidate

    script_candidate = SCRIPT_DIR / path_arg
    if script_candidate.is_file():
        return script_candidate

    if Path(path_arg).name == path_arg:
        matches = [p for p in REPO_ROOT.rglob(path_arg) if p.is_file()]
        if len(matches) == 1:
            return matches[0]

    return candidate


def load_all_keys(path: str | Path) -> dict[int, str]:
    with open(path) as f:
        text = f.read()
    result: dict[int, str] = {}
    for m in re.finditer(r'public const uint (\w+)\s*=\s*(0x[0-9A-Fa-f]+)', text):
        result[int(m.group(2), 16)] = m.group(1)
    return result


def find_unconfirmed(path: str | Path, known: dict[int, str]) -> set[int]:
    """Return keys with fallback hex names that have no confirmed Hash input doc comment."""
    with open(path) as f:
        text = f.read()

    confirmed: set[int] = set()
    for m in re.finditer(
        r'Hash input:\s*<c>([^<]+)</c>.*?public const uint (\w+)\s*=\s*(0x[0-9A-Fa-f]+)',
        text, re.DOTALL
    ):
        confirmed.add(int(m.group(3), 16))

    unconfirmed = set()
    for k, name in known.items():
        if (re.match(r'^KOptions_[0-9A-F]{8}$', name) or
                re.match(r'^KFeature_[0-9A-F]{8}$', name)) and k not in confirmed:
            unconfirmed.add(k)
    return unconfirmed


def gen_capture() -> list[str]:
    return [f"Compass_RNGSkew_{n}" for n in range(1, 1201)]


def gen_setting() -> list[str]:
    cands = []
    for prefix in COMPASS_PREFIXES:
        for noun in SETTING_NOUNS:
            cands.append(f"{prefix}_{noun}")
            for n in NUMERIC_SUFFIXES:
                cands.append(f"{prefix}_{noun}_{n}")
    return cands


def gen_prefix() -> list[str]:
    cands = []
    for prefix in COMPASS_PREFIXES:
        for noun in SETTING_NOUNS:
            cands.append(f"{noun}")
            cands.append(f"{prefix}_{noun}")
        for n in range(0, 30):
            cands.append(f"{prefix}_{n}")
    return cands


def extract_strings_from_binary(path: str | Path) -> list[str]:
    with open(path, 'rb') as f:
        data = f.read()
    results, buf = [], []
    for byte in data:
        ch = chr(byte)
        if ch in PRINTABLE:
            buf.append(ch)
        else:
            if len(buf) >= MIN_LEN:
                results.append(''.join(buf))
            buf = []
    if len(buf) >= MIN_LEN:
        results.append(''.join(buf))
    return results


def brute(targets: set[int], candidates: list[str]) -> dict[int, str]:
    found: dict[int, str] = {}
    for s in candidates:
        h = fnv_key(s)
        if h in targets:
            found[h] = s
            targets = targets - {h}
            if not targets:
                break
    return found


def main() -> None:
    args = sys.argv[1:]
    if not args or "--help" in args or "-h" in args:
        print(__doc__)
        sys.exit(0)

    if not KEYS_FILE.is_file():
        print(f"Error: could not locate keys file at {KEYS_FILE}", file=sys.stderr)
        sys.exit(1)

    known = load_all_keys(KEYS_FILE)

    if "--unknown" in args:
        targets = find_unconfirmed(KEYS_FILE, known)
        print(f"Auto-detected {len(targets)} unconfirmed keys to brute-force:")
        for k in sorted(targets):
            print(f"  0x{k:08X}  {known[k]}")
    elif "--targets" in args:
        idx = args.index("--targets")
        raw = []
        for a in args[idx+1:]:
            if a.startswith("--"):
                break
            raw.append(a)
        targets = {int(x, 16) for x in raw}
    else:
        print("Error: specify --unknown or --targets 0xKEY1 0xKEY2 ...", file=sys.stderr)
        sys.exit(1)

    if not targets:
        print("No targets to resolve.")
        sys.exit(0)

    pattern_arg_idx = args.index("--patterns") if "--patterns" in args else -1
    if pattern_arg_idx >= 0:
        selected = []
        for a in args[pattern_arg_idx+1:]:
            if a.startswith("--"):
                break
            selected.append(a)
    else:
        selected = ["capture", "setting", "prefix"]

    blua_path = None
    if "--blua" in args:
        idx = args.index("--blua")
        blua_path = resolve_input_path(args[idx+1])
        if not blua_path.is_file():
            print(f"Error: --blua file not found: {args[idx+1]}", file=sys.stderr)
            sys.exit(1)

    candidates: list[str] = []
    if "capture" in selected:
        candidates += gen_capture()
    if "setting" in selected:
        candidates += gen_setting()
    if "prefix" in selected:
        candidates += gen_prefix()
    if blua_path:
        lua_strings = extract_strings_from_binary(blua_path)
        print(f"Extracted {len(lua_strings)} strings from {blua_path}")
        candidates += lua_strings

    seen: set[str] = set()
    unique: list[str] = []
    for c in candidates:
        if c not in seen:
            seen.add(c)
            unique.append(c)
    print(f"Trying {len(unique)} candidate strings against {len(targets)} target(s)...\n")

    found = brute(set(targets), unique)

    remaining = targets - set(found.keys())
    if found:
        print("FOUND:")
        for k, s in sorted(found.items()):
            print(f"  0x{k:08X}  {known.get(k, '?'):35s}  <=  {s!r}")
    if remaining:
        print(f"\nNot found ({len(remaining)}):")
        for k in sorted(remaining):
            print(f"  0x{k:08X}  {known.get(k, '?')}")


if __name__ == "__main__":
    main()
