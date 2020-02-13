using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace IlwydsMod.Projectiles.Throwing {
    class SpectreShurikenProjectile : ModProjectile {
        public override void SetStaticDefaults() {
            Main.projFrames[projectile.type] = 1;
        }

        public override void SetDefaults() {
            projectile.damage = 20;
            projectile.width = 43;
            projectile.height = 43;
            projectile.friendly = true;
            projectile.thrown = true;
            aiType = ProjectileID.Shuriken;
        }
    }
}