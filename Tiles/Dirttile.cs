﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Tiles
{
	public class Dirttile : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			drop = ItemType<Items.Blocks.Dirt>();
			AddMapEntry(new Color(170, 220, 0));
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
	}
}