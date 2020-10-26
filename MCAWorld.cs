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
		public static bool Raid = false;
		public static int Raidkills = 0;
		public static int RaidKillCount = 0;
		public static int MaxRaidKillCount = 100;

		public static bool downedRaid = false;
		public override TagCompound Save()
		{
			TagCompound data = new TagCompound();
			var downed = new List<string>();
			if (Raid)
				downed.Add("Raid");
			data.Add("downed", downed);

			return data;
		}
		public override void Load(TagCompound tag)
		{
			var downed = tag.GetList<string>("downed");
			Raid = downed.Contains("Raid");
		}
		public override void NetReceive(BinaryReader reader)
		{
			BitsByte flags = reader.ReadByte();
			downedRaid = flags[0];
			flags = reader.ReadByte();
			Raid = flags[1];
			RaidKillCount = reader.ReadInt32();
		}
		public override void PreUpdate()
		{
			MaxRaidKillCount = 100;
			if (RaidKillCount >= MaxRaidKillCount)
			{
				Raid = false;
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
			}
		}
	}
}