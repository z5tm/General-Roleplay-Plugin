namespace GRPP.Extensions;

using System;
using System.Text.RegularExpressions;
using Exiled.API.Features;

public static class AudioExtensions
{
    public/* async*/ static /*Task<*/bool/*>*/ IsItOgg(this string? arguedFile) // UGHHH i wanted to use my beautiful asyncccc
    {
        try
        {
            if (arguedFile == null)
                return false;
            var regex = new Regex("\\.ogg$");
            return regex.IsMatch(arguedFile);
        }
        catch (Exception e)
        {
            Log.Error($"Exception caught. {e.Message}");
            return false;
        }
    }
}