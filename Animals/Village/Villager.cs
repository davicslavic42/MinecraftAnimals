using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MinecraftAnimals.Animals.Village
{
	// [AutoloadHead] and npc.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
	[AutoloadHead]
	public class Villager : ModNPC
	{

		public override bool Autoload(ref string name)
		{
			name = "Villager";
			return mod.Properties.Autoload;
		}

		public override void SetStaticDefaults()
		{
			// DisplayName automatically assigned from .lang files, but the commented line below is the normal approach.
			// DisplayName.SetDefault("Example Person");
			Main.npcFrameCount[npc.type] = 5;
			NPCID.Sets.DangerDetectRange[npc.type] = 700;
			NPCID.Sets.HatOffsetY[npc.type] = 4;
		}

		public override void SetDefaults()
		{
			npc.townNPC = true;
			npc.friendly = true;
			npc.width = 18;
			npc.height = 40;
			npc.aiStyle = 7;
			npc.damage = 10;
			npc.defense = 15;
			npc.lifeMax = 250;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.knockBackResist = 0.5f;
		}


		public override bool CanTownNPCSpawn(int numTownNPCs, int money)
		{
			for (int k = 0; k < 255; k++)
			{
				Player player = Main.player[k];
				if (!player.active)
				{
					continue;
				}

				foreach (Item item in player.inventory)
				{
					if (item.type == ItemID.Emerald || item.type == ItemID.Emerald)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override string TownNPCName()
		{
			switch (WorldGen.genRand.Next(4))
			{
				case 0:
					return "Villager";
				case 1:
					return "Villager";
				case 2:
					return "Villager";
				default:
					return "Villager";
			}
		}

		public override void FindFrame(int frameHeight)
		{
			/*npc.frame.Width = 40;
			if (((int)Main.time / 10) % 2 == 0)
			{
				npc.frame.X = 40;
			}
			else
			{
				npc.frame.X = 0;
			}*/
		}

		public override string GetChat()
		{
			int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
			if (partyGirl >= 0 && Main.rand.NextBool(4))
			{
				return "Can you please tell " + Main.npc[partyGirl].GivenName + " to stop decorating my house with colors?";
			}
			switch (Main.rand.Next(5))
			{
				case 0:
					return "What's wrong with emeralds for some iron leggings?";
				case 1:
					return "I've heard rumors of Illagers coming to these areas too, watch out for the raid captains";
				case 2:
					{
						// Main.npcChatCornerItem shows a single item in the corner, like the Angler Quest chat.
						Main.npcChatCornerItem = ItemID.Emerald;
						return $"Hey, if you find a [i:{ItemID.Emerald}], I can trade with you for some fairly useless stuff.";
					}
				default:
					return "What? I don't have any arms or hands? I'd slap you, but my hands are stuck";
				case 3:
					return "So this is what it feels like to be in the second dimension";
				case 4:
					return "This place is uh, much more magical than what im used to";
			}
		}

		/* 
		// Consider using this alternate approach to choosing a random thing. Very useful for a variety of use cases.
		// The WeightedRandom class needs "using Terraria.Utilities;" to use
		public override string GetChat()
		{
			WeightedRandom<string> chat = new WeightedRandom<string>();

			int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
			if (partyGirl >= 0 && Main.rand.NextBool(4))
			{
				chat.Add("Can you please tell " + Main.npc[partyGirl].GivenName + " to stop decorating my house with colors?");
			}
			chat.Add("Sometimes I feel like I'm different from everyone else here.");
			chat.Add("What's your favorite color? My favorite colors are white and black.");
			chat.Add("What? I don't have any arms or legs? Oh, don't be ridiculous!");
			chat.Add("This message has a weight of 5, meaning it appears 5 times more often.", 5.0);
			chat.Add("This message has a weight of 0.1, meaning it appears 10 times as rare.", 0.1);
			return chat; // chat is implicitly cast to a string. You can also do "return chat.Get();" if that makes you feel better
		}
		*/

		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("LegacyInterface.28");
			button2 = "Awesomeify";
			if (Main.LocalPlayer.HasItem(ItemID.HiveBackpack))
				button = "Upgrade " + Lang.GetItemNameValue(ItemID.HiveBackpack);
		}


		public override void SetupShop(Chest shop, ref int nextSlot)
		{
			shop.item[nextSlot].SetDefaults(ItemID.RegenerationPotion);
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ItemID.ManaPotion);
			nextSlot++;
		}
	}
}