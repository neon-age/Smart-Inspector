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
		private static readonly string HarmonyID = typeof(Patcher).Assembly.GetName().Name;

		private static Harmony harmony;
		private static List<PatchBase> patches;
		
		private static FieldInfo currentSkinField;

		[InitializeOnLoadMethod]
		//[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static async void Init()
		{
			GetPatches();
			
			ApplyPatches(PatchBase.Apply.OnLoad);
			
			while (!GUISkinHasLoaded())
				await Task.Delay(1);
			
			ApplyPatches(PatchBase.Apply.OnGUI);
		}

		internal static bool GUISkinHasLoaded()
		{
			if (currentSkinField == null)
				currentSkinField = typeof(GUISkin).GetField("current", BindingFlags.Static | BindingFlags.NonPublic);
			
			var skin = (GUISkin)currentSkinField.GetValue(null);

			return skin != null && skin.name != "GameSkin";
		}

		private static void GetPatches()
		{
			if (patches != null) 
				return;
			patches = new List<PatchBase>();
				
			foreach (var type in TypeCache.GetTypesDerivedFrom(typeof(PatchBase)))
			{
				if (type.IsAbstract) 
					continue;
					
				var instance = Activator.CreateInstance(type) as PatchBase;
				patches.Add(instance);
			}
		}

		private static void ApplyPatches(PatchBase.Apply apply)
		{
			if (harmony == null)
				harmony = new Harmony(HarmonyID);
			
			foreach (var patch in patches)
				patch.ApplyPatches(harmony, apply);
		}
		

		internal static void RemovePatches()
		{
			if (harmony == null) 
				return;
			
			foreach (var patch in patches)
				patch.UnpatchAll(harmony);
		}
	}
}