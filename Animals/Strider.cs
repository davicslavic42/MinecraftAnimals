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
	public class Strider : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Strider");
			Main.npcFrameCount[npc.type] = 5;
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
			if (Main.tile[(int)(npc.Center.X / 16), (int)(npc.Center.Y + 2f) / 16].liquid > 50)
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

