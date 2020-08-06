﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinecraftAnimals.Animals
{
	public class SnowFox : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("SnowFox");
			Main.npcFrameCount[npc.type] = 7;
		}
		public override void SetDefaults()
		{
			npc.noGravity = true;
			npc.width = 48;
			npc.height = 12;
			npc.lifeMax = 35;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.aiStyle = -1;
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return SpawnCondition.OverworldDaySnowCritter.Chance * 0.02f;
		}
		private const int AI_State_Slot = 0;
		private const int AI_Timer_Slot = 1;
		// Here I define some values I will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
		private const int State_Walk = 0;
		private const int State_Idle = 1;
		private const int State_Sleep = 2;
		private const int State_Follow = 3;

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
			Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
			if (AI_State == State_Walk)
			{
				AI_Timer++;
				npc.velocity.X = 1 * npc.direction;
				npc.velocity.Y += 0.5f;
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
				if (Main.dayTime)
				{
					AI_State = State_Sleep;
					AI_Timer = 0;
				}
				if (AI_Timer == 500)
				{
					switch (Main.rand.Next(2))
					{
						case 0:
							AI_State = State_Follow;
							AI_Timer = 0;
							return;
						case 1:
							AI_State = State_Sleep;
							AI_Timer = 0;
							return;
					}
				}
			}
			else if (AI_State == State_Follow)
			{
				AI_Timer++;
				npc.velocity.X = 1.5f * npc.direction;
				npc.velocity.Y += 0.5f;
				Player player = Main.player[npc.target];
				npc.TargetClosest(true);
				if (AI_Timer == 300)
				{
					switch (Main.rand.Next(2))
					{
						case 0:
							AI_State = State_Walk;
							AI_Timer = 0;
							return;
						case 1:
							AI_State = State_Idle;
							AI_Timer = 0;
							return;
					}
				}
				if (Main.dayTime)
				{
					AI_State = State_Sleep;
					AI_Timer = 0;
				}
			}
			else if (AI_State == State_Idle)
			{
				AI_Timer++;
				npc.velocity.X = 0;
				npc.velocity.Y += 0.5f;
				if (AI_Timer == 300)
				{
					switch (Main.rand.Next(2))
					{
						case 0:
							AI_State = State_Walk;
							AI_Timer = 0;
							return;
						case 1:
							AI_State = State_Follow;
							AI_Timer = 0;
							return;
					}
					if (Main.dayTime)
					{
						AI_State = State_Sleep;
						AI_Timer = 0;
					}
				}
			}
			else if (AI_State == State_Sleep)
			{
				npc.velocity.X = 0;
				npc.velocity.Y += 0.5f;
				if (!Main.dayTime)
				{
					switch (Main.rand.Next(2))
					{
						case 0:
							AI_State = State_Walk;
							AI_Timer = 0;
							return;
						case 1:
							AI_State = State_Idle;
							AI_Timer = 0;
							return;
					}
				}
			}
		}
		private const int Frame_Walk = 0;
		private const int Frame_Walk_2 = 1;
		private const int Frame_Walk_3 = 2;
		private const int Frame_Walk_4 = 3;
		private const int Frame_Walk_5 = 4;
		private const int Frame_Walk_6 = 5;
		private const int Frame_Sleep = 6;
		public override void FindFrame(int frameHeight)
		{
			// This makes the sprite flip horizontally in conjunction with the npc.direction.
			npc.spriteDirection = npc.direction;
			if (AI_State == State_Idle)
			{
				npc.frameCounter++;
				if (npc.frameCounter < 10)
				{
					npc.frame.Y = Frame_Walk * frameHeight;
				}
				else
				{
					npc.frameCounter = 0;
				}
			}
			else if (AI_State == State_Walk)
			{
				npc.frameCounter++;
				if (npc.frameCounter < 10)
				{
					npc.frame.Y = Frame_Walk * frameHeight;
				}
				else if (npc.frameCounter < 20)
				{
					npc.frame.Y = Frame_Walk_2 * frameHeight;
				}
				else if (npc.frameCounter < 30)
				{
					npc.frame.Y = Frame_Walk_3 * frameHeight;
				}
				else if (npc.frameCounter < 40)
				{
					npc.frame.Y = Frame_Walk_4 * frameHeight;
				}
				else if (npc.frameCounter < 50)
				{
					npc.frame.Y = Frame_Walk_5 * frameHeight;
				}
				else if (npc.frameCounter < 50)
				{
					npc.frame.Y = Frame_Walk_6 * frameHeight;
				}
				else
				{
					npc.frameCounter = 0;
				}
			}
			else if (AI_State == State_Follow)
			{
				npc.frameCounter++;
				if (npc.frameCounter < 10)
				{
					npc.frame.Y = Frame_Walk * frameHeight;
				}
				else if (npc.frameCounter < 20)
				{
					npc.frame.Y = Frame_Walk_2 * frameHeight;
				}
				else if (npc.frameCounter < 30)
				{
					npc.frame.Y = Frame_Walk_3 * frameHeight;
				}
				else if (npc.frameCounter < 40)
				{
					npc.frame.Y = Frame_Walk_4 * frameHeight;
				}
				else if (npc.frameCounter < 50)
				{
					npc.frame.Y = Frame_Walk_5 * frameHeight;
				}
				else if (npc.frameCounter < 50)
				{
					npc.frame.Y = Frame_Walk_6 * frameHeight;
				}
				else
				{
					npc.frameCounter = 0;
				}
			}
			else if (AI_State == State_Sleep)
			{
				npc.frameCounter++;
				if (npc.frameCounter < 125)
				{
					npc.frame.Y = Frame_Sleep * frameHeight;
				}
				else
				{
					npc.frameCounter = 0;
				}
			}
		}
	}
}