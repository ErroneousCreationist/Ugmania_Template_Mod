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
        public const string MOD_VERSION = "0.0.1";

        public void OnEnable()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {MOD_ID} is loaded!");

            //add 'hooks' to existing methods
            //hooks just divert from away from the existing method to this method.
            //using MonoMod.RuntimeDetour.HookGen here
            On.FirstPersonMovement.Start += Movement_Start;
            On.Combat.Hit += Combat_Hit;
            On.SaveData.RunAfterSaveLoaded += SaveData_SaveLoaded;

            //get an existing prefab, you cannot change these however
            Logger.LogInfo("Titanium sword damage: " + ModHelper.GetPrefab("titansword_icon").GetComponent<MeleeWeapon>().EnemyDamage);

            //change an existing prefab (this makes titanium swords do a ton of damage)
            //note you have to access the prefab dictionary directly
            ModManager.instance.Objects.PrefabDictionary["titansword_icon"].GetComponent<MeleeWeapon>().EnemyDamage = 100;

            //modify an ammo preset (this makes iron bullets do a ton of damage)
            AmmoListStruct ammo = ModHelper.GetAmmoPreset("regulargun").UsableAmmo[0];
            ammo.ammoTypeBaseDamage = 100;
            ModHelper.GetAmmoPreset("regulargun").UsableAmmo[0] = ammo;
        }

        private void SaveData_SaveLoaded(On.SaveData.orig_RunAfterSaveLoaded orig, SaveData self)
        {
            //a tip: if we do not run orig, then other mods will not be able to use this hook. even if the method doesn't have anything in it
            //(like this one), still run the orig method!
            orig(self);

            //ALWAYS REFERENCE SCENE OBJECTS IN A FUNCTION LIKE THIS! these scene objects do not exist in any other scene,
            //so referencing them here is the best place if you want them modified
            //also note: all SceneObjects are MonoBehaviours, so you can get their actual type as shown (since they all inherit from
            //monobehaviour). You can also check if they are of a type like this: GetSceneObject("spacelander") is SpaceLander
            //here we get the health bar ui object as a slider and set its max value to 100
            (ModHelper.GetSceneObject("ui_ahpbar") as UnityEngine.UI.Image).color = Color.black;

            //some classes have static singleton references, such as this one. as such, you can access them without going through
            //the scene objects
            playerHealth.instance.MaxHealth = 100;
            playerHealth.instance.currentHeath = 100;
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
