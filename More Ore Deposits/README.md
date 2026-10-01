![Ore Deposits](https://i.imgur.com/P5YqhgM.png)

# More Ore Deposits
The goal of this mod is to add additional small ore deposits to the world. You can mine the deposits just like you would any other and you'll also receive drops. Each ore deposit will be added during world generation (see known issues) and appear in its relevant biome. 


## Features
- Adds small gold ore deposit to the Blackforest biome
- Adds small iron ore deposit to the Swamp biome
- Adds small silver ore deposit to the Mountain biome
- Adds small blackmetal ore deposit to the Plains biome
- Adds small coal deposits to the Swamp biome
- Adds a custom gold ore item. The smelter produces 20 coins per custom gold ore.
- Ore drop rates are configurable in configuration manager
- Translated in all 36 Valheim languages

## Instructions
**New World**: No additional steps required; ore deposits spawn during world generation.

**Existing World**:
Deposits can appear when the game first generates new areas. To add deposits to areas that the game has already generated:

1. Back up your world save.
2. Enable the game console.
3. Install [Upgrade World](https://thunderstore.io/c/valheim/p/JereKuusela/Upgrade_World/).
4. Open your world.
5. Press F5 to open the console.
6. Run the command below.

This command removes and adds the listed deposits in the selected biomes. It can restore deposits that you have already mined.

`vegetation_reset MineRock_gold,MineRock_iron,MineRock_silver_small,MineRock_blackmetal,MineRock_coal biomes=BlackForest,Swamp,Mountain,Plains start`

If you'd like to only add some of the ore: First adjust the command by removing the prefab name and the biome. Then run the altered command.


## Mod details:

Prefab name: MineRock_gold
- Biomes: Blackforest
- Tool tier requirement: 0 (Antler Pickaxe)
- Spawn per zone: 0 - 2
- Drops: 1 - 2 Gold Ore

Prefab name: MineRock_iron
- Biomes: Swamp
- Tool tier requirement: 1 (Bronze Pickaxe)
- Spawn per zone: 0 - 2
- Drops: 2 - 3 Iron Scrap

Prefab name: MineRock_silver_small
- Biomes: Mountain
- Tool tier requirement: 2 (Iron Pickaxe)
- Spawn per zone: 0 - 1
- Drops: 1 - 2 Silver Ore
- Maximum spawn altitude: 100 meters
- A Wishbone can detect the deposit within 30 meters

Prefab name: MineRock_blackmetal
- Biomes: Plains
- Tool tier requirement: 2 (Iron Pickaxe)
- Spawn per zone: 0 - 2
- Drops: 2 - 3 BlackMetal Scrap

Prefab name: MineRock_coal
- Biomes: Swamp
- Tool tier requirement: 0 (Antler Pickaxe)
- Spawn per zone: 0 - 2
- Drops: 2 - 3 Coal

The iron deposit drops Iron Scrap. Drop amounts above are the default values.

## Valheim 1.0

Version 1.4.0 uses `MoreOreDeposits_GoldOre` for the custom gold ore item. Valheim's `GoldOre` is a separate item. The custom smelter conversion and coin multiplier apply to `MoreOreDeposits_GoldOre`.

The configuration keys remain `GoldOre Drop Min` and `GoldOre Drop Max`.

Install the mod and its dependencies on the server and each client.

## Known issues
1. Not currently compatible with SmoothBrain's Mining skill mod. I would like to make them compatible but struggled to get the code working.
2. Drop settings require whole numbers. Invalid text can cause configuration errors.
3. VNEI can show one coin per custom gold ore. The mod changes the output to 20 coins when the smelter produces the item.

## Support & Feedback
Please give me feedback if you have any thoughts about the mods! Whether it's balance, models / textures, or just more ideas, I'd love to hear your input. If you have any issues you can also ask. You can find my in the OdinPlus discord.

## Credit & thanks
I'd like to thank CookieMilk, Searica, Margmus, and Horem for their guidance and help in making this mod. I am amazed by the kindness and willingness to help in the Valheim mod developer community. If you're someone that's interested in making mods, come by the Discord channels and ask for help! Give it a shot!

Github link: https://github.com/jneb802/More-Ore-Deposits
