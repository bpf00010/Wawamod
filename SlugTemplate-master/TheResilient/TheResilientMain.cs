using System.Runtime.CompilerServices;
using System.Security.Permissions;
using BepInEx;
using UnityEngine;

#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace TheResilient
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class TheResilientMain : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "klovar.theresilient";
        public const string PLUGIN_NAME = "The Resilient";
        public const string PLUGIN_VERSION = "1.0.0";

        //Reference to the SlugcatStats.Name for the new slugcat, grabbed fron json file, so we can use it.
      
        public static readonly SlugcatStats.Name ResilientName = new SlugcatStats.Name("TheResilient", false);


        public static readonly ConditionalWeakTable<PlayerGraphics, StrongBox<int>> WhiskerIndices = new();

        private void OnEnable()
        {
            //hook the graphics setup and locational drawing to see if we can add the sprites.
            On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
            On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
            On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;

        }

        //try a spawn based implementation first?

        private void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser spriteLeaser, RoomCamera rCam)
        {
            orig(self, spriteLeaser, rCam);
            //draw in cam graph and sprite set local var.

            //Checks if player is scoog

            if (!self.player.SlugCatClass.Equals(ResilientName))
            {
                return;
            }

            //sets a starting index set equal to sprite leaser.sprites.length, which is the last index sprite array.
            int startIndex = spriteLeaser.sprites.Length;
            WhiskerIndices.Add(self, new StrongBox<int>(startIndex));


            //expand by 2 for the whiskers.

            System.Array.Resize(ref spriteLeaser.sprites, startIndex + 2);


            //try 2 bubble sprites??
            //remember where the index starts if you dont get it below
            spriteLeaser.sprites[startIndex] = new FSprite("BubbleWeed", true);
            spriteLeaser.sprites[startIndex + 1] = new FSprite("BubbleWeed", true);

            spriteLeaser.sprites[startIndex].scale = 0.5f;
            spriteLeaser.sprites[startIndex + 1].scale = 0.5f;
            //trying a smaller scaling.


            self.AddToContainer(spriteLeaser, rCam, null);


        }


        private void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser spriteLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, spriteLeaser, rCam, timeStacker, camPos);

            if (!self.player.SlugCatClass.Equals(ResilientName))
            {
                return;
            }
            //
            if (!WhiskerIndices.TryGetValue(self, out StrongBox<int> whiskerIndexBox))
            {
                return;
            }
            int startIndex = whiskerIndexBox.Value;

            FSprite head = spriteLeaser.sprites[3];


            //left
            //try these values for now 9,2,15 (Ok but large)
            spriteLeaser.sprites[startIndex].x = head.x - 9f;
            spriteLeaser.sprites[startIndex].y = head.y + 2f;
            spriteLeaser.sprites[startIndex].rotation = head.rotation - 15f;


            //right



            //right
            spriteLeaser.sprites[startIndex + 1].x = head.x + 9f;
            spriteLeaser.sprites[startIndex + 1].y = head.y - 2f;
            spriteLeaser.sprites[startIndex + 1].rotation = head.rotation + 15f;


        }


        public void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser spriteLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            orig(self, spriteLeaser, rCam, newContatiner);
            if (!self.player.SlugCatClass.Equals(ResilientName))
            {
                return;
            }
            if (!WhiskerIndices.TryGetValue(self, out StrongBox<int> whiskerIndexBox))
            {
                return;
            }
            int startIndex = whiskerIndexBox.Value;
            FContainer midground = rCam.ReturnFContainer("Midground");

            for (int i = 0; i < 2; i++)
            {
                if (startIndex + i < spriteLeaser.sprites.Length && spriteLeaser.sprites[startIndex + i] != null)
                {
                    spriteLeaser.sprites[startIndex + i].RemoveFromContainer();
                    midground.AddChild(spriteLeaser.sprites[startIndex + i]);
                }




            }   }

















        private void PlayerUpdateHook(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);

        }
    }
}
