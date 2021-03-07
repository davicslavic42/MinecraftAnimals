using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.projectiles.Passive
{
    public class Bolt : ModProjectile
    {
        public int TargetWhoAmI
        {
            get => (int)projectile.ai[1];
            set => projectile.ai[1] = value;
        }
        internal ref float Charge => ref projectile.localAI[0];
        internal ref float LoadedCharge => ref projectile.localAI[1];
        internal ref float ProjectileState => ref projectile.ai[0];
        bool IsAtMaxCharge => Charge == MAX_CHARGE;
        bool ReadyToFire => LoadedCharge >= PREFIRE;


        private const float MAX_CHARGE = 150f;
        private const float PREFIRE = 20f;//this is the time before firing the shot after the crossbow is loaded
        bool Ready = false;
        internal enum AIStates
        {
            Charge = 0,
            Loaded = 1,
            Launch = 2,
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 10;
            projectile.hide = true;
            projectile.damage = 5;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.timeLeft = 200;
            projectile.localNPCHitCooldown = -1; // 1 hit per npc max
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 1;
        }
        #region basic charging and mp

        private void UpdatePlayer(Player player)
        {
            // Multiplayer support here, only run this code if the client running it is the owner of the projectile
            Vector2 aim = Vector2.Normalize(Main.MouseWorld - projectile.Center);
            if (projectile.owner == Main.myPlayer)
            {
                projectile.velocity = aim;
                projectile.rotation = aim.ToRotation();
                projectile.direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
                projectile.netUpdate = true;
            }
            int dir = projectile.direction;
            player.ChangeDir(dir); // Set player direction to where we are shooting
            player.heldProj = projectile.whoAmI; // Update player's held projectile
            player.itemTime = 2; // Set item time to 2 frames while we are used
            player.itemAnimation = 2; // Set item animation time to 2 frames while we are used
            player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * dir, projectile.velocity.X * dir); // Set the item rotation to where we are shooting  aim.ToRotation();
            int dustIndex = Dust.NewDust(new Vector2(player.position.X, player.position.Y - 2), player.width, (player.height / 2 - 1), DustType<Dusts.Poteffect>(), 0f, 0f, 100, default(Color), 1f);// causes little potion effects to flaot around
            Main.dust[dustIndex].scale = 0.85f;
        }

        #endregion basic charging and mp


        #region basic ai and mp
        public override void AI()
        {
            //float AimCircle = 75f;// as in a circle that will be used to guide the arrow so it moves on that circle, mostly so the drawn bolt looks like its near the end of the crossbow
            Player player = Main.player[projectile.owner];
            Vector2 aim = Vector2.Normalize(Main.MouseWorld - projectile.Center);
            projectile.timeLeft = 900;
            projectile.tileCollide = false;
            projectile.owner = Main.myPlayer;
            //projectile.DirectionTo(Main.MouseWorld);
            if (ProjectileState == (int)AIStates.Charge)
            {
                projectile.Center = player.Center;// TEST LATER * (AimCircle * (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X))
                UpdatePlayer(player);
                projectile.damage = 0;
                if (!player.channel && !IsAtMaxCharge) projectile.Kill();
                if (!IsAtMaxCharge && player.channel) Charge++;
                if (IsAtMaxCharge)
                {
                    CombatText.NewText(new Rectangle((int)player.position.X + (5 * player.direction), (int)player.position.Y - 5, player.width * 2, player.height / 2), Color.SteelBlue, "Bolt Loaded!");
                    player.channel = false;
                    ProjectileState = (int)AIStates.Loaded;
                }
            }
            if (ProjectileState == (int)AIStates.Loaded)
            {
                projectile.Center = player.Center;// TEST LATER * (AimCircle * (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X))
                Ready = true;
                if (player.channel && Ready && !ReadyToFire) LoadedCharge++;
                if (!player.channel) LoadedCharge = 18f;// The purpose of this is that after the player loads the bolt they if they let go it sets the charge right before the prefire number so when they channel again it fires
                if (ReadyToFire)
                {
                    projectile.damage = 40;
                    projectile.velocity = aim * 26f;// fires the bolt
                    player.channel = false;
                    ProjectileState = (int)AIStates.Launch;
                }
            }
            if (ProjectileState == (int)AIStates.Launch)
            {
                projectile.hide = false;
                projectile.netUpdate = true;
                projectile.alpha = 0;
                projectile.velocity.Y += 0.1f;// weighs it down like an arrow
                projectile.velocity.X *= 0.99f;// weighs it down like an arrow
                projectile.tileCollide = true;
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);

            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int startY = frameHeight * projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            origin.X = (float)(projectile.spriteDirection == 1 ? sourceRectangle.Width - 2 : 2);

            Color drawColor = projectile.GetAlpha(lightColor);
            if (ProjectileState == (int)AIStates.Launch)//ProjectileState == (int)AIStates.Charge || ProjectileState == (int)AIStates.Loaded || 
            {
                Main.spriteBatch.Draw(texture,
                projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
                sourceRectangle, drawColor, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);
            }
            else
            {
                Main.spriteBatch.Draw(texture,
                projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
                sourceRectangle, drawColor * 0.05f, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);
            }

            /*
            if (ProjectileState == (int)AIStates.Launch)//ProjectileState == (int)AIStates.Charge || ProjectileState == (int)AIStates.Loaded || 
            {
                Main.spriteBatch.Draw(texture,
                projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
                sourceRectangle, drawColor, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);
            }
            else
            {
                Main.spriteBatch.Draw(texture,
                projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
                sourceRectangle, drawColor * 0.75f, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);

            }
            */
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // We can only collide if we are at max charge, which is when the laser is actually fired
            if (!ReadyToFire) return false;

            Player player = Main.player[projectile.owner];
            Vector2 unit = projectile.velocity;
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center,
                projectile.Center, 22, ref point);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 50;
            //3b: target.immune[projectile.owner] = 5;
        }
        #endregion basic ai and mp
    }
}
