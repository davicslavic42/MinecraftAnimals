﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace MinecraftAnimals.projectiles.Passive
{
    // The following laser shows a channeled ability, after charging up the laser will be fired
    // Using custom drawing, dust effects, and custom collision checks for tiles
    public class Bolt : ModProjectile
    {
        // Use a different style for constant so it is very clear in code when a constant is used

        // The maximum charge value

        public int TargetWhoAmI
        {
            get => (int)projectile.ai[1];
            set => projectile.ai[1] = value;
        }
        internal ref float Charge => ref projectile.localAI[0];
        internal ref float LoadedCharge => ref projectile.localAI[1];
        internal ref float ProjectileState => ref projectile.ai[0];
        private const float MAX_CHARGE = 250f;
        //private const float PREFIRE = 50f;//this is the time before firing the shot after the crossbow is loaded
        bool Ready = false;
        internal enum AIStates
        {
            Charge = 0,
            Loaded = 1,
            Launch = 2,
        }


        // Are we at max charge? With c#6 you can simply use => which indicates this is a get only property
        public bool IsAtMaxCharge => Charge == MAX_CHARGE;
        ////public bool ReadyToFire => Charge >= PREFIRE;

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
            Vector2 aim = Vector2.Normalize(Main.MouseWorld - player.Center);
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
        }

        #endregion basic charging and mp
        public void Drawarrow(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 unit, float step, int damage, float rotation = 0f, float scale = 1f, Color color = default(Color))
        {
            //float r = new Vector2(projectile.velocity).ToRotation();
            Rectangle boltRec = new Rectangle(0, 0, projectile.width, projectile.height);
            spriteBatch.Draw(texture, projectile.position, boltRec, default(Color));
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // We start drawing the laser if we have charged up
            if (IsAtMaxCharge)
            {
                Drawarrow(spriteBatch, Main.projectileTexture[projectile.type], Main.player[projectile.owner].Center - Main.screenPosition,
                    projectile.velocity, 20, 12, projectile.rotation, 1f, default(Color));
            }
            return false;
        }


        #region basic ai and mp
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            Vector2 aim = Vector2.Normalize(Main.MouseWorld - player.Center);
            projectile.position = aim;
            projectile.timeLeft = 500;
            float speed = 10f;
            projectile.tileCollide = false;

            UpdatePlayer(player);
            if (ProjectileState == (int)AIStates.Charge)
            {
                projectile.damage = 0;
                if (!player.channel) projectile.Kill();
                if (!IsAtMaxCharge && player.channel) Charge++;
                if (IsAtMaxCharge && player.channel)
                {
                    projectile.damage = 20;
                    aim *= speed;//bolt speed
                    projectile.velocity = aim;// fire the bolt
                    projectile.velocity.Y += 0.1f;// weighs it down like an arrow
                    projectile.tileCollide = true;
                    projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);

                    //ProjectileState = (int)AIStates.Loaded;
                    //Ready = true;
                    //CombatText.NewText(new Rectangle(0, 0, player.width, player.height / 2), Color.LightSteelBlue,"Bolt Loaded!");
                    //Charge = 0;
                }
                //if (ReadyToFire && player.channel)Charge++;
                //if (player.channel && Ready && ReadyToFire) LoadedCharge++
                Main.NewText(LoadedCharge);
                Main.NewText(Charge);
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
            if (ProjectileState == (int)AIStates.Loaded)
            {
                if (player.channel) LoadedCharge++;
                if (ReadyToFire && player.channel)
                {
                    ProjectileState = (int)AIStates.Launch;
                }
            }
            if (ProjectileState == (int)AIStates.Launch)
            {
                projectile.damage = 20;
                projectile.velocity = aim.RotatedByRandom(0.1f) * 12f;
                projectile.velocity.Y += 0.1f;
                projectile.tileCollide = true;
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            }
            */
        /*
         *             if (ProjectileState == (int)AIStates.Loaded)
        {
            if (player.channel) LoadedCharge++;
            if (ReadyToFire && player.channel)
            {
                ProjectileState = (int)AIStates.Launch;
            }
        }
        if (ProjectileState == (int)AIStates.Launch)
        {
            projectile.damage = 20;
            projectile.velocity = aim.RotatedByRandom(0.1f) * 12f;
            projectile.velocity.Y += 0.1f;
            projectile.tileCollide = true;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
        }


                public override void AI()
        {
            Player player = Main.player[projectile.owner];
            projectile.position = player.Center + projectile.velocity * 1.5f;
            projectile.timeLeft = 2;
            UpdatePlayer(player);
            Chargeshot(player);
            // If laser is not charged yet, stop the AI here.
            if (!IsAtMaxCharge) return;

            Firebolt(player);
            projectile.tileCollide = true;
            projectile.velocity.Y += 0.1f;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            Main.NewText(Charge);
        }


                private void Chargeshot(Player player)
        {
            if (!player.channel) projectile.Kill();
            if (!IsAtMaxCharge && player.channel) Charge++;
        }
        private void Firebolt(Player player)
        {
            if (IsAtMaxCharge)
            {
                Vector2 aim = Vector2.Normalize(Main.MouseWorld - player.Center);
                projectile.velocity += (aim.RotatedByRandom(0.05f) * 12f) ;
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            }
            projectile.timeLeft = 100;
        }

         */

    }
}
