using System;
using BepInEx;
using UnityEngine;

namespace TemplateMod
{
    //initialises our mod as a BepInPlugin (using our constants)
    [BepInPlugin(MOD_ID, MOD_NAME, MOD_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        //mod id, used internally. MUST BE UNIQUE!!! (make this the same as the one in the info.json file)
        public const string MOD_ID = "erroneous.templatemod";
        //mod name (probably unused)
        public const string MOD_NAME = "Template Mod";
        //mod version (probably unused)
        public const string MOD_VERSION = "0.1";

        public void OnEnable()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {MOD_ID} is loaded!");

            //add 'hooks' to existing methods
            //hooks just divert from away from the existing method to this method.
            //using MonoMod.RuntimeDetour.HookGen here
            On.FirstPersonMovement.Start += Movement_Start;
            On.Combat.Hit += Combat_Hit;
            On.SaveData.RunBeforeSavedValuesInit += SaveData_AddNewData;
            On.MenuButtons.Start += DoPrefabChanges;
        }

        private void DoPrefabChanges(On.MenuButtons.orig_Start orig, MenuButtons self)
        {
            orig(self);
            //change an existing prefab (this makes titanium swords do a ton of damage)
            ModHelper.GetPrefab("titansword_icon").GetComponent<MeleeWeapon>().EnemyDamage = 100;

            //get an existing prefab,
            ModHelper.Log(MOD_ID, "Titanium sword damage: " + ModHelper.GetPrefab("titansword_icon").GetComponent<MeleeWeapon>().EnemyDamage);

            //modify an ammo preset (this makes iron bullets do a ton of damage)
            AmmoListStruct ammo = ModHelper.GetAmmoPreset("regulargun").UsableAmmo[0];
            ammo.ammoTypeBaseDamage = 100;
            ModHelper.GetAmmoPreset("regulargun").UsableAmmo[0] = ammo;
        }

        //an example of adding new saved data to our save file
        private void SaveData_AddNewData(On.SaveData.orig_RunBeforeSavedValuesInit orig, SaveData self)
        {
            //a tip: if we do not run orig, then other mods will not be able to use this hook. even if the method doesn't have anything in it
            //(like this one), still run the orig method!
            orig(self);

            //add a new saved boolean to the list, which is saved in the save file.It can then be accessed later.
            SaveBooleans newsavedbool = new SaveBooleans
            {
                myKey = "custom_saved_bool",
                MyValue = false
            };
            self.savedBools = self.savedBools.AddtoArray(newsavedbool);

            //adds a new saved float. These are the same as saved booleans but as a float
            SaveFloats newsavefloat = new SaveFloats
            {
                myKey = "custom_saved_bool",
                MyValue = 1.0f
            };
            self.savedFloats = self.savedFloats.AddtoArray(newsavefloat);

            //access them using SaveData.StaticSavedBools and SaveData.StaticSavedFloats, but do it after this function!

            
        }

        //this hook is connected to the start function in FirstPersonMovement (which moves the player)
        private void Movement_Start(On.FirstPersonMovement.orig_Start orig, FirstPersonMovement self)
        {
            //orig runs the original function
            orig(self);
            //set our jump strength to a ton
            self.JumpStrength = 12;
        }

        //this hook is connected to the Hit function in the players Combat script (it does attacks and mana)
        private void Combat_Hit(On.Combat.orig_Hit orig, Combat self)
        {
            //runs the original function
            orig(self);
            //gives us free mana
            self.GiveMana(100);
        }
    }
}
