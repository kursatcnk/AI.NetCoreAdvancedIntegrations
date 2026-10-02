# AI .NET Core Advanced Integrations

Farklı yapay zekâ sağlayıcılarını aynı çözüm içinde, yan yana denediğim .NET 9 konsol uygulamaları. Amaç her sağlayıcının kimlik doğrulamasını, istek biçimini ve cevap yapısını tanımak; böylece bir projede hangisini neden seçeceğimi bilmek.

Tüm çağrılar `HttpClient` ile doğrudan REST üzerinden yapılıyor, SDK kullanılmadı.

## Projeler

**Anthropic Claude**

- `AdvancedAI.ClaudeChatBot`: konsoldan soru sorup Claude ile cevap alma.
- `AdvancedAI.SummarizePDFWithClaude`: PDF'ten metni çıkarıp özetletme.
- `AdvancedAI.SendJobApplicationMailWithClaude`: iş başvurusu e-postası taslağı yazdırma.

**Google Gemini**

- `AdvancedAI.GeminiChatBot`: Gemini ile sohbet.
- `AdvancedAI.GeminiAutoPromptChain`: kullanıcının fikrini alıp beş adımda sorularla netleştiren, sonunda bir içerik planı çıkaran prompt zinciri.
- `AdvancedAI.GeminiRoleBasedSimulation`: beş rolden birini seçip (koç, beslenme uzmanı, finans danışmanı, araştırmacı, gezi danışmanı) o rolle konuşma.

**OpenAI**

- `AdvancedAI.CodeAssistantWithOpenAI`: yapıştırılan kod için açıklama, refactor önerisi ya da unit test senaryoları üretme.

**Hugging Face Inference API**

- `AdvancedAI.HuggingFaceSentimentAnalysis`: Türkçe metinde duygu analizi (`savasy/bert-base-turkish-sentiment-cased`).
- `AdvancedAI.HuggingFaceDetectToxicBehaviour`: zararlı / toksik içerik tespiti (`unitary/toxic-bert`).
- `AdvancedAI.HuggingFaceSeparateEntitiesWithNER`: metindeki kişi, kurum ve yer adlarını ayırma (`dslim/bert-base-NER`).
- `AdvancedAI.HuggingFaceQaWithRobertaBase`: verilen metne göre soru cevaplama (`deepset/roberta-base-squad2`).
- `AdvancedAI.HuggingFaceSummarizeNovels`: uzun metni özetleme (`facebook/bart-large-cnn`).

**Azure AI Services**

- `AdvancedAI.AnalyzeImageWithAzure`: görselin açıklaması, kategorileri, etiketleri ve renkleri.
- `AdvancedAI.DetailedImageDetectionWithAzure`: nesne, yüz ve marka tespiti dahil ayrıntılı analiz.
- `AdvancedAI.TTSWithAzure`: token alıp metni sese çevirme.

**Görsel ve ses üretimi**

- `AdvancedAI.TextToImageWithStabilityAI`: Stable Diffusion ile tariften görsel.
- `AdvancedAI.CreateImageWithReplicate`: Replicate üzerinden tariften görsel üretme.
- `AdvancedAI.DeepGramVoice`: Deepgram ile ses dosyasını yazıya çevirme.

## Çalıştırma

Gerekenler: .NET 9 SDK.

```bash
git clone https://github.com/kursatcnk/AI.NetCoreAdvancedIntegrations.git
cd AI.NetCoreAdvancedIntegrations
dotnet run --project AdvancedAI.GeminiChatBot
```

Kodda anahtar yok; her proje ihtiyaç duyduğu anahtarı ortam değişkeninden okur. Değişken tanımlı değilse hangi değişkenin eksik olduğunu yazıp çıkar, anahtarın değerini hiçbir yere yazdırmaz.

| Sağlayıcı | Ortam değişkeni | Kullanan projeler |
|---|---|---|
| Anthropic Claude | `ANTHROPIC_API_KEY` | ClaudeChatBot, SummarizePDFWithClaude, SendJobApplicationMailWithClaude |
| Google Gemini | `GEMINI_API_KEY` | GeminiChatBot, GeminiAutoPromptChain, GeminiRoleBasedSimulation |
| OpenAI | `OPENAI_API_KEY` | CodeAssistantWithOpenAI |
| Hugging Face | `HF_TOKEN` | HuggingFace* projeleri |
| Azure AI Vision | `AZURE_VISION_KEY`, `AZURE_VISION_ENDPOINT` | AnalyzeImageWithAzure, DetailedImageDetectionWithAzure |
| Azure AI Speech | `AZURE_SPEECH_KEY`, isteğe bağlı `AZURE_SPEECH_REGION` (varsayılan `westeurope`) | TTSWithAzure |
| Stability AI | `STABILITY_API_KEY` | TextToImageWithStabilityAI |
| Replicate | `REPLICATE_API_TOKEN` | CreateImageWithReplicate |
| Deepgram | `DEEPGRAM_API_KEY` | DeepGramVoice |

PowerShell'de yalnızca açık oturum için:

```powershell
$env:GEMINI_API_KEY = "..."
dotnet run --project AdvancedAI.GeminiChatBot
```

Kalıcı olarak tanımlamak için `setx GEMINI_API_KEY "..."` (yeni açılan terminallerde geçerli olur).

Azure görsel analiz projelerinde analiz edilecek dosyanın yolu `Program.cs` içindeki `imagePath` alanında, Deepgram projesinde ses dosyası `force.mp3` olarak bekleniyor.

Anahtarların okunduğu ortak sınıf `Shared/ApiKeys.cs`; `Directory.Build.props` onu her projeye link olarak ekliyor, projeler birbirine referans vermiyor.

## Lisans

[MIT](LICENSE)
