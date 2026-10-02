using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

class Program
{
    static async Task Main(string[] args)
    {
        // Anthropic Claude API anahtarını ortam değişkeninden alıyorum
        string apiKey = ApiKeys.Require("ANTHROPIC_API_KEY");

        // Prompt mühendisliği ile hazırlanmış istek:
        // Claude’a rol veriyorum, bağlam sunuyorum ve çıktının formatını belirtiyorum.
        string prompt =
        @"Sen profesyonel bir kariyer danışmanısın. 
        Görevin: Yazılım Geliştirici (Full-Stack Developer) pozisyonu için bir iş başvuru e-postası hazırlamak. 
        Gereksinimler:
        - Profesyonel, güven veren ama samimi bir dil kullan.
        - Benim hakkımda: Adım Kürşat, 3 yıldır React ve .NET Core teknolojileriyle full-stack developer olarak çalışıyorum.
        - Özelliklerim: problem çözme becerim güçlü, zamanında proje teslim etme alışkanlığım var, hibrit çalışmaya uygunum.
        - Çıktıyı, standart iş başvuru e-postası formatında ver (konu, selamlama, gövde, kapanış).";

        // HttpClient ile API'ye bağlanıyorum
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://api.anthropic.com/");
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // API'ye gönderilecek JSON gövdesi
        var requestBody = new
        {
            model = "claude-3-opus-20240229",  // Claude modeli
            max_tokens = 1000,                // Çıktı uzunluğu sınırı
            temperature = 0.5,                // Yaratıcılık seviyesi
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = prompt
                }
            }
        };

        // JSON’u UTF8 formatında paketleyip gönderiyorum
        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        // API çağrısını yapıyorum
        var response = await client.PostAsync("v1/messages", jsonContent);
        var responseString = await response.Content.ReadAsStringAsync();

        // Dönen JSON yanıtını parse ediyorum
        var json = JsonNode.Parse(responseString);
        string? textContent = json?["content"]?[0]?["text"]?.ToString();

        // E-postayı ekrana yazdırıyorum
        Console.WriteLine("Oluşturulan İş Başvuru E-Postası:\n");
        Console.WriteLine(textContent);
    }
}
