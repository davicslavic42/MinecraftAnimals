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
            item.useAnimation = 2;
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
            for (int k = 0; k < (int)((Main.maxTilesX * (int)WorldGen.worldSurface) * 0.75); k++)
            {
                // The inside of this for loop corresponds to one single splotch of our Ore.
                // First, we randomly choose any coordinate in the world by choosing a random x and y value.
                int x = WorldGen.genRand.Next(2) == 0 ? WorldGen.genRand.Next(60, Main.maxTilesX / 5) : WorldGen.genRand.Next(Main.maxTilesX / 5 * 4, Main.maxTilesX - 60);
                int y = (int)(WorldGen.worldSurface * 0.35);
                y = GeneralMethods.FindType(x, y, -1, TileID.Grass);
                if (y > 1)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    WorldGen.SquareTileFrame(x, y);
                    if (tile.active() && tile.type == TileID.Grass)//|| tile.type == TileID.FleshGrass || tile.type == TileID.CorruptGrass
                    {
                        WorldGen.TileRunner(x, y, WorldGen.genRand.Next(2, 3), WorldGen.genRand.Next(2, 3), TileType<Tiles.GrassTiles.GrassTile>(), false, 0, 0, false, true);//WorldGen.genRand.Next(2, 3)
                    }
                }
                y = (int)(WorldGen.worldSurface * 0.35);
                y = GeneralMethods.FindType(x, y, -1, TileID.FleshGrass);
                if (y > 1)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    WorldGen.SquareTileFrame(x, y);
                    if (tile.active() && tile.type == TileID.FleshGrass)
                    {
                        WorldGen.TileRunner(x, y, 1, 1, TileType<Tiles.GrassTiles.GrassTile>(), false, 0, 0, false, true);//WorldGen.genRand.Next(2, 3)
                    }
                }
                y = (int)(WorldGen.worldSurface * 0.35);
                y = GeneralMethods.FindType(x, y, -1, TileID.CorruptGrass);
                if (y > 1)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    WorldGen.SquareTileFrame(x, y);
                    if (tile.active() && tile.type == TileID.CorruptGrass)//||  || tile.type == TileID.CorruptGrass
                    {
                        WorldGen.TileRunner(x, y, 1, 1, TileType<Tiles.GrassTiles.GrassTile>(), false, 0, 0, false, true);//WorldGen.genRand.Next(2, 3)
                    }
                }
            }
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
