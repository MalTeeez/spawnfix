using System;
using System.ComponentModel;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace spawnfix
{
	public class spawnfix : Mod
	{

	}

    public class ModConfigSpawnRates : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(20)]
        [Range(1, 10000)]
        [LabelKey("$Mods.CustomSpawnRate.Configs.Common.SpawnRateLabel")]
        [TooltipKey("$Mods.CustomSpawnRate.Configs.Common.SpawnRateTooltip")]
        public int SpawnRate;

        [DefaultValue(false)]
        [LabelKey("$Mods.CustomSpawnRate.Configs.Common.DisableOnBossLabel")]
        [TooltipKey("$Mods.CustomSpawnRate.Configs.Common.DisableOnBossTooltip")]
        public bool DisableOnBoss;

        [DefaultValue(false)]
        [LabelKey("$Mods.CustomSpawnRate.Configs.Common.CustomMaxSpawnsToggleLabel")]
        [TooltipKey("$Mods.CustomSpawnRate.Configs.Common.CustomMaxSpawnsToggleTooltip")]
        public bool CustomMaxSpawnsToggle;

        [DefaultValue(40)]
        [Range(1, 10000)]
        [LabelKey("$Mods.CustomSpawnRate.Configs.Common.CustomMaxSpawnsLabel")]
        [TooltipKey("$Mods.CustomSpawnRate.Configs.Common.CustomMaxSpawnsTooltip")]
        public int CustomMaxSpawns;

        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message)
        {
            if (!NetMessage.DoesPlayerSlotCountAsAHost(whoAmI)) // Player is not host
            {
                message = NetworkText.FromKey("tModLoader.ModConfigRejectChangesNotHost");
                return false;
            }

            return true;
        }
    }

    public class GlobalNPCRateModifier : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            ModConfigSpawnRates config = ModContent.GetInstance<ModConfigSpawnRates>();

            if (config.DisableOnBoss)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && npc.boss)
                    {
                        return;
                    }
                }
            }

            if ( !player.enemySpawns)
            {
                return;
            }

            maxSpawns = (config.CustomMaxSpawnsToggle ? config.CustomMaxSpawns : spawnRate * config.SpawnRate); // If the user has enabled custom max spawns, use that value instead
            spawnRate /= System.Math.Max(config.SpawnRate, 1);
        }
    }
}
