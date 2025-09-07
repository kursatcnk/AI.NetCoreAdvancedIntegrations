using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        // Hugging Face API anahtarımı burada tanımlıyorum
        var apiKey = "";

        // Kullanıcıdan analiz etmek istediğim yorumu alıyorum
        Console.Write("Analiz edilecek yorumu giriniz: ");
        string inputText = Console.ReadLine();

        using (var client = new HttpClient())
        {
            // API anahtarımı Authorization başlığına ekliyorum
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            // API'ye göndereceğim veriyi JSON formatına çeviriyorum
            var requestBody = new { inputs = inputText };
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Hugging Face API'sine POST isteği yapıyorum
            var response = await client.PostAsync("https://api-inference.huggingface.co/models/unitary/toxic-bert", content);
            var responseString = await response.Content.ReadAsStringAsync();

            // Eğer hata mesajı dönerse kontrol ediyorum
            if (responseString.TrimStart().StartsWith("{"))
            {
                var errorDoc = JsonDocument.Parse(responseString);
                if (errorDoc.RootElement.TryGetProperty("error", out var errorMessage))
                {
                    Console.WriteLine("Model yükleniyor veya hata oluştu: " + errorMessage.GetString());
                    return;
                }
            }

            // Dönen cevabı JSON olarak parse ediyorum
            var doc = JsonDocument.Parse(responseString);

            Console.WriteLine("\n--- Yorum Analiz Sonucu ---\n");

            // API çıktısı -> [[ {label, score}, ... ]] olduğu için diziyi dönüyorum
            var predictions = doc.RootElement[0].EnumerateArray();
            foreach (var item in predictions)
            {
                // Modelin verdiği etiket ve güven skorunu alıyorum
                string label = item.GetProperty("label").GetString();
                double score = Math.Round(item.GetProperty("score").GetDouble() * 100, 2);

                // Sonuçları ekrana yazdırıyorum
                Console.WriteLine($"{label.ToUpper()} --> %{score}");
            }
        }
    }
}
