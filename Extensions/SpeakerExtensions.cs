namespace GRPP.Extensions;

using System.Collections.Generic;

public static class SpeakerExtensions
{
    public static byte GetFreeId()
    {
        // Find a free ControllerId
        var usedIds = new HashSet<byte>();
        foreach (var obj in AudioPlayer.AudioPlayerByName)
            usedIds.Add(obj.Value.ControllerID);

        byte freeId = 0;
        for (byte i = 0; i < byte.MaxValue; i++)
        {
            if (usedIds.Contains(i)) continue;
            freeId = i;
            break;
        }

        return freeId;
    }
}