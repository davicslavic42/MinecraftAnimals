using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Items.Herbs
{
    public class BoneMeal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bone Meal");
            Tooltip.SetDefault("Important for healthy saplings.");
        }

        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 14;
            item.maxStack = 99;
            item.rare = ItemRarityID.White;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item81;
            item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            return TileLoader.IsSapling(Main.tile[Player.tileTargetX, Player.tileTargetY].type);
        }

        // Note that this item does not work in Multiplayer, but serves as a learning tool for other things.
        public override bool UseItem(Player player)
        {
            if (WorldGen.GrowTree(Player.tileTargetX, Player.tileTargetY))
            {
                WorldGen.TreeGrowFXCheck(Player.tileTargetX, Player.tileTargetY);
            }
            /*
            else
            {
                item.stack++;
            }
            */
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<Materials.Bone>(), 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this, 3);
            recipe.AddRecipe();
        }
    }
}