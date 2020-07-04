using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinecraftAnimals.Animals
{
	public class Chicken : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Chicken");
			Main.npcFrameCount[npc.type] = 11;
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
		
		public override void AI()
		{
			npc.velocity *= 0.99f;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return 0.08f;
        }
    }
}

