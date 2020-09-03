using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace MinecraftAnimals.Animals
{
	public class MagmaPiece : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Magma Piece");
			Main.npcFrameCount[npc.type] = 3;
		}
		public override void SetDefaults()
		{
			npc.width = 15;
			npc.height = 15;
			npc.lifeMax = 15;
			npc.damage = 10;
			npc.lavaImmune = true;
			npc.defense = 2;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.aiStyle = -1;
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return SpawnCondition.Underworld.Chance * 0.02f;
		}
		private const int AI_State_Slot = 0;
		private const int AI_Timer_Slot = 1;
		// Here I define some values I will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
		private const int State_Search = 0;
		private const int State_Notice = 1;
		private const int State_Jump = 2;

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
		// Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
		public override void AI()
		{
			Player player = Main.player[npc.target];
			npc.TargetClosest(true);
			// The npc starts in the asleep state, waiting for a player to enter range
			if (AI_State == State_Search)
			{
				// TargetClosest sets npc.target to the player.whoAmI of the closest player. the faceTarget parameter means that npc.direction will automatically be 1 or -1 if the targeted player is to the right or left. This is also automatically flipped if npc.confused
				// Now we check the make sure the target is still valid and within our specified notice range (500)
				if (npc.HasValidTarget && player.Distance(npc.Center) < 640f)
				{
					// Since we have a target in range, we change to the Notice state. (and zero out the Timer for good measure)
					AI_State = State_Notice;
					AI_Timer = 0;
				}
			}
			// In this state, a player has been targeted
			else if (AI_State == State_Notice)
			{
				npc.velocity.X = npc.direction * 0;
				// If the targeted player is in attack range (250).
				if (player.Distance(npc.Center) < 620f)
				{
					AI_Timer++;
					if (AI_Timer >= 20)
					{
						AI_State = State_Jump;
						AI_Timer = 0;
					}
				}
				else
				{
					if (!npc.HasValidTarget || player.Distance(npc.Center) > 640f)
					{
						// Out targeted player seems to have left our range, so we'll go back to sleep.
						AI_State = State_Search;
						AI_Timer = 0;
					}
				}
			}
			else if (AI_State == State_Jump)
			{
				AI_Timer++;
				if (AI_Timer == 5)
				{
					npc.velocity = new Vector2(npc.direction * 3, -8f);
				}
				if (AI_Timer == 5 && player.Distance(npc.Center) < 220f)
				{
					npc.velocity = new Vector2(npc.direction * 5, -6f);
				}

				if (AI_Timer >= 45 && Collision.SolidCollision(npc.position, (npc.width), npc.height + 1))
				{
					AI_State = State_Notice;
					AI_Timer = 0;
				}
				if (AI_Timer > 10 && Collision.SolidCollision(npc.position, (npc.width), npc.height + 1))
				{
					npc.velocity.X = npc.direction * 0;

				}
			}
		}
		private const int Frame_Walk = 0;
		private const int Frame_Walk_2 = 1;
		private const int Frame_Walk_3 = 2;
		public override void FindFrame(int frameHeight)
		{
			// This makes the sprite flip horizontally in conjunction with the npc.direction.
			npc.spriteDirection = npc.direction;
			if (AI_State == State_Search)
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
			else if (AI_State == State_Notice)
			{
				npc.frameCounter++;
				if (npc.frameCounter < 10)
				{
					npc.frame.Y = Frame_Walk_2 * frameHeight;
				}
				else
				{
					npc.frameCounter = 0;
				}
			}
			else if (AI_State == State_Jump)
			{
				npc.frameCounter++;
				if (npc.frameCounter < 10)
				{
					npc.frame.Y = Frame_Walk_3 * frameHeight;
				}
				else
				{
					npc.frameCounter = 0;
				}
			}
		}
	}
}