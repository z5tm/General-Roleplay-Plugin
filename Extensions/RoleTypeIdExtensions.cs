namespace GRPP.Extensions;

using System;
using System.Collections.Generic;
using PlayerRoles;

public static class RoleTypeIdExtensions
{
    private static readonly HashSet<string> RoleTypeNames = [..Enum.GetNames(typeof(RoleTypeId))];
    
    extension(RoleTypeId)
    {
        public static HashSet<string> All() => RoleTypeNames;
    }
}