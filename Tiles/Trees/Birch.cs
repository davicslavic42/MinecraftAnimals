using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Tiles.Trees
{
	public class Birch : ModTree
	{
		private Mod mod => ModLoader.GetMod("MinecraftAnimals");

		public override int DropWood()
		{
			ItemType<Items.Blocks.Birch>();
			return ItemType<Items.Herbs.BirchSapling>();
		}
		public override int GrowthFXGore()
		{
			return mod.GetGoreSlot("Gores/BirchFX");
		}

		public override Texture2D GetTexture()
		{
			return mod.GetTexture("Tiles/Trees/Birch");
		}

		public override Texture2D GetTopTextures(int i, int j, ref int frame, ref int frameWidth, ref int frameHeight, ref int xOffsetLeft, ref int yOffset)
		{
			return mod.GetTexture("Tiles/Trees/Birch_Tops");
		}

		public override Texture2D GetBranchTextures(int i, int j, int trunkOffset, ref int frame)
		{
			return mod.GetTexture("Tiles/Trees/Birch_Branches");
		}
	}
}