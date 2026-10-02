using System.Net.Http.Headers;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        // Anahtar ortam değişkeninden, bölge tanımlı değilse westeurope
        string subscriptionKey = ApiKeys.Require("AZURE_SPEECH_KEY");
        string region = ApiKeys.Optional("AZURE_SPEECH_REGION", "westeurope");

        // Token alacağım endpoint'i hazırlıyorum
        string tokenEndPoint = $"https://{region}.api.cognitive.microsoft.com/sts/v1.0/issuetoken";

        // Token isteği gönderiyorum ve token'ı alıyorum
        var token = await GetTokenAsync(subscriptionKey, tokenEndPoint);

        // Sese çevirmek istediğim metni hazırlıyorum
        string userText = "Merhaba, bu bir test mesajıdır. Güç sizinle olsun!.";

        // Metni sese çeviriyorum
        await SynthesizeSpeechAsync(token, region, userText);
    }

    static async Task<string> GetTokenAsync(string key, string endPoint)
    {
        // HTTP client ile token isteği gönderiyorum
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
        var response = await client.PostAsync(endPoint, null);

        // Dönen token'ı okuyorum
        return await response.Content.ReadAsStringAsync();
    }

    static async Task SynthesizeSpeechAsync(string token, string region, string text)
    {
        // HTTP client ile ses oluşturma isteği için başlıkları hazırlıyorum
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("User-Agent", "AzureTTSClient");
        client.DefaultRequestHeaders.Add("X-Microsoft-OutputFormat", "riff-16khz-16bit-mono-pcm");

        // SSML formatında metni hazırlıyorum
        string ssml = $@"
<speak version='1.0' xml:lang='en-US'>
  <voice xml:lang='tr-TR' name='tr-TR-AhmetNeural'>{text}</voice>
</speak>";

        var content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml");

        // Azure TTS servisine isteği gönderiyorum
        var result = await client.PostAsync($"https://{region}.tts.speech.microsoft.com/cognitiveservices/v1", content);

        if (result.IsSuccessStatusCode)
        {
            // Dönen ses verisini .wav dosyası olarak kaydediyorum
            var audioBytes = await result.Content.ReadAsByteArrayAsync();
            File.WriteAllBytes("output2.wav", audioBytes);
            Console.WriteLine("Ses dosyası başarıyla oluşturuldu: output2.wav");
        }
        else
        {
            // Hata oluşursa durumu yazdırıyorum
            Console.WriteLine("Hata oluştu: " + result.StatusCode);
            Console.WriteLine(await result.Content.ReadAsStringAsync());
        }
    }
}
