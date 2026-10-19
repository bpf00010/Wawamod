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


        private const int BubbleSpriteCount = 8; // Number of bubble sprites to add
        private const float BubbleRadius = 0.35f;

        private const float StemWidth = 1.5f;

        public static readonly ConditionalWeakTable<RoomCamera.SpriteLeaser, StrongBox<int>> BubbleIndices = new();

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
            BubbleIndices.Remove(spriteLeaser);
            BubbleIndices.Add(spriteLeaser, new StrongBox<int>(startIndex));



            //Now sets the bubble sprite count to the start index + bubble sprite count, which is 8, so we can add 8 sprites to the array.

            System.Array.Resize(ref spriteLeaser.sprites, startIndex + BubbleSpriteCount);









            for (int side = 0; side < 2; side++)
            {
                int index = startIndex + side * 4; //Side *4
                spriteLeaser.sprites[index] = new FSprite("pixel", true); //Stalk
                spriteLeaser.sprites[index].anchorY = 0f; //Anchor at bottom

                spriteLeaser.sprites[index + 1] = new FSprite("Circle20", true); //Bubble
                spriteLeaser.sprites[index + 2] = new FSprite("Circl20", true); //Bubble


            }





            /*
            //try 2 bubble sprites??
            //remember where the index starts if you dont get it below
            //Just targeting DangleFruit0A does not work, have to build out 4 sprites, stalks + bubbles.
            spriteLeaser.sprites[startIndex] = new FSprite("pixel", true);
            spriteLeaser.sprites[startIndex + 1] = new FSprite("DangleFruit0A", true);


            spriteLeaser.sprites[startIndex + 2] = new FSprite("pixel", true);
            spriteLeaser.sprites[startIndex + 3] = new FSprite("DangleFruit0A", true);

            spriteLeaser.sprites[startIndex].scaleX = 2f; 
            spriteLeaser.sprites[startIndex].scaleY = 12f;
            spriteLeaser.sprites[startIndex + 2].scaleX = 2f; //stalks legnth + width
            spriteLeaser.sprites[startIndex + 2].scaleY = 12f;

            spriteLeaser.sprites[startIndex + 1].scale = 0.35f; //targeting bubbles here
            spriteLeaser.sprites[startIndex + 3].scale = 0.35f;
            spriteLeaser.sprites[startIndex + 3].scaleX = -0.35f;

            //trying a smaller scaling.

            self.AddToContainer(spriteLeaser, rCam, null);

            */




        }   


        private void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser spriteLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, spriteLeaser, rCam, timeStacker, camPos);

            if (!self.player.SlugCatClass.Equals(ResilientName))
            {
                return;
            }
            //
            if (!BubbleIndices.TryGetValue(self, out StrongBox<int> whiskerIndexBox))
            {
                return;
            }
            int startIndex = whiskerIndexBox.Value;

            if (startIndex + 3 >= spriteLeaser.sprites.Length)
            {
                return;
            }
            FSprite head = spriteLeaser.sprites[3];


            //left
            spriteLeaser.sprites[startIndex].x = head.x - 7f;
            spriteLeaser.sprites[startIndex].y = head.y + 1f;
            spriteLeaser.sprites[startIndex].rotation = head.rotation - 40f;
            
            spriteLeaser.sprites[startIndex + 1].x = head.x - 14f;
            spriteLeaser.sprites[startIndex + 1].y = head.y + 4f;
            spriteLeaser.sprites[startIndex + 1].rotation = head.rotation - 40f;

            //Right
            spriteLeaser.sprites[startIndex + 2].x = head.x + 7f;
            spriteLeaser.sprites[startIndex + 2].y = head.y + 1f;
            spriteLeaser.sprites[startIndex + 2].rotation = head.rotation + 40f;

            
            spriteLeaser.sprites[startIndex + 3].x = head.x + 14f;
            spriteLeaser.sprites[startIndex + 3].y = head.y + 4f;
            spriteLeaser.sprites[startIndex + 3].rotation = head.rotation + 40f;


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

            for (int i = 0; i < 4; i++)
            {
                int index = startIndex + i;
                if (index < spriteLeaser.sprites.Length && spriteLeaser.sprites[index] != null)
                {
                    spriteLeaser.sprites[index].RemoveFromContainer();
                    midground.AddChild(spriteLeaser.sprites[index]);
                }




            }   }

















        private void PlayerUpdateHook(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);

        }
    }
}
