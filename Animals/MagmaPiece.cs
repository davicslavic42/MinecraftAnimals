using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;

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
		internal enum AIStates
		{
			Passive = 0,
			Jump = 1,
			Crouch = 2,
			Death = 3
		}
		internal ref float GlobalTimer => ref npc.ai[0];
		internal ref float Phase => ref npc.ai[1];
		internal ref float ActionPhase => ref npc.ai[2];
		internal ref float AttackTimer => ref npc.ai[3];
		public float Rotations = 6.6f;
		// Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
		public override void AI()
		{
			Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
			Player player = Main.player[npc.target];
			npc.TargetClosest(true);
			GlobalTimer++;
			// The npc starts in the asleep state, waiting for a player to enter range
			if (Phase == (int)AIStates.Passive)
			{
				if (GlobalTimer == 5)
				{
					_ = Main.rand.Next(2) == 1 ? npc.direction = 1 : npc.direction = -1;
					npc.velocity = new Vector2(npc.direction * 2, -4f);
				}
				if (GlobalTimer >= 245 && Collision.SolidCollision(npc.position, (npc.width), npc.height + 1))
				{
					GlobalTimer = 0;
				}
				if (npc.HasValidTarget && player.Distance(npc.Center) < 740f)
				{
					// Since we have a target in range, we change to the Notice state. (and zero out the Timer for good measure)
					Phase = (int)AIStates.Crouch;
					GlobalTimer = 0;
				}
			}
			// In this state, a player has been targeted
			if (Phase == (int)AIStates.Crouch)
			{
				npc.velocity.X = npc.direction * 0;
				// If the targeted player is in attack range (250).
				if (player.Distance(npc.Center) < 720f)
				{
					AttackTimer++;
					if (AttackTimer >= 20)
					{
						Phase = (int)AIStates.Jump;
						AttackTimer = 0;
						GlobalTimer = 0;
					}
				}
				else
				{
					if (!npc.HasValidTarget || player.Distance(npc.Center) > 740f)
					{
						// Out targeted player seems to have left our range, so we'll go back to sleep.
						Phase = (int)AIStates.Passive;
						GlobalTimer = 0;
					}
				}
			}
			if (Phase == (int)AIStates.Jump)
			{
				if (GlobalTimer >= 5 && GlobalTimer <= 20)
				{
					npc.velocity = new Vector2(npc.direction * 2, -5f);
				}
				if (GlobalTimer >= 5 && GlobalTimer <= 20 && player.Distance(npc.Center) < 220f)
				{
					npc.velocity = new Vector2(npc.direction * 4, -3f);
				}

				if (GlobalTimer >= 50 && Collision.SolidCollision(npc.position, (npc.width), npc.height + 1))
				{
					Phase = (int)AIStates.Crouch;
					GlobalTimer = 0;
				}
				if (GlobalTimer > 15 && Collision.SolidCollision(npc.position, (npc.width), npc.height + 1))
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
			if (Phase == (int)AIStates.Passive)
			{
				npc.frameCounter++;
				if (npc.frameCounter < 7)
				{
					npc.frame.Y = Frame_Walk * frameHeight;
				}
				if (npc.frameCounter < 14)
				{
					npc.frame.Y = Frame_Walk * frameHeight;
				}
				else
				{
					npc.frameCounter = 0;
				}
			}
			if (Phase == (int)AIStates.Crouch)
			{
				npc.frame.Y = Frame_Walk_2 * frameHeight;
			}
			else if (Phase == (int)AIStates.Jump)
			{
				npc.frameCounter++;
				if (npc.frameCounter < 6)
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