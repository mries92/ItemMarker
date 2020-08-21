using System;
using BepInEx;
using R2API;

namespace ItemMarker
{
    [BepInPlugin("org.mries.itemmarker", "ItemMarker", "1.0.0")]
    [BepInProcess("Risk of Rain 2.exe")]
    public class ItemMarker : BaseUnityPlugin
    {
        void Awake()
        {
            UnityEngine.Debug.Log("Loaded ItemMarker");
            On.RoR2.GenericPickupController.CreatePickup += CreatePickup;
        }

        private static RoR2.GenericPickupController CreatePickup(On.RoR2.GenericPickupController.orig_CreatePickup orig, ref global::RoR2.GenericPickupController.CreatePickupInfo createPickupInfo)
        {
            UnityEngine.Debug.Log("YOU PICKED UP AN ITEM");
            return orig(ref createPickupInfo);
        }
    }
}
