using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MinecraftAnimals.Animals.Neutral
{
	public class Strider : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Strider");
			Main.npcFrameCount[npc.type] = 8;
		}
		public override void SetDefaults()
		{
			npc.width = 42;
			npc.height = 48;
			npc.lifeMax = 100;
			npc.lavaImmune = true;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.aiStyle = 7;
			aiType = NPCID.Bunny;
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return SpawnCondition.Underworld.Chance * 0.1f;
		}
		public override void AI()
		{
			Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
			if (Main.tile[(int)(npc.Center.X / 16), (int)(npc.Center.Y / 16) / 16].liquid > 255)
			{
				npc.velocity = new Vector2(npc.direction * 2, -4f);
				npc.velocity.Y = 0;
			}
		}
		private const int Frame_Walk = 0;
		private const int Frame_Walk_2 = 1;
		private const int Frame_Walk_3 = 2;
		private const int Frame_Walk_4 = 3;
		private const int Frame_Walk_5 = 4;
		private const int Frame_Walk_6 = 5;
		private const int Frame_Walk_7 = 6;
		private const int Frame_Walk_8 = 7;

		public override void FindFrame(int frameHeight)
		{
			// This makes the sprite flip horizontally in conjunction with the npc.direction.
			npc.spriteDirection = npc.direction;
			{
				npc.frameCounter++;
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
	}
}

