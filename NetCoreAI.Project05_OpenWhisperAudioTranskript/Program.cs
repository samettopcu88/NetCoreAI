using System.Net.Http.Headers;


// AssemblyAI ile

//class Program
//{
//    static async Task Main(string[] args)
//    {
//        string apiKey = "--- buraya key gelecek ---";
//        string audioFilePath = "audio1.mp3";

//        using (var client = new HttpClient())
//        {
//            client.DefaultRequestHeaders.Add("authorization", apiKey);

//            Console.WriteLine("Ses dosyası yükleniyor...");

//            // 1. Dosyayı AssemblyAI'ye yükle
//            using var fileStream = File.OpenRead(audioFilePath);
//            using var content = new StreamContent(fileStream);
//            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

//            var uploadResponse = await client.PostAsync("https://api.assemblyai.com/v2/upload", content);
//            var uploadJson = await uploadResponse.Content.ReadAsStringAsync();
//            string audioUrl = JsonDocument.Parse(uploadJson).RootElement.GetProperty("upload_url").GetString();

//            // 2. Transkripsiyon başlat
//            Console.WriteLine("Transkripsiyon başlatılıyor...");

//            var json = $"{{\"audio_url\": \"{audioUrl}\", \"language_code\": \"tr\"}}";
//            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

//            var transcriptResponse = await client.PostAsync("https://api.assemblyai.com/v2/transcript", stringContent);
//            var transcriptJson = await transcriptResponse.Content.ReadAsStringAsync();
//            string transcriptId = JsonDocument.Parse(transcriptJson).RootElement.GetProperty("id").GetString();

//            // 3. Transkripsiyon tamamlanana kadar bekle
//            string pollingUrl = $"https://api.assemblyai.com/v2/transcript/{transcriptId}";
//            string status = "";

//            Console.WriteLine("İşleniyor, lütfen bekleyin...");
//            while (status != "completed")
//            {
//                await Task.Delay(3000);
//                var pollingResponse = await client.GetAsync(pollingUrl);
//                var pollingJson = await pollingResponse.Content.ReadAsStringAsync();
//                var pollingDoc = JsonDocument.Parse(pollingJson).RootElement;

//                status = pollingDoc.GetProperty("status").GetString();

//                if (status == "completed")
//                {
//                    string text = pollingDoc.GetProperty("text").GetString();
//                    Console.WriteLine("Transkript:");
//                    Console.WriteLine(text);
//                }
//                else if (status == "error")
//                {
//                    string error = pollingDoc.GetProperty("error").GetString();
//                    Console.WriteLine($"Hata: {error}");
//                    break;
//                }
//            }
//        }
//    }
//}


// OpenAI ile


class Program
{
    static async Task Main(string[] args)
    {
        // OpenAI API anahtarı ve ses dosyasının yolu
        string apiKey = "--- key gelecek ---";
        string audioFilePath = "audio1.mp3";

        // HttpClient nesnesi oluşturuluyor
        using (var client = new HttpClient())
        {
            // API anahtarı Authorization header'ına ekleniyor (Bearer token formatında)
            // Olmazsa yine 401 Unauthorized hatası alınır (sayfaya erişimin engellediğinde ortaya çıkar)
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            // Form-data yapısı oluşturuluyor (multipart/form-data)
            // Bu satır olmazsa gönderilen veri uygun formatta olmaz, sunucu işlemez
            var form = new MultipartFormDataContent();

            // Ses dosyası byte dizisine dönüştürülüyor
            // Olmazsa sunucuya gönderecek bir içerik kalmaz
            var audioContent = new ByteArrayContent(File.ReadAllBytes(audioFilePath));

            // İçerik türü (MIME type) belirtiliyor: mp3 dosyası olduğu için "audio/mpeg"
            audioContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/mpeg");

            // Ses dosyası form'a ekleniyor
            form.Add(audioContent, "file", Path.GetFileName(audioFilePath));

            // Kullanılacak model belirtiliyor (OpenAI Whisper modeli)
            form.Add(new StringContent("whisper-1"), "model");

            Console.WriteLine("Ses Dosyası İşleniyor, Lütfen Bekleyiniz..");

            // POST isteği gönderiliyor: OpenAI Whisper transkripsiyon API'sine
            var response = await client.PostAsync("https://api.openai.com/v1/audio/transcriptions", form);

            // Eğer istek başarılıysa sonucu ekrana yazdır
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Transkript: ");
                Console.WriteLine(result);
            }
            else
            {
                // Hata durumunda hem HTTP durumu hem de hata mesajı yazdırılır
                Console.WriteLine($"Hata: {response.StatusCode}");
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }
    }
}