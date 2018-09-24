using ModCommon;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OverhaulMod
{
    public class OverhaulMod : Mod<OverhaulModSaveSettings, OverhaulModSettings>, ITogglableMod
    {

        public static OverhaulMod instance;
        protected string SettingsFilename = Application.persistentDataPath + ModHooks.PathSeperator.ToString() + "OverhaulMod.settings.json";
        public Minimap minimap;


        public override void Initialize()
        {
            instance = this;

            SetupSettings();
            RegisterCallbacks();
        }

        private void SetupSettings()
        {
            bool flag = base.GlobalSettings.SettingsVersion != "v0.0.1";
            if (flag || !File.Exists(SettingsFilename))
            {
                if (flag)
                {
                    Log("Settings outdated! Rebuilding.");
                }
                else
                {
                    Log("Settings not found, rebuilding... File will be saved to: " + SettingsFilename);
                }

                base.GlobalSettings.Reset();
            }
            SaveGlobalSettings();
        }

        private void RegisterCallbacks()
        {
            Dev.Where();

            // AutoMinimap
            if (GlobalSettings.MinimapAutoUpdate)
            {
                ModHooks.Instance.HeroUpdateHook += new HeroUpdateHandler(HeroUpdate);
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += this.UpdateMinimap;
                UnityEngine.SceneManagement.SceneManager.activeSceneChanged += UpdateMinimap;
            }
        }

        private void UnregisterCallbacks()
        {
            ModHooks.Instance.HeroUpdateHook -= HeroUpdate;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= this.UpdateMinimap;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= UpdateMinimap;
        }

        public void SaveGame(SaveGameData data)
        {
            
        }

        private void UpdateMinimap(Scene from, Scene to)
        {
            GameManager.instance.StartCoroutine(UpdateMap());
        }

        private void UpdateMinimap(Scene from, LoadSceneMode lsm)
        {
            GameManager.instance.StartCoroutine(UpdateMap());
        }

        private IEnumerator UpdateMap()
        {
            yield return new WaitForSeconds(0.2f);
            UpdateMinimap();
        }

        private void UpdateMinimap()
        {
            if (minimap != null)
            {
                GameMap map = GameManager.instance.gameMap.GetComponent<GameMap>();
                map.SetupMap();
                minimap.UpdateAreas();
            }
        }
        public void HeroUpdate()
        {
            bool equippedCompass = PlayerData.instance.equippedCharm_2;
            if (equippedCompass)
            {
                GameManager.instance.UpdateGameMap();
                if (GameManager.instance.gameMap != null)
                {
                    
                    if (minimap == null)
                    {
                        GameMap map = GameManager.instance.gameMap.GetComponent<GameMap>();
                        minimap = new Minimap(map);
                        minimap.UpdateMap();
                        minimap.UpdateAreas();
                    }
                    else
                    {
                        minimap.UpdateMap();
                    }
                }
                
            }
        }

        public override string GetVersion()
        {
            return GlobalSettings.SettingsVersion;
        }

        public void Unload()
        {
            UnregisterCallbacks();
            instance = null;
        }
    }
}
