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

namespace MinecraftAnimals
{
	public class MCAWorld : ModWorld
	{
		public static bool RaidEvent = false;
		public static int RaidWaves = NPC.waveNumber;
		public static float RaidKillCount = NPC.waveKills;
		public static int MaxRaidWaves = 7;
		public static bool downedRaid = false;
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
			NetMessage.SendData(78, 1, -1, null, (int)RaidKillCount, progressPerWave, 2f, RaidWaves);
			if (RaidKillCount >= progressPerWave)
			{
				RaidKillCount = 0;
				RaidWaves += 1;
			}
			if (RaidWaves == MaxRaidWaves)
			{
				RaidEvent = false;
				downedRaid = true;
				if (Main.netMode == NetmodeID.Server)
					NetMessage.SendData(MessageID.WorldData); // Immediately inform clients of new world state.
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
				RaidKillCount = 0;
				RaidWaves = 0;
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

						*/
		}
	}
}