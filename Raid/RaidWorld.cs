using Microsoft.Xna.Framework;
using MinecraftAnimals.Raid.Illagers;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using MinecraftAnimals.Raid;
using System.Linq;

namespace MinecraftAnimals.Raid
{
    public class RaidWorld : ModWorld
    {
        public static bool RaidEvent = false;
        public static int RaidWaves = 0;
        public static float RaidKillCount = 0f;
		public static bool downedRaid = false;
        public static int RaiderCounter = 0;// MAKE SURE TO FIGURE OUT WHETHRE TO USE RAIDERS INT OR RAID ENEMY TYPE
		public static int townNpcCount = 0;
		public static int[] Raiders = (new int[5]
        {
                NPCType<Evoker>(),
                NPCType<Pillager>(),
                NPCType<Ravager>(),
                NPCType<Witch>(),
                NPCType<Vindicator>()
        });
		internal ref int progressCurrentWave => ref RaidWaves;

		public static int progressPerWave = 25;



		/*
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
		 */

		public override void Initialize()
        {
            downedRaid = false;
            RaidEvent = false;
            RaidWaves = 0;
            RaidKillCount = 0f;
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
			writer.Write(RaidWaves);
			writer.Write(progressPerWave);
		}

		public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedRaid = flags[0];
            flags = reader.ReadByte();

            RaidEvent = flags[1];
            RaidKillCount = reader.ReadInt32();
			RaidWaves = reader.ReadInt32();
			progressPerWave = reader.ReadInt32();

		}
		//progressPerWave = (int)(10 + (progressCurrentWave * 15));
		public override void PreUpdate()
		{
			if (RaidEvent)
			{
				for (int i = 0; i < Main.maxNPCs; i++)//I.active
				{
					NPC I = Main.npc[i];
                    float TownNPCDistancetospawn = I.Distance(new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16));
                }
                townNpcCount = GeneralMethods.CountTownNPCRaid();
                RaiderCounter = NPC.CountNPCS(NPCType<Pillager>()) + NPC.CountNPCS(NPCType<Evoker>()) + NPC.CountNPCS(NPCType<Ravager>()) + NPC.CountNPCS(NPCType<Witch>()) + NPC.CountNPCS(NPCType<Vindicator>());
			}
		}

        public override void PostUpdate()
        {
			if (RaidEvent)
            {
				if (((int)RaidKillCount >= progressPerWave))
				{
					RaidKillCount = 0f;
					IncreaseRaidWave();
				}
				if (RaidWaves >= 7) EndRaidEvent();
                if (townNpcCount <= 0) LostRaidEvent();
			}
			base.PostUpdate();
        }
		public static void IncreaseRaidWave()
        {
            if (RaidWaves != 0)
            {
                string wavekey = ("Wave " + RaidWaves + " has been defeated!");
                Color messageColor1 = Color.Red;
				RaidWaves += 1;
				RaidKillCount = 0f;
                if (Main.netMode == NetmodeID.Server) // Server
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(wavekey), messageColor1);
                }
                else if (Main.netMode == NetmodeID.SinglePlayer) // Single Player
                {
                    Main.NewText(wavekey, messageColor1);
                }
            }
            else
            {
				RaidKillCount = 0f;
				RaidWaves += 1;
            }
        }

        public static void LostRaidEvent()
        {
            if (RaidEvent)
            {
                Color messageColor = Color.Orange;
                RaidEvent = false;
                string loseKey = "Your town members were slaughtered!";
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    RaidKillCount = 0f;
                    RaidWaves = 0;
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                    RaidWaves = 0;
                    RaidKillCount = 0f;
                    // Immediately inform clients of new world state.
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(loseKey), messageColor);//if(RaidWaves >= 7)
                }
                else if (Main.netMode == NetmodeID.SinglePlayer) // Single Player
                {
                    Main.NewText((loseKey), messageColor);//if(RaidWaves >= 7)
                                                          //else Main.NewText((loseKey), messageColor);
                }
            }
        }
        public static void EndRaidEvent()
        {
            RaidEvent = false;
            downedRaid = true;
            Color messageColor = Color.Orange;
            string key = "The Raid has been defeated!";
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                RaidKillCount = 0f;
                RaidWaves = 0;
            }
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData);
                RaidWaves = 0;
                RaidKillCount = 0f;
                // Immediately inform clients of new world state.
                NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);//if(RaidWaves >= 7)
            }
            else if (Main.netMode == NetmodeID.SinglePlayer) // Single Player
            {
                Main.NewText((key), messageColor);//if(RaidWaves >= 7)
            }
        }

    }
}