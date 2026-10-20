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

        //Sprites
        private const int BubblePerSide = 4;
        private const int SpritePerSide = BubblePerSide * 4; //Two sides, each with 4 sprites
        private const int BubbleSpriteCount = SpritePerSide * 2;
        //2.25 radius is good, lets test a little higher REVERT BACK IF BAD
        private const float BubbleRadius = 2.25f;


        private const float StemWidth = 1.5f;

        //Features

        private const float SurfaceSwimSpeed = 5f;
        private const float UnderwaterSwimForce = 1.5f;




        public static readonly ConditionalWeakTable<RoomCamera.SpriteLeaser, StrongBox<int>> BubbleIndices = new();

        private void OnEnable()
        {
            //hook the graphics setup and locational drawing to see if we can add the sprites.
            On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
            On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
            On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;
            On.Player.UpdateAnimation += Player_UpdateAnimation;

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










            for (int bubble = 0; bubble < BubblePerSide * 2; bubble++)
            {
                int index = startIndex + bubble * 4; //Side *4
                spriteLeaser.sprites[index] = new FSprite("pixel", true); //Stalk
                spriteLeaser.sprites[index].anchorY = 0f; //Anchor at bottom

                spriteLeaser.sprites[index + 1] = new FSprite("Circle20", true); //Bubble
                spriteLeaser.sprites[index + 2] = new FSprite("Circle20", true);
                spriteLeaser.sprites[index + 3] = new FSprite("Circle20", true); //Bubble


            }


            self.AddToContainer(spriteLeaser, rCam, null);


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
            if (!BubbleIndices.TryGetValue(spriteLeaser, out var indexBox))
            {
                return;
            }
            int startIndex = indexBox.Value;

            if (startIndex + BubbleSpriteCount > spriteLeaser.sprites.Length)
            {
                return;
            }
            FSprite head = spriteLeaser.sprites[3];


            Vector2 headPos = new Vector2(head.x, head.y);

            RoomPalette palette = rCam.currentPalette;

            Color outerColor = Color.Lerp(Color.Lerp(palette.blackColor, new Color(0f, 1f, 0.4f), 0.4f), palette.fogColor, 0.2f);

            Color innerColor = Color.Lerp(outerColor, Color.Lerp(palette.blackColor, new Color(0f, 0f, 1f), 0.4f), 0.6f);

            Color stemColor = Color.Lerp(palette.blackColor, outerColor, 0.8f);

            for (int side = 0; side < 2; side++)
            {
                //check top for sprite per side math
                int sideIndex = startIndex + side * SpritePerSide;
                float direction = side == 0 ? -1f : 1f;

                //6, -4 should attack roughly to the cheeks will need to see how it reacts when parired with the change in the outward facing angles.
                Vector2 cheekRoot = headPos + RotateWithHead(new Vector2(direction * 6f, -4f), head.rotation);

                // Slightly higher than the previous -6f angle.
                Vector2 stalkOffset = new Vector2(direction * 16f, -3f);

                Vector2 tip = cheekRoot + RotateWithHead(stalkOffset, head.rotation);

                Vector2 branchDirection = new Vector2(direction * 3f, 16f).normalized;



                for (int bubble = 0; bubble < BubblePerSide; bubble++)
                {
                    int index = sideIndex + bubble * 4;

                    Vector2 root = cheekRoot;
                    Vector2 bubblePos = tip;

                    if (bubble > 0)
                    {
                        float alongStalk = bubble == 1 ? 0.35f : bubble == 2 ? 0.58f : 0.80f; float branchOffset = bubble == 2 ? -6f : 6f;

                        root = cheekRoot + RotateWithHead(stalkOffset * alongStalk, head.rotation);

                        bubblePos = root + RotateWithHead(branchDirection * branchOffset, head.rotation);
                    }




                    FSprite stem = spriteLeaser.sprites[index];
                    FSprite outer = spriteLeaser.sprites[index + 1];
                    FSprite inner = spriteLeaser.sprites[index + 2];
                    FSprite shine = spriteLeaser.sprites[index + 3];

                    Vector2 stemDirection = bubblePos - root;

                    float stemLength = Mathf.Max(0f, stemDirection.magnitude - BubbleRadius + 1f);

                    stem.x = root.x;
                    stem.y = root.y;
                    stem.rotation = Mathf.Atan2(
                    stemDirection.x,
                    stemDirection.y) * Mathf.Rad2Deg;

                    stem.scaleX = bubble == 0 ? StemWidth : 1f;
                    stem.scaleY = stemLength;
                    stem.color = stemColor;
                    stem.alpha = 1f;

                    outer.x = bubblePos.x;
                    outer.y = bubblePos.y;
                    outer.scale = BubbleRadius / 10f;
                    outer.color = outerColor;
                    outer.alpha = 1f;

                    inner.x = bubblePos.x;
                    inner.y = bubblePos.y;
                    inner.scale = (BubbleRadius - 0.75f) / 10f;
                    inner.color = innerColor;
                    inner.alpha = 1f;

                    shine.x = bubblePos.x - BubbleRadius * 0.4f;
                    shine.y = bubblePos.y + BubbleRadius * 0.4f;
                    shine.scale = BubbleRadius / 20f;
                    shine.color = Color.white;
                    shine.alpha = 0.3f;

                    for (int part = 0; part < 4; part++)
                    {
                        spriteLeaser.sprites[index + part].isVisible =
                            head.isVisible;
                    }
                }



            }



        }


            

        


        public void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser spriteLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            orig(self, spriteLeaser, rCam, newContatiner);
            if (!self.player.SlugCatClass.Equals(ResilientName))
            {
                return;
            }
            if (!BubbleIndices.TryGetValue(spriteLeaser, out var indexBox))
            {
                return;
            }
            int startIndex = indexBox.Value;
            if (startIndex + BubbleSpriteCount > spriteLeaser.sprites.Length)
            {
                return;
            }

            FContainer container = newContatiner ?? rCam.ReturnFContainer("Midground");

            for (int i = 0; i < BubbleSpriteCount; i++)
            {
               
                FSprite sprite = spriteLeaser.sprites[startIndex + i];

                sprite.RemoveFromContainer();
                container.AddChild(sprite);




            }
















        }


        private static Vector2 RotateWithHead(Vector2 offset, float rotation)
        {
            float radians = rotation * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);

            return new Vector2(
                offset.x * cos + offset.y * sin,
                -offset.x * sin + offset.y * cos);
        }

        private void Player_UpdateAnimation(On.Player.orig_UpdateAnimation orig, Player self)
        {
            orig(self);
            if (!self.SlugCatClass.Equals(ResilientName))
            {
                return;
            }
            
            if (self.grasps[0] != null && self.grasps[0].grabbed is JetFish jetfish && jetfish.Consious)
            {
                return;
            }
            if (self.bodyMode == Player.BodyModeIndex.Swimming && self.animation == Player.AnimationIndex.DeepSwim)
            {

                if (self.airInLungs > 0.3f && (self.input[0].ZeroGGamePadIntVec.x != 0 || self.input[0].ZeroGGamePadIntVec.y != 0))
                {
                    float extraForce = Mathf.Max(0f, UnderwaterSwimForce - 1f);

                    Vector2 swimDirection = self.SwimDir(true);

                    self.bodyChunks[0].vel +=swimDirection * (0.9f * self.swimForce * self.bodyChunks[0].submersion * extraForce);
                }





                return;


            }








            if (self.bodyMode != Player.BodyModeIndex.Swimming ||self.animation != Player.AnimationIndex.SurfaceSwim)
            {
                return;
            }

            self.dynamicRunSpeed[0] = SurfaceSwimSpeed;
        }




        private void PlayerUpdateHook(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);
            
        }
    }
}
