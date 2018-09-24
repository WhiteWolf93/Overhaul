using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod
{
    public class Minimap
    {
        public static Minimap instance;

        public GameMap map;
        public bool ShowMinimap;

        public GameObject minimapParent, minimapCamera;
        public Vector2 offset;

        private float originOffsetX, originOffsetY, sceneWidth, sceneHeight;

        public Dictionary<string, GameObject> areas, pins;
        public Dictionary<string, GameObject[]> markers;

        public Minimap(GameMap GameMap)
        {
            OverhaulMod.instance.Log("instance = this");
            instance = this;
            OverhaulMod.instance.Log("map = gamemap.getcomponent<gamemap>()");
            map = GameMap;
            /*
            map.WorldMap();
            map.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            GameObject.Destroy(map);
            */
            OverhaulMod.instance.Log("Setting up the minimap");
            minimapParent = new GameObject("Minimap");
            minimapParent.transform.position = map.transform.position;
            GameObject.DontDestroyOnLoad(minimapParent);
            OverhaulMod.instance.Log("Parent: " + map.transform.parent.name);
            SetupAreas();
            SetupCamera();
            minimapParent.transform.SetParent(minimapCamera.transform);
            
        }

        private void SetupCamera()
        {
            minimapCamera = new GameObject("MinimapCamera");
            GameObject.DontDestroyOnLoad(minimapCamera);
            minimapCamera.transform.position = GameCameras.instance.hudCamera.transform.position;
            Camera c = minimapCamera.AddComponent<Camera>();
            /*
            System.Reflection.FieldInfo[] fields = typeof(Camera).GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(c, field.GetValue(GameCameras.instance.hudCamera));
            }
            */
            c.orthographic = true;
            c.orthographicSize = GameCameras.instance.hudCamera.orthographicSize / 4;
            c.depth = GameCameras.instance.hudCamera.depth - 1;
            c.clearFlags = CameraClearFlags.Depth;
            c.cullingMask = GameCameras.instance.hudCamera.cullingMask;
            c.farClipPlane = GameCameras.instance.hudCamera.farClipPlane;
            c.nearClipPlane = GameCameras.instance.hudCamera.nearClipPlane;
            c.backgroundColor = Color.clear;
            c.rect = new Rect(0.78f, 0.75f, 0.2f, 0.2f);
            minimapCamera.SetActive(true);
        }

        public GameObject PinFromGameObject(GameObject g, string name)
        {
            GameObject pin = GameObject.Instantiate(g);
            pin.name = name;
            pin.transform.localScale = Vector3.one;
            foreach(Component c in pin.GetComponents<MonoBehaviour>())
            {
                if (c is PlayMakerFSM || c is EventRegister)
                {
                    OverhaulMod.instance.Log("Destroyed type of " + c.GetType().ToString());
                    GameObject.Destroy(c);
                }
            }
            /*
            SpriteRenderer r = pin.AddComponent<SpriteRenderer>();
            r.sprite = g.GetComponent<SpriteRenderer>().sprite;
            */
            pin.SetActive(true);

            return pin;
        }

        public void SetupAreas()
        {
            OverhaulMod.instance.Log("Setting up areas");
            if (areas != null)
            {
                if (areas.Keys.Count > 0)
                {
                    foreach (string key in areas.Keys)
                    {
                        GameObject.Destroy(areas[key]);
                    }
                }
            }
            
            areas = new Dictionary<string, GameObject>()
            {
                { "AncientBasin", GameObject.Instantiate(map.areaAncientBasin) }, { "CityOfTears", GameObject.Instantiate(map.areaCity)}, { "Crossroads", GameObject.Instantiate(map.areaCrossroads)},
                { "CrystalPeak", GameObject.Instantiate(map.areaCrystalPeak)}, { "Deepnest", GameObject.Instantiate(map.areaDeepnest)}, { "FogCanyon", GameObject.Instantiate(map.areaFogCanyon)},
                { "FungalWastes", GameObject.Instantiate(map.areaFungalWastes)}, { "Greenpath",GameObject.Instantiate( map.areaGreenpath)}, { "KingdomsEdge", GameObject.Instantiate(map.areaKingdomsEdge)},
                { "QueensGardens", GameObject.Instantiate(map.areaQueensGardens)}, { "RestingGrounds", GameObject.Instantiate(map.areaRestingGrounds)}, { "Dirtmouth", GameObject.Instantiate(map.areaDirtmouth)},
                { "RoyalWaterways", GameObject.Instantiate(map.areaWaterways) }, {"Cliffs", GameObject.Instantiate(map.areaCliffs)}
            };

            foreach (string area in areas.Keys)
            {
                areas[area].name = "Minimap_" + area;
            }
            
            
            pins = new Dictionary<string, GameObject>()
            {
                /*{"Flame", map.flamePins }, {"Dreamers", map.dreamerPins }, {"DreamGate", map.dreamGateMarker }, {"Shade", map.shadeMarker }, */{"Compass", PinFromGameObject(map.compassIcon,"Minimap_Compass")}
            };
          
            /*
            markers = new Dictionary<string, GameObject[]>()
            {
                {"Blue", map.mapMarkersBlue }, {"Red", map.mapMarkersRed }, {"White", map.mapMarkersWhite }, {"Yellow", map.mapMarkersYellow }
            };
            */
            OverhaulMod.instance.Log("Setting up the areas to child of the minimap");
            foreach (string area in areas.Keys)
            {
                areas[area].transform.parent = minimapParent.transform;
            }
            
            foreach (string pin in pins.Keys)
            {
                pins[pin].transform.parent = minimapParent.transform;
            }
            
        }

        public void ShowArea(string area)
        {
            //OverhaulMod.instance.Log("Setting up if show the areas");
            bool f = HeroController.instance.playerData.equippedCharm_2;
            //areas[area].SetActive(true);
            
            switch (area)
            {
                case "AncientBasin":
                    areas["AncientBasin"].SetActive(HeroController.instance.playerData.mapAbyss && f);
                    break;
                case "CityOfTears":
                    areas["CityOfTears"].SetActive(HeroController.instance.playerData.mapCity && f);
                    break;
                case "Crossroads":
                    areas["Crossroads"].SetActive(HeroController.instance.playerData.mapCrossroads && f);
                    break;
                case "CrystalPeak":
                    areas["CrystalPeak"].SetActive(HeroController.instance.playerData.mapMines && f);
                    break;
                case "Deepnest":
                    areas["CrystalPeak"].SetActive(HeroController.instance.playerData.mapDeepnest && f);
                    break;
                case "FogCanyon":
                    areas["FogCanyon"].SetActive(HeroController.instance.playerData.mapFogCanyon && f);
                    break;
                case "FungalWastes":
                    areas["FungalWastes"].SetActive(HeroController.instance.playerData.mapFungalWastes && f);
                    break;
                case "Greenpath":
                    areas["Greenpath"].SetActive(HeroController.instance.playerData.mapGreenpath && f);
                    break;
                case "KingdomsEdge":
                    areas["KingdomsEdge"].SetActive(HeroController.instance.playerData.mapOutskirts && f);
                    break;
                case "QueensGardens":
                    areas["QueensGardens"].SetActive(HeroController.instance.playerData.mapRoyalGardens && f);
                    break;
                case "RestingGrounds":
                    areas["RestingGrounds"].SetActive(HeroController.instance.playerData.mapRestingGrounds && f);
                    break;
                case "Dirtmouth":
                    areas["Dirtmouth"].SetActive(HeroController.instance.playerData.mapDirtmouth && f);
                    break;
                case "RoyalWaterways":
                    areas["Dirtmouth"].SetActive(HeroController.instance.playerData.mapWaterways && f);
                    break;
                case "Cliffs":
                    areas["Cliffs"].SetActive(HeroController.instance.playerData.mapCliffs && f);
                    break;
            }
            
        }

        public void UpdateAreas()
        {
            //PositionCompass(false);
            //map.compassIcon.SetActive(false);
            //minimapCamera.transform.position = GameCameras.instance.hudCamera.transform.position + new Vector3(100, 0, 0);
            //OverhaulMod.instance.Log("Updating areas");
            foreach (string s in areas.Keys)
            {
                UpdateArea(s);
                ShowArea(s);
            }
            //map.PositionCompass(false);
            //pins["Compass"].transform.localPosition = map.compassIcon.transform.localPosition;
        }

        private GameObject[] FromTransform(Transform[] list)
        {
            List<GameObject> values = new List<GameObject>();
            foreach(Transform t in list)
            {
                values.Add(t.gameObject);
            }
            return values.ToArray();
        }

        public GameObject GetOriginalAreaByName(string name)
        {
            switch (name)
            {
                case "AncientBasin":
                    return map.areaAncientBasin;
                case "CityOfTears":
                    return map.areaCity;
                case "Crossroads":
                    return map.areaCrossroads;
                case "CrystalPeak":
                    return map.areaCrystalPeak;
                case "Deepnest":
                    return map.areaDeepnest;
                case "FogCanyon":
                    return map.areaFogCanyon;
                case "FungalWastes":
                    return map.areaFungalWastes;
                case "Greenpath":
                    return map.areaGreenpath;
                case "KingdomsEdge":
                    return map.areaKingdomsEdge;
                case "QueensGardens":
                    return map.areaQueensGardens;
                case "RestingGrounds":
                    return map.areaRestingGrounds;
                case "Dirtmouth":
                    return map.areaDirtmouth;
                case "RoyalWaterways":
                    return map.areaWaterways;
                case "Cliffs":
                    return map.areaCliffs;
                default:
                    return null;
            }
        }



        public void SetupMap(bool pinsOnly = false)
        {
            PlayerData pd = HeroController.instance.playerData;
            for (int i = 0; i < minimapParent.transform.childCount; i++)
            {
                GameObject gameObject = minimapParent.transform.GetChild(i).gameObject;
                for (int j = 0; j < gameObject.transform.childCount; j++)
                {
                    GameObject gameObject2 = gameObject.transform.GetChild(j).gameObject;
                    if (pd.scenesMapped.Contains(gameObject2.transform.name) || pd.mapAllRooms)
                    {
                        if (pd.hasQuill && !pinsOnly)
                        {
                            gameObject2.SetActive(true);
                        }
                        for (int k = 0; k < gameObject2.transform.childCount; k++)
                        {
                            GameObject gameObject3 = gameObject2.transform.GetChild(k).gameObject;
                            if (gameObject3.name == "pin_blue_health" && !gameObject3.activeSelf && pd.scenesEncounteredCocoon.Contains(gameObject2.transform.name) && pd.hasPinCocoon)
                            {
                                gameObject3.SetActive(true);
                            }
                            if (gameObject3.name == "pin_dream_tree" && !gameObject3.activeSelf && pd.scenesEncounteredDreamPlant.Contains(gameObject2.transform.name) && pd.hasPinDreamPlant)
                            {
                                gameObject3.SetActive(true);
                            }
                            if (gameObject3.name == "pin_dream_tree" && gameObject3.activeSelf && pd.scenesEncounteredDreamPlantC.Contains(gameObject2.transform.name))
                            {
                                gameObject3.SetActive(false);
                            }
                        }
                    }
                }
            }
        }

        public void UpdateArea(string area)
        {
            
            map.displayNextArea = false;
            HeroController.instance.playerData.scenesMapped.Add(GameManager.instance.sceneName);
            SetupMap();
            /*
            GameObject[] childs = FromTransform(areas[area].transform.GetComponentsInChildren<Transform>());
            GameObject[] originalchilds = FromTransform(GetOriginalAreaByName(area).GetComponentsInChildren<Transform>());

            int num = childs.Length;
            
            for (int i = 0; i < num; i++)
            {
                childs[i].SetActive(originalchilds[i].activeSelf);
            }
            */
        }

        public void UpdateMap()
        {
            //PositionCompass(false);
            minimapCamera.transform.position = GameCameras.instance.hudCamera.transform.position + new Vector3(100, 0, 0);
            //OverhaulMod.instance.Log("Updating areas");
            //map.PositionCompass(false);
            bool active = map.compassIcon.activeSelf;
            PositionCompass(false);
            pins["Compass"].transform.localPosition = map.compassIcon.transform.localPosition;
            if (minimapParent != null)
            {
                //OverhaulMod.instance.Log(minimapParent.transform.position);
                
                minimapParent.transform.localPosition = new Vector3(-map.compassIcon.transform.localPosition.x, -map.compassIcon.transform.localPosition.y, minimapParent.transform.localPosition.z);
            }
            
        }

        public void PositionCompass(bool posShade)
        {
            GameObject gameObject = null;
            GameManager gm = GameManager.instance;
            PlayerData pd = HeroController.instance.playerData;
            string currentMapZone = gm.GetCurrentMapZone();
            if (currentMapZone == "DREAM_WORLD" || currentMapZone == "WHITE_PALACE" || currentMapZone == "GODS_GLORY")
            {
                pins["Compass"].SetActive(false);
                return;
            }
            string sceneName;
            if (!map.inRoom)
            {
                sceneName = gm.sceneName;
            }
            else
            {
                currentMapZone = map.doorMapZone;
                sceneName = map.doorScene;
            }
            if (currentMapZone == "ABYSS")
            {
                gameObject = map.areaAncientBasin;
                for (int i = 0; i < map.areaAncientBasin.transform.childCount; i++)
                {
                    GameObject gameObject2 = map.areaAncientBasin.transform.GetChild(i).gameObject;
                    if (gameObject2.name == sceneName)
                    {
                        map.currentScene = gameObject2;
                        break;
                    }
                }
            }
            else if (currentMapZone == "CITY" || currentMapZone == "KINGS_STATION" || currentMapZone == "SOUL_SOCIETY" || currentMapZone == "LURIENS_TOWER")
            {
                gameObject = map.areaCity;
                for (int j = 0; j < map.areaCity.transform.childCount; j++)
                {
                    GameObject gameObject3 = map.areaCity.transform.GetChild(j).gameObject;
                    if (gameObject3.name == sceneName)
                    {
                        map.currentScene = gameObject3;
                        break;
                    }
                }
            }
            else if (currentMapZone == "CLIFFS")
            {
                gameObject = map.areaCliffs;
                for (int k = 0; k < map.areaCliffs.transform.childCount; k++)
                {
                    GameObject gameObject4 = map.areaCliffs.transform.GetChild(k).gameObject;
                    if (gameObject4.name == sceneName)
                    {
                        map.currentScene = gameObject4;
                        break;
                    }
                }
            }
            else if (currentMapZone == "CROSSROADS" || currentMapZone == "SHAMAN_TEMPLE")
            {
                gameObject = map.areaCrossroads;
                for (int l = 0; l < map.areaCrossroads.transform.childCount; l++)
                {
                    GameObject gameObject5 = map.areaCrossroads.transform.GetChild(l).gameObject;
                    if (gameObject5.name == sceneName)
                    {
                        map.currentScene = gameObject5;
                        break;
                    }
                }
            }
            else if (currentMapZone == "MINES")
            {
                gameObject = map.areaCrystalPeak;
                for (int m = 0; m < map.areaCrystalPeak.transform.childCount; m++)
                {
                    GameObject gameObject6 = map.areaCrystalPeak.transform.GetChild(m).gameObject;
                    if (gameObject6.name == sceneName)
                    {
                        map.currentScene = gameObject6;
                        break;
                    }
                }
            }
            else if (currentMapZone == "DEEPNEST" || currentMapZone == "BEASTS_DEN")
            {
                gameObject = map.areaDeepnest;
                for (int n = 0; n < map.areaDeepnest.transform.childCount; n++)
                {
                    GameObject gameObject7 = map.areaDeepnest.transform.GetChild(n).gameObject;
                    if (gameObject7.name == sceneName)
                    {
                        map.currentScene = gameObject7;
                        break;
                    }
                }
            }
            else if (currentMapZone == "FOG_CANYON" || currentMapZone == "MONOMON_ARCHIVE")
            {
                gameObject = map.areaFogCanyon;
                for (int num = 0; num < map.areaFogCanyon.transform.childCount; num++)
                {
                    GameObject gameObject8 = map.areaFogCanyon.transform.GetChild(num).gameObject;
                    if (gameObject8.name == sceneName)
                    {
                        map.currentScene = gameObject8;
                        break;
                    }
                }
            }
            else if (currentMapZone == "WASTES" || currentMapZone == "QUEENS_STATION")
            {
                gameObject = map.areaFungalWastes;
                for (int num2 = 0; num2 < map.areaFungalWastes.transform.childCount; num2++)
                {
                    GameObject gameObject9 = map.areaFungalWastes.transform.GetChild(num2).gameObject;
                    if (gameObject9.name == sceneName)
                    {
                        map.currentScene = gameObject9;
                        break;
                    }
                }
            }
            else if (currentMapZone == "GREEN_PATH")
            {
                gameObject = map.areaGreenpath;
                for (int num3 = 0; num3 < map.areaGreenpath.transform.childCount; num3++)
                {
                    GameObject gameObject10 = map.areaGreenpath.transform.GetChild(num3).gameObject;
                    if (gameObject10.name == sceneName)
                    {
                        map.currentScene = gameObject10;
                        break;
                    }
                }
            }
            else if (currentMapZone == "OUTSKIRTS" || currentMapZone == "HIVE" || currentMapZone == "COLOSSEUM")
            {
                gameObject = map.areaKingdomsEdge;
                for (int num4 = 0; num4 < map.areaKingdomsEdge.transform.childCount; num4++)
                {
                    GameObject gameObject11 = map.areaKingdomsEdge.transform.GetChild(num4).gameObject;
                    if (gameObject11.name == sceneName)
                    {
                        map.currentScene = gameObject11;
                        break;
                    }
                }
            }
            else if (currentMapZone == "ROYAL_GARDENS")
            {
                gameObject = map.areaQueensGardens;
                for (int num5 = 0; num5 < map.areaQueensGardens.transform.childCount; num5++)
                {
                    GameObject gameObject12 = map.areaQueensGardens.transform.GetChild(num5).gameObject;
                    if (gameObject12.name == sceneName)
                    {
                        map.currentScene = gameObject12;
                        break;
                    }
                }
            }
            else if (currentMapZone == "RESTING_GROUNDS")
            {
                gameObject = map.areaRestingGrounds;
                for (int num6 = 0; num6 < map.areaRestingGrounds.transform.childCount; num6++)
                {
                    GameObject gameObject13 = map.areaRestingGrounds.transform.GetChild(num6).gameObject;
                    if (gameObject13.name == sceneName)
                    {
                        map.currentScene = gameObject13;
                        break;
                    }
                }
            }
            else if (currentMapZone == "TOWN" || currentMapZone == "KINGS_PASS")
            {
                gameObject = map.areaDirtmouth;
                for (int num7 = 0; num7 < map.areaDirtmouth.transform.childCount; num7++)
                {
                    GameObject gameObject14 = map.areaDirtmouth.transform.GetChild(num7).gameObject;
                    if (gameObject14.name == sceneName)
                    {
                        map.currentScene = gameObject14;
                        break;
                    }
                }
            }
            else if (currentMapZone == "WATERWAYS" || currentMapZone == "GODSEEKER_WASTE")
            {
                gameObject = map.areaWaterways;
                for (int num8 = 0; num8 < map.areaWaterways.transform.childCount; num8++)
                {
                    GameObject gameObject15 = map.areaWaterways.transform.GetChild(num8).gameObject;
                    if (gameObject15.name == sceneName)
                    {
                        map.currentScene = gameObject15;
                        break;
                    }
                }
            }
            if (map.currentScene != null)
            {
                map.currentScenePos = new Vector3(map.currentScene.transform.localPosition.x + gameObject.transform.localPosition.x, map.currentScene.transform.localPosition.y + gameObject.transform.localPosition.y, 0f);
                if (posShade)
                {
                    if (!map.inRoom)
                    {
                        map.shadeMarker.transform.localPosition = new Vector3(map.currentScenePos.x, map.currentScenePos.y, 0f);
                    }
                    else
                    {
                        float x = map.currentScenePos.x - map.currentScene.GetComponent<SpriteRenderer>().sprite.rect.size.x / 100f / 2f + (map.doorX + map.doorOriginOffsetX) / map.doorSceneWidth * (map.currentScene.GetComponent<SpriteRenderer>().sprite.rect.size.x / 100f * map.transform.localScale.x) / map.transform.localScale.x;
                        float y = map.currentScenePos.y - map.currentScene.GetComponent<SpriteRenderer>().sprite.rect.size.y / 100f / 2f + (map.doorY + map.doorOriginOffsetY) / map.doorSceneHeight * (map.currentScene.GetComponent<SpriteRenderer>().sprite.rect.size.y / 100f * map.transform.localScale.y) / map.transform.localScale.y;
                        map.shadeMarker.transform.localPosition = new Vector3(x, y, 0f);
                    }
                    pd.shadeMapPos = new Vector3(map.currentScenePos.x, map.currentScenePos.y, 0f);
                }
            }
            else
            {
                Debug.Log("Couldn't find current scene object!");
                if (posShade)
                {
                    pd.shadeMapPos = new Vector3(-10000f, -10000f, 0f);
                    map.shadeMarker.transform.localPosition = pd.shadeMapPos;
                }
            }
            Vector2 vector = map.currentScene.GetComponent<SpriteRenderer>().sprite.bounds.size;
            HeroController hero = HeroController.instance;
            tk2dTileMap tilemap = gm.tilemap;
            sceneWidth = (float)tilemap.width;
            sceneHeight = (float)tilemap.height;
            if (!map.inRoom)
            {
                float x = map.currentScenePos.x - vector.x / 2f + (hero.transform.position.x + this.originOffsetX) / sceneWidth * (vector.x * map.transform.localScale.x) / map.transform.localScale.x;
                float y = map.currentScenePos.y - vector.y / 2f + (hero.transform.position.y + this.originOffsetY) / sceneHeight * (vector.y * map.transform.localScale.y) / map.transform.localScale.y;
                map.compassIcon.transform.localPosition = new Vector3(x, y, -1f);
            }
            else
            {
                float x = map.currentScenePos.x - vector.x / 2f + (map.doorX + map.doorOriginOffsetX) / map.doorSceneWidth * (vector.x * map.transform.localScale.x) / map.transform.localScale.x;
                float y = map.currentScenePos.y - vector.y / 2f + (map.doorY + map.doorOriginOffsetY) / map.doorSceneHeight * (vector.y * map.transform.localScale.y) / map.transform.localScale.y;
                map.compassIcon.transform.localPosition = new Vector3(x, y, -1f);
            }
        }
    }
}
