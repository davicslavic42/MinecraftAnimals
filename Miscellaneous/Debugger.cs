using Microsoft.Xna.Framework;
using MinecraftAnimals.Raid;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;



namespace MinecraftAnimals.Miscellaneous
{
    public class Debugger : ModItem
    {
        public override void SetDefaults()
        {
            // Alter any of these values as you see fit, but you should probably keep useStyle on 1, as well as the noUseGraphic and noMelee bools
            item.knockBack = 5f;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 1;
            item.useTime = 2;
            item.width = 30;
            item.height = 30;
            item.maxStack = 1;
            item.rare = ItemRarityID.Pink;

            item.consumable = false;
            item.autoReuse = false;

            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(silver: 5);
            item.createTile = TileType<Tiles.GrassTiles.GrassTile>();
        }
        //thanks gabe
        public override bool CanUseItem(Player player)
        {
            return true;
        }

        public override bool UseItem(Player player)
        {
            Main.NewText(RaidWorld.RaiderCounter + " enemy counter");
            Main.NewText(RaidWorld.RaidKillCount + " kills");
            Main.NewText(RaidWorld.progressPerWave + " progress per wave");
            Main.NewText(RaidWorld.RaidWaves + " current wave");
            Main.NewText(RaidWorld.townNpcCount + " Town memebers");

            return true;
        }
        /*
         *             if (RaidWorld.RaidWaves == 0)
            {
                string key = "The Illagers are coming!";
                Color messageColor = Color.Orange;
                RaidWorld.RaidKillCount = 0;
                RaidWorld.RaidWaves += 1;
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
                }
            }
            else RaidWorld.IncreaseRaidWave();

         */
    }
}
