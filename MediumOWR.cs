using ICities;

using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.IO;
using ColossalFramework.UI;
using ColossalFramework.Steamworks;

using UnityEngine;

using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using ColossalFramework.Plugins;


namespace MediumOWR
{
    public class MediumOWRMod : IUserMod
    {
   
        public string Name
        {
            get { return "Medium One-Way Roads"; }
        }

        public string Description
        {
            get { return "Adds Four-Lane One-Way Roads to the game."; }
        }

    }


    public class MediumOWRLoader : LoadingExtensionBase
    {


        private static void DebugLog(String str)
        {
           // DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, "[MoreRoads] " + str);
            Debug.Log(str);
        }
        public override void OnCreated(ILoading loading)
        {
            InitMod();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
            {
                InitMod();
            }
        }

        private void InitMod()
        {
            try
            {
                var thumbnails = loadThumbnails("thumbnails", 111, 80, new String[] { "4OWR" , "4OWRTrees", "4OWRGrass" });

                String UICategory =  "RoadsMedium";
                var UIPriority = 99;
                Action<NetInfo> makeOneWaySetup = (NetInfo ni) =>
                {
                    ni.m_Atlas = thumbnails;
                    ni.m_hasBackwardVehicleLanes = false;


                    for (int i = 0; i < ni.m_lanes.Length; i++)
                    {
                        if (ni.m_lanes[i].m_laneType == NetInfo.LaneType.Vehicle)
                        {
                            ni.m_lanes[i].m_direction = NetInfo.Direction.Forward;
                            ni.m_lanes[i].m_finalDirection = NetInfo.Direction.Forward;
                        }
                    }
                };
                var mediumOWR = new ModdedNetwork
                {
                    name = "Four-Lane One-Way Road",
                    baseNetwork = "Medium Road",
                    title = "Four-Lane One-Way Road",
                    description = "A four-lane, oneway road with parking spaces. Supports medium traffic",
                    category = UICategory,
                    setupAction = (NetInfo ni) =>
                    {
                        ni.m_UIPriority = ++UIPriority;
                        ni.m_Thumbnail = "4OWR";
                        makeOneWaySetup(ni);
                    }
                };
                mediumOWR.Load();
                var mediumOWRGrass = new ModdedNetwork {
                    name = "Four-Lane One-Way Road with Grass",
                    baseNetwork = "Medium Road Decoration Grass",
                    title = "Four-Lane One-Way Road with Grass",
                    description = "A four-lane, oneway road with decorative grass. Decorations lower noise pollution. Supports medium traffic",
                    category = UICategory,
                    setupAction = (NetInfo ni) =>
                    {
                        ni.m_UIPriority = ++UIPriority;
                        ni.m_Thumbnail = "4OWRGrass";
                        makeOneWaySetup(ni);
                    }
                };
                mediumOWRGrass.Load();
                var mediumOWRTrees = new ModdedNetwork {
                    name = "Four-Lane One-Way Road with Trees",
                    baseNetwork = "Medium Road Decoration Trees",
                    title = "Four-Lane One-Way Road with Trees",
                    description = "A four-lane, oneway road with decorative trees. Decorations lower noise pollution. Supports medium traffic",
                    category = UICategory,
                    setupAction = (NetInfo ni) =>
                    {
                        ni.m_UIPriority = ++UIPriority;
                        ni.m_Thumbnail = "4OWRTrees";
                        makeOneWaySetup(ni);
                    }
                };
                mediumOWRTrees.Load();

                DebugLog("Finished loading Medium OWRs");
            }
            catch (Exception e)
            {
                DebugLog(e.ToString());
            }
        }

        //The below methods were borrowed from the SomeRoads Mod.

        private String getModPath()
        {
            return "/Users/jonathan/Library/Application Support/Colossal Order/Cities_Skylines/Addons/Mods/Roads";
            //string workshopPath = ".";
            //foreach (PublishedFileId mod in Steam.workshop.GetSubscribedItems())
            //{
            //    if (mod.AsUInt64 == SomeRoadsMod.workshop_id)
            //    {
            //        workshopPath = Steam.workshop.GetSubscribedItemPath(mod);
            //        Debug.Log("SOME ROADS: Workshop path: " + workshopPath);
            //        break;
            //    }
            //}
            //string localPath = DataLocation.modsPath + "/SomeRoads";
            //Debug.Log("SOME ROADS: " + localPath);
            //if (System.IO.Directory.Exists(localPath))
            //{
            //    Debug.Log("SOME ROADS: Local path exists, looking for assets here: " + localPath);
            //    return localPath;
            //}
            //return workshopPath;
        }

        private UITextureAtlas loadThumbnails(String name, int width, int height, String[] rows)
        {

            var thumbnails = ScriptableObject.CreateInstance<UITextureAtlas>();
            thumbnails.padding = 0;
            thumbnails.name = name;

            Shader shader = Shader.Find("UI/Default UI Shader");
            if (shader != null) thumbnails.material = new Material(shader);

            string path = System.IO.Path.Combine(getModPath(), name);
            path = System.IO.Path.ChangeExtension(path, "png");
            DebugLog("Loading thumbnails: " + path);
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(System.IO.File.ReadAllBytes(path));
            thumbnails.material.mainTexture = tex;

            Texture2D tx = new Texture2D(width, height);

            float w = tex.width;
            float h = tex.height;

            string[] ts = new String[] { "", "Disabled", "Focused", "Hovered", "Pressed" };
            for (int i = 0; i < rows.Length; i++)
            {
                for (int j = 0; j < ts.Length; j++)
                {
                    UITextureAtlas.SpriteInfo sprite = new UITextureAtlas.SpriteInfo();
                    sprite.name = rows[i] + ts[j];
                    sprite.region = new Rect((j * width) / w, (i * height) / h, width / w, height / h);
                    sprite.texture = tx;
                    thumbnails.AddSprite(sprite);
                }
            }
            return thumbnails;
        }

    }
    
}
