using BepInEx;
using RoR2;
using UnityEngine;
using UnityEngine.Events;
using R2API;
using R2API.Utils;
using System.Linq;
using System.Xml;
using System.Collections.ObjectModel;
using RoR2.UI;

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
            On.RoR2.UI.PickupPickerPanel.SetPickupOptions += SetPickupOptions;
            bundle_ = AssetBundle.LoadFromMemory(Properties.Resources.itemmarker);
        }

        private GenericPickupController CreatePickup(On.RoR2.GenericPickupController.orig_CreatePickup orig, ref GenericPickupController.CreatePickupInfo createPickupInfo)
        {
            GameObject particles = Instantiate((GameObject)bundle_.LoadAsset("beam"));
            particles.transform.position = createPickupInfo.position;
            PickupDef def = PickupCatalog.GetPickupDef(createPickupInfo.pickupIndex);
            ItemDef idef = ItemCatalog.GetItemDef(def.itemIndex);
            EquipmentDef edef = EquipmentCatalog.GetEquipmentDef(def.equipmentIndex);
            Color color = Color.white;
            if(idef != null)
            {
                switch (idef.tier)
                {
                    case ItemTier.Tier2:
                        color = Color.green;
                        break;
                    case ItemTier.Tier3:
                        color = Color.red;
                        break;
                    case ItemTier.Boss:
                        color = Color.yellow;
                        break;
                    case ItemTier.Lunar:
                        color = Color.blue;
                        break;
                    default:
                        break;
                }
            }
            if(edef != null)
            {
                color = Color.magenta;
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
            var equipmentindex = PickupCatalog.FindPickupIndex(EquipmentIndex.Fruit);

            Color color = Color.white;
            if (self.IsChoiceAvailable(tier2index))
                color = Color.green;
            else if (self.IsChoiceAvailable(tier3index))
                color = Color.red;
            else if (self.IsChoiceAvailable(bossindex))
                color = Color.yellow;
            else if (self.IsChoiceAvailable(lunarindex))
                color = Color.blue;
            else if (self.IsChoiceAvailable(equipmentindex))
                color = Color.magenta;

            particles.GetComponent<MeshRenderer>().material.color = color;
        }

        public void SetPickupOptions(On.RoR2.UI.PickupPickerPanel.orig_SetPickupOptions orig, RoR2.UI.PickupPickerPanel self, RoR2.PickupPickerController.Option[] options)
        {
            orig(self, options);
            var allocator = self.GetFieldValue<UIElementAllocator<MPButton>>("buttonAllocator");
            ReadOnlyCollection<MPButton> buttons = allocator.elements;
            TooltipProvider provider = new TooltipProvider();

            int i = 0;
            foreach(var button in buttons)
            {
                TooltipContent content = new TooltipContent();
                var def = PickupCatalog.GetPickupDef(options[i].pickupIndex);
                var idef = ItemCatalog.GetItemDef(def.itemIndex);
                var edef = EquipmentCatalog.GetEquipmentDef(def.equipmentIndex);
                if(idef != null)
                {
                    content.titleColor = def.darkColor;
                    content.titleToken = idef.nameToken;
                    content.bodyToken = idef.descriptionToken;
                }
                else
                {
                    content.titleColor = def.darkColor;
                    content.titleToken = edef.nameToken;
                    content.bodyToken = edef.descriptionToken;
                }
                
                button.gameObject.AddComponent<TooltipProvider>().SetContent(content);
                i++;
            }
        }
    }
}
