using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinecraftAnimals.Animals
{
	public class Mooshroom : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mooshroom");
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
		
		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Mushroom, Main.rand.Next(5, 10));
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return 0.06f;
        }
    }
}

