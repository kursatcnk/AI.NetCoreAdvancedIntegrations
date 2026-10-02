using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        // Google Gemini API anahtarını ortam değişkeninden alıyorum
        string apiKey = ApiKeys.Require("GEMINI_API_KEY");
        string model = "gemini-1.5-pro";
        string endpoint = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={apiKey}";

        // Kullanıcıdan bir soru alıyorum
        Console.Write("Yanıt almak istediğiniz soruyu yazın: ");
        string question = Console.ReadLine();

        // API'ye göndereceğim isteği hazırlıyorum
        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = question }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // İsteği gönderiyor ve yanıtı alıyorum
        var response = await client.PostAsync(endpoint, content);
        var responseText = await response.Content.ReadAsStringAsync();

        try
        {
            // API yanıtını parse edip cevabı ekrana yazdırıyorum
            var doc = JsonDocument.Parse(responseText);
            string answer = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            Console.WriteLine("Gemini API Yanıtı:");
            Console.WriteLine(answer);
        }
        catch (Exception ex)
        {
            // Yanıt parse edilemezse hata mesajı gösteriyorum
            Console.WriteLine("Yanıt çözümlenemedi.");
            Console.WriteLine("Gelen Yanıt: " + responseText);
            Console.WriteLine("Hata Detayı: " + ex.Message);
        }
    }
}
