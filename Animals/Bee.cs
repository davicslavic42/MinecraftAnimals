using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinecraftAnimals.Animals
{
	public class Bee : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bee");
			Main.npcFrameCount[npc.type] = 8;
		}

		public override void SetDefaults()
		{
			npc.width = 34;
			npc.height = 30;
			npc.lifeMax = 100;
            npc.damage = 25;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.aiStyle = 24;
			aiType = NPCID.Bird;
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return SpawnCondition.OverworldDay.Chance * 0.03f;
		}
        private const int Frame_Float = 0;
        private const int Frame_Float_2 = 1;
        private const int Frame_Float_3 = 2;
        private const int Frame_Float_4 = 3;
        private const int Frame_AngryFloat = 4;
        private const int Frame_AngryFloat_1 = 5;
        private const int Frame_AngryFloat_2 = 6;
        private const int Frame_AngryFloat_3 = 7;
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            npc.spriteDirection = npc.direction;
            {
                npc.frameCounter++;
                if (npc.frameCounter < 9)
                {
                    npc.frame.Y = Frame_Float * frameHeight;
                }
                else if (npc.frameCounter < 18)
                {
                    npc.frame.Y = Frame_Float_2 * frameHeight;
                }
                else if (npc.frameCounter < 27)
                {
                    npc.frame.Y = Frame_Float_3 * frameHeight;
                }
                else if (npc.frameCounter < 36)
                {
                    npc.frame.Y = Frame_Float_4 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
            if (npc.aiStyle == 3)
            {
                if (npc.frameCounter < 8)
                {
                    npc.frame.Y = Frame_AngryFloat * frameHeight;
                }
                else if (npc.frameCounter < 16)
                {
                    npc.frame.Y = Frame_AngryFloat_1 * frameHeight;
                }
                else if (npc.frameCounter < 24)
                {
                    npc.frame.Y = Frame_AngryFloat_2 * frameHeight;
                }
                else if (npc.frameCounter < 36)
                {
                    npc.frame.Y = Frame_AngryFloat_3 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
        }
    }
}


