using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        // API anahtarımı ve modeli ayarlıyorum
        string apiKey = ""; // 
        string model = "gemini-1.5-pro";
        string endpoint = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={apiKey}";

        // Yapay zekaya vereceğim görevleri ve bağlamı tanımlıyorum
        string context = @"Sen bir içerik stratejisi ve planlama uzmanısın.
Kullanıcının iş fikrini veya projesini alacak, detaylı analiz yapacak ve
adım adım sorular sorarak onu doğru şekilde yönlendireceksin.
Amacın, kullanıcıya sonunda kapsamlı ve uygulanabilir bir içerik planı sunmak.";

        // Kullanıcıdan proje fikri alıyorum
        Console.Write("Proje veya fikirinizi kısaca yazın: ");
        string userInput = Console.ReadLine();

        // Başlangıç promptunu hazırlıyorum
        string userPrompt = $"{context}\n\nKullanıcının verdiği fikir: {userInput}\nŞimdi ona aşama aşama sorular sorarak detaylandır.";

        // Kullanıcıyla 5 tur boyunca etkileşim kuruyorum
        for (int i = 0; i < 5; i++)
        {
            string question = await SendToGemini(apiKey, endpoint, userPrompt);
            Console.WriteLine($"\nYapay Zeka: {question}");

            Console.Write("Cevabınız: ");
            string answer = Console.ReadLine();

            userPrompt += $"\n\nKullanıcının cevabı: {answer}\nYeni soruyu sor.";
        }

        // Tüm bilgiler toplandıktan sonra içerik planını oluşturuyorum
        string finalPrompt = $"{userPrompt}\n\nArtık yeterli bilgiye sahipsin. Kullanıcı için detaylı ve uygulanabilir bir içerik planı oluştur.";
        string finalOutput = await SendToGemini(apiKey, endpoint, finalPrompt);

        Console.WriteLine("\n--- Nihai İçerik Planı ---\n");
        Console.WriteLine(finalOutput);
    }

    static async Task<string> SendToGemini(string apiKey, string endpoint, string prompt)
    {
        // Promptu JSON formatına çeviriyorum
        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.PostAsync(endpoint, content);
        var responseText = await response.Content.ReadAsStringAsync();

        try
        {
            var doc = JsonDocument.Parse(responseText);
            return doc.RootElement
                      .GetProperty("candidates")[0]
                      .GetProperty("content")
                      .GetProperty("parts")[0]
                      .GetProperty("text")
                      .GetString();
        }
        catch
        {
            return "Bir hata oluştu!";
        }
    }
}
