using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UglyToad.PdfPig;

class Program
{
    static async Task Main(string[] args)
    {
        // Kendi bilgisayarımdaki PDF dosyasının yolunu buraya yazıyorum
        string pdfPath = "BURAYA_PDF_YOLUNU_YAZ";

        // Anthropic Claude API anahtarını ortam değişkeninden alıyorum
        string apiKey = ApiKeys.Require("ANTHROPIC_API_KEY");

        // Eğer PDF dosyası yoksa programı bitiriyorum
        if (!File.Exists(pdfPath))
        {
            Console.WriteLine("PDF dosyası bulunamadı.");
            return;
        }

        // PDF içeriğini satır satır okuyup tek bir string içinde topluyorum
        string pdfText = "";
        using (var document = PdfDocument.Open(pdfPath))
        {
            foreach (var page in document.GetPages())
            {
                pdfText += page.Text + "\n";
            }
        }

        // Claude'a göndereceğim istemi (prompt) hazırlıyorum
        string prompt = $"Aşağıdaki PDF içeriğini detaylıca özetler misin?\n\n{pdfText}";

        // HTTP istemcisini ayarlıyorum
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://api.anthropic.com/");
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // API'ye göndereceğim JSON gövdesini oluşturuyorum
        var requestBody = new
        {
            model = "claude-3-opus-20240229",
            max_tokens = 1000,
            temperature = 0.5,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = prompt
                }
            }
        };

        // JSON içeriğini UTF8 formatında hazırlıyorum
        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        // API'ye isteği gönderiyorum
        var response = await client.PostAsync("v1/messages", jsonContent);
        var responseString = await response.Content.ReadAsStringAsync();

        // Claude'dan gelen yanıtı ekrana yazdırıyorum
        Console.WriteLine("Claude PDF Özeti:");
        Console.WriteLine(responseString);
    }
}
