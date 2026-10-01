#!/usr/bin/env python3
"""Validate metadata, build Release, and package only release files. Never upload."""

import json
from pathlib import Path
import re
import struct
import subprocess
import sys
import tempfile
from urllib.parse import urlsplit
import xml.etree.ElementTree as ET
import zipfile


def prepare(root, version):
    project = root / "More Ore Deposits"
    package = project / "Package"
    manifest = json.loads((package / "manifest.json").read_text(encoding="utf-8-sig"))
    if not isinstance(manifest, dict):
        raise ValueError("Manifest must be a JSON object")
    if not re.fullmatch(r"[0-9]+\.[0-9]+\.[0-9]+", version):
        raise ValueError("Version must have the form Major.Minor.Patch")
    if manifest.get("version_number") != version:
        raise ValueError("Requested version does not match manifest version")
    if not re.fullmatch(r"[A-Za-z0-9_]{1,128}", manifest.get("name", "")):
        raise ValueError("Invalid package name")
    description = manifest.get("description")
    if not isinstance(description, str) or len(description) > 250:
        raise ValueError("Description must be a string with at most 250 characters")
    website = manifest.get("website_url")
    if not isinstance(website, str) or (website and (
        urlsplit(website).scheme not in ("https", "http") or not urlsplit(website).netloc
    )):
        raise ValueError("website_url must be an HTTP URL or an empty string")
    dependencies = manifest.get("dependencies")
    if not isinstance(dependencies, list) or any(
        not isinstance(dep, str) or not re.fullmatch(
            r"[A-Za-z0-9_]+-[A-Za-z0-9_]+-[0-9]+\.[0-9]+\.[0-9]+", dep
        ) for dep in dependencies
    ):
        raise ValueError("Invalid dependencies")
    source = (project / "More Ore Deposits.cs").read_text(encoding="utf-8-sig")
    if f'PluginVersion = "{version}";' not in source:
        raise ValueError("Plugin version does not match manifest version")
    assembly = (project / "Properties/AssemblyInfo.cs").read_text(encoding="utf-8-sig")
    for attribute in ("AssemblyVersion", "AssemblyFileVersion"):
        if f'{attribute}("{version}.0")' not in assembly:
            raise ValueError(f"{attribute} does not match manifest version")
    csproj = project / "More Ore Deposits.csproj"
    if ET.parse(csproj).findtext(".//Version") != version:
        raise ValueError("Project version does not match manifest version")
    icon = (package / "icon.png").read_bytes()
    if (len(icon) < 33 or icon[:8] != b"\x89PNG\r\n\x1a\n" or
        icon[12:16] != b"IHDR" or struct.unpack(">II", icon[16:24]) != (256, 256)):
        raise ValueError("icon.png must be a 256 by 256 PNG")
    readme = (project / "README.md").read_text(encoding="utf-8-sig")
    changelog = (root / "CHANGELOG.md").read_text(encoding="utf-8-sig")
    if not readme.strip() or not changelog.strip():
        raise ValueError("README and changelog must contain text")
    return project, package, csproj, readme, changelog


def main():
    if len(sys.argv) != 2:
        raise ValueError("Usage: package_release.py <version>")
    root = Path(__file__).resolve().parents[1]
    version = sys.argv[1]
    project, package, csproj, readme, changelog = prepare(root, version)
    subprocess.run(["dotnet", "build", str(csproj), "--configuration", "Release",
                    "--no-incremental", "-warnaserror"], cwd=root, check=True)
    dll = project / "bin/Release/More Ore Deposits.dll"
    if not dll.is_file() or dll.stat().st_size == 0:
        raise ValueError("Release DLL is missing or empty")
    # Keep tracked package documentation aligned with its canonical sources.
    (package / "README.md").write_text(readme, encoding="utf-8")
    (package / "CHANGELOG.md").write_text(changelog, encoding="utf-8")
    files = {
        "manifest.json": package / "manifest.json",
        "README.md": package / "README.md",
        "CHANGELOG.md": package / "CHANGELOG.md",
        "icon.png": package / "icon.png",
        "plugins/More Ore Deposits.dll": dll,
    }
    destination = dll.parent / f"MoreOreDeposits.{version}.zip"
    with tempfile.TemporaryDirectory(dir=dll.parent) as temporary:
        staged = Path(temporary) / destination.name
        with zipfile.ZipFile(staged, "w", compression=zipfile.ZIP_DEFLATED) as archive:
            for name, path in files.items():
                archive.write(path, name)
        with zipfile.ZipFile(staged) as archive:
            if set(archive.namelist()) != set(files) or archive.testzip() is not None:
                raise ValueError("ZIP verification failed")
            for name, path in files.items():
                if archive.read(name) != path.read_bytes():
                    raise ValueError(f"ZIP content differs from source: {name}")
        staged.replace(destination)
    print(f"Created and verified {destination}")


if __name__ == "__main__":
    try:
        main()
    except (ValueError, OSError, subprocess.CalledProcessError) as error:
        sys.exit(f"Error: {error}")
