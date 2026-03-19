// General Roleplay Plugin
// Copyright (C) 2025 Site-12, VisLuke, SticksDeveloper
// Copyright (C) 2026 Site-27, z5tm
//
// This file is part of General Roleplay Plugin.
//
// General Roleplay Plugin is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// General Roleplay Plugin is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with Foobar. If not, see <https://www.gnu.org/licenses/>.
//

namespace GRPP;

using System;
using System.Collections.Generic;
using System.IO;
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
using System.Reflection.Emit;
using CustomPlayerEffects;
using Exiled.Loader;

/// <summary>
/// The main plugin class for this assembly.
/// </summary>
public sealed class Plugin : Plugin<Config>
{
    static Plugin()
    {
        Harmony = new Harmony("com.grpp.main");
        Harmony.PatchAll();
        /*AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => // subscribe to AssemblyResolve get notified when shit fails, args contains name that tried to load but failed - sender due to convention - we also use a lambda operator, which is cool - basically tells the code that this is the function to call after this event fires, and here - take these two args :sunglasses:
        {
            if (args.Name.StartsWith("System.ComponentModel.DataAnnotations")) // if the args provided by assemblyresolve's broken shi ~= dataannotations, continue - we use startswith because the actual output has too much shit going on 
            {
                var name = new AssemblyName("System.ComponentModel.DataAnnotations") // easier on the eyes. we could totally just put it right into the definedynamicassembly
                {
                    Version = new Version(4, 0, 0, 0) // set that version to 4.0.0.0
                };
                return AppDomain.CurrentDomain.DefineDynamicAssembly( // Create a empty assembly, in memory (hence AssemblyBuilderAccess.Run), with said identity, faking having the DLL :sunglasses:
                    name,
                    System.Reflection.Emit.AssemblyBuilderAccess.Run // this allows us to create assemblies at runtime, in memory, to disk, etc! RunAndSave = run + save to disk, Save = make, write to file
                );
            }
            return null; // If we can't handle it, we say so - null basically tells AssemblyResolve that we do not know how to handle this error and to move on. This also applies to non-DataAnnotations breaks.
        };*/// I did NOT just spend like 5 hours on this just for it to be an upstream issue. FUCKKKKKKKK
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

    // public override void OnDisabled()
    // {
    //     base.OnDisabled();
    //     // if (!Config.ConfigurationComplete)
    //     // {
    //     //     Log.Error(
    //     //         "GRPP has either not been configured or encountered an error during loading the configuration. This WILL cause issues, due to the early state of the plugin. For safety, the plugin has been disabled.");
    //     //     return;
    //     // }
    // }
    // need to find out how to make that thing above stop the webserver - a bit out of me but i shall figure it out soon
    
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