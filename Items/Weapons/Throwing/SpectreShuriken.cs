using Terraria.ModLoader;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using IlwydsMod.Projectiles.Throwing;

namespace IlwydsMod.Items.Weapons.Throwing {
    class SpectreShuriken : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Spectre Shuriken");
			Tooltip.SetDefault("Throws an infinite shuriken");
        }

        public override void SetDefaults() {
            item.thrown = true;
            item.useTime = 40;
            item.UseSound = SoundID.Item3;
            item.noMelee = true;
            item.shoot = ProjectileType<SpectreShurikenProjectile>();
            item.shootSpeed = 1f;
            item.maxStack = 1;
            item.damage = 20;
            item.rare = 1;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
        }

        public override void AddRecipes() {
            ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.SpectreBar, 1);
            recipe.AddIngredient(ItemID.Shuriken, 1);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
        }

        public override bool Shoot(Terraria.Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) {
            return true;
        }
    }
}