using MinecraftAnimals.Raid.Illagers;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;


namespace MinecraftAnimals.Raid
{
    public class RaidNPc : GlobalNPC
    {

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            int activePlayers = 0;
            for (int i = 0; i < 255; i++)
            {
                if (Main.player[i].active)
                    activePlayers++;
            }
            spawnRate = (int)1.5;
            if (RaidWorld.RaidKillCount + RaidWorld.RaiderCounter >= RaidWorld.progressPerWave) maxSpawns = 0;

        }
        public static List<IDictionary<int, float>> RaidSpawnpool = new List<IDictionary<int, float>>
        { //Spawn info for the Raid npcs and spawn info
			new Dictionary<int, float> { //wave 1
				{NPCType<Evoker>(), 0f},
                {NPCType<Pillager>(), 5.35f},
                {NPCType<Ravager>(), 0f},
                {NPCType<Vindicator>(), 4.73f},
                {NPCType<Witch>(), 0f},
            },
            new Dictionary<int, float> { //wave 2
				{NPCType<Evoker>(), 0f},
                {NPCType<Pillager>(), 4.35f},
                {NPCType<Ravager>(), 0f},
                {NPCType<Vindicator>(), 4.73f},
                {NPCType<Witch>(), 4.0f},
            },
            new Dictionary<int, float> { //wave 3
				{NPCType<Evoker>(), 0f},
                {NPCType<Pillager>(), 4.35f},
                {NPCType<Ravager>(), 0f},
                {NPCType<Vindicator>(), 4.73f},
                {NPCType<Witch>(), 4.135f},
            },
            new Dictionary<int, float> { //wave 4
				{NPCType<Evoker>(), 3.65f},
                {NPCType<Pillager>(), 4.25f},
                {NPCType<Ravager>(), 0f},
                {NPCType<Vindicator>(), 4.43f},
                {NPCType<Witch>(), 4.235f},
            },
            new Dictionary<int, float> { //wave 5
				{NPCType<Evoker>(), 3.85f},
                {NPCType<Pillager>(), 5.35f},
                {NPCType<Ravager>(), 2.15f},
                {NPCType<Vindicator>(), 4.73f},
                {NPCType<Witch>(), 4.135f},
            },
            new Dictionary<int, float> { //wave 6
				{NPCType<Evoker>(), 7.35f},
                {NPCType<Pillager>(), 5.35f},
                {NPCType<Ravager>(), 1.95f},
                {NPCType<Vindicator>(), 1.73f},
                {NPCType<Witch>(), 4.135f},
            }
        };
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (RaidWorld.RaidEvent)
            {
                pool.Clear();
                if (RaidWorld.RaidKillCount + RaidWorld.RaiderCounter <= RaidWorld.progressPerWave)
                {
                    IDictionary<int, float> spawnpool = RaidSpawnpool.ElementAt(RaidWorld.RaidWaves - 1); //find the spawn pool dictionary corresponding to the current tide wave
                    foreach (KeyValuePair<int, float> key in spawnpool)
                    { //then add that dictionary info to the actual spawn pool
                        pool.Add(key.Key, key.Value);
                    }
                }
                else
                {
                    pool.Clear();
                }
            }
        }


        public override void NPCLoot(NPC npc)
        {
            if (RaidWorld.RaidEvent)
            { //check for ongoing raid

                IDictionary<int, float> spawnpool = RaidSpawnpool.ElementAt(RaidWorld.RaidWaves - 1); //find the spawn pool dictionary corresponding to the current tide wave
                foreach (KeyValuePair<int, float> key in spawnpool)
                { //iterate through the spawn pool, and check if the killed npc's type is in the spawn pool
                    if (key.Key == npc.type)
                    { //add points
                        RaidWorld.RaidKillCount += 1f;
                    }
                }
            }
        }
    }
}