namespace GRPP.Extensions;

using System;
using System.Text.RegularExpressions;
using Exiled.API.Features;

public static class WebhookExtensions
{
    public/* async*/ static /*Task<*/bool/*>*/ IsItAWebhook(this string? arguedLink) // UGHHH i wanted to use my beautiful asyncccc
    {
        try
        {
            if (arguedLink == null)
                return false;
            var regex = new Regex("https://discord\\.com/api/webhooks/[0-9]+/");
            return regex.IsMatch(arguedLink);
        }
        catch (Exception e)
        {
            Log.Error($"Exception caught. {e.Message}");
            return false;
        }
    }
}