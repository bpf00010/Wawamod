using System.Security.Permissions;
using BepInEx;
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
        private void OnEnable()
        {
            On.Player.Update += PlayerUpdateHook;
        }

        void PlayerUpdateHook(On.Player.orig_Update orig, Player self, bool eu)
        {

        }
    }
}



