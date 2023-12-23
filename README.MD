![Ore Deposits](https://i.imgur.com/P5YqhgM.png)

# More Ore Deposits
The goal of this mod is to add additional small ore deposits to the world. You can mine the deposits just like you would any other and you'll also receive drops. Each ore deposit will be added during world generation (see known issues) and appear in its relevant biome. 


## Features
- Adds small gold ore deposit to the Blackforest biome
- Adds small iron ore deposit to the Swamp biome
- Adds small silver ore deposit to the Mountain biome
- Adds small blackmetal ore deposit to the Plains biome

## Instructions
**New World**: No additional steps required; ore deposits spawn during world generation.

**Existing World**:
1. Enable console in game, see [guide here](https://valheim.fandom.com/wiki/Console_Commands)
3. Install [UpgradeWorld](https://valheim.thunderstore.io/package/JereKuusela/Upgrade_World/) mod
4. Open Valheim game and enter your existing world 
5. Once in world, open console in game by pressing F5
6. Paste below command into console, hit enter
7. You should see a success message in the console

`vegetation_reset MineRock_gold,MineRock_iron,MineRock_silver_small,MineRock_blackmetal biomes=BlackForest,Swamp,Mountain,Plains start`

If you'd like to only add some of the ore: First adjust the command by removing the prefab name and the biome. Then run the altered command.


## Mod details:
``
Prefab name: MineRock_gold
- Biomes: Blackforest
- Tool tier requirement: 0 (Antler Pickaxe)
- Spawn per zone: 0 - 2
- Drops: 10 - 25 coins
``

Prefab name: MineRock_iron
- Biomes: Swamp
- Tool tier requirement: 1 (Bronze Pickaxe)
- Spawn per zone: 0 - 2
- Drops: 2 - 3 iron_ore

Prefab name: MineRock_silver
- Biomes: Mountain
- Tool tier requirement: 2 (Iron Pickaxe)
- Spawn per zone: 0 - 2
- Drops: 2 - 3 silver_ore

Prefab name: MineRock_blackmetal
- Biomes: Plains
- Tool tier requirement: 3 (Blackmetal Pickaxe)
- Spawn per zone: 0 - 2
- Drops: 2 - 3 blackmetal_scraps


## Known issues
1. Not currently comptible with SmoothBrain's Mining skill mod. I would like to make them compatible but struggled to get the code working.
2. The hover text for the ores has square brackets around it.

## Support & Feedback
Please give me feedback if you have any thoughts about the mods! Whether it's balance, models / textures, or just more ideas, I'd love to hear your input. If you have any issues you can also ask. You can find my in the OdinPlus discord.

Github link: https://github.com/jneb802/More-Ore-Deposits
