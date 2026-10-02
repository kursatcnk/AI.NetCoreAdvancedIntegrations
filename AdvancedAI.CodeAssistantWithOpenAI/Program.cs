using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("OpenAI Kod Asistanına Hoş Geldiniz\n");

        // Önce kullanıcı kodunu alıyorum
        Console.WriteLine("Kodunuzu girin (bitirmek için 'END' yazın): ");
        StringBuilder userCode = new();

        string? line;
        while ((line = Console.ReadLine()) != null && line.Trim() != "END")
        {
            userCode.AppendLine(line);
        }

        // Kullanıcının ne yapmak istediğini soruyorum
        Console.WriteLine("\nHangi işlemi yapmak istiyorsunuz?");
        Console.WriteLine("1- Kod Açıklaması Üret");
        Console.WriteLine("2- Kod Refactor Et");
        Console.WriteLine("3- Unit Test Case Oluştur");
        Console.Write("\nSeçiminiz (1/2/3): ");
        var choice = Console.ReadLine();

        // Seçime göre promptu hazırlıyorum
        string prompt = choice switch
        {
            "1" => $"Lütfen aşağıdaki C# kodunu detaylı ve anlaşılır şekilde açıklayın:\n\n{userCode}",
            "2" => $"Lütfen aşağıdaki C# kodunu daha temiz, okunabilir ve profesyonel bir şekilde refactor edin:\n\n{userCode}",
            "3" => $"Lütfen aşağıdaki C# kodu için kapsamlı ve doğru Unit Test case'leri oluşturun:\n\n{userCode}"
        };

        string result = await AskOpenAI(prompt);

        Console.WriteLine("\nOpenAI Yanıtı:\n");
        Console.WriteLine(result);
    }

    static async Task<string> AskOpenAI(string prompt)
    {
        string apiKey = ApiKeys.Require("OPENAI_API_KEY");
        const string endpoint = "https://api.openai.com/v1/chat/completions";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var requestBody = new
        {
            model = "gpt-4",
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = "Sen uzman bir C# yazılım geliştiricisisin. Kodları açıkla, refactor et veya unit test üret."
                },
                new
                {
                    role = "user",
                    content = prompt
                }
            },
            temperature = 0.7
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(endpoint, content);
        var responseJson = await response.Content.ReadAsStringAsync();

        var doc = JsonDocument.Parse(responseJson);
        return doc.RootElement
                  .GetProperty("choices")[0]
                  .GetProperty("message")
                  .GetProperty("content")
                  .ToString();
    }
}
