using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MinecraftAnimals.Animals
{
	public class Wolf : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wolf");
			Main.npcFrameCount[npc.type] = 16;
		}
		public override void SetDefaults()
		{
			npc.width = 56;
			npc.height = 30;
			npc.lifeMax = 50;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.aiStyle = -1;
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return SpawnCondition.OverworldDay.Chance * 0.05f;
		}
		private const int AI_State_Slot = 0;
		private const int AI_Timer_Slot = 1;
		// Here I define some values I will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
		private const int State_Walk = 0;
		private const int State_Idle = 1;
		private const int State_Follow = 2;
		private const int State_Attack = 3;
		public bool hostile = false;

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
			Player player = Main.player[npc.target];
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
				if (AI_Timer == 500)
				{
					AI_State = State_Idle;
					AI_Timer = 0;
				}
			}
			else if (AI_State == State_Follow)
			{
				npc.TargetClosest(true);
				AI_Timer++;
				npc.velocity.X = 1.5f * npc.target;
				npc.velocity.Y += 0.5f;
				if (!(player.HeldItem.type == mod.ItemType("Bone")))
				{
					AI_State = State_Walk;
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
					AI_State = State_Walk;
					AI_Timer = 0;
				}
			}
			if (player.HeldItem.type == mod.ItemType("Bone"))
			{
				AI_State = State_Follow;
				AI_Timer = 0;
			}
			else if (AI_State == State_Attack)
			{
				npc.velocity.X = 1 * npc.direction;
				npc.velocity.Y += 0.5f;
				npc.TargetClosest(true);
				npc.damage = 25;
			}
            if (hostile == true)
            {
				AI_State = State_Attack;
			}
		}
        public override void HitEffect(int hitDirection, double damage)
        {
			AI_State = State_Attack;
			AI_Timer = 0;
			for (int n = 0; n < 200; n++)
			{
				NPC N = Main.npc[n];
				if (N.active && (N.type == mod.NPCType("Wolf")))
				{
					N.target = npc.target;
					N.netUpdate = true;
					hostile = true;
				}
			}
			base.HitEffect(hitDirection, damage);
        }
		private const int Frame_Walk = 0;
		private const int Frame_Walk_2 = 1;
		private const int Frame_Walk_3 = 2;
		private const int Frame_Walk_4 = 3;
		private const int Frame_Walk_5 = 4;
		private const int Frame_Walk_6 = 5;
		private const int Frame_Walk_7 = 6;
		private const int Frame_Walk_8 = 7;
		private const int Frame_Angry = 8;
		private const int Frame_Angry_2 = 9;
		private const int Frame_Angry_3 = 10;
		private const int Frame_Angry_4 = 11;
		private const int Frame_Angry_5 = 12;
		private const int Frame_Angry_6 = 13;
		private const int Frame_Angry_7 = 14;
		private const int Frame_Angry_8 = 15;
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
				if (npc.frameCounter < 8)
				{
					npc.frame.Y = Frame_Walk * frameHeight;
				}
				else if (npc.frameCounter < 16)
				{
					npc.frame.Y = Frame_Walk_2 * frameHeight;
				}
				else if (npc.frameCounter < 24)
				{
					npc.frame.Y = Frame_Walk_3 * frameHeight;
				}
				else if (npc.frameCounter < 32)
				{
					npc.frame.Y = Frame_Walk_4 * frameHeight;
				}
				else if (npc.frameCounter < 40)
				{
					npc.frame.Y = Frame_Walk_5 * frameHeight;
				}
				else if (npc.frameCounter < 48)
				{
					npc.frame.Y = Frame_Walk_6 * frameHeight;
				}
				else if (npc.frameCounter < 56)
				{
					npc.frame.Y = Frame_Walk_7 * frameHeight;
				}
				else if (npc.frameCounter < 64)
				{
					npc.frame.Y = Frame_Walk_8 * frameHeight;
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
				else
				{
					npc.frameCounter = 0;
				}
			}
			if (AI_State == State_Attack)
			{
				npc.frameCounter++;
				if (npc.frameCounter < 8)
				{
					npc.frame.Y = Frame_Angry * frameHeight;
				}
				else if (npc.frameCounter < 16)
				{
					npc.frame.Y = Frame_Angry_2 * frameHeight;
				}
				else if (npc.frameCounter < 24)
				{
					npc.frame.Y = Frame_Angry_3 * frameHeight;
				}
				else if (npc.frameCounter < 32)
				{
					npc.frame.Y = Frame_Angry_4 * frameHeight;
				}
				else if (npc.frameCounter < 40)
				{
					npc.frame.Y = Frame_Angry_5 * frameHeight;
				}
				else if (npc.frameCounter < 48)
				{
					npc.frame.Y = Frame_Angry_6 * frameHeight;
				}
				else if (npc.frameCounter < 56)
				{
					npc.frame.Y = Frame_Angry_7 * frameHeight;
				}
				else if (npc.frameCounter < 64)
				{
					npc.frame.Y = Frame_Angry_8 * frameHeight;
				}
				else
				{
					npc.frameCounter = 0;
				}
			}
		}
	}
}