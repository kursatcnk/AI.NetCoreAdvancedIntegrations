using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        // Hugging Face token'ı (HF_TOKEN)
        string apiKey = ApiKeys.Require("HF_TOKEN");

        // Kullanıcıdan metin alınır
        Console.WriteLine("Analiz edilecek metni giriniz:");
        string context = Console.ReadLine();

        // Kullanıcıdan soru alınır
        Console.WriteLine("\nMetinle ilgili soruyu giriniz:");
        string question = Console.ReadLine();

        // HTTP istemcisi oluşturulur ve yetkilendirme eklenir
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        // API'ye gönderilecek istek gövdesi hazırlanır
        var requestBody = new
        {
            inputs = new
            {
                question = question,
                context = context
            }
        };

        string json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        // API'ye POST isteği gönderilir
        var response = await client.PostAsync("https://api-inference.huggingface.co/models/deepset/roberta-base-squad2", content);
        string responseString = await response.Content.ReadAsStringAsync();

        try
        {
            // API yanıtı JSON formatında çözümlenir
            using var doc = JsonDocument.Parse(responseString);

            if (doc.RootElement.TryGetProperty("answer", out var answer))
            {
                // Yanıt bulunduysa ekrana yazdırılır
                Console.WriteLine("\nSoru: " + question);
                Console.WriteLine("Metin: " + context);
                Console.WriteLine("Cevap: " + answer.GetString());
            }
            else
            {
                // Yanıt bulunamazsa bilgilendirme yapılır
                Console.WriteLine("Cevap bulunamadı veya model hazır değil.");
            }
        }
        catch (JsonException)
        {
            // JSON çözümlenemediğinde hata mesajı gösterilir
            Console.WriteLine("API cevabı işlenemedi. Ham çıktı: " + responseString);
        }
    }
}
