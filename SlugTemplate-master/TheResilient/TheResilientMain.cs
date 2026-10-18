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

        public static readonly SlugcatStats.Name ResilientName = new SlugcatStats.Name("TheResilient", false);

        private void OnEnable()
        {
            On.Player.Update += PlayerUpdateHook;
            On.Player.LungUpdate += PlayerLungUpdateHook;
        }

        private void PlayerUpdateHook(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);

 






















           if (self.SlugCatClass == ResilientName)
            {
                if (self.submerged && (self.input[0].x != 0 || self.input[0].y != 0))
                {
                    self.firstChunk.vel *= 1.04f;
                }
            }
        }

        private void PlayerLungUpdateHook(On.Player.orig_LungUpdate orig, Player self)
        {
            orig(self);

            if (self.SlugCatClass == ResilientName












 && self.submerged)
            {
                self.airInLungs = Mathf.Min(1f, self.airInLungs + 0.008f);

                if (self.room != null && Random.value < 0.15f)
                {
                    self.room.AddObject(new Bubble(self.firstChunk.pos, RWCustom.Custom.RNV() * 2f, false, false));
                }
            }
 








       }
    }
}
