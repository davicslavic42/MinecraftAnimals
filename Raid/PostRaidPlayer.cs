using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using MinecraftAnimals.Raid.Illagers;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using MinecraftAnimals.Raid;
using Terraria.World.Generation;
using Terraria.ObjectData;


namespace MinecraftAnimals.Raid
{
    public class PostRaidPlayer : ModPlayer
    {
        public override void PostUpdate()
        {
            if (!RaidWorld.RaidEvent)
            {
                RaidWorld.RaidWaves = 0;
                RaidWorld.RaidKillCount = 0f;
            }
            else if ((int)RaidWorld.RaidKillCount >= RaidWorld.progressPerWave && RaidWorld.progressPerWave != 0)
            {
                RaidWorld.RaidKillCount = 0f;
                if (RaidWorld.RaidWaves == 7)
                {
                    RaidWorld.EndRaidEvent();
                }
                else
                {
                    RaidWorld.RaidWaves += 1;
                }
            }
            if (RaidWorld.townNpcCount <= 0) RaidWorld.LostRaidEvent();
        }
    }
}