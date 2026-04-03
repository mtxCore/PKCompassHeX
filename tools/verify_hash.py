#!/usr/bin/env python3
"""Hash strings using PKHeX's FNV-1a variant compares known Compass block key constants.

Usage:
    python3 tools/verify_hash.py "Compass_RNGSkew_25"
    python3 tools/verify_hash.py "Compass_RNGSkew_25" "nixskip_selfie" "roo_flag"
    echo "my_string" | python3 tools/verify_hash.py -
"""
import re
import sys

OFFSET_BASIS = 0xCBF29CE484222645
FNV_PRIME = 0x100000001B3
KEYS_FILE = "PKHeX.Core/Compass/CompassBlockKeys.cs"


def fnv_key(s: str) -> int:
    h = OFFSET_BASIS
    for c in s:
        h ^= ord(c)
        h = (h * FNV_PRIME) & 0xFFFFFFFFFFFFFFFF
    return h & 0xFFFFFFFF


def load_known_keys(path: str) -> dict[int, str]:
    with open(path) as f:
        text = f.read()
    result: dict[int, str] = {}
    for m in re.finditer(r'public const uint (\w+)\s*=\s*(0x[0-9A-Fa-f]+)', text):
        result[int(m.group(2), 16)] = m.group(1)
    return result


def main() -> None:
    if not sys.argv[1:] or sys.argv[1] in ("-h", "--help"):
        print(__doc__)
        sys.exit(0)

    known = load_known_keys(KEYS_FILE)

    if sys.argv[1] == "-":
        strings = [line.rstrip('\n') for line in sys.stdin if line.strip()]
    else:
        strings = sys.argv[1:]

    for s in strings:
        h = fnv_key(s)
        name = known.get(h, "(not a named constant)")
        print(f"  {s!r:50s}  =>  0x{h:08X}  {name}")


if __name__ == "__main__":
    main()
