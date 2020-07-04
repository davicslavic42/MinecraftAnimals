﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinecraftAnimals.Animals
{
	public class Dolphin : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dolphin");
			Main.npcFrameCount[npc.type] = 5;
		}
		public override void SetDefaults()
		{
			npc.noGravity = true;
			npc.width = 48;
			npc.height = 12;
			npc.lifeMax = 10;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.aiStyle = -1;
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return SpawnCondition.Ocean.Chance * 0.02f;
		}
		private const int AI_State_Slot = 0;
		private const int AI_Timer_Slot = 1;
		// Here I define some values I will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
		private const int State_Swim_1 = 0;
		private const int State_Swim_2 = 1;
		private const int State_Follow = 2;
		private const int State_Jump = 3;

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
			// The npc starts in the asleep state, waiting for a player to enter range
			if (AI_State == State_Swim_1)
			{
				AI_Timer++;
				npc.velocity.X = 1 * npc.direction;
				npc.velocity.Y = 0;
				if (AI_Timer > 400)
				{
					switch (Main.rand.Next(3))
					{
						case 0:
							AI_State = State_Swim_2;
							break;
						case 1:
							AI_State = State_Swim_2;
							break;
						case 2:
							AI_State = State_Follow;
							break;
					}
					AI_Timer = 0;
				}
			}
			else if (AI_State == State_Swim_2)
			{
				AI_Timer++;
				npc.velocity.X = 1 * npc.direction;
				npc.velocity.Y = 0;
				if (AI_Timer > 400)
				{
					switch (Main.rand.Next(3))
					{
						case 0:
							AI_State = State_Swim_1;
							break;
						case 1:
							AI_State = State_Jump;
							break;
						case 2:
							AI_State = State_Follow;
							break;

					}
					AI_Timer = 0;
				}
			}
			else if (AI_State == State_Follow)
			{
				AI_Timer++;
				npc.velocity.X = 2 * npc.direction;
				npc.velocity.Y = 0;
				Player player = Main.player[npc.target];
				npc.TargetClosest(true);
				if (AI_Timer > 400)
				{
					switch (Main.rand.Next(2))
					{
						case 0:
							AI_State = State_Swim_1;
							break;
						case 1:
							AI_State = State_Swim_2;
							break;
					}
					AI_Timer = 0;
				}
			}
		}
		private const int Frame_Walk = 0;
		private const int Frame_Walk_2 = 1;
		private const int Frame_Walk_3 = 2;
		private const int Frame_Walk_4 = 3;
		private const int Frame_Walk_5 = 4;
		public override void FindFrame(int frameHeight)
		{
			// This makes the sprite flip horizontally in conjunction with the npc.direction.
			npc.spriteDirection = npc.direction;
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
				else
				{
					npc.frameCounter = 0;
				}
			}
		}
	}
}