using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

// Hugging Face token'ı ortam değişkeninden geliyor; herkes kendi token'ını kullanıyor.
var apiKey = ApiKeys.Require("HF_TOKEN");

// Kullanıcıdan metin alıyorum
Console.Write("Analiz etmek istediğin metni gir: ");
var text = Console.ReadLine();

// Türkçe duygu analizi yapacak modeli seçtim
var modelUrl = "https://api-inference.huggingface.co/models/savasy/bert-base-turkish-sentiment-cased";

// HttpClient ile API çağrısı yapacağım, authorization ekliyorum
using var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

// Metni JSON formatına çevirip API'ye gönderiyorum
var json = JsonSerializer.Serialize(new { inputs = text });
var content = new StringContent(json, Encoding.UTF8, "application/json");

// API'yi çağırıyorum ve cevabı alıyorum
var response = await client.PostAsync(modelUrl, content);
var result = await response.Content.ReadAsStringAsync();

// JSON'u parse edip, dönen sonuçları okuyorum
var doc = JsonDocument.Parse(result);
var items = doc.RootElement[0];

// En yüksek skorlu sonucu seçiyorum
var topLabel = items
    .EnumerateArray()
    .OrderByDescending(e => e.GetProperty("score").GetDouble())
    .First();

// Label ve score'u alıyorum
var label = topLabel.GetProperty("label").GetString();
var score = topLabel.GetProperty("score").GetDouble();

// Labeli Türkçeye çeviriyorum
string labelText = label switch
{
    "negative" => "NEGATİF 😡",
    "neutral" => "NÖTR 😐",
    "positive" => "POZİTİF 😍",
    _ => "BİLİNMİYOR"
};

// Console'da emoji görünmesi için UTF-8 ayarlıyorum
Console.OutputEncoding = Encoding.UTF8;

// Sonuçları ekrana basıyorum
Console.WriteLine("\n🗒️ Girdi Metni: ");
Console.WriteLine($"{text}");

Console.WriteLine("🎭 Duygu Analizi: ");
Console.WriteLine($"✅ Duygu Durumu: {labelText}");
Console.WriteLine($"🎯 Güven Skoru: %{(score * 100).ToString("F2", CultureInfo.InvariantCulture)}");
