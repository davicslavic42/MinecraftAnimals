using Microsoft.Xna.Framework;
using System;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using MinecraftAnimals.Tiles;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.Generation;
using Terraria.World.Generation;

namespace MinecraftAnimals
{
    public class GeneralMethods
    {
        public static float ManualMobRotation(float currentAngle, float nextAngle, float speed)
        {

            float rotatetimer = 0f;
            float initialAngle = new Vector2((float)Math.Cos(currentAngle), (float)Math.Sin(currentAngle)).ToRotation();// takes the 
            nextAngle = new Vector2((float)Math.Cos(nextAngle), (float)Math.Sin(nextAngle)).ToRotation();

            int nextDirection = Math.Abs(initialAngle - nextAngle) > Math.PI ? nextDirection = 1 : nextDirection = -1; // choose shorter length based on distance from pi
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
        }
        //thanks dan for the idea
        public static Vector2 GetTargetEntity(Vector2 currentCenter, float searchRange = 500f, int TargetType = 0, bool TargetClosest = true) //(float, float) target type -1 is player, target type zero is just for a set of conidiotns like town npcs, or use the ID of the npc you put in
        {
            Vector2 newTargetCenter = new Vector2(0, 0);
            float DistanceToTargetPos = 0f;
            float DistanceToNewTargetPos = 0f;
            bool CloserTargets = DistanceToTargetPos > DistanceToNewTargetPos;//if any of the other valid Centers ends up closer to the main Center the target will be changed

            for (int i = 0; i < Main.maxNPCs; i++)//range of targets
            {
                NPC I = Main.npc[i];
                if (I.active && I.chaseable && Vector2.Distance(currentCenter, I.Center) < searchRange && TargetType == -2)
                {
                    newTargetCenter = I.Center;
                    DistanceToTargetPos = Vector2.Distance(currentCenter, newTargetCenter);
                    DistanceToNewTargetPos = Vector2.Distance(currentCenter, I.Center);
                    if (TargetClosest){// if I want to target the closest target then the 
                        if (CloserTargets) newTargetCenter = I.Center;
                    }
                }
            }
            for (int y = 0; y < Main.ActivePlayersCount; y++)//targets based on active players
            {
                Player player = Main.player[y];
                if (!player.dead && !player.ghost && player.active && Vector2.Distance(currentCenter, player.Center) < searchRange && TargetType == -1 ) //player.Distance(currentCenter)
                {
                    newTargetCenter = player.Center;
                    DistanceToTargetPos = Vector2.Distance(currentCenter, newTargetCenter);
                    DistanceToNewTargetPos = Vector2.Distance(currentCenter, player.Center);// checks other player Centers
                    if (TargetClosest)
                    {// if I want to target the closest target then the 
                        if (CloserTargets) newTargetCenter = player.Center;
                    }
                }
            }
            for (int i = 0; i < Main.maxNPCs; i++)//targets a specific npc by ID
            {
                NPC I = Main.npc[i];
                if (I.active && I.chaseable && Vector2.Distance(currentCenter, I.Center) < searchRange && I.type == TargetType)
                {
                    newTargetCenter = I.Center;
                    DistanceToTargetPos = Vector2.Distance(currentCenter, newTargetCenter);//distance to current target
                    DistanceToNewTargetPos = Vector2.Distance(currentCenter, I.Center);//this takes the distance to the different valid Centers of the npcs aka potential new targets
                    if (TargetClosest)
                    {// if I want to target the closest target this is set to true
                        if (CloserTargets) newTargetCenter = I.Center;
                    }
                }
            }
            return newTargetCenter;
        }
        public static int FindType(int x, int y, int maxDepth = -1, params int[] types)//thanks gabe
        {
            if (maxDepth == -1) maxDepth = (int)(WorldGen.worldSurface); //Set default
            while (true)
            {
                if (y >= maxDepth)
                    break;
                if (Main.tile[x, y].active() && types.Any(i => i == Main.tile[x, y].type))
                    return y; //Returns first valid tile under intitial Y pos, -1 if max depth is reached
                y++;
            }
            return -1; //fallout case
        }

    }
}
