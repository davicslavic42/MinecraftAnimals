using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.World.Generation;
using Terraria.ID;
using MinecraftAnimals.Tiles;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.Generation;

namespace MinecraftAnimals.Tiles.GrassTiles
{
    public class GrassTile : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
			Main.tileMerge[Type][TileType<GrassTile>()] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
			Main.tileBlendAll[Type] = true;
			TileID.Sets.Grass[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
			TileID.Sets.Conversion.Grass[Type] = true;
			TileID.Sets.NeedsGrassFramingDirt[Type] = TileID.Dirt;
			drop = ItemType<Items.Blocks.Dirt>();
            AddMapEntry(new Color(100, 220, 25));
			SetModTree(new Trees.Birch());
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
		public override int SaplingGrowthType(ref int style)
		{
			style = 0;
			return TileType<Trees.BirchSaplingtile>();
		}
        public override void RandomUpdate(int i, int j)
        {
			WorldGen.SpreadGrass(i, j, TileType<Dirttile>(), TileType<GrassTile>());
            base.RandomUpdate(i, j);
        }
        /*
		 * 		public override void RandomUpdate(int i, int j)
        {
			for (int k = 0; k < (int)((Main.maxTilesX * (int)WorldGen.worldSurface) * 0.75); k++)
            {
				i = WorldGen.genRand.Next(60, Main.maxTilesX - 60);
				j = (int)(WorldGen.worldSurface * 0.35);
				j = GeneralMethods.FindType(i, j, -1, TileType<Dirttile>());
				int tileChooseDir = Main.rand.NextBool() ?  1 : -1;
				if (j > 1)
				{
					Tile tile = Framing.GetTileSafely(i , j );
					WorldGen.SquareTileFrame(i , j );//+ tileChooseDir
					if (tile.active() && tile.type == TileType<Dirttile>())
					{
						WorldGen.TileRunner(i, j, WorldGen.genRand.Next(1, 2), WorldGen.genRand.Next(1, 2), TileType<GrassTile>(), false, 0, 0, false, true);
					}
					//Main.NewText(tile);
				}
			}
        }

        		public static void SpreadGrass(int i, int j, int dirt = 0, int grass = 2, bool repeat = true, byte color = 0)
		{
			try
			{
				if (InWorld(i, j, 1) && Main.tile[i, j].type == dirt && Main.tile[i, j].active() && ((double)j < Main.worldSurface || dirt != 0))
				{
					int num = i - 1;
					int num2 = i + 2;
					int num3 = j - 1;
					int num4 = j + 2;
					if (num < 0)
					{
						num = 0;
					}
					if (num2 > Main.maxTilesX)
					{
						num2 = Main.maxTilesX;
					}
					if (num3 < 0)
					{
						num3 = 0;
					}
					if (num4 > Main.maxTilesY)
					{
						num4 = Main.maxTilesY;
					}
					bool flag = true;
					for (int k = num; k < num2; k++)
					{
						for (int l = num3; l < num4; l++)
						{
							if (!Main.tile[k, l].active() || !Main.tileSolid[Main.tile[k, l].type])
							{
								flag = false;
							}
							if (Main.tile[k, l].lava() && Main.tile[k, l].liquid > 0)
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag && TileID.Sets.CanBeClearedDuringGeneration[Main.tile[i, j].type] && (grass != 23 || Main.tile[i, j - 1].type != 27) && (grass != 199 || Main.tile[i, j - 1].type != 27))
					{
						Main.tile[i, j].type = (ushort)grass;
						Main.tile[i, j].color(color);
						for (int m = num; m < num2; m++)
						{
							for (int n = num3; n < num4; n++)
							{
								if (Main.tile[m, n].active() && Main.tile[m, n].type == dirt)
								{
									try
									{
										if (repeat && grassSpread < 1000)
										{
											grassSpread++;
											SpreadGrass(m, n, dirt, grass, repeat: true, 0);
											grassSpread--;
										}
									}
									catch
									{
									}
								}
							}
						}
					}
				}
			}
			catch
			{
			}
		}

        */
    }
}