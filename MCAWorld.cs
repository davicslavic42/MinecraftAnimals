using MinecraftAnimals.Tiles;
using MinecraftAnimals.Tiles.Trees;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent.Biomes;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using Terraria.Utilities;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;
using MinecraftAnimals.Animals.Raid;

namespace MinecraftAnimals
{
	public class MCAWorld : ModWorld
	{
		public static bool RaidEvent = false;
		public static int RaidWaves = NPC.waveNumber;
		public static float RaidKillCount = NPC.waveKills;
		public static bool downedRaid = false;
		static int RaidEnemyType = 0;
		int Raiders = (new int[5]
		{
				NPCType<Evoker>(),
				NPCType<Pillager>(),
				NPCType<Ravager>(),
				NPCType<Witch>(),
				NPCType<Vindicator>(),
		})[RaidEnemyType];

		public override void Initialize()
		{
			downedRaid = false;
		}
		public override TagCompound Save()
		{
			return new TagCompound
			{
				{"downedRaid", downedRaid},
			};
		}
		public override void Load(TagCompound tag)
		{
			downedRaid = tag.GetBool("downedRaid");
		}
		public override void NetSend(BinaryWriter writer)
		{
			BitsByte flags = new BitsByte();
			flags[0] = downedRaid;
			writer.Write(flags);
			flags = new BitsByte();
			flags[1] = RaidEvent;
			writer.Write(flags);
			writer.Write(RaidKillCount);
		}

		public override void NetReceive(BinaryReader reader)
		{
			BitsByte flags = reader.ReadByte();
			downedRaid = flags[0];
			flags = reader.ReadByte();
			RaidEvent = flags[1];
			RaidKillCount = reader.ReadInt32();
		}
		public override void PreUpdate()
		{
			RaidWaves = 0;
			RaidKillCount = 0f;
			int MaxRaidWaves = 7;
			int progressPerWave = (new int[8]
			{
				0,
				25,
				40,
				50,
				80,
				100,
				160,
				0
			})[RaidWaves];
			Main.ReportInvasionProgress((int)RaidKillCount, progressPerWave, 1, RaidWaves);
			int RaiderCounter = NPC.CountNPCS(Raiders);
			if (RaidWaves == MaxRaidWaves)
			{
				EndRaidEvent();
			}
		}
		public static void EndRaidEvent()
		{
			if (RaidEvent)
			{
				RaidEvent = false;
				downedRaid = true;
				if (Main.netMode != 1)
				{
					RaidKillCount = 0;
					RaidWaves = 0;
				}
				if (Main.netMode == NetmodeID.Server)
                {
					NetMessage.SendData(MessageID.WorldData); // Immediately inform clients of new world state.
				}
				string key = "The Raid has been defeated!";
				Color messageColor = Color.Orange;
				if (Main.netMode == 2) // Server
				{
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}
				else if (Main.netMode == 0) // Single Player
				{
					Main.NewText("The Raid has been defeated!", messageColor);
				}
				RaidWaves = 0;
				RaidKillCount = 0f;
			}
		}

		/*
			 * 			if (RaidEvent)
			{
				for (int k = 0; k < 7; k++)
				{
					progressPerWave = (RaidWaves * 20);
				}
				NetMessage.SendData(78, 1, -1, null, (int)RaidKillCount, progressPerWave, 2f, RaidWaves);
				return;
			}
			int progressPerWave = (new int[8])[RaidWaves];
					NetMessage.SendData(78, 1, -1, null, (int)RaidKillCount, progressPerWave, 1, RaidWaves);
					float progress = MathHelper.Clamp(nPC.ai[0] / 450f, 0f, 1f);
					if (waveKills >= (float)num && num != 0)
			{
				waveKills = 0f;
				waveNumber++;
				num = array[waveNumber];
				if (networkText != NetworkText.Empty)
				{
					if (Main.netMode == 0)
					{
						Main.NewText(networkText.ToString(), 175, 75);
					}
					else if (Main.netMode == 2)
					{
						NetMessage.BroadcastChatMessage(networkText, new Color(175, 75, 255));
					}
					if (waveNumber == 15)
					{
						AchievementsHelper.NotifyProgressionEvent(14);
					}
				}
			}
			if (waveKills != num3 && num2 != 0f)
			{
				if (Main.netMode != 1)
				{
					Main.ReportInvasionProgress((int)waveKills, num, 1, waveNumber);
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(78, -1, -1, null, Main.invasionProgress, Main.invasionProgressMax, 1f, waveNumber);
				}
			}

			*/

	}
}