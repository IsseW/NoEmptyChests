using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

namespace NoEmptyChests
{


    [BepInPlugin(Guid, Name, Version)]
    public class MainClass : BaseUnityPlugin
    {
        public const string
            Name = "NoEmptyChests",
            Author = "Isse",
            Guid = Author + "." + Name,
            Version = "1.0.0.0";

        public static MainClass instance;
        public Harmony harmony;
        public ManualLogSource log;


        void Awake()
        {
            if (!instance)
                instance = this;
            else
                Destroy(this);

            log = Logger;
            harmony = new Harmony(Guid);

            harmony.PatchAll(typeof(ChestPatch));
        }
    }

    [HarmonyPatch(typeof(LootContainerInteract))]
    public class ChestPatch
    {
        [HarmonyPatch(nameof(LootContainerInteract.OpenContainer)), HarmonyPrefix]
        static bool Open(LootContainerInteract __instance)
        {
            MainClass.instance.log.LogMessage("OPENED CHEST!!!!");
            IEnumerator DestroyChest()
            {
                yield return new WaitForSeconds(1);
                while (__instance.animator.transform.position.y > 10)
                {
                    __instance.animator.transform.position += Vector3.down * Time.deltaTime;
                    yield return null;
                }
                Object.Destroy(__instance.animator.gameObject);
            }
            MainClass.instance.StartCoroutine(DestroyChest());
            return true;
        }
    }


}
