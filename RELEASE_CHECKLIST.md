# More Ore Deposits 1.4.0 release checks

Status on 2026-10-01: package prepared. In-game release validation is incomplete.

## Published baseline

- Thunderstore currently lists [1.3.5](https://thunderstore.io/c/valheim/p/warpalicious/More_Ore_Deposits/versions/).
- The published package contains the DLL, README, changelog, manifest, and icon.
- The candidate keeps the package name `More_Ore_Deposits` and all five deposit prefab names.
- The candidate moves the DLL into `plugins/`. All six asset bundles remain embedded in the DLL.
- The candidate requires [BepInExPack 5.4.2351](https://thunderstore.io/c/valheim/p/denikson/BepInExPack_Valheim/) and [Jotunn 2.30.2](https://thunderstore.io/c/valheim/p/ValheimModding/Jotunn/). These are the latest published versions checked for this package update. The earlier startup proof used BepInExPack 5.4.2350 and Jotunn 2.30.0.

## Completed checks

- Release build passes with warnings treated as errors.
- Plugin, project, assembly, and manifest versions are 1.4.0.
- The PNG icon is 256 by 256 pixels.
- The package command creates a fresh Release build before packaging.
- The ZIP contains exactly the manifest, README, changelog, icon, and candidate DLL.
- The package command excludes `.DS_Store`, unrelated files, and stale DLLs in the Package directory.
- ZIP integrity and file contents match the release inputs.
- Invalid requested versions and an invalid icon are rejected.
- README includes coal, the correct silver prefab and defaults, and the 20-coin custom gold conversion used by the code.

Build and package locally with:

```sh
./publish_release.sh 1.4.0
```

This command needs Python 3, .NET, and the game references configured in `Environment.props`. It creates `More Ore Deposits/bin/Release/MoreOreDeposits.1.4.0.zip`. It does not upload the package.

## Earlier in-game evidence

[Pull request #5](https://github.com/jneb802/More-Ore-Deposits/pull/5) records a test on Valnet client 01 with Valheim 1.0.12, BepInEx 5.4.2350, and Jotunn 2.30.0:

- Version 1.3.5 failed during item registration.
- Version 1.4.0 reached the main menu without errors after Steam initialized.
- Jotunn registered all five deposit definitions.
- Both custom `MoreOreDeposits_GoldOre` and vanilla `GoldOre` could be added to inventory.
- All five deposit prefabs could be spawned.

These tests do not prove mining, smelting, natural world generation, save migration, or multiplayer behavior.

## Required in-game checks before publishing

Use the current deployed Praetoris Season 8 release on Valdev and the participating Valnet clients. Record profiles, actual loaded versions, candidate DLL hashes, and logs. Back up candidate replacements and restore the maintained profiles after testing.

- [ ] Install the candidate ZIP through the mod manager on a client. Confirm the expected DLL loads and there is no duplicate installation.
- [ ] Load the candidate on Valdev and two clients. Connect both clients and check all three logs for registration errors.
- [ ] Mine gold, iron, silver, blackmetal, and coal deposits. Test the required pickaxe tier and confirm the expected item and drop range.
- [ ] Smelt one custom gold ore. Confirm 20 coins and correct fuel consumption. Repeat with a stack.
- [ ] Process vanilla `GoldOre`. Confirm the custom conversion and output multiplier do not affect vanilla processing.
- [ ] Generate previously unexplored areas in each target biome. Confirm deposits appear naturally at the expected scale and altitude.
- [ ] Confirm the Wishbone detects a small silver deposit.
- [ ] Change each drop setting through the supported configuration flow. Confirm changes take effect and check behavior on both clients.
- [ ] Test save and reload after mining, collecting custom gold ore, and using the smelter.
- [ ] Upgrade a copy of a 1.3.5 save with custom gold ore in inventories, containers, and a smelter. Check what happens to the old `GoldOre` references. Record any required migration before release.
- [ ] Test the documented Upgrade World command on a backed-up existing world. Confirm all five deposit types are included.
- [ ] Check the reported SmoothBrain Mining incompatibility and VNEI display limitation. Update the README if the result changes.
- [ ] Inspect a client screenshot of each deposit and the custom gold item. Check models, textures, hover text, and localization.
- [ ] Review complete server and client log windows. Explain any new warnings or errors.
- [ ] Restore profile files and metadata, remove test additions, verify restoration, and release device leases.

Valdev and Valnet client 01 were leased for another validation task during this package preparation. No device state was changed for this work.

## Publish gate

The package is not yet approved for upload. Complete the in-game checks and review the result before requesting explicit approval to publish on Thunderstore.
