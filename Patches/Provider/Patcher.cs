using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEditor;
using UnityEngine;

namespace AV.Inspector
{
	internal static class Patcher
	{
		static readonly string HarmonyID = typeof(Patcher).Assembly.GetName().Name;

		static InspectorPrefs prefs = InspectorPrefs.Loaded;

		static Harmony harmony;
		static List<PatchBase> patches;
		
		static FieldInfo currentSkinField;

		
		[InitializeOnLoadMethod]
		internal static async void ApplyPatches()
		{
			if (!prefs.enabled)
				return;
				
			// https://github.com/neon-age/Smart-Inspector/issues/5
			if (Application.isBatchMode)
				return;
			
			GetPatches();
			
			ApplyPatches(PatchBase.Apply.OnLoad);
			
			while (!GUISkinHasLoaded())
				await Task.Delay(1);
			
			ApplyPatches(PatchBase.Apply.OnGUI);
		}

		static bool GUISkinHasLoaded()
		{
			if (currentSkinField == null)
				currentSkinField = typeof(GUISkin).GetField("current", BindingFlags.Static | BindingFlags.NonPublic);
			
			var skin = (GUISkin)currentSkinField.GetValue(null);

			return skin != null && skin.name != "GameSkin";
		}

		internal static List<PatchBase> GetPatches()
		{
			if (harmony == null)
				harmony = new Harmony(HarmonyID);
			
			if (patches == null)
			{
				patches = new List<PatchBase>();

				foreach (var type in TypeCache.GetTypesDerivedFrom<PatchBase>())
				{
					if (type.IsAbstract)
						continue;

					var patch = (PatchBase)Activator.CreateInstance(type);
					patch.harmony = harmony;
					
					patches.Add(patch);
				}
			}
			return patches;
		}

		static void ApplyPatches(PatchBase.Apply apply)
		{
			foreach (var patch in patches)
				patch.ApplyPatches(apply);
		}
		

		internal static void UnpatchAll()
		{
			if (harmony == null) 
				return;
			
			foreach (var patch in patches)
				patch.UnpatchAll();
		}
	}
}