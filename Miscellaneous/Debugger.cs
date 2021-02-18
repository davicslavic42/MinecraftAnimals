using Microsoft.Xna.Framework;
using MinecraftAnimals.Raid;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;



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
        }
        //thanks gabe
        private static int FindType(int x, int y, int maxDepth = -1, params int[] types)
        {
            if (maxDepth == -1) maxDepth = (int)(WorldGen.worldSurface); //Set default
            while (true)
            {
                if (y >= maxDepth)
                    break;
                if (Main.tile[x, y].active() && types.Any(i => i == Main.tile[x, y].type))
                    return y; //Returns first valid tile under intitial Y pos, -1 if max depth is reached
                y++;
            }
            return 1; //fallout case
        }
        public override bool CanUseItem(Player player)
        {
            return true;
        }

        public override bool UseItem(Player player)
        {
            if (RaidWorld.RaidWaves == 0)
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
            return true;
        }
    }
}
