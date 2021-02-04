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
		static int RaidEnemyType = 0; //attribute to contain the raid enemeies
	    public static int Raiders = (new int[5]
		{
				NPCType<Evoker>(),
				NPCType<Pillager>(),
				NPCType<Ravager>(),
				NPCType<Witch>(),
				NPCType<Vindicator>()
		})[RaidEnemyType];
		public static int RaiderCounter = NPC.CountNPCS(RaidEnemyType);// MAKE SURE TO FIGURE OUT WHETHRE TO USE RAIDERS INT OR RAID ENEMY TYPE
		public static int progressPerWave = (new int[8]
		{
				0,
				20,
				30,
				40,
				50,
				70,
				90,
				0
		})[RaidWaves];


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
			string wavekey = ("Wave has been defeated!");
			Color messageColor1 = Color.Red;
			RaidWaves = 0;
			RaidKillCount = 0f;
			if (RaidKillCount >= progressPerWave && progressPerWave != 0)
            {
				RaidWaves += 1;
				RaidKillCount = 0f;
				if (Main.netMode == 2) // Server
				{
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(wavekey), messageColor1);
				}
				else if (Main.netMode == 0) // Single Player
				{
					Main.NewText("Wave has been defeated!", messageColor1);
				}
			}
			if (RaidWaves == 7)
			{
				EndRaidEvent();
			}
			if (RaidKillCount != 0 ) //num3 is wavekilss and num2 is a float value added to wave kills
			{
				if (Main.netMode != 1)
				{
					Main.ReportInvasionProgress((int)RaidKillCount, progressPerWave, 1, RaidWaves);
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(78, -1, -1, null, 1, progressPerWave, RaidKillCount, RaidWaves);
				}
			}
			if(RaidKillCount % 10f == 0 && RaidKillCount != 0)
            {
				Main.NewText("your raidkills are actually working!");
            }
		}
		public static void EndRaidEvent()
		{
			if (RaidEvent)
			{
				Color messageColor = Color.Orange;
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
			if (waveKills != num3 && num2 != 0f) //num3 is wavekilss and num2 is a float value added to wave kills
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
				private void CheckProgressPumpkinMoon()
		{
			if (!Main.pumpkinMoon)
			{
				return;
			}
			NetworkText networkText = NetworkText.Empty;
			int[] array = new int[16]
			{
				0,
				25,
				40,
				50,
				80,
				100,
				160,
				180,
				200,
				250,
				300,
				375,
				450,
				525,
				675,
				0
			};
			int num = array[waveNumber];
			switch (waveNumber)
			{
			case 1:
				networkText = Lang.GetInvasionWaveText(2, 305, 326);
				break;
			case 2:
				networkText = Lang.GetInvasionWaveText(3, 305, 326, 329);
				break;
			case 3:
				networkText = Lang.GetInvasionWaveText(4, 305, 326, 329, 325);
				break;
			case 4:
				networkText = Lang.GetInvasionWaveText(5, 305, 326, 329, 330, 325);
				break;
			case 5:
				networkText = Lang.GetInvasionWaveText(6, 326, 329, 330, 325);
				break;
			case 6:
				networkText = Lang.GetInvasionWaveText(7, 305, 329, 330, 327);
				break;
			case 7:
				networkText = Lang.GetInvasionWaveText(8, 326, 329, 330, 327);
				break;
			case 8:
				networkText = Lang.GetInvasionWaveText(9, 305, 315, 325, 327);
				break;
			case 9:
				networkText = Lang.GetInvasionWaveText(10, 326, 329, 330, 315, 325, 327);
				break;
			case 10:
				networkText = Lang.GetInvasionWaveText(11, 305, 326, 329, 330, 315, 325, 327);
				break;
			case 11:
				networkText = Lang.GetInvasionWaveText(12, 326, 329, 330, 315, 325, 327);
				break;
			case 12:
				networkText = Lang.GetInvasionWaveText(13, 329, 330, 315, 325, 327);
				break;
			case 13:
				networkText = Lang.GetInvasionWaveText(14, 315, 325, 327);
				break;
			case 14:
				networkText = Lang.GetInvasionWaveText(-1, 325, 327);
				break;
			}
			float num2 = 0f;
			switch (type)
			{
			case 305:
			case 306:
			case 307:
			case 308:
			case 309:
			case 310:
			case 311:
			case 312:
			case 313:
			case 314:
				num2 = 1f;
				break;
			case 315:
				num2 = 25f;
				break;
			case 325:
				num2 = 75f;
				break;
			case 326:
				num2 = 2f;
				break;
			case 327:
				num2 = 150f;
				break;
			case 329:
				num2 = 4f;
				break;
			case 330:
				num2 = 8f;
				break;
			}
			if (Main.expertMode)
			{
				num2 *= 2f;
			}
			float num3 = waveKills;
			waveKills += num2;
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
						AchievementsHelper.NotifyProgressionEvent(15);
					}
				}
			}
			if (waveKills != num3 && num2 != 0f)
			{
				if (Main.netMode != 1)
				{
					Main.ReportInvasionProgress((int)waveKills, num, 2, waveNumber);
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(78, -1, -1, null, Main.invasionProgress, Main.invasionProgressMax, 2f, waveNumber);
				}
			}
		}

		public static void ResetKillCount()
		{
			for (int i = 0; i < killCount.Length; i++)
			{
				killCount[i] = 0;
			}
		}


			*/

	}
}