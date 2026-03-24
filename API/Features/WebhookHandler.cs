namespace GRPP.API.Features;

using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

public class WebhookHandler
{
    // // int threads = Environment.ProcessorCount; // just in case we ever need to modify this to optimize it for better-suited systems in the future, i'll keep this here - as i access this file often.
    // public string WebhookUrl { get; set; }= "unset";
    // public static string WebhookNameToUse { get; set; }
    // public static bool WebhookIsRunning { get; set; } = false;
    //
    // wait
    public void UseWebhook(string webhookNameToUse, string webhookUrl, string arg3, string description, string title,
        string color, bool inline, bool timestamps)
    {
        if (timestamps)
        {
            var embed = new
            {
                username = webhookNameToUse,
                embeds = new[]
                {
                    new
                    {
                        title = title,
                        description = description,
                        color = int.Parse(color),
                        fields = new[]
                        {
                            new { name = $"{arg3}", value = $"{arg3}", inline = inline},
                        },
                        timestamp = DateTime.UtcNow.ToString("O")
                    }
                },
            };

            PostWebhook(webhookUrl, embed);
        }
        else
        {
            var embed = new
            {
                username = webhookNameToUse,
                embeds = new[]
                {
                    new
                    {
                        title = title,
                        description = description,
                        color = int.Parse(color),
                        fields = new[]
                        {
                            new { name = $"{arg3}", value = $"{arg3}", inline = inline},
                        },
                    }
                },
            };
            PostWebhook(webhookUrl, embed);
        }
    }

    private void PostWebhook(string url, object payload)
    {
        var jsonPayload = JsonConvert.SerializeObject(payload);

        var request = WebRequest.Create(url);
        request.ContentType = "application/json";
        request.Method = "POST";

        using (var writer = new StreamWriter(request.GetRequestStream()))
        {
            writer.Write(jsonPayload);
        }

        var response = request.GetResponse();

        using (var reader = new StreamReader(response.GetResponseStream() ?? Stream.Null))
        {
            var responseText = reader.ReadToEnd();
            Console.WriteLine($"Webhook Response: {responseText}");
        }
    }
}