#!/usr/bin/env python3
"""Hash strings report matches against known Compass block keys.

Usage:
    python3 tools/hash_lua_strings.py <strings_file>
    python3 tools/hash_lua_strings.py --extract <file.blua>          # extract + hash inline
    python3 tools/hash_lua_strings.py --extract --all <file.blua>    # also show non-matching

To extract printable strings from a Lua bytecode file first:
    strings <file.blua> > strings.txt
    python3 tools/hash_lua_strings.py strings.txt
"""
import re
import sys
import string

OFFSET_BASIS = 0xCBF29CE484222645
FNV_PRIME = 0x100000001B3
KEYS_FILE = "PKHeX.Core/Compass/CompassBlockKeys.cs"

PRINTABLE = set(string.printable) - set('\x0b\x0c')
MIN_LEN = 4


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


def extract_strings_from_binary(path: str) -> list[str]:
    """Extract printable ASCII strings (len >= MIN_LEN) from a binary file."""
    with open(path, 'rb') as f:
        data = f.read()
    results = []
    buf = []
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


def hash_and_match(strings: list[str], known: dict[int, str], show_all: bool) -> None:
    matched = 0
    for s in strings:
        h = fnv_key(s)
        if h in known:
            print(f"  MATCH  0x{h:08X}  {known[h]:35s}  <=  {s!r}")
            matched += 1
        elif show_all:
            print(f"         0x{h:08X}  {'':35s}      {s!r}")
    print(f"\n{matched} match(es) found in {len(strings)} string(s).")


def main() -> None:
    args = sys.argv[1:]
    if not args or args[0] in ("-h", "--help"):
        print(__doc__)
        sys.exit(0)

    show_all = "--all" in args
    args = [a for a in args if a != "--all"]

    extract = "--extract" in args
    args = [a for a in args if a != "--extract"]

    if not args:
        print("Error: no input file specified.", file=sys.stderr)
        sys.exit(1)

    known = load_known_keys(KEYS_FILE)

    if extract:
        strings = extract_strings_from_binary(args[0])
        print(f"Extracted {len(strings)} strings from {args[0]}")
    else:
        with open(args[0]) as f:
            strings = [line.rstrip('\n') for line in f if line.strip()]
        print(f"Loaded {len(strings)} strings from {args[0]}")

    hash_and_match(strings, known, show_all)


if __name__ == "__main__":
    main()
