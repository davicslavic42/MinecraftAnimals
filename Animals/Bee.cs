using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

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
			npc.damage = 0;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.aiStyle = -1;
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return SpawnCondition.OverworldDay.Chance * 0.09f;
		}
		private const int AI_State_Slot = 0;
		private const int AI_Timer_Slot = 1;
		// Here I define some values I will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
		private const int State_Walk = 0;
		private const int State_Idle = 1;
		private const int State_Fly = 2;
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
			Player player = Main.player[npc.target];
			Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
			AI_Timer++;
			if (AI_State == State_Walk)
			{
				AI_Timer++;
				npc.velocity.X = 1 * npc.direction;
				npc.velocity.Y += 0.5f;
				if (AI_Timer == 500)
				{
					switch (Main.rand.Next(2))
					{
						case 0:
							AI_State = State_Idle;
							AI_Timer = 0;
							return;
						case 1:
							AI_State = State_Fly;
							AI_Timer = 0;
							return;
					}
				}
			}
			else if (AI_State == State_Idle)
			{
				AI_Timer++;
				npc.velocity.X = 0;
				npc.velocity.Y += 0.5f;
				if (AI_Timer == 500)
				{
					switch (Main.rand.Next(2))
					{
						case 0:
							AI_State = State_Walk;
							AI_Timer = 0;
							return;
						case 1:
							AI_State = State_Fly;
							AI_Timer = 0;
							return;
					}
				}
			}
			else if (AI_State == State_Fly)
			{
				AI_Timer++;
				if (player.position.Y < npc.position.Y + 130)
				{
					npc.velocity.Y -= npc.velocity.Y > 0f ? 0.0375f : .1f;
				}
				if (player.position.Y > npc.position.Y + 130)
				{
					npc.velocity.Y += npc.velocity.Y < 0f ? 0.0375f : .1f;
				}
				if (player.position.X > npc.position.X + 10)
				{
					npc.velocity.X -= npc.velocity.X < .1f ? 0.0075f : .1f;
					npc.direction = -1;
				}
				if (player.position.X < npc.position.X + 10)
				{
					npc.velocity.X += npc.velocity.X > .1f ? 0.0075f : .1f;
					npc.direction = 1;
				}
				if (AI_Timer == 350)
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
			else if (AI_State == State_Attack)
			{
				npc.velocity.X = 1.5f * npc.direction;
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
				if (N.active && N.Distance(npc.Center) < 275f && (N.type == mod.NPCType("Bee")))
				{
					N.target = npc.target;
					N.netUpdate = true;
					hostile = true;
				}
			}
			base.HitEffect(hitDirection, damage);
		}
		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			base.OnHitPlayer(target, damage, crit);
			target.AddBuff(BuffID.Poisoned, 675);
		}
		private const int Frame_Float = 0;
		private const int Frame_Float_2 = 1;
		private const int Frame_Float_3 = 2;
		private const int Frame_Float_4 = 3;
		private const int Frame_AngryFloat = 4;
		private const int Frame_AngryFloat_2 = 5;
		private const int Frame_AngryFloat_3 = 6;
		private const int Frame_AngryFloat_4 = 7;
		public override void FindFrame(int frameHeight)
		{
			// This makes the sprite flip horizontally in conjunction with the npc.direction.
			npc.spriteDirection = npc.direction;
			if (AI_State == State_Attack)
			{
				npc.frameCounter++;
				if (npc.frameCounter < 10)
				{
					npc.frame.Y = Frame_AngryFloat * frameHeight;
				}
				else if (npc.frameCounter < 20)
				{
					npc.frame.Y = Frame_AngryFloat_2 * frameHeight;
				}
				else if (npc.frameCounter < 30)
				{
					npc.frame.Y = Frame_AngryFloat_3 * frameHeight;
				}
				else if (npc.frameCounter < 40)
				{
					npc.frame.Y = Frame_AngryFloat_4 * frameHeight;
				}
				else
				{
					npc.frameCounter = 0;
				}
			}
			else
			{
				npc.frameCounter++;
				if (npc.frameCounter < 9)
				{
					npc.frame.Y = Frame_Float * frameHeight;
				}
				else if (npc.frameCounter < 20)
				{
					npc.frame.Y = Frame_Float_2 * frameHeight;
				}
				else if (npc.frameCounter < 30)
				{
					npc.frame.Y = Frame_Float_3 * frameHeight;
				}
				else if (npc.frameCounter < 40)
				{
					npc.frame.Y = Frame_Float_4 * frameHeight;
				}
				else
				{
					npc.frameCounter = 0;
				}
			}
		}
	}
}