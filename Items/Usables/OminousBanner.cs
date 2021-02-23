using MinecraftAnimals.Raid;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria.Localization;

namespace MinecraftAnimals.Items.Usables
{
    public class OminousBanner : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("waving this around probably isn't the greatest idea");
        }

        public override void SetDefaults()
        {
            item.maxStack = 64;
            item.width = 20;
            item.height = 30;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.value = 0;
            item.noMelee = true;
            item.consumable = true;
        }
        public override bool CanUseItem(Player player)
        {
            return true;
        }
        public override bool UseItem(Player player)
        {
            //float distanceToSpawn = Vector2.Distance(new Vector2(player.position.X, player.position.Y), new Vector2(player.SpawnX, player.SpawnY));
            Vector2 OriginalSpawn = new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16);
            if (!RaidWorld.RaidEvent && RaidWorld.RaidWaves == 0 && player.Distance(OriginalSpawn) <= 50f)//&& 
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
                }
                else
                {
                    string warn = "5 town npcs near your original spawnpoint are needed to start the Raid, current active town members: " + RaidWorld.townNpcCount;
                    Main.NewText(Language.GetTextValue(warn), messageColor);
                }
            }
            return true;
        }
    }
}