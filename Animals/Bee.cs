using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
using System;
using MinecraftAnimals.BaseAI;

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
		internal enum AIStates
		{
			Passive = 0,
			Attack = 1,
			Death = 2,
		}
		internal ref float GlobalTimer => ref npc.ai[0];
		internal ref float Phase => ref npc.ai[1];
		internal ref float ActionPhase => ref npc.ai[2];
		internal ref float AttackTimer => ref npc.ai[3];

		public override void AI()
		{
			int x = (int)(npc.Center.X + (((npc.width / 2) + 16) * npc.direction)) / 16;
			int y = (int)(npc.Center.Y + (npc.height / 2) - 4) / 16;
			Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
			GlobalTimer++;
			Player player = Main.player[npc.target];
			if (Phase == (int)AIStates.Passive)
			{
				npc.damage = 0;
				npc.TargetClosest(false);
				if (GlobalTimer == 5)
				{
					npc.direction = Main.rand.Next(2) == 1 ? npc.direction = 1 : npc.direction = -1;
				}

				float isMoving = GlobalTimer <= 500 ? npc.velocity.X = 1 * npc.direction : npc.velocity.X = 0 * npc.direction; //basic passive movement for 500 ticks then stationary 300
				if (GlobalTimer >= 800)
				{
					GlobalTimer = 0;
				}
			}
			if (Phase == (int)AIStates.Attack)
			{
				npc.TargetClosest(true);
				npc.damage = 30;
				npc.velocity.X = 1.45f * npc.direction;
				if (player.Distance(npc.Center) > 925f)
				{
					Phase = (int)AIStates.Passive;
				}
			}
			if (Phase == (int)AIStates.Death)
			{
				npc.damage = 0;
				npc.ai[2] += 1f; // increase our death timer.
				npc.netUpdate = true;
				npc.velocity.X = 0;
				npc.velocity.Y += 1.5f;
				npc.dontTakeDamage = true;
				npc.rotation = GeneralMethods.ManualMobRotation(npc.rotation, MathHelper.ToRadians(180f), 16f);
				if (npc.ai[2] >= 110f)
				{
					for (int i = 0; i < 20; i++)
					{
						int dustIndex = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustType<Dusts.Poof>(), 0f, 0f, 100, default(Color), 1f); //spawns ender dust
						Main.dust[dustIndex].noGravity = true;
					}
					npc.life = 0;
				}
			}
			if (player.position.Y < npc.position.Y + 30)
			{
				npc.velocity.Y -= npc.velocity.Y > 0f ? 0.5f : .2f;
			}
			if (player.position.Y > npc.position.Y + 30)
			{
				npc.velocity.Y += npc.velocity.Y < 0f ? 0.5f : .1f;
			}
            if (npc.ai[3] == -10)
            {
				Phase = (int)AIStates.Attack;
			}
		}
		public override void HitEffect(int hitDirection, double damage)
		{
			npc.friendly = false;
			Phase = (int)AIStates.Attack;
			// Thanks Joost
			for (int n = 0; n < 200; n++)
			{
				NPC N = Main.npc[n];
				if (N.active && N.Distance(npc.Center) < 275f && (N.type == NPCType<Bee>()))
				{
					N.netUpdate = true;
					N.target = npc.target;
					N.ai[3] = -10;
				}
			}
			if (npc.life <= 0)
			{
				GlobalTimer = 0;
				npc.life = 1;
				Phase = (int)AIStates.Death;
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
			int i = 1;
			// This makes the sprite flip horizontally in conjunction with the npc.direction.
			npc.spriteDirection = npc.direction;
			if (Phase == (int)AIStates.Passive)
			{
				npc.frameCounter++;
				if (GlobalTimer <= 500)
				{
					if (++npc.frameCounter % 7 == 0)
						npc.frame.Y = (npc.frame.Y / frameHeight + 1) % (Main.npcFrameCount[npc.type] / 2) * frameHeight;
				}
				else
				{
					npc.frame.Y = Frame_Float * frameHeight;
				}
			}
			if (Phase == (int)AIStates.Attack)
			{
				npc.frameCounter++;
				if (npc.frameCounter < 7)
				{
					npc.frame.Y = Frame_AngryFloat * frameHeight;
				}
				else if (npc.frameCounter < 14)
				{
					npc.frame.Y = Frame_AngryFloat_2 * frameHeight;
				}
				else if (npc.frameCounter < 21)
				{
					npc.frame.Y = Frame_AngryFloat_3 * frameHeight;
				}
				else if (npc.frameCounter < 28)
				{
					npc.frame.Y = Frame_AngryFloat_4 * frameHeight;
				}
				else
				{
					npc.frameCounter = 0;
				}
			}
			if (Phase == (int)AIStates.Death)
			{
				npc.frame.Y = Frame_Float * frameHeight;
			}
		}
	}
}
