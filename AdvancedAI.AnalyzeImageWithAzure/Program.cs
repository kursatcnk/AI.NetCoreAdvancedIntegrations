using System.Net.Http.Headers;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        // Aanaliz etmek istediğim görselin yolunu belirliyorum
        string imagePath = ""; // Görsel dosya yolu buraya gelecek
        string subscriptionKey = ""; // Azure Vision API abonelik anahtarım buraya gelecek
        string endpoint = ""; // Endpoint adresim

        string apiUrl = $"{endpoint}/vision/v3.2/analyze";
        string requestParameters = "visualFeatures=Categories,Description,Tags,Color&language=en";
        string uri = apiUrl + "?" + requestParameters;

        // Görselin mevcut olup olmadığını kontrol ediyorum
        if (!File.Exists(imagePath))
        {
            Console.WriteLine("Görsel dosyası bulunamadı: " + imagePath);
            return;
        }

        // Görseli byte dizisine çeviriyorum
        byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);

        using (HttpClient client = new HttpClient())
        using (ByteArrayContent content = new ByteArrayContent(imageBytes))
        {
            // API key ile kimlik doğrulaması yapıyorum
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            // API'ye POST isteği gönderiyorum
            HttpResponseMessage response = await client.PostAsync(uri, content);
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // Dönen yanıtı parse ediyorum
                JsonDocument json = JsonDocument.Parse(result);
                var descriptionElement = json.RootElement.GetProperty("description").GetProperty("captions")[0];
                string descriptionText = descriptionElement.GetProperty("text").GetString();
                double confidenceScore = descriptionElement.GetProperty("confidence").GetDouble();

                // Yanıtı daha anlaşılır bir şekilde ekrana yazıyorum
                Console.WriteLine("Görsel Analiz Sonucu:");
                Console.WriteLine($" - Açıklama: {descriptionText}");
                Console.WriteLine($" - Güven Skoru: {Math.Round(confidenceScore, 2)}");
            }
            else
            {
                // Hata durumunu bildiriyorum
                Console.WriteLine("İstek sırasında bir hata oluştu!");
                Console.WriteLine($"HTTP Durum Kodu: {response.StatusCode}");
                Console.WriteLine("Yanıt: " + result);
            }
        }
    }
}
