using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace MinecraftAnimals.Animals.Neutral
{
    public class Bat : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bat");
            Main.npcFrameCount[npc.type] = 7;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.width = 25;
            npc.height = 35;
            npc.lifeMax = 100;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
			npc.scale = 0.70f;
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Cavern.Chance * 0.08f;
        }
		internal enum AIStates
		{
			Normal = 0,
			Attack = 1,
			Shoot = 2,
			Death = 3
		}
		internal ref float GlobalTimer => ref npc.ai[0];
		internal ref float Phase => ref npc.ai[1];
		internal ref float ActionPhase => ref npc.ai[2];
		internal ref float AttackTimer => ref npc.ai[3];

		public override void AI()
		{
			int ChangeYDir = 1;
			float SpeedChange = Main.rand.Next(0.75f, 0.95f);
			GlobalTimer++;
			if (Phase == (int)AIStates.Normal)
			{
				npc.velocity.X = 1 * npc.direction;
				npc.velocity.Y += 0.5f;
				if(GlobalTimer % 200 == 0)
                {
					ChangeYDir = Main.rand.NextBool() == true ? 1 : -1;
					npc.velocity.Y = 0.2f * ChangeYDir;
				}
			}
			else if (AI_State == State_Down)
			{
				AI_Timer++;
				npc.velocity.X = 1 * npc.direction;
				npc.velocity.Y += 0.5f;
				npc.velocity = new Vector2(npc.direction * 3, 2f);
				if (AI_Timer == 5)
				{
					switch (Main.rand.Next(2))
					{
						case 0:
							npc.direction = -1;
							return;
						case 1:
							npc.direction = 1;
							return;
					}
				}
				if (AI_Timer == 200)
				{
					switch (Main.rand.Next(2))
					{
						case 0:
							AI_State = State_Fly;
							AI_Timer = 0;
							return;
						case 1:
							AI_State = State_Up;
							AI_Timer = 0;
							return;
					}
				}
			}
			else if (AI_State == State_Fly)
			{
				AI_Timer++;
				npc.velocity.X = 1 * npc.direction;
				npc.velocity.Y += 0.5f;
				npc.velocity = new Vector2(npc.direction * 0, -5f);
				if (Collision.SolidCollision(npc.position, (npc.width ), npc.height + 1))
				{
					AI_Timer = 0;
					AI_State = State_Rest;
				}
			}
			else if (AI_State == State_Rest)
			{
				AI_Timer++;
				npc.velocity.X = 0 * npc.direction;
				npc.velocity.Y = 0;
				if (AI_Timer == 600)
				{
					AI_State = State_Down;
					AI_Timer = 0;
				}
			}
		}
		private const int Frame_Float = 0;
        private const int Frame_Float_2 = 1;
        private const int Frame_Float_3 = 2;
        private const int Frame_Float_4 = 3;
		private const int Frame_Float_5 = 4;
		private const int Frame_Float_6 = 5;
		private const int Frame_Rest = 6;
		public override void FindFrame(int frameHeight)
        {
			// This makes the sprite flip horizontally in conjunction with the npc.direction.
			npc.spriteDirection = npc.direction;
			if (AI_State == 0)
			{
				npc.frameCounter++;
				if (npc.frameCounter < 6)
				{
					npc.frame.Y = Frame_Float * frameHeight;
				}
				else if (npc.frameCounter < 12)
				{
					npc.frame.Y = Frame_Float_2 * frameHeight;
				}
				else if (npc.frameCounter < 18)
				{
					npc.frame.Y = Frame_Float_3 * frameHeight;
				}
				else if (npc.frameCounter < 24)
				{
					npc.frame.Y = Frame_Float_4 * frameHeight;
				}
				else if (npc.frameCounter < 32)
				{
					npc.frame.Y = Frame_Float_5 * frameHeight;
				}
				else if (npc.frameCounter < 38)
				{
					npc.frame.Y = Frame_Float_6 * frameHeight;
				}
				else
				{
					npc.frameCounter = 0;
				}
			}
			else if (AI_State == 2)
			{
				npc.frameCounter++;
				if (npc.frameCounter < 6)
				{
					npc.frame.Y = Frame_Float * frameHeight;
				}
				else if (npc.frameCounter < 12)
				{
					npc.frame.Y = Frame_Float_2 * frameHeight;
				}
				else if (npc.frameCounter < 18)
				{
					npc.frame.Y = Frame_Float_3 * frameHeight;
				}
				else if (npc.frameCounter < 24)
				{
					npc.frame.Y = Frame_Float_4 * frameHeight;
				}
				else if (npc.frameCounter < 32)
				{
					npc.frame.Y = Frame_Float_5 * frameHeight;
				}
				else if (npc.frameCounter < 38)
				{
					npc.frame.Y = Frame_Float_6 * frameHeight;
				}
				else
				{
					npc.frameCounter = 0;
				}
			}
			else if (AI_State == 1)
			{
				npc.frameCounter++;
				if (npc.frameCounter < 6)
				{
					npc.frame.Y = Frame_Float * frameHeight;
				}
				else if (npc.frameCounter < 12)
				{
					npc.frame.Y = Frame_Float_2 * frameHeight;
				}
				else if (npc.frameCounter < 18)
				{
					npc.frame.Y = Frame_Float_3 * frameHeight;
				}
				else if (npc.frameCounter < 24)
				{
					npc.frame.Y = Frame_Float_4 * frameHeight;
				}
				else if (npc.frameCounter < 32)
				{
					npc.frame.Y = Frame_Float_5 * frameHeight;
				}
				else if (npc.frameCounter < 38)
				{
					npc.frame.Y = Frame_Float_6 * frameHeight;
				}
				else
				{
					npc.frameCounter = 0;
				}
			}
			else if (AI_State == 3)
			{
				npc.frameCounter++;
				if (npc.frameCounter < 10)
				{
					npc.frame.Y = Frame_Rest * frameHeight;
				}
				else
				{
					npc.frameCounter = 0;
				}
			}
		}
	}
}

