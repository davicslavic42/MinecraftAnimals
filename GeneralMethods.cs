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
        //thanks dan for the idea
        public static Vector2 GetTargetEntity(Vector2 currentCenter, float searchRange = 500f, int TargetType = 0) //(float, float) target type -1 is player, target type zero is just for a set of conidiotns like town npcs, or use the ID of the npc you put in
        {
            Vector2 newTarget = new Vector2(0, 0);
            float DistanceToTargetPos = 0f;
            float DistanceToNewTargetPos = 0f;
            for (int i = 0; i < Main.maxNPCs; i++)//targets a specific npc by ID
            {
                NPC I = Main.npc[i];
                if (I.active && I.chaseable && Vector2.Distance(currentCenter, I.position) < searchRange && I.type == TargetType)
                {
                    newTarget = I.position;
                    DistanceToTargetPos = Vector2.Distance(currentCenter, newTarget);
                    DistanceToNewTargetPos = Vector2.Distance(currentCenter, I.position);
                    if (DistanceToTargetPos > DistanceToNewTargetPos) newTarget = I.position;
                }
            }
            for (int i = 0; i < Main.maxNPCs; i++)//range of targets,more specifically for hunting town npcs and future golem npc usage
            {
                NPC I = Main.npc[i];
                if (I.active && I.chaseable && Vector2.Distance(currentCenter, I.position) < searchRange && TargetType == 0)
                {
                    newTarget = I.position;
                    DistanceToTargetPos = Vector2.Distance(currentCenter, newTarget);
                    DistanceToNewTargetPos = Vector2.Distance(currentCenter, I.position);
                    if (DistanceToTargetPos > DistanceToNewTargetPos) newTarget = I.position;
                }
            }
            for (int y = 0; y < Main.ActivePlayersCount; y++)//targets based on active players
            {
                Player player = Main.player[y];
                if (!player.dead && !player.ghost && player.active && Vector2.Distance(currentCenter, player.position) < searchRange && TargetType == -1 ) //player.Distance(currentCenter)
                {
                    newTarget = player.position;
                    DistanceToTargetPos = Vector2.Distance(currentCenter, newTarget);
                    DistanceToNewTargetPos = Vector2.Distance(currentCenter, player.position);
                    if (DistanceToTargetPos > DistanceToNewTargetPos) newTarget = player.position;
                }
            }
            return newTarget;

            /*
             *             Main.NewText(newTarget);
             *    TargetDirection = npc.position.X > newTarget.X ? TargetDirection = -1 : TargetDirection = 1;
            */
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
