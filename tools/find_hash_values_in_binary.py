#!/usr/bin/env python3
"""Find unknown Compass hash values as raw uint32 constants inside binary files.

Helps determine whether unknown keys are embedded as constants or generated at runtime.

Usage:
	python3 tools/find_hash_values_in_binary.py --root <mod_dir>
	python3 tools/find_hash_values_in_binary.py --file <mod_dir>/exefs/subsdk9
"""

from __future__ import annotations

import argparse
import re
from pathlib import Path

SCRIPT_DIR = Path(__file__).resolve().parent
REPO_ROOT = SCRIPT_DIR.parent
KEYS_FILE = REPO_ROOT / "PKHeX.Core" / "Compass" / "CompassBlockKeys.cs"


def load_unknown_keys(path: Path) -> dict[int, str]:
	text = path.read_text(encoding="utf-8")

	known: dict[int, str] = {}
	for m in re.finditer(r"public const uint (\w+)\s*=\s*(0x[0-9A-Fa-f]+)", text):
		known[int(m.group(2), 16)] = m.group(1)

	confirmed: set[int] = set()
	for m in re.finditer(
		r"Hash input:\s*<c>([^<]+)</c>.*?public const uint (\w+)\s*=\s*(0x[0-9A-Fa-f]+)",
		text,
		re.DOTALL,
	):
		confirmed.add(int(m.group(3), 16))

	unknown: dict[int, str] = {}
	for key, name in known.items():
		if (name.startswith("KOptions_") or name.startswith("KFeature_")) and key not in confirmed:
			unknown[key] = name
	return unknown


def collect_files(root_arg: str, explicit: list[str]) -> list[Path]:
	out: list[Path] = []

	root = Path(root_arg)
	if not root.is_absolute():
		root = REPO_ROOT / root
	if root.is_dir():
		for p in root.rglob("*"):
			if p.is_file():
				out.append(p)

	for raw in explicit:
		p = Path(raw)
		if not p.is_absolute():
			p = REPO_ROOT / p
		if p.is_file():
			out.append(p.resolve())

	return sorted(list(dict.fromkeys(out)))


def find_all(data: bytes, needle: bytes) -> list[int]:
	offsets: list[int] = []
	i = data.find(needle)
	while i != -1:
		offsets.append(i)
		i = data.find(needle, i + 1)
	return offsets


def main() -> None:
	ap = argparse.ArgumentParser(description="Find unknown hash values in binaries.")
	ap.add_argument("--root", default=None, help="Root directory to scan recursively.")
	ap.add_argument("--file", action="append", default=[], help="Extra file to scan (repeatable).")
	args = ap.parse_args()

	if not KEYS_FILE.is_file():
		raise SystemExit(f"Error: keys file not found: {KEYS_FILE}")

	unknown = load_unknown_keys(KEYS_FILE)
	files = collect_files(args.root or "", args.file)
	if not files:
		raise SystemExit("No files to scan. Specify --root <dir> or --file <path>.")

	print(f"Unknown keys: {len(unknown)}")
	print(f"Files scanned: {len(files)}")

	total_hits = 0
	for f in files:
		try:
			data = f.read_bytes()
		except OSError:
			continue

		hits: list[str] = []
		for key, name in sorted(unknown.items()):
			le = key.to_bytes(4, "little")
			be = key.to_bytes(4, "big")
			le_off = find_all(data, le)
			be_off = find_all(data, be)
			if le_off:
				hits.append(f"  0x{key:08X} {name} LE offsets: {', '.join(hex(x) for x in le_off[:8])}")
				total_hits += len(le_off)
			if be_off:
				hits.append(f"  0x{key:08X} {name} BE offsets: {', '.join(hex(x) for x in be_off[:8])}")
				total_hits += len(be_off)

		if hits:
			rel = f.relative_to(REPO_ROOT) if f.is_relative_to(REPO_ROOT) else f
			print(f"\n{rel}:")
			for line in hits:
				print(line)

	if total_hits == 0:
		print("\nNo unknown hash constants found as raw uint32 values.")


if __name__ == "__main__":
	main()
