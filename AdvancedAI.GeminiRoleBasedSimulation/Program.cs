using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        // Google Gemini API için kendi anahtarımı ekliyorum
        string apiKey = "";
        string model = "gemini-1.5-pro";
        string endpoint = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={apiKey}";

        // Kullanıcıya sunacağım uzmanlık rollerini seçiyorum örnek amaçlı ben 5 tane girdim ama buraya daha detaylı şekilde roller ekleyebilirim yada api üzerinden rolü de dinamik çekebilirim. Test amaçlı statik girdim.
        Console.WriteLine("Rolünüzü seçin:");
        Console.WriteLine("1- Lider Koç");
        Console.WriteLine("2- Sağlık ve Beslenme Uzmanı");
        Console.WriteLine("3- Kripto ve Finans Danışmanı");
        Console.WriteLine("4- Bilimsel Araştırmacı");
        Console.WriteLine("5- Gezi ve Kültür Danışmanı");
        Console.WriteLine();
        Console.Write("Seçiminiz: ");
        string roleChoice = Console.ReadLine();

        // Rol seçimine göre promptlar oluşturuyorum
        string rolePrompt = roleChoice switch
        {
            "1" => "Sen liderlik koçusun. Kullanıcıya iş ve liderlik stratejileri konusunda kapsamlı, uygulanabilir ve motive edici öneriler sun.",
            "2" => "Sen sağlık ve beslenme uzmanısın. Kullanıcının yaşam tarzına uygun bilimsel temelli, açık ve uygulanabilir öneriler ver.",
            "3" => "Sen kripto ve finans danışmanısın. Kullanıcının hedeflerine uygun risk analizi yaparak net ve stratejik yatırım tavsiyeleri sun.",
            "4" => "Sen bir bilimsel araştırmacısın. Sorulan soruya kanıta dayalı, detaylı ve tarafsız bir şekilde yanıt ver.",
            "5" => "Sen bir gezi ve kültür danışmanısın. Sorulan şehri çok iyi bilen biri olarak görülmesi gereken yerleri, deneyimlenmesi gereken aktiviteleri ve yöresel lezzetleri detaylı listele."
        };

        Console.WriteLine();

        // Kullanıcıdan soruyu alıyorum ve prompt ile birleştiriyorum
        Console.Write("Sormak istediğiniz soruyu yazın: ");
        string userInput = Console.ReadLine();
        string finalPrompt = $"{rolePrompt}\n\nKullanıcıdan Gelen Soru: {userInput}";

        // API isteği için gerekli JSON gövdesini hazırlıyorum
        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = finalPrompt }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // API isteğini gönderiyor ve yanıtı alıyorum
        var response = await client.PostAsync(endpoint, content);
        var responseText = await response.Content.ReadAsStringAsync();

        try
        {
            // Yanıtı parse edip kullanıcıya gösteriyorum
            var doc = JsonDocument.Parse(responseText);
            string answer = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            Console.WriteLine("\n--- Gemini API Yanıtı ---\n");
            Console.WriteLine(answer);
        }
        catch
        {
            Console.WriteLine("Yanıt çözümlenemedi:");
            Console.WriteLine(responseText);
        }
    }
}
