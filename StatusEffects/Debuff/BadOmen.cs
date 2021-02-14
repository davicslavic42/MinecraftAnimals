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
            Description.SetDefault("You have a bad feeling about going back to spawn");
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 500;
            float distanceToSpawn = Vector2.Distance(new Vector2(player.position.X, player.position.Y), new Vector2(Main.spawnTileX, Main.spawnTileY));
            if (distanceToSpawn <= 350f)//&& RaidWorld.townNpcCount > 1
            {
                string key = "The Illagers are coming!";
                Color messageColor = Color.Orange;
                RaidWorld.RaidKillCount = 0;
                RaidWorld.RaidWaves = 0;
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
                }
                if (Main.netMode == NetmodeID.Server && player.whoAmI == Main.myPlayer)
                {
                    ModPacket packet = mod.GetPacket();
                    packet.Write((byte)MinecraftAnimals.ModMessageType.StartRaidEvent);
                    packet.Send();
                    player.buffTime[buffIndex] = 0;
                }
            }
        }
    }
}
