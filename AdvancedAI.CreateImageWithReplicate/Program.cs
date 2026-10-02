using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        //kullanıcıdan görsel için açıklama alıyorum
        Console.Write("Görsel oluşturmak için bir açıklama giriyorum: ");
        string prompt = Console.ReadLine();

        // Replicate API token'ını ortam değişkeninden alıyorum
        string apiToken = ApiKeys.Require("REPLICATE_API_TOKEN");
        string apiUrl = "https://api.replicate.com/v1/predictions";

        //  API'ye göndereceğim veriyi hazırlıyorum
        var requestBody = new
        {
            version = "7762fd07cf82c948538e41f63f77d685e02b063e37e496e96eefd46c929f9bdc",
            input = new { prompt = prompt }
        };

        //  veriyi JSON formatına çeviriyorum
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", apiToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        Console.WriteLine("\nBen görseli oluşturmak için isteği API'ye gönderiyorum, lütfen bekliyorum...\n");

        //  API isteğini gönderiyorum ve yanıtı alıyorum
        var response = await client.PostAsync(apiUrl, jsonContent);
        string responseContent = await response.Content.ReadAsStringAsync();

        //  API'den dönen yanıtı ekrana yazdırıyorum
        Console.WriteLine("Ben API'den gelen yanıtı alıyorum:");
        Console.WriteLine(responseContent);
    }
}
