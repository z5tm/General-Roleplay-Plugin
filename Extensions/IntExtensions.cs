namespace GRPP.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class IntExtensions
{
    extension(IEnumerable<int> integers)
    {
        public int GetHighestIntegerPositive() => integers.Prepend(0).Max();
        public int GetHighestInteger() => integers?.Max() ?? throw new ArgumentNullException(nameof(integers));
    }
    
    extension(ref int grppKeycardLevel)
    {
        public bool GetCustomKeycardLevel(out int? cKeycardLevel, int minKeycardLevel = 0, int maxKeycardLevel = 5)
        {
            grppKeycardLevel = Mathf.Clamp(grppKeycardLevel, minKeycardLevel, maxKeycardLevel);
            
            cKeycardLevel = grppKeycardLevel switch
            {
                >= 5 => 3,
                4 or 3 => 2,
                2 or 1 => 1,
                _ => 0
            };
            return true;
        }
    }
}