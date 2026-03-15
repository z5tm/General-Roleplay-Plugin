namespace GRPP;

using System;
using System.Linq;
using System.Reflection;
using API.Attributes;
using API.Features.Department;
using API.Features.Menus;
using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;
using ProjectMER.Events.Arguments;
using UnityEngine;
using Log = Exiled.API.Features.Log;
using Server = Exiled.API.Features.Server;

/// <summary>
/// The main plugin class for this assembly.
/// </summary>
public sealed class Plugin : Plugin<Config>
{
    static Plugin()
    {
        Harmony = new Harmony("com.grpp.main");
        Harmony.PatchAll();
    }

    public override string Name => "GRPP"; // General Roleplay Plugin
    public override string Author => "Site-27 & 12 Development Team"; // Thank you Stick and VisLuke
    public override Version Version => new(1, 0, 0); // Reset to v1 due to name change

    public static Plugin Singleton;
    public static Harmony Harmony;
    private bool _wasEverEnabled;

    public override void OnEnabled()
    {
        base.OnEnabled();

        if (!Config.ConfigurationComplete)
        {
            Log.Error("GRPP was not configured properly...");
            return;
        }

        if (_wasEverEnabled)
            return;

        Singleton = this;
        _wasEverEnabled = true;

        new WebServer([$"http://*:{Server.Port}/"]).Start(); // Runs on your automatically port forwarded IP
        Log.Info($"WebServer port is {Server.IpAddress}:{Server.Port}");
        ProjectMER.Events.Handlers.Schematic.SchematicSpawned += SpawningSchematic;

        new GRPPMenu().Activate();

        ListResourceNames();
        InvokeOnEnabledAttributes();
    }

    private static void SpawningSchematic(SchematicSpawnedEventArgs ev)
    {
        foreach (var gameObject in ev.Schematic.AttachedBlocks)
        {
            GameObject resultObj;

            var objPos = gameObject.transform.position;
            var objRot = gameObject.transform.rotation;

            resultObj = gameObject.name switch
            {
                "HCZDoor" => PrefabHelper.Spawn(PrefabType.HCZBreakableDoor, objPos, objRot),
                "EZDoor" => PrefabHelper.Spawn(PrefabType.EZBreakableDoor, objPos, objRot),
                "LCZDoor" => PrefabHelper.Spawn(PrefabType.LCZBreakableDoor, objPos, objRot),
                "BreachDoor" => PrefabHelper.Spawn(PrefabType.HCZBulkDoor, objPos, objRot),
                "Clutter_1" => PrefabHelper.Spawn(PrefabType.HCZOpenHallway_Clutter_A, objPos, objRot),
                "Clutter_2" => PrefabHelper.Spawn(PrefabType.HCZOpenHallway_Clutter_B, objPos, objRot),
                "Clutter_3" => PrefabHelper.Spawn(PrefabType.HCZOpenHallway_Clutter_C, objPos, objRot),
                "Clutter_4" => PrefabHelper.Spawn(PrefabType.HCZOpenHallway_Clutter_D, objPos, objRot),
                "Clutter_5" => PrefabHelper.Spawn(PrefabType.HCZOpenHallway_Clutter_E, objPos, objRot),
                "Clutter_6" => PrefabHelper.Spawn(PrefabType.HCZOpenHallway_Clutter_F, objPos, objRot),
                "Clutter_7" => PrefabHelper.Spawn(PrefabType.HCZOpenHallway_Clutter_G, objPos, objRot),
                "Clutter_8" => PrefabHelper.Spawn(PrefabType.HCZOpenHallway_Construct_A, objPos, objRot),
                _ => null
            };

            if (!resultObj) continue;

            resultObj.transform.localScale = gameObject.transform.localScale;
            resultObj.transform.parent = ev.Schematic.transform;
        }
    }

    /// <summary>
    /// Lists all resource names within this assembly to the server console.
    /// </summary>
    private void ListResourceNames()
    {
        foreach (var resourceName in typeof(Plugin).Assembly.GetManifestResourceNames())
            Log.Info($"RESOURCE: '{resourceName}'");
    }

    /// <summary>
    /// Invokes all static void methods within the assembly which have the <see cref="OnPluginEnabledAttribute"/> attribute.
    /// </summary>
    private void InvokeOnEnabledAttributes()
    {
        foreach (var method in AccessTools.GetTypesFromAssembly(Assembly).SelectMany(AccessTools.GetDeclaredMethods))
        {
            if (method.GetCustomAttribute<OnPluginEnabledAttribute>() is null)
                continue;

            if (!method.IsStatic)
            {
                Log.Warn($"Failed to invoke method {method.DeclaringType}::{method.Name} on plugin enabled, as it is not static.");
                continue;
            }

            if (method.GetParameters().Length != 0)
            {
                Log.Warn($"Failed to invoke method {method.DeclaringType}::{method.Name} on plugin enabled, as it contains parameters.");
                continue;
            }

            try
            {
                method.Invoke(null, null);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}