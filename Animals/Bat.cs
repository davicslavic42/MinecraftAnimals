using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace MinecraftAnimals.Animals
{
    public class Bat : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bat");
            Main.npcFrameCount[npc.type] = 5;
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
			npc.scale = 0.45f;
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Cavern.Chance * 0.08f;
        }
		private const int AI_State_Slot = 0;
		private const int AI_Timer_Slot = 1;
		// Here I define some values I will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
		private const int State_Up = 0;
		private const int State_Down = 1;
		private const int State_Fly = 2;
		private const int State_Rest = 3;

		// This is a property (https://msdn.microsoft.com/en-us/library/x9fsa0sw.aspx), it is very useful and helps keep out AI code clear of clutter.
		// Without it, every instance of "AI_State" in the AI code below would be "npc.ai[AI_State_Slot]". 
		// Also note that without the "AI_State_Slot" defined above, this would be "npc.ai[0]".
		// This is all to just make beautiful, manageable, and clean code.
		public float AI_State
		{
			get => npc.ai[AI_State_Slot];
			set => npc.ai[AI_State_Slot] = value;
		}

		public float AI_Timer
		{
			get => npc.ai[AI_Timer_Slot];
			set => npc.ai[AI_Timer_Slot] = value;
		}
		public override void AI()
		{
			if (AI_State == State_Up)
			{
				AI_Timer++;
				npc.velocity.X = 1 * npc.direction;
				npc.velocity.Y += 0.5f;
				npc.velocity = new Vector2(npc.direction * 3, -4f);
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
				if (AI_Timer == 180)
				{
					AI_State = State_Down;
					AI_Timer = 0;
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
				if (Main.tileSolid[Main.tile[(int)((npc.Center.X) / 32), (int)((npc.Center.Y) / 16)].type])
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
		private const int Frame_Rest = 4;
		public override void FindFrame(int frameHeight)
        {
			// This makes the sprite flip horizontally in conjunction with the npc.direction.
			npc.spriteDirection = npc.direction;
			if (AI_State == State_Up)
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
				else
				{
					npc.frameCounter = 0;
				}
			}
			else if (AI_State == State_Down)
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
				else
				{
					npc.frameCounter = 0;
				}
			}
			else if (AI_State == State_Fly)
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
				else
				{
					npc.frameCounter = 0;
				}
			}
			else if (AI_State == State_Rest)
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

