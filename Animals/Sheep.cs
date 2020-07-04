using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinecraftAnimals.Animals
{
	public class Sheep : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sheep");
			Main.npcFrameCount[npc.type] = 8;
		}
		
		public override void SetDefaults()
		{
			npc.width = 32;
			npc.height = 24;
			npc.lifeMax = 10;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.aiStyle = 7;
			aiType = NPCID.Bunny;
			animationType = NPCID.Bunny;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return 0.09f;
        }
    }
}

