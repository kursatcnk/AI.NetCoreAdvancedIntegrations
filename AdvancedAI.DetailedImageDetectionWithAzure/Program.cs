using System.Net.Http.Headers;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        // Analiz etmek istediğim görselin yolunu belirtiyorum
        string imagePath = "";
        // Azure Computer Vision endpoint adresimi buraya yazıyorum
        string endpoint = "";
        // Azure abonelik anahtarımı buraya ekliyorum
        string subscriptionKey = "";

        // API isteği için URL ve parametreleri hazırlıyorum
        string apiUrl = $"{endpoint}/vision/v3.2/analyze";
        string requestParameters = "visualFeatures=Categories,Description,Tags,Color,Faces,Objects,Brands,Adult,ImageType&language=en&model-version=latest";
        string uri = apiUrl + "?" + requestParameters;

        // Görsel dosyasının mevcut olup olmadığını kontrol ediyorum
        if (!File.Exists(imagePath))
        {
            Console.WriteLine("Belirtilen görsel bulunamadı: " + imagePath);
            return;
        }

        // Görseli byte dizisine dönüştürüyorum
        byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);

        using (HttpClient client = new HttpClient())
        using (ByteArrayContent content = new ByteArrayContent(imageBytes))
        {
            // API kimlik doğrulamasını ve içerik tipini ayarlıyorum
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            // API'ye isteği gönderiyor ve yanıtı alıyorum
            HttpResponseMessage response = await client.PostAsync(uri, content);
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // Yanıtı parse ederek nesneleri ekrana yazdırıyorum
                JsonDocument json = JsonDocument.Parse(result);
                Console.WriteLine("Görsel analizi tamamlandı. Tespit edilen nesneler:");

                var objects = json.RootElement.GetProperty("objects");
                foreach (var obj in objects.EnumerateArray())
                {
                    string name = obj.GetProperty("object").GetString();
                    double confidence = obj.GetProperty("confidence").GetDouble();
                    Console.WriteLine($"- Nesne: {name} (Tahmini güven seviyesi: {confidence:P1})");
                }
            }
            else
            {
                Console.WriteLine("İstek sırasında bir hata oluştu!");
                Console.WriteLine($"Durum Kodu: {response.StatusCode}");
                Console.WriteLine("Yanıt: " + result);
            }
        }
    }
}
