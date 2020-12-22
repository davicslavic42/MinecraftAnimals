using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
using System;
using System.Security.Cryptography.X509Certificates;

namespace MinecraftAnimals
{
    public class GeneralMethods
    {
        public static float ManualMobRotation(float currentAngle, float nextAngle, float speed)
        {
            int rotatetimer = 0;
            int nextDirection = 1;
            float initialAngle = new Vector2((float)Math.Cos(currentAngle), (float)Math.Sin(currentAngle)).ToRotation();
            nextAngle = new Vector2((float)Math.Cos(nextAngle), (float)Math.Sin(nextAngle)).ToRotation();

            rotatetimer++;
            int chooseShorterLength = Math.Abs(initialAngle - nextAngle) > Math.PI ? nextDirection = 1 : nextDirection = -1; // choose shorter length based on distance from pi
            float Addspeed = initialAngle * nextDirection <= nextAngle ? initialAngle += MathHelper.ToRadians(speed) : initialAngle -= MathHelper.ToRadians(speed); //starts to add speed to the current angle changing the direction
            float speedOverTime = (float)(rotatetimer > 65 ? currentAngle = nextAngle : (speed * 0.85f)); //slows the rotation speed down, and auto sets to target angle after time. this attempts to replicate the mc death anim effect

            if (initialAngle <= nextAngle + MathHelper.ToRadians(speed) && initialAngle >= nextAngle - MathHelper.ToRadians(speed))
            {
                initialAngle = nextAngle;
            }
            return initialAngle;

        }
    }
}
