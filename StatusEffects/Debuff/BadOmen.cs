using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.ID;
using MinecraftAnimals.Raid;
using System.Linq;
using Terraria.Localization;



namespace MinecraftAnimals.StatusEffects.Debuff
{
    public class BadOmen : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("BadOmen");
            Description.SetDefault("You feel like there are Illagers watching");
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //float distanceToSpawn = Vector2.Distance(new Vector2(player.position.X, player.position.Y), new Vector2(player.SpawnX, player.SpawnY));
            Vector2 OriginalSpawn = new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16);
            Vector2 BedSpawn = new Vector2(player.SpawnX * 16, player.SpawnY * 16);
            if (!RaidWorld.RaidEvent && RaidWorld.RaidWaves == 0 && (player.Distance(OriginalSpawn) <= 150f || player.Distance(BedSpawn) <= 150f))//&& 
            {
                Color messageColor = Color.Orange;
                if (RaidWorld.townNpcCount >= 5)
                {
                    string key = "An Illager Raid is beginning, protect your towns people!";
                    RaidWorld.RaidKillCount = 0;
                    RaidWorld.RaidWaves += 1;
                    RaidWorld.RaidEvent = true;
                    if (Main.netMode == NetmodeID.Server) // Server
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.SinglePlayer) // Single Player
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.PlaySound(SoundID.Roar, player.position, 0);
                        RaidWorld.RaidEvent = true;
                        RaidWorld.RaidKillCount = 0;
                        RaidWorld.RaidWaves += 1;
                    }
                    if (Main.netMode == NetmodeID.Server && player.whoAmI == Main.myPlayer)
                    {
                        ModPacket packet = mod.GetPacket();
                        packet.Write((byte)MinecraftAnimals.ModMessageType.StartRaidEvent);
                        packet.Send();
                    }
                    player.buffTime[buffIndex] = 0;
                }
                else
                {
                    string warn = "5 town npc near your bed or original spawnpoint is needed to start the Raid, current active town members: " + RaidWorld.townNpcCount; // or bed spawnpoint

                    Main.NewText(Language.GetTextValue(warn), messageColor);
                    player.buffTime[buffIndex] = 0;
                }
            }
        }
    }
}
