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
        private const float PREFIRE = 300f;//this is the time before firing the shot after the crossbow is loaded
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
            Main.dust[dustIndex].scale = 0.25f;
        }

        #endregion basic charging and mp
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
            origin.X = (float)(projectile.spriteDirection == 1 ? sourceRectangle.Width - 20 : 20);

            Color drawColor = projectile.GetAlpha(lightColor);
            if (ProjectileState == (int)AIStates.Launch)//ProjectileState == (int)AIStates.Charge || ProjectileState == (int)AIStates.Loaded || 
            {
                Main.spriteBatch.Draw(texture,
                projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
                sourceRectangle, Color.White, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);
            }

            return false;
        }


        #region basic ai and mp
        public override void AI()
        {
            //float AimCircle = 75f;// as in a circle that will be used to guide the arrow so it moves on that circle, mostly so the drawn bolt looks like its near the end of the crossbow
            Player player = Main.player[projectile.owner];
            Vector2 aim = Vector2.Normalize(Main.MouseWorld - projectile.Center);
            projectile.timeLeft = 900;
            projectile.tileCollide = false;
            //projectile.DirectionTo(Main.MouseWorld);
            if (ProjectileState == (int)AIStates.Charge)
            {
                projectile.Center = player.Center;// TEST LATER * (AimCircle * (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X))
                UpdatePlayer(player);
                projectile.damage = 0;
                if (!player.channel && !IsAtMaxCharge) projectile.Kill();
                if (!IsAtMaxCharge && player.channel) Charge++;
                if (IsAtMaxCharge)//&& player.channel
                {
                    CombatText.NewText(new Rectangle((int)player.position.X + (5 * player.direction), (int)player.position.Y - 5, player.width * 2, player.height / 2), Color.LightSteelBlue, "Bolt Loaded!");
                    player.channel = false;
                    ProjectileState = (int)AIStates.Loaded;
                }
            }
            if (ProjectileState == (int)AIStates.Loaded)
            {
                projectile.Center = player.Center;// TEST LATER * (AimCircle * (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X))
                Ready = true;
                if (player.channel && Ready && !ReadyToFire) LoadedCharge++;
                if (!player.channel) LoadedCharge = 298f;
                if (ReadyToFire)
                {
                    projectile.damage = 10;
                    projectile.velocity = aim * 20f;// fires the bolt
                    player.channel = false;
                    ProjectileState = (int)AIStates.Launch;
                }
            }
            if (ProjectileState == (int)AIStates.Launch)
            {
                projectile.velocity.Y += 0.1f;// weighs it down like an arrow
                projectile.velocity.X *= 0.99f;// weighs it down like an arrow
                projectile.tileCollide = true;
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);

            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // We can only collide if we are at max charge, which is when the laser is actually fired
            if (!IsAtMaxCharge) return false;

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
            base.OnHitNPC(target, damage, knockback, crit);
        }

        #endregion basic ai and mp
             /*
            public void Drawarrow(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Color color, int damage, float rotation = 0f, float scale = 1f )
            {
                //float r = new Vector2(projectile.velocity).ToRotation();
                texture = Main.projectileTexture[projectile.type];//TEST LATER
                Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, Main.projectileTexture[projectile.type].Height * 0.5f);

                Rectangle boltRec = new Rectangle(0, 0, texture.Width, texture.Height);//
                spriteBatch.Draw(texture, projectile.position, Color.White);
                //spriteBatch.Draw(texture, projectile.position, boltRec, Color.White, projectile.rotation, drawOrigin, SpriteEffects.None, 0f);
            }
            */
    }
}
