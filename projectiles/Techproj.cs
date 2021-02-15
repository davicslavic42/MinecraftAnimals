using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.projectiles
{
    public class Techproj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Techproj");
        }
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 3;
            projectile.hide = true;
            projectile.damage = 5;
            projectile.timeLeft = 200;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Die immediately if ai[1] isn't 0 (We set this to 1 for the 5 extra explosives we spawn in Kill)
            if (projectile.timeLeft != 0)
            {
                projectile.timeLeft = 8;
            }
            return false;
        }

        // Change this number if you want to alter how the alpha changes
        public override void AI()
        {
            if (projectile.owner == Main.myPlayer && projectile.timeLeft <= 8)
            {
                projectile.tileCollide = true;
                projectile.Center = projectile.position;
                projectile.timeLeft = 8;
            }
            else
            {
                int dustIndex = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y - 2), projectile.width, projectile.height, 31, 0f, 0f, 100, default(Color), 1f);
                Main.dust[dustIndex].scale = 0.1f + (float)Main.rand.Next(5) * 0.1f;
            }
        }
        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(new Vector2(projectile.position.X, projectile.position.Y - 150f), projectile.velocity, ProjectileType<Fang>(), 20, 2, Main.LocalPlayer.whoAmI); //Multiply velocity with a larger number for more speed
        }
    }
}