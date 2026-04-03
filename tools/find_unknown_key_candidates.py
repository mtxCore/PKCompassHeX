#!/usr/bin/env python3
"""
Find candidate source strings for currently unknown Compass block keys.

Scans .blua binaries, text files, and additional binaries for printable strings,
generates name variants, and checks for FNV-1a hash matches against unknown keys
in CompassBlockKeys.cs.

Usage:
    python3 tools/find_unknown_key_candidates.py --blua <path/to/main.blua>
    python3 tools/find_unknown_key_candidates.py --blua <path/to/main.blua> --text-root <mod_dir>
    python3 tools/find_unknown_key_candidates.py --blua <path/to/main.blua> --binary-root <mod_dir>/exefs
    python3 tools/find_unknown_key_candidates.py --max-len 48 --min-len 3
"""

from __future__ import annotations

import argparse
import re
import string
from pathlib import Path

OFFSET_BASIS = 0xCBF29CE484222645
FNV_PRIME = 0x100000001B3

PRINTABLE = set(string.printable) - set("\x0b\x0c")

SCRIPT_DIR = Path(__file__).resolve().parent
REPO_ROOT = SCRIPT_DIR.parent
KEYS_FILE = REPO_ROOT / "PKHeX.Core" / "Compass" / "CompassBlockKeys.cs"
DEFAULT_BLUA_GLOB = None
DEFAULT_TEXT_ROOT = None
DEFAULT_BINARY_ROOT = None

PREFIXES = ("Compass_", "compass_", "roo_", "nix_", "nixskip_", "yin_", "yinskip_")
IDENT_RE = re.compile(r"[A-Za-z0-9_./:-]+")
TOKEN_SPLIT_RE = re.compile(r"[./:-]")
TEXT_TOKEN_RE = re.compile(r"[A-Za-z0-9_./:-]{3,64}")
TEXT_EXTENSIONS = {
    ".txt", ".md", ".json", ".toml", ".yaml", ".yml", ".ini", ".cfg", ".conf",
    ".lua", ".xml", ".csv", ".tsv", ".log", ".msbt", ".msbf", ".msg",
    ".sarc", ".pack", ".info",
}


def fnv_key(s: str) -> int:
    h = OFFSET_BASIS
    for c in s:
        h ^= ord(c)
        h = (h * FNV_PRIME) & 0xFFFFFFFFFFFFFFFF
    return h & 0xFFFFFFFF


def load_all_keys(path: Path) -> dict[int, str]:
    text = path.read_text(encoding="utf-8")
    result: dict[int, str] = {}
    for m in re.finditer(r"public const uint (\w+)\s*=\s*(0x[0-9A-Fa-f]+)", text):
        result[int(m.group(2), 16)] = m.group(1)
    return result


def find_unconfirmed(path: Path, known: dict[int, str]) -> set[int]:
    text = path.read_text(encoding="utf-8")

    confirmed: set[int] = set()
    for m in re.finditer(
        r"Hash input:\s*<c>([^<]+)</c>.*?public const uint (\w+)\s*=\s*(0x[0-9A-Fa-f]+)",
        text,
        re.DOTALL,
    ):
        confirmed.add(int(m.group(3), 16))

    unknown: set[int] = set()
    for k, name in known.items():
        if (name.startswith("KOptions_") or name.startswith("KFeature_")) and k not in confirmed:
            unknown.add(k)
    return unknown


def extract_strings_from_binary(path: Path, min_len: int) -> list[str]:
    data = path.read_bytes()
    out: list[str] = []
    buf: list[str] = []
    for b in data:
        ch = chr(b)
        if ch in PRINTABLE:
            buf.append(ch)
            continue
        if len(buf) >= min_len:
            out.append("".join(buf))
        buf = []

    if len(buf) >= min_len:
        out.append("".join(buf))
    return out


def collect_blua_files(explicit: list[str]) -> list[Path]:
    if explicit:
        files = []
        for raw in explicit:
            p = Path(raw)
            if not p.is_absolute():
                p = REPO_ROOT / raw
            files.append(p.resolve())
        return files

    return []


def collect_text_files(root_arg: str | None) -> list[Path]:
    if not root_arg:
        return []
    root = Path(root_arg)

    if not root.is_absolute():
        root = REPO_ROOT / root

    if not root.is_dir():
        return []

    files: list[Path] = []
    for p in root.rglob("*"):
        if not p.is_file():
            continue
        if p.suffix.lower() in TEXT_EXTENSIONS:
            files.append(p)
    return sorted(files)


def collect_binary_files(explicit_files: list[str], root_arg: str | None) -> list[Path]:
    out: list[Path] = []

    for raw in explicit_files:
        p = Path(raw)
        if not p.is_absolute():
            p = REPO_ROOT / raw
        if p.is_file():
            out.append(p.resolve())

    root = Path(root_arg) if root_arg else None
    if root:
        if not root.is_absolute():
            root = REPO_ROOT / root
        if root.is_dir():
            for p in root.rglob("*"):
                if p.is_file():
                    out.append(p)

    return list(dict.fromkeys(out))


def extract_tokens_from_text_files(paths: list[Path], min_len: int, max_len: int) -> list[str]:
    out: list[str] = []
    for p in paths:
        try:
            text = p.read_text(encoding="utf-8", errors="ignore")
        except OSError:
            continue

        for m in TEXT_TOKEN_RE.finditer(text):
            tok = m.group(0)
            if min_len <= len(tok) <= max_len:
                out.append(tok)
    return out


def variant_strings(s: str) -> set[str]:
    vals = {s, s.lower(), s.upper(), s.title()}
    if "_" in s:
        vals.add(s.replace("_", ""))

    for p in PREFIXES:
        vals.add(p + s)
        vals.add(p + s.lower())
    return vals


def main() -> None:
    ap = argparse.ArgumentParser(description="Find candidate names for unknown Compass hashed keys.")
    ap.add_argument("--blua", action="append", default=[], help="Path to a .blua file (repeatable).")
    ap.add_argument(
        "--text-root",
        default=DEFAULT_TEXT_ROOT,
        help="Folder to scan for text-like files with possible key names.",
    )
    ap.add_argument("--no-text-scan", action="store_true", help="Disable scanning text-like files.")
    ap.add_argument("--binary", action="append", default=[], help="Additional binary file to scan (repeatable).")
    ap.add_argument(
        "--binary-root",
        default=DEFAULT_BINARY_ROOT,
        help="Folder to recursively scan for additional binaries.",
    )
    ap.add_argument("--no-binary-scan", action="store_true", help="Disable additional binary file scan.")
    ap.add_argument("--min-len", type=int, default=4, help="Minimum extracted string length.")
    ap.add_argument("--max-len", type=int, default=64, help="Maximum candidate length after filtering.")
    args = ap.parse_args()

    if not KEYS_FILE.is_file():
        raise SystemExit(f"Error: keys file not found: {KEYS_FILE}")

    known = load_all_keys(KEYS_FILE)
    unknown = find_unconfirmed(KEYS_FILE, known)

    print(f"Unknown targets: {len(unknown)}")
    for k in sorted(unknown):
        print(f"  0x{k:08X}  {known[k]}")

    blua_files = collect_blua_files(args.blua)
    if not blua_files:
        raise SystemExit("No .blua files found. Specify one or more paths with --blua.")

    print("\nScanning .blua files:")
    for p in blua_files:
        print(f"  {p.relative_to(REPO_ROOT) if p.is_relative_to(REPO_ROOT) else p}")

    raw_strings: list[str] = []
    for p in blua_files:
        if not p.is_file():
            print(f"Skipping missing file: {p}")
            continue
        raw_strings.extend(extract_strings_from_binary(p, min_len=args.min_len))

    print(f"\nExtracted raw strings from .blua: {len(raw_strings)}")

    text_tokens: list[str] = []
    text_files: list[Path] = []
    if not args.no_text_scan:
        text_files = collect_text_files(args.text_root)
        print(f"Text-like files to scan: {len(text_files)}")
        text_tokens = extract_tokens_from_text_files(text_files, args.min_len, args.max_len)
        print(f"Extracted tokens from text-like files: {len(text_tokens)}")

    binary_strings: list[str] = []
    binary_files: list[Path] = []
    if not args.no_binary_scan:
        binary_files = collect_binary_files(args.binary, args.binary_root)
        # Avoid double-processing .blua files already scanned above.
        blua_set = {p.resolve() for p in blua_files if p.is_file()}
        binary_files = [p for p in binary_files if p.resolve() not in blua_set]
        print(f"Additional binary files to scan: {len(binary_files)}")
        for p in binary_files:
            binary_strings.extend(extract_strings_from_binary(p, min_len=args.min_len))
        print(f"Extracted strings from additional binaries: {len(binary_strings)}")

    filtered: list[str] = []
    for s in raw_strings + text_tokens + binary_strings:
        if len(s) > args.max_len:
            continue
        if not IDENT_RE.fullmatch(s):
            continue
        filtered.append(s)

    tokens: list[str] = []
    for s in filtered:
        for t in TOKEN_SPLIT_RE.split(s):
            if args.min_len <= len(t) <= args.max_len and re.fullmatch(r"[A-Za-z0-9_]+", t):
                tokens.append(t)

    candidates = list(dict.fromkeys(filtered + tokens))
    print(f"Candidate base strings: {len(candidates)}")

    found: dict[int, str] = {}
    tested = 0
    for s in candidates:
        for v in variant_strings(s):
            tested += 1
            h = fnv_key(v)
            if h in unknown and h not in found:
                found[h] = v

    print(f"Variant strings tested: {tested}")

    if found:
        print(f"\nFOUND ({len(found)}):")
        for k in sorted(found):
            print(f"  0x{k:08X}  {known[k]:35s} <= {found[k]!r}")

    remaining = sorted(unknown - set(found.keys()))
    print(f"\nRemaining ({len(remaining)}):")
    for k in remaining:
        print(f"  0x{k:08X}  {known[k]}")


if __name__ == "__main__":
    main()
