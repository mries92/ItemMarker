using System;
using System.IO;
using BepInEx;
using IL.RoR2;
using R2API;
using RoR2;
using UnityEngine;

namespace ItemMarker
{
    [BepInPlugin("org.mries.itemmarker", "ItemMarker", "1.0.0")]
    [BepInProcess("Risk of Rain 2.exe")]
    public class ItemMarker : BaseUnityPlugin
    {
        AssetBundle bundle_;
        void Awake()
        {
            On.RoR2.GenericPickupController.CreatePickup += CreatePickup;
            bundle_ = AssetBundle.LoadFromMemory(Properties.Resources.itemmarker);
        }

        private RoR2.GenericPickupController CreatePickup(On.RoR2.GenericPickupController.orig_CreatePickup orig, ref global::RoR2.GenericPickupController.CreatePickupInfo createPickupInfo)
        {
            UnityEngine.GameObject particles = Instantiate((GameObject)bundle_.LoadAsset("beam"));
            RoR2.PickupDef def = RoR2.PickupCatalog.GetPickupDef(createPickupInfo.pickupIndex);
            RoR2.ItemDef idef = RoR2.ItemCatalog.GetItemDef(def.itemIndex);
            switch(idef.tier)
            {
                case ItemTier.Tier1:
                    particles.GetComponent<MeshRenderer>().material.color = Color.white;
                    break;
                case ItemTier.Tier2:
                    particles.GetComponent<MeshRenderer>().material.color = Color.green;
                    break;
                case ItemTier.Tier3:
                    particles.GetComponent<MeshRenderer>().material.color = Color.red;
                    break;
                case ItemTier.Boss:
                    particles.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    break;
                case ItemTier.Lunar:
                    particles.GetComponent<MeshRenderer>().material.color = Color.blue;
                    break;
                default:
                    break;
            }
            particles.transform.position = createPickupInfo.position;
            var pickup = orig(ref createPickupInfo);
            particles.transform.parent = pickup.transform;
            return pickup;
        }
    }
}
