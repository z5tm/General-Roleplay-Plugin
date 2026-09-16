namespace GRPP.API.Attributes;

using System;

/// <summary>
/// Attribute that specifies a method to call when the plugin is first enabled.
/// </summary>
/// <remarks>
/// The method must be static and with no parameters to be called.
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public sealed class OnPluginEnabledAttribute : Attribute;

[AttributeUsage(AttributeTargets.Method)]
public sealed class OnPluginDisabledAttribute : Attribute;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class RegionAttribute(string regionName) : Attribute
{
    public string RegionName { get; private set; } = regionName;
}