namespace GRPP.API.Features;

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Newtonsoft.Json;

public class WebhookHandler
{
    // // int threads = Environment.ProcessorCount; // just in case we ever need to modify this to optimize it for better-suited systems in the future, i'll keep this here - as i access this file often.
    private static readonly HttpClient _client  = new HttpClient();
    
    public async Task UseWebhook(string webhookNameToUse, string webhookUrl, string arg3, string arg4, string description, string title,
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
                            new { name = arg3, value = arg4, inline = inline},
                        },
                        timestamp = DateTime.UtcNow.ToString("O")
                    }
                },
            };
            await PostWebhook(webhookUrl, embed);
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
            await PostWebhook(webhookUrl, embed);
        }
    }

    private static async Task PostWebhook(string url, object payload)
    {
        var jsonPayload = JsonConvert.SerializeObject(payload);

        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        try
        {
            var response = await _client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                Log.Debug(("WOOO SUCCESS ON WEBHOOK!!!"));
            }
            else
            {
                Log.Debug($"Failed. {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
            }
        }
        catch (Exception exception)
        {
            Log.Debug($"Exception. {exception}");
        }
        // var request = WebRequest.CreateHttp(url);
        // request.ContentType = "application/json";
        // request.Method = "POST";
        //
        // using (var writer = new StreamWriter(request.GetRequestStream()))
        // {
        //     writer.Write(jsonPayload);
        // }
        //
        // var response = request.GetResponse();
        //
        // using (var reader = new StreamReader(response.GetResponseStream() ?? Stream.Null))
        // {
        //     var responseText = reader.ReadToEnd();
        //     Console.WriteLine($"Webhook Response: {responseText}");
        // }
    }
}