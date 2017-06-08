#r "System.Web"
#r "Newtonsoft.json"

using System.Net;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Collections.Generic;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"C# HTTP trigger function processed a request. RequestUri={req.RequestUri}");

    byte[] data = await req.Content.ReadAsByteArrayAsync();

    var client = new HttpClient();
    var appSettings = ConfigurationManager.AppSettings;

    client.DefaultRequestHeaders.Add(appSettings["accessKeyName"], appSettings["OcrKeyValue"]);

    var uri = appSettings["OcrURL"];
    log.Info($"calling: {uri}");

    HttpResponseMessage response;

    using (var content = new ByteArrayContent(data))
    {
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        response = client.PostAsync(uri, content).Result;

        string imageInfo = response.Content.ReadAsStringAsync().Result;

        string bingSearchJson = GetBingSearchJson(imageInfo);

        return req.CreateResponse(HttpStatusCode.OK, bingSearchJson);
    }

    return req.CreateResponse(HttpStatusCode.OK, "default");
}

private static string GetBingSearchJson(string res)
{
    string formatted = res.Replace("\"", "'");

    JObject scanned = JObject.Parse(formatted);

    string[] sentences = new string[scanned["regions"][0]["lines"].Count()];

    for (int lineCounter = 0; lineCounter < scanned["regions"][0]["lines"].Count(); lineCounter++)
    {
        string sentence = "";

        for (int wordCounter = 0; wordCounter < scanned["regions"][0]["lines"][lineCounter]["words"].Count(); wordCounter++)
        {
            string word = scanned["regions"][0]["lines"][lineCounter]["words"][wordCounter]["text"].ToString();

            sentence += word + " ";
        }

        sentences[lineCounter] = sentence.TrimEnd(' ');
    }

    MenuItems menuItems = new MenuItems();

    menuItems.menus = new List<Menu>();

    foreach (string line in sentences)
    {
        Menu menuItem = new Menu();

        menuItem.name = line;
        menuItem.contentURL = getImageUrl(line);

        menuItems.menus.Add(menuItem);
    }

    JObject obj = JObject.FromObject(menuItems);

    string response = obj.ToString();

    return response;
}

private static string getImageUrl(string query)
{
    var client = new HttpClient();
    var appSettings = ConfigurationManager.AppSettings;

    client.DefaultRequestHeaders.Add(appSettings["accessKeyName"], appSettings["imageSearchKeyValue"]);

    var uri = appSettings["imageSearchURL"] + query + "&count=1";

    /*Task<string> response = client.GetStringAsync(uri);

    string result = response.Result;

    string part1 = result.Substring(result.IndexOf($"\"contentUrl\": \"") + $"\"contentUrl\": \"".Length);
    string part2 = part1.Substring(0, part1.IndexOf($"\", \"")).Replace($"\\","");
    */

    WebClient wc = new WebClient();

    wc.Headers.Add(appSettings["accessKeyName"], appSettings["imageSearchKeyValue"]);

    string result = wc.UploadString(uri, "");

    string part1 = result.Substring(result.IndexOf($"\"contentUrl\": \"") + $"\"contentUrl\": \"".Length);
    string part2 = part1.Substring(0, part1.IndexOf($"\", \"")).Replace($"\\", "");

    return part2;
}
private class Menu
{
    public string name { get; set; }
    public string contentURL { get; set; }
}

private class MenuItems
{
    public List<Menu> menus { get; set; }
}