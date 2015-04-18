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

    public class ModdedNetwork
    {
        public String category { get; set; }
        public String baseNetwork { get; set; }
        public String name { get; set; }
        public String title { get; set; }
        public String description { get; set; }
        public Action<NetInfo> setupAction { get; set; }
        public virtual void setup(NetInfo info)
        {
            setupAction(info);
        }
        public void Load()
        {
            GameObject newRoad = GameObject.Instantiate<GameObject>(PrefabCollection<NetInfo>.FindLoaded(baseNetwork).gameObject);
            if (newRoad == null)
            {
                Debug.Log("newRoad is null");
            }
            newRoad.name = name;
            NetInfo ni = newRoad.GetComponent<NetInfo>();
            ni.m_prefabInitialized = false;
            ni.m_netAI = null;
            typeof(NetInfo).GetField("m_UICategory", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(ni, category);

            Locale locale = (Locale)typeof(LocaleManager).GetField("m_Locale", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(SingletonLite<LocaleManager>.instance);

            locale.AddLocalizedString(new Locale.Key() { m_Identifier = "NET_TITLE", m_Key = name }, title);
            locale.AddLocalizedString(new Locale.Key() { m_Identifier = "NET_DESC", m_Key = name }, description);

            MethodInfo initMethod = typeof(NetCollection).GetMethod("InitializePrefabs", BindingFlags.Static | BindingFlags.NonPublic);
            Singleton<LoadingManager>.instance.QueueLoadingAction((IEnumerator)initMethod.Invoke(null, new object[] {
				category,
				new[] { ni },
				new string[] { }
			}));
            Singleton<LoadingManager>.instance.QueueLoadingAction(inCoroutine(() =>
            {
                setup(ni);
                PrefabCollection<NetInfo>.BindPrefabs();
                GameObject.Find(category + "Panel").GetComponent<GeneratedScrollPanel>().RefreshPanel();
            }));
        }
        private IEnumerator inCoroutine(Action a)
        {
            a.Invoke();
            yield break;
        }

    }
}
