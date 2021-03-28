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
using Terraria.World.Generation;
using Terraria.ObjectData;

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
		public static int progressPerWave = 25;
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
            writer.Write(townNpcCount);
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
            townNpcCount = reader.ReadInt32();

        }
        public override void PreUpdate()
		{
            townNpcCount = CountTownNPCRaid(); // counts npcs in range of player's spawn point or bed to ensure they meet
            if (RaidEvent)
			{
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
                if (townNpcCount <= 0) LostRaidEvent();// if town npcs die the player loses
			}
			base.PostUpdate();
        }
		public static void IncreaseRaidWave()
        {
            if (RaidWaves != 0)
            {
                string wavekey = ("Wave " + RaidWaves + " has been defeated!");
                Color messageColor1 = Color.GreenYellow;
				RaidWaves += 1;
				RaidKillCount = 0f;
                if (Main.netMode == NetmodeID.Server) // Server
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(wavekey), messageColor1);
                    for (int y = 0; y < Main.ActivePlayersCount; y++)//targets based on active players 
                    {
                        Player player = Main.player[y];
                        Main.PlaySound(SoundID.Roar, player.position, 0);
                    }
                }
                else if (Main.netMode == NetmodeID.SinglePlayer) // Single Player
                {
                    Main.NewText(wavekey, messageColor1);
                    for (int y = 0; y < Main.ActivePlayersCount; y++)//targets based on active players 
                    {
                        Player player = Main.player[y];
                        Main.PlaySound(SoundID.Roar, player.position, 0);
                    }
                }
            }
            else
            {
				RaidKillCount = 0f;
				RaidWaves += 1;
            }
        }

        public static void LostRaidEvent()// in the event that all town memebers are killed the player loses the raid
        {
            if (RaidEvent)
            {
                Color messageColor = Color.Red;
                string loseKey = "Your town members were slaughtered!";
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    RaidKillCount = 0f;
                    RaidWaves = 0;
                    RaidEvent = false;
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    for (int y = 0; y < Main.ActivePlayersCount; y++)//targets based on active players 
                    {
                        Player player = Main.player[y];
                        Main.PlaySound(SoundID.Roar, player.position, 0);
                    }
                    NetMessage.SendData(MessageID.WorldData);
                    RaidWaves = 0;
                    RaidKillCount = 0f;
                    RaidEvent = false;
                    // Immediately inform clients of new world state.
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(loseKey), messageColor);//if(RaidWaves >= 7)
                }
                else if (Main.netMode == NetmodeID.SinglePlayer) // Single Player
                {
                    Main.NewText((loseKey), messageColor);//if(RaidWaves >= 7)
                    for (int y = 0; y < Main.ActivePlayersCount; y++)//targets based on active players 
                    {
                        Player player = Main.player[y];
                        Main.PlaySound(SoundID.Roar, player.position, 0);
                    }
                }
            }
        }
        public static void EndRaidEvent()
        {
            RaidEvent = false;
            downedRaid = true;
            Color messageColor = Color.Green;
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
                for (int y = 0; y < Main.ActivePlayersCount; y++)//targets based on active players 
                {
                    Player player = Main.player[y];
                    Main.PlaySound(SoundID.MoonLord, player.position, 0);
                }

            }
        }
        internal int CountTownNPCRaid()
        {
            int TownMembersleft = 0;
            for (int i = 0; i < Main.maxNPCs; i++)//I.active
            {
                Player player = Main.player[i];
                Vector2 OriginalSpawn = new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16);
                Vector2 BedSpawn = new Vector2(player.SpawnX * 16, player.SpawnY * 16);
                NPC I = Main.npc[i];
                if (I.active && I.townNPC && I.friendly && I.aiStyle == 7 && I.chaseable && I.HasGivenName && !NPCID.Sets.TownCritter[I.type] && (I.Distance(OriginalSpawn) <= 3850f || I.Distance(BedSpawn) <= 3850f) && (!I.homeless || I.homeless)) TownMembersleft++;//needed parameters to check for all town npcs cuz vanilla is pain
            }
            return TownMembersleft;
        }

    }
}