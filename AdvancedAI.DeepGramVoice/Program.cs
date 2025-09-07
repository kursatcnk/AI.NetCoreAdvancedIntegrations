using System.Text.Json;

// Burada kendi Deepgram API anahtarımı tanımlıyorum. Bilerek boş bıraktım ki anahtarımı paylaşmayayım.
var apiKey = "";

// Çevirmek istediğim ses dosyasının yolunu burada belirtiyorum.
var filePath = "force.mp3";

// Eğer dosya bulunamazsa kullanıcıya hata verdiriyorum.
if (!File.Exists(filePath))
{
    Console.WriteLine("Çalınacak Dosya Bulunamadı.");
    return;
}

// HTTP istemcisini burada oluşturuyorum.
using var client = new HttpClient();

// API’ye erişim için gerekli olan Authorization başlığını ekliyorum.
client.DefaultRequestHeaders.Add("Authorization", $"Token {apiKey}");

// Ses dosyasını okumak için stream açıyorum.
using var fileStream = File.OpenRead(filePath);

// Dosya içeriğini HTTP isteğine ekliyorum.
var content = new StreamContent(fileStream);

// İçeriğin türünü mp3 olarak belirtiyorum.
content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/mp3");

// Deepgram API’sine POST isteği atıyorum. Burada ses dosyamı gönderiyorum.
var response = await client.PostAsync("https://api.deepgram.com/v1/listen?punctuate=true&language=en", content);

// API’den gelen cevabı string olarak alıyorum.
var json = await response.Content.ReadAsStringAsync();

try
{
    // JSON cevabını parse ediyorum.
    var doc = JsonDocument.Parse(json);

    // JSON içinden transkript metnini çekiyorum.
    var transcript = doc.RootElement
        .GetProperty("results")
        .GetProperty("channels")[0]
        .GetProperty("alternatives")[0]
        .GetProperty("transcript")
        .GetString();

    // Çözümlenen metni ekrana yazdırıyorum.
    Console.WriteLine("Transkript Edilmiş Metin: " + transcript);
}
catch (Exception ex)
{
    // Bir hata olursa konsola hatayı yazdırıyorum.
    Console.WriteLine("Hata Oluştu: " + ex.Message);

    // Ayrıca API’nin döndürdüğü cevabı da yazdırıyorum ki debug edebileyim.
    Console.WriteLine("API Cevabı: " + json);

    // Hatanın dışarı fırlatılmasını sağlıyorum.
    throw;
}
