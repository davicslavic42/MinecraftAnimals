﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinecraftAnimals.Animals
{
	public class SlimePiece : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slime piece");
			Main.npcFrameCount[npc.type] = 2;
		}

		public override void SetDefaults()
		{
			npc.width = 9;
			npc.height = 12;
			npc.lifeMax = 35;
			npc.damage = 10;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.aiStyle = NPCID.BlueSlime;
			aiType = NPCID.BlueSlime;
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return SpawnCondition.OverworldDay.Chance * 0.01f;
		}
		private const int Frame_Hop = 0;
		private const int Frame_Hop_2 = 1;

		public override void FindFrame(int frameHeight)
		{
			// This makes the sprite flip horizontally in conjunction with the npc.direction.
			npc.spriteDirection = npc.direction;
			{
				npc.frameCounter++;
				if (npc.frameCounter < 10)
				{
					npc.frame.Y = Frame_Hop * frameHeight;
				}
				else if (npc.frameCounter < 20)
				{
					npc.frame.Y = Frame_Hop_2 * frameHeight;
				}
				else
				{
					npc.frameCounter = 0;
				}
			}
		}
	}
}