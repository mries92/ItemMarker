using BepInEx;
using RoR2;
using UnityEngine;
using R2API;
using R2API.Utils;
using System.Linq;
using System.Xml;

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
            On.RoR2.PickupPickerController.SetOptionsInternal += SetOptions;
            On.RoR2.PickupPickerController.OnInteractionBegin += OnInteractionBegin;
            bundle_ = AssetBundle.LoadFromMemory(Properties.Resources.itemmarker);
        }

        private GenericPickupController CreatePickup(On.RoR2.GenericPickupController.orig_CreatePickup orig, ref GenericPickupController.CreatePickupInfo createPickupInfo)
        {
            GameObject particles = Instantiate((GameObject)bundle_.LoadAsset("beam"));
            particles.transform.position = createPickupInfo.position;
            PickupDef def = PickupCatalog.GetPickupDef(createPickupInfo.pickupIndex);
            ItemDef idef = ItemCatalog.GetItemDef(def.itemIndex);
            Color color = Color.white;
            switch (idef.tier)
            {
                case ItemTier.Tier2:
                    color = Color.green;
                    break;
                case ItemTier.Tier3:
                    color = Color.red;
                    break;
                case ItemTier.Boss:
                    color = new Color(255, 236, 31);
                    break;
                case ItemTier.Lunar:
                    color = Color.blue;
                    break;
                default:
                    color = Color.cyan;
                    break;
            }
            particles.GetComponent<MeshRenderer>().material.color = color;

            var pickup = orig(ref createPickupInfo);
            particles.transform.parent = pickup.transform;
            return pickup;
        }

        public void SetOptions(On.RoR2.PickupPickerController.orig_SetOptionsInternal orig, global::RoR2.PickupPickerController self, global::RoR2.PickupPickerController.Option[] newOptions)
        {
            orig(self, newOptions);
            GameObject particles = Instantiate((GameObject)bundle_.LoadAsset("beam"));
            particles.transform.position = self.transform.position;
            particles.transform.parent = self.transform;

            var tier2index = PickupCatalog.FindPickupIndex(ItemIndex.Feather);
            var tier3index = PickupCatalog.FindPickupIndex(ItemIndex.Clover);
            var bossindex = PickupCatalog.FindPickupIndex(ItemIndex.BeetleGland);
            var lunarindex = PickupCatalog.FindPickupIndex(ItemIndex.LunarDagger);

            Color color = Color.white;
            if (self.IsChoiceAvailable(tier2index))
                color = Color.green;
            if (self.IsChoiceAvailable(tier3index))
                color = Color.red;
            if (self.IsChoiceAvailable(bossindex))
                color = new Color(255, 236, 31);
            if (self.IsChoiceAvailable(lunarindex))
                color = Color.blue;

            particles.GetComponent<MeshRenderer>().material.color = color;
        }

        public void OnInteractionBegin(On.RoR2.PickupPickerController.orig_OnInteractionBegin orig, global::RoR2.PickupPickerController self, global::RoR2.Interactor activator)
        {
            // Get all the items in the menu
            var options = self.GetFieldValue<PickupPickerController.Option[]>("options");
            // Print the available options
            foreach(var option in options)
            {
                UnityEngine.Debug.Log("Available Choice: " + PickupCatalog.GetPickupDef(option.pickupIndex).itemIndex);
            }
            orig(self, activator);
        }
    }
}
