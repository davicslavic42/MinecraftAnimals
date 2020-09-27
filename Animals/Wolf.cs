using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using static Terraria.ModLoader.ModContent;
using MinecraftAnimals.Items;
using System;

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
		public enum AIStates
		{
			Passive = 0,
			Attack = 1,
			Death = 2,
			Follow = 3
		}
		internal ref float GlobalTimer => ref npc.ai[0];
		internal ref float Phase => ref npc.ai[1];
		internal ref float AttackPhase => ref npc.ai[2];
		internal ref float AttackTimer => ref npc.ai[3];
		public float Rotations = 6.6f;
		public bool hostile = false;

		public override void AI()
		{
			Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
			GlobalTimer++;
			Player player = Main.player[npc.target];
			if (Phase == (int)AIStates.Passive)
			{
				npc.damage = 0;
				npc.TargetClosest(false);
				if (GlobalTimer == 5)
				{
					_ = Main.rand.Next(2) == 1 ? npc.direction = 1 : npc.direction = -1;
				}

				_ = GlobalTimer <= 500 ? npc.velocity.X = 1 * npc.direction : npc.velocity.X = 0 * npc.direction;
				if (GlobalTimer >= 800)
				{
					GlobalTimer = 0;
				}
				if (player.HeldItem.type == ItemType<Bone>()) Phase = (int)AIStates.Follow;
			}
			if (Phase == (int)AIStates.Attack)
			{
				npc.TargetClosest(true);
				npc.damage = 30;
				npc.velocity.X = 2 * npc.direction;
				AttackTimer++;
				if (player.Distance(npc.Center) > 925f)
				{
					hostile = false;
					Phase = (int)AIStates.Passive;
				}
			}
			if (Phase == (int)AIStates.Death)
			{
				GlobalTimer = 0;
				npc.velocity.X = 0;
				float rotslow = 0.60f;
				for (int i = 0; i < 60; i++)
				{
					Rotations *= rotslow;
				}
				_ = GlobalTimer <= 60 ? npc.rotation *= MathHelper.ToRadians(Rotations * 3.5f) : npc.rotation = MathHelper.ToRadians(90f);
			}
			if (Phase == (int)AIStates.Follow)
			{
				npc.TargetClosest(true);
				npc.velocity.X = 1.5f * npc.direction;
				if (player.Distance(npc.Center) < 45f)
				{
					npc.velocity.X = 0;
				}
				if (player.HeldItem.type != ItemType<Bone>())
                {
					Phase = (int)AIStates.Passive;
				}
			}
			if(hostile == true)
            {
				Phase = (int)AIStates.Attack;
			}
		}
		public override bool CheckDead()
		{
			Phase = (int)AIStates.Death;
			if (GlobalTimer <= 100)
			{
				npc.dontTakeDamage = true;
				npc.friendly = true;
				npc.damage = 0;
				npc.netUpdate = true;
			}
			return false;
		}
		//Thanks oli//

		public override void HitEffect(int hitDirection, double damage)
		{
			Phase = (int)AIStates.Attack;
			GlobalTimer = 0;
			for (int n = 0; n < 200; n++)
			{
				NPC N = Main.npc[n];
				if (N.active && N.Distance(npc.Center) < 475f && (N.type == ModContent.NPCType<Wolf>()))
				{
					N.target = npc.target;
					N.netUpdate = true;
					hostile = true;
				}
			}
			base.HitEffect(hitDirection, damage);
		}
		// The npc starts in the asleep state, waiting for a player to enter range
		// Our texture is 32x32 with 2 pixels of padding vertically, so 34 is the vertical spacing.  These are for my benefit and the numbers could easily be used directly in the code below, but this is how I keep code organized.
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

		// Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
		// We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, I have defined some consts above.
		public override void FindFrame(int frameHeight)
		{
			// This makes the sprite flip horizontally in conjunction with the npc.direction.
			Player player = Main.player[npc.target];
			npc.spriteDirection = npc.direction;
			if (Phase == (int)AIStates.Passive)
			{
				npc.frameCounter++;
				if (GlobalTimer <= 500)
				{
					if (npc.frameCounter < 7)
					{
						npc.frame.Y = Frame_Walk * frameHeight;
					}
					else if (npc.frameCounter < 14)
					{
						npc.frame.Y = Frame_Walk_2 * frameHeight;
					}
					else if (npc.frameCounter < 21)
					{
						npc.frame.Y = Frame_Walk_3 * frameHeight;
					}
					else if (npc.frameCounter < 28)
					{
						npc.frame.Y = Frame_Walk_4 * frameHeight;
					}
					else if (npc.frameCounter < 35)
					{
						npc.frame.Y = Frame_Walk_5 * frameHeight;
					}
					else if (npc.frameCounter < 42)
					{
						npc.frame.Y = Frame_Walk_6 * frameHeight;
					}
					else if (npc.frameCounter < 49)
					{
						npc.frame.Y = Frame_Walk_7 * frameHeight;
					}
					else if (npc.frameCounter < 56)
					{
						npc.frame.Y = Frame_Walk_8 * frameHeight;
					}
					else
					{
						npc.frameCounter = 0;
					}
				}
				else
				{
					npc.frame.Y = Frame_Walk * frameHeight;
				}
			}
			if (Phase == (int)AIStates.Follow)
            {
				npc.frameCounter++;
				if (player.Distance(npc.Center) < 45f)
                {
					npc.frame.Y = Frame_Walk * frameHeight;
				}
                else
                {
					if (npc.frameCounter < 7)
					{
						npc.frame.Y = Frame_Walk * frameHeight;
					}
					else if (npc.frameCounter < 14)
					{
						npc.frame.Y = Frame_Walk_2 * frameHeight;
					}
					else if (npc.frameCounter < 21)
					{
						npc.frame.Y = Frame_Walk_3 * frameHeight;
					}
					else if (npc.frameCounter < 28)
					{
						npc.frame.Y = Frame_Walk_4 * frameHeight;
					}
					else if (npc.frameCounter < 35)
					{
						npc.frame.Y = Frame_Walk_5 * frameHeight;
					}
					else if (npc.frameCounter < 42)
					{
						npc.frame.Y = Frame_Walk_6 * frameHeight;
					}
					else if (npc.frameCounter < 49)
					{
						npc.frame.Y = Frame_Walk_7 * frameHeight;
					}
					else if (npc.frameCounter < 56)
					{
						npc.frame.Y = Frame_Walk_8 * frameHeight;
					}
					else
					{
						npc.frameCounter = 0;
					}
				}
			}

			if (Phase == (int)AIStates.Attack)
			{
				npc.frameCounter++;
				if (npc.frameCounter < 7)
				{
					npc.frame.Y = Frame_Angry * frameHeight;
				}
				else if (npc.frameCounter < 14)
				{
					npc.frame.Y = Frame_Angry_2 * frameHeight;
				}
				else if (npc.frameCounter < 21)
				{
					npc.frame.Y = Frame_Angry_3 * frameHeight;
				}
				else if (npc.frameCounter < 28)
				{
					npc.frame.Y = Frame_Angry_4 * frameHeight;
				}
				else if (npc.frameCounter < 35)
				{
					npc.frame.Y = Frame_Angry_5 * frameHeight;
				}
				else if (npc.frameCounter < 42)
				{
					npc.frame.Y = Frame_Angry_6 * frameHeight;
				}
				else if (npc.frameCounter < 49)
				{
					npc.frame.Y = Frame_Angry_7 * frameHeight;
				}
				else if (npc.frameCounter < 56)
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