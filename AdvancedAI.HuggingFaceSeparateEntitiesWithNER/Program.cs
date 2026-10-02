using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{

    static async Task Main(string[] args)
    {
        // Hugging Face token'ı ortam değişkeninden geliyor (HF_TOKEN).
        string apiKey = ApiKeys.Require("HF_TOKEN");

        // Kullanıcıdan analiz edilecek metni alıyorum
        Console.Write("NER Analizi Yapılacak Metni Giriniz: ");
        string inputText = Console.ReadLine();

        // HttpClient'ı using ile açıyorum ki işim bitince otomatik dispose olsun
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        // API'ye göndereceğim JSON veriyi hazırlıyorum
        var requestBody = new { inputs = inputText };
        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            // Hugging Face NER modeline POST isteği atıyorum
            var response = await client.PostAsync("https://api-inference.huggingface.co/models/dslim/bert-base-NER", content);
            string responseString = await response.Content.ReadAsStringAsync();

            // Eğer API'den hata gelirse kullanıcıya bildiriyorum
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(" API hatası: " + responseString);
                return;
            }

            // JSON cevabı parse ediyorum
            using var doc = JsonDocument.Parse(responseString);

            Console.WriteLine("\n NER Analizi Sonuçları:");
            Console.WriteLine("----------------------------------------");

            // JSON dizisini döngüyle geziyorum ve her entity'i düzgün şekilde yazdırıyorum
            foreach (var item in doc.RootElement.EnumerateArray())
            {
                string entity = item.GetProperty("entity_group").GetString();
                string word = item.GetProperty("word").GetString();
                double score = Math.Round(item.GetProperty("score").GetDouble() * 100, 2);

                // Ben kullanıcıya net bir şekilde entity tipi ve güven skorunu gösteriyorum
                Console.WriteLine($"📝 Kelime: {word}");
                Console.WriteLine($"    |- Türü: {entity}");
                Console.WriteLine($"    |- Güven: %{score}");
                Console.WriteLine("----------------------------------------");
            }
        }
        catch (Exception ex)
        {
            // Eğer herhangi bir hata olursa kullanıcıya gösteriyorum
            Console.WriteLine(" Bir hata oluştu: " + ex.Message);
        }
    }
}
