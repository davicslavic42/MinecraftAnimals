using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinecraftAnimals.projectiles
{
    public class FireCharge : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire Charge");
        }
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 3;
            projectile.hide = false;
            projectile.damage = 1;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            base.OnHitPlayer(target, damage, crit);
            target.AddBuff(BuffID.OnFire, 180);
        }
    }
}