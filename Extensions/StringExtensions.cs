namespace GRPP.Extensions;

using System;
using UnityEngine;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using Exiled.API.Features;

public static class StringExtensions
{
    extension(string inputString)
    {
        [Pure]
        public bool GetCustomKeycardLevel(out int? parsedGrppKeycardLevel, out int? cKeycardLevel, int minKeycardLevel = 0, int maxKeycardLevel = 5)
        {
            if (!int.TryParse(inputString, out var grppLevel))
            {
                cKeycardLevel = null;
                parsedGrppKeycardLevel = null;
                return false;
            }
            
            parsedGrppKeycardLevel = Mathf.Clamp(grppLevel, minKeycardLevel, maxKeycardLevel);
            
            cKeycardLevel = parsedGrppKeycardLevel switch
            {
                >= 5 => 3,
                4 or 3 => 2,
                2 or 1 => 1,
                _ => 0
            };
            return true;
        }

        /// <summary>
        /// Replaces underscores with spaces.<br/>
        /// Does not replace underscores prefaced with "\".
        /// </summary>
        public string HandleTightInput() => inputString.RegexReplace(@"(?<!\\)_", " ");
        public string RegexReplace(string pattern, string replacement) => Regex.Replace(inputString, pattern, replacement);
        public string CutStringToValue(int cutLength)
        {
            if (!string.IsNullOrEmpty(inputString)) return inputString.Length <= cutLength ? inputString : inputString.AsSpan(0, cutLength).ToString();
            Log.Error("Error when attempting to cut string. String was null or empty.");
            return string.Empty; // if string is null or empty, return nothing.
        }
    }
}