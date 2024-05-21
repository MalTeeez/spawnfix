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
        [LabelKey("$Mods.Spawnfix.Configs.Common.SpawnRateLabel")]
        [TooltipKey("$Mods.Spawnfix.Configs.Common.SpawnRateTooltip")]
        public int SpawnRate;

        [DefaultValue(false)]
        [LabelKey("$Mods.Spawnfix.Configs.Common.DisableOnBossLabel")]
        [TooltipKey("$Mods.Spawnfix.Configs.Common.DisableOnBossTooltip")]
        public bool DisableOnBoss;

        [DefaultValue(false)]
        [LabelKey("$Mods.Spawnfix.Configs.Common.CustomMaxSpawnsToggleLabel")]
        [TooltipKey("$Mods.Spawnfix.Configs.Common.CustomMaxSpawnsToggleTooltip")]
        public bool CustomMaxSpawnsToggle;

        [DefaultValue(40)]
        [Range(1, 10000)]
        [LabelKey("$Mods.Spawnfix.Configs.Common.CustomMaxSpawnsLabel")]
        [TooltipKey("$Mods.Spawnfix.Configs.Common.CustomMaxSpawnsTooltip")]
        public int CustomMaxSpawns;

        [DefaultValue(58)]
        [Range(1,100)]
        [LabelKey("$Mods.Spawnfix.Configs.Common.EffectiveFactor")]
        [TooltipKey("$Mods.Spawnfix.Configs.Common.EffectiveFactorTooltip")]
        public int EffectiveFactor;

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



            maxSpawns = (config.CustomMaxSpawnsToggle ? config.CustomMaxSpawns : spawnRate * config.SpawnRate);
            spawnRate /= System.Math.Max(config.SpawnRate, 1);
        }
    }

    public class CustomPlayer : ModPlayer
    {
        readonly int defenseScaling = ModContent.GetInstance<ModConfigSpawnRates>().EffectiveFactor;

        public override void PostUpdateMiscEffects()
        {
            Player.DefenseEffectiveness *= defenseScaling / 100.0f;
        }

    }
}
