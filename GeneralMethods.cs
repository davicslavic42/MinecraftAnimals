using Microsoft.Xna.Framework;
using System;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals
{
    public class GeneralMethods
    {
        public static float ManualMobRotation(float currentAngle, float nextAngle, float speed)
        {

            float rotatetimer = 0f;
            int nextDirection = 1;
            float initialAngle = new Vector2((float)Math.Cos(currentAngle), (float)Math.Sin(currentAngle)).ToRotation();
            nextAngle = new Vector2((float)Math.Cos(nextAngle), (float)Math.Sin(nextAngle)).ToRotation();

            int chooseShorterLength = Math.Abs(initialAngle - nextAngle) > Math.PI ? nextDirection = 1 : nextDirection = -1; // choose shorter length based on distance from pi
            float Addspeed = initialAngle * nextDirection <= nextAngle ? initialAngle += MathHelper.ToRadians(speed * 0.5f) : initialAngle -= MathHelper.ToRadians(speed * 0.5f); //starts to add speed to the current angle changing the direction
            rotatetimer += 1f;
            if (initialAngle <= nextAngle + MathHelper.ToRadians(speed * 1.5f) && initialAngle >= nextAngle - MathHelper.ToRadians(speed * 1.5f))
            {
                initialAngle = nextAngle;
                rotatetimer = 86;
            }
            if (rotatetimer >= 85) rotatetimer = 86;
            if (rotatetimer % 5f == 1f)
            {
                speed += MathHelper.ToRadians(speed * 0.5f) * nextDirection;
            }
            return initialAngle;
            //            float speedOverTime = (float)(rotatetimer >= 85f ? initialAngle = nextAngle : (speed -= 1.5f)); //slows the rotation speed down, and auto sets to target angle after time. this attempts to replicate the mc death anim effect
        }
        public static Vector2 GetTargetEntity(Vector2 currentPosition,  float searchRange = 500f, int TargetType = 0) //(float, float) target type 0 is npc tpye 1 is player
        {
            Vector2 newTarget = new Vector2(0,0);
            int Targetwho = 0;
            int possibleTarget = 0;
            //float toTargetPos =  Vector2.Distance(currentPosition, targetPosition); Vector2 targetPosition,
            if(Targetwho == 0)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC I = Main.npc[i];
                    Vector2 toNpcTarget = (I.position - currentPosition);
                    if (I.active && toNpcTarget.Length() < searchRange && TargetType == 0)
                    {

                    }
                }
                for (int y = 0; y < Main.ActivePlayersCount; y++)
                {
                    Player player = Main.player[y];
                    if (player.active && player.Distance(currentPosition) < searchRange && TargetType == 1)
                    {

                    }
                }

            }
            return newTarget;
        }
    }
}
