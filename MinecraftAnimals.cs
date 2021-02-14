using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinecraftAnimals.Raid;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;


namespace MinecraftAnimals
{
    public class MinecraftAnimals : Mod
    {
        static public MinecraftAnimals instance;
        private static Vector2[] LocalCursor = new Vector2[Main.player.Length];
        public static Vector2 GetLocalCursor(int id)
        {
            LocalCursor[id] = Main.MouseWorld;

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = instance.GetPacket();
                packet.Write((byte)ModMessageType.UpdateLocalCursor); // Message type, you would need to create an enum for this
                packet.Write((byte)id);
                packet.WriteVector2(LocalCursor[id]);
                packet.Send();
            }
            return LocalCursor[id];
        }
        public MinecraftAnimals()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true,
                AutoloadBackgrounds = true
            };
        }

        public override void Load()
        {
            if (!Main.dedServ)
                instance = this;
        }
        public override void Unload()
        {
            if (!Main.dedServ)
            {
            }
            instance = null;
        }
        internal enum ModMessageType : byte
        {
            StartRaidEvent,
            UpdateLocalCursor,
            SummonBoss,
            UpdateWaveInfo
        }
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            ModMessageType msgType = (ModMessageType)reader.ReadByte();
            switch (msgType)
            {

                case ModMessageType.StartRaidEvent:
                    RaidWorld.RaidEvent = true;
                    RaidWorld.RaidKillCount = 0f;
                    RaidWorld.RaidWaves = 1;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.WorldData); // Immediately inform clients of new world state.
                    }
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        RaidWorld.RaidEvent = true;
                        RaidWorld.RaidKillCount = 0f;
                        RaidWorld.RaidWaves = 1;
                    }

                    break;

                case ModMessageType.SummonBoss:
                    Vector2 sumonerLocation = reader.ReadVector2();
                    int type = reader.ReadInt32();
                    int num7 = NPC.NewNPC((int)sumonerLocation.X, (int)sumonerLocation.Y - 1000, type);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", new object[]
                        {
                                Main.npc[num7].GetTypeNetName()
                        }), new Color(175, 75, 255), -1);
                    }
                    break;
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            MCAWorld modWorld = (MCAWorld)GetModWorld("MCAWorld");
            /*
            RaidWorld modWorldInvasion = (RaidWorld)GetModWorld("RaidWorld");
            */

            if (RaidWorld.RaidEvent)
            {
                int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
                LegacyGameInterfaceLayer orionProgress = new LegacyGameInterfaceLayer("Raid ",
                    delegate
                    {
                        DrawRaidEvent(Main.spriteBatch);
                        return true;
                    },
                    InterfaceScaleType.UI);
                layers.Insert(index, orionProgress);
            }
        }
                /*
        try
        {
            //draw the background for the waves counter
            const int offsetX = 20;
            const int offsetY = 20;
            int width = (int)(200f * scaleMultiplier);
            int height = (int)(46f * scaleMultiplier);
            Rectangle waveBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - (Main.screenWidth / 2), (Main.screenHeight * 0.2f) - offsetY - 23f), new Vector2(width, height));
            // TESTING WAVE BG POSITION CHANGE
            Utils.DrawInvBG(spriteBatch, waveBackground, new Color(63, 65, 151, 255) * 0.785f);

            //draw wave text

            string waveText = Language.GetTextValue("Raid") + (int)(((float)MCAWorld.RaidKillCount / 150f) * 100) + "%";
            Utils.DrawBorderString(spriteBatch, waveText, new Vector2(waveBackground.X + waveBackground.Width / 2, waveBackground.Y), Color.White, scaleMultiplier, 0.5f, -0.1f);

            //draw the progress bar

            if (MCAWorld.RaidKillCount == 0)
            {
            }
            Rectangle waveProgressBar = Utils.CenteredRectangle(new Vector2(waveBackground.X + waveBackground.Width * 0.5f, waveBackground.Y + waveBackground.Height * 0.75f), new Vector2(progressColor.Width, progressColor.Height));
            Rectangle waveProgressAmount = new Rectangle(0, 0, (int)(progressColor.Width * MathHelper.Clamp(((float)MCAWorld.RaidKillCount / 100f), 0f, 1f)), progressColor.Height);
            Vector2 offset = new Vector2((waveProgressBar.Width - (int)(waveProgressBar.Width * scaleMultiplier)) * 0.5f, (waveProgressBar.Height - (int)(waveProgressBar.Height * scaleMultiplier)) * 0.5f);
            /*
            spriteBatch.Draw(progressBg, waveProgressBar.Location.ToVector2() + offset, null, Color.White * alpha, 0f, new Vector2(0f), scaleMultiplier, SpriteEffects.None, 0f);

            spriteBatch.Draw(progressBg, waveProgressBar.Location.ToVector2() + offset, waveProgressAmount, waveColor, 0f, new Vector2(0f), scaleMultiplier, SpriteEffects.None, 0f);
            spriteBatch.Draw(RaidBar, waveProgressBar.Location.ToVector2() + offset, null, waveColor, 0f, new Vector2(0f), scaleMultiplier, SpriteEffects.None, 0f);

            //draw the icon with the event description

            //draw the background
            const int internalOffset = 6;
            Vector2 descSize = new Vector2(154, 40) * scaleMultiplier;
            Rectangle barrierBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - (Main.screenWidth / 2), (Main.screenHeight * 0.2f) - offsetY - 23f), new Vector2(width, height));
            Rectangle descBackground = Utils.CenteredRectangle(new Vector2(barrierBackground.X + barrierBackground.Width * 0.5f, barrierBackground.Y - internalOffset - descSize.Y * 0.5f), descSize);
            Utils.DrawInvBG(spriteBatch, descBackground, descColor * alpha);

            //draw the icon
            int descOffset = (descBackground.Height - (int)(32f * scaleMultiplier)) / 2;
            Rectangle icon = new Rectangle(descBackground.X + descOffset, descBackground.Y + descOffset, (int)(24 * scaleMultiplier), (int)(62 * scaleMultiplier));
            spriteBatch.Draw(orionIcon, icon, Color.White);

            //draw text

            Utils.DrawBorderString(spriteBatch, Language.GetTextValue("Raid"), new Vector2(barrierBackground.X + barrierBackground.Width * 0.5f, barrierBackground.Y - internalOffset - descSize.Y * 0.5f), Color.White, 0.80f, 0.3f, 0.4f);
        }
        catch (Exception e)
        {
            ErrorLogger.Log(e.ToString());
        }
        */

        public void DrawRaidEvent(SpriteBatch spriteBatch)
        {
            if (RaidWorld.RaidEvent && !Main.gameMenu)
            {
                float scaleMultiplier = 0.5f + 1 * 0.5f;
                float alpha = 0.5f;
                Texture2D progressBg = Main.colorBarTexture;
                Texture2D progressColor = Main.colorBarTexture;
                Texture2D orionIcon = GetTexture("Items/Blocks/Banners/Ominousbanner"); //MAKE RAID BANNER ITEM FOR NOW
                Texture2D RaidBar = GetTexture("RaidProgressBar");
                const string orionDescription = "Raid";
                Color descColor = new Color(39, 86, 134);

                Color waveColor = new Color(255, 20, 20);
                Color barrierColor = new Color(255, 241, 51);
                try
                {
                    //draw the background for the waves counter
                    const int offsetX = 20;
                    const int offsetY = 20;
                    int width = (int)(200f * scaleMultiplier);
                    int height = (int)(46f * scaleMultiplier);
                    Rectangle waveBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - (Main.screenWidth / 2 - 100), (Main.screenHeight * 0.2f) - offsetY - 23f), new Vector2(width, height));
                    // TESTING WAVE BG POSITION CHANGE
                    Vector2 raidBarPosition = new Vector2((Main.screenWidth - (Main.screenWidth / 2)), ((Main.screenHeight * 0.2f) - offsetY - 23f));
                    //draw wave text

                    string waveText = Language.GetTextValue("Raid enemies left ") + (int)(((float)(RaidWorld.progressPerWave - RaidWorld.RaidKillCount) / 150f) * 100);
                    Utils.DrawBorderString(spriteBatch, waveText, new Vector2(raidBarPosition.X + 400, raidBarPosition.Y - 25), Color.White, scaleMultiplier, 0.5f, -0.1f);

                    //draw the progress bar

                    if (RaidWorld.RaidKillCount == 0)
                    {
                    }
                    Rectangle waveProgressBar = Utils.CenteredRectangle(new Vector2(waveBackground.X - 1950f, waveBackground.Y), new Vector2(progressColor.Width, progressColor.Height));
                    Rectangle waveProgressAmount = new Rectangle(0, 0, (int)(progressColor.Width * MathHelper.Clamp((float)((RaidWorld.progressPerWave - RaidWorld.RaidKillCount) / 100f), 0f, 100f)), progressColor.Height);//CHECK TO SEE IF RAIDER KILL COUNT OR RAIDERCOUNTER SHOULD BE USED FOR TH UI
                    Vector2 offset = new Vector2((waveProgressBar.Width - (int)(waveProgressBar.Width * scaleMultiplier)) * 0.5f, (waveProgressBar.Height - (int)(waveProgressBar.Height * scaleMultiplier)) * 0.5f);
                    /*
                    spriteBatch.Draw(progressBg, waveProgressBar.Location.ToVector2() + offset, null, Color.White * alpha, 0f, new Vector2(0f), scaleMultiplier, SpriteEffects.None, 0f);
                    */
                    spriteBatch.Draw(progressBg, raidBarPosition + offset, waveProgressAmount, waveColor, 0f, new Vector2(0f), scaleMultiplier * 1.5f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(RaidBar, raidBarPosition + offset, null, waveColor, 0f, new Vector2(0f), scaleMultiplier * 1.5f, SpriteEffects.None, 0f);

                    //draw the icon with the event description

                    //draw the background

                    //draw text

                }
                catch (Exception e)
                {
                    ErrorLogger.Log(e.ToString());
                }
            }

        }

    }
}
