using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        // Anthropic API anahtarını ortam değişkeninden alıyorum
        string apiKey = ApiKeys.Require("ANTHROPIC_API_KEY");

        // Kullanıcıdan soru alıyorum
        Console.Write("Sorunuzu Giriniz: ");
        string prompt = Console.ReadLine();

        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://api.anthropic.com");
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // API'ye göndereceğim istek gövdesini hazırlıyorum
        var requestBody = new
        {
            model = "claude-3-opus-20240229", // Kullanılacak model
            max_tokens = 1000,                // Döndürülecek maksimum token sayısı token arttıkça döndüğü içerik artar
            temperature = 0.7,                // Yaratıcılık ayarı
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        // JSON içeriğini oluşturuyorum
        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        // API'ye isteği gönderiyorum
        var response = await client.PostAsync("v1/messages", jsonContent);
        var responseString = await response.Content.ReadAsStringAsync();

        // Dönen cevabı parse ediyorum
        var doc = JsonDocument.Parse(responseString);
        var contentElement = doc.RootElement.GetProperty("content")[0];
        var text = contentElement.GetProperty("text").GetString();

        // Sonucu ekrana yazdırıyorum
        Console.WriteLine("\n--- CLAUDE YANITI ---\n");
        Console.WriteLine(text);
        Console.WriteLine("\n--- Cevap tamamlandı. ---\n");
    }
}
