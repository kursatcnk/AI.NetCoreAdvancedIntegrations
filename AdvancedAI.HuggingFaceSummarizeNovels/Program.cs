using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // Konsolda Türkçe karakterler düzgün gözüksün diye
        Console.OutputEncoding = Encoding.UTF8;

        // Kullanıcıdan özetlemek istediğim metni alıyorum
        Console.WriteLine("Özetlemek istediğiniz metni giriniz:");
        string inputText = Console.ReadLine();

        // Hugging Face token'ını ortam değişkeninden alıyorum
        string apiKey = ApiKeys.Require("HF_TOKEN");
        // Kullanacağım özetleme modelinin URL'si
        string modelUrl = "https://api-inference.huggingface.co/models/facebook/bart-large-cnn";

        using var client = new HttpClient();
        // API'ye yetkilendirme başlığı ekliyorum
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        // Göndereceğim JSON request'i hazırlıyorum
        var requestBody = new
        {
            inputs = inputText,
            parameters = new
            {
                min_length = 50, // minimum özet uzunluğu
                max_length = 200 // maksimum özet uzunluğu
            }
        };

        // JSON'a çeviriyorum
        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            // API'ye POST isteği gönderiyorum
            var response = await client.PostAsync(modelUrl, content);
            string result = await response.Content.ReadAsStringAsync();

            // Eğer API hatası varsa ekrana yazdırıyorum ve çıkıyorum
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("API hatası: " + result);
                return;
            }

            // API'den dönen JSON'u parse ediyorum
            using var doc = JsonDocument.Parse(result);
            // summary_text alanını alıyorum, işte bu benim özetim
            string summary = doc.RootElement[0].GetProperty("summary_text").GetString();

            // Özetlenen metni ekrana yazdırıyorum
            Console.WriteLine("\n📝 Özetlenen Metin:\n");
            Console.WriteLine(summary);
        }
        catch (Exception ex)
        {
            // Hata oluşursa kullanıcıya bildiriyorum
            Console.WriteLine("Hata oluştu: " + ex.Message);
        }
    }
}
