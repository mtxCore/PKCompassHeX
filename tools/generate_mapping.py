#!/usr/bin/env python3
"""Regenerate the CaptureBonusKeys span and species switch in CompassBlockKeys.cs.

Usage:
    python3 tools/generate_mapping.py            # print C# output to stdout
    python3 tools/generate_mapping.py --verify   # verify span vs expected hashes
"""
import re
import sys

OFFSET_BASIS = 0xCBF29CE484222645
FNV_PRIME = 0x100000001B3
KEYS_FILE = "PKHeX.Core/Compass/CompassBlockKeys.cs"

# SByte keys that are settings/flags, NOT per-species capture bonuses
NON_CAPTURE = {
    0x1C25C049,  # Compass_PicnicExp
    0x34780809,  # Compass_BattleCam
    0x46E0EDCB,  # Compass_LetsGoEV
    0x4D38ED1E,  # Compass_shiny_spoiler  (KShinyNotification)
    0x837B300E,  # Compass_Levelcap
    0x8B12B6B8,  # Compass_Expmulti
    0x92569278,  # Compass_ColorProfile
    0x96C58F45,  # Compass_SpawnRate
    0xA9296A9C,  # Compass_TrainerSeed    (KTeamSeedTable)
    0xB1366CBF,  # Compass_RNGSkew        (base, no suffix)
    0xCC806ED6,  # Compass_Expshare
    0xD2E638A3,  # Compass_CamDither
    0xD73E5336,  # Compass_PEPPERTALKALREADY (KPepperTalkAlready)
    0xFB067137,  # Compass_AnimRate
}


def fnv_key(s: str) -> int:
    h = OFFSET_BASIS
    for c in s:
        h ^= ord(c)
        h = (h * FNV_PRIME) & 0xFFFFFFFFFFFFFFFF
    return h & 0xFFFFFFFF


def load_span_keys(path: str) -> set[int]:
    with open(path) as f:
        text = f.read()
    m = re.search(r'CaptureBonusKeys\s*=>\s*\[(.*?)\];', text, re.DOTALL)
    if not m:
        raise ValueError("CaptureBonusKeys span not found in " + path)
    span_body = m.group(1)
    keys = set()
    for line in span_body.split('\n'):
        code = line.split('//')[0]
        for match in re.finditer(r'0x[0-9A-Fa-f]+', code):
            keys.add(int(match.group(), 16))
    return keys


def build_species_map(capture_keys: list[int]) -> tuple[dict[int, int], list[int]]:
    key_set = set(capture_keys)
    species_map: dict[int, int] = {}
    for n in range(1, 1100):
        h = fnv_key(f"Compass_RNGSkew_{n}")
        if h in key_set:
            species_map[h] = n
    unmatched = [k for k in capture_keys if k not in species_map]
    return species_map, unmatched


def verify(span_keys: set[int]) -> bool:
    capture_keys = sorted(span_keys - NON_CAPTURE)
    species_map, unmatched = build_species_map(capture_keys)
    print(f"Keys in span:          {len(span_keys)}")
    print(f"Non-capture removed:   {len(span_keys & NON_CAPTURE)}")
    print(f"Capture keys:          {len(capture_keys)}")
    print(f"Matched to species:    {len(species_map)}")
    if unmatched:
        print(f"UNMATCHED ({len(unmatched)}):")
        for k in unmatched:
            print(f"  0x{k:08X}")
        return False
    print("All keys verified.")
    return True


def generate_cs(span_keys: set[int]) -> None:
    capture_keys = sorted(span_keys - NON_CAPTURE)
    species_map, unmatched = build_species_map(capture_keys)

    if unmatched:
        print(f"// WARNING: {len(unmatched)} unmatched keys:", file=sys.stderr)
        for k in unmatched:
            print(f"//   0x{k:08X}", file=sys.stderr)

    # CaptureBonusKeys span (sorted by key, 5 per line with species comments)
    print("    public static ReadOnlySpan<uint> CaptureBonusKeys =>")
    print("    [")
    for i in range(0, len(capture_keys), 5):
        chunk = capture_keys[i:i+5]
        vals = ", ".join(f"0x{k:08X}" for k in chunk)
        labels = ", ".join(
            str(species_map[k]) if k in species_map else "???" for k in chunk
        )
        print(f"        {vals}, // {labels}")
    print("    ];")
    print()

    # GetSpeciesForKey switch
    print("    public static ushort GetSpeciesForKey(uint key) => key switch")
    print("    {")
    for k in sorted(species_map.keys()):
        sp = species_map[k]
        print(f"        0x{k:08X} => {sp:4d}, // Compass_RNGSkew_{sp}")
    print("        _ => 0,")
    print("    };")


def main() -> None:
    span_keys = load_span_keys(KEYS_FILE)
    if "--verify" in sys.argv:
        ok = verify(span_keys)
        sys.exit(0 if ok else 1)
    else:
        generate_cs(span_keys)


if __name__ == "__main__":
    main()
