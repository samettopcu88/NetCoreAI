using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


// AssemblyAI ile
class Program
{
    static async Task Main(string[] args)
    {
        string apiKey = "--- buraya key gelecek ---";
        string audioFilePath = "audio1.mp3";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("authorization", apiKey);

            Console.WriteLine("Ses dosyası yükleniyor...");

            // 1. Dosyayı AssemblyAI'ye yükle
            using var fileStream = File.OpenRead(audioFilePath);
            using var content = new StreamContent(fileStream);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var uploadResponse = await client.PostAsync("https://api.assemblyai.com/v2/upload", content);
            var uploadJson = await uploadResponse.Content.ReadAsStringAsync();
            string audioUrl = JsonDocument.Parse(uploadJson).RootElement.GetProperty("upload_url").GetString();

            // 2. Transkripsiyon başlat
            Console.WriteLine("Transkripsiyon başlatılıyor...");

            var json = $"{{\"audio_url\": \"{audioUrl}\", \"language_code\": \"tr\"}}";
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var transcriptResponse = await client.PostAsync("https://api.assemblyai.com/v2/transcript", stringContent);
            var transcriptJson = await transcriptResponse.Content.ReadAsStringAsync();
            string transcriptId = JsonDocument.Parse(transcriptJson).RootElement.GetProperty("id").GetString();

            // 3. Transkripsiyon tamamlanana kadar bekle
            string pollingUrl = $"https://api.assemblyai.com/v2/transcript/{transcriptId}";
            string status = "";

            Console.WriteLine("İşleniyor, lütfen bekleyin...");
            while (status != "completed")
            {
                await Task.Delay(3000);
                var pollingResponse = await client.GetAsync(pollingUrl);
                var pollingJson = await pollingResponse.Content.ReadAsStringAsync();
                var pollingDoc = JsonDocument.Parse(pollingJson).RootElement;

                status = pollingDoc.GetProperty("status").GetString();

                if (status == "completed")
                {
                    string text = pollingDoc.GetProperty("text").GetString();
                    Console.WriteLine("Transkript:");
                    Console.WriteLine(text);
                }
                else if (status == "error")
                {
                    string error = pollingDoc.GetProperty("error").GetString();
                    Console.WriteLine($"Hata: {error}");
                    break;
                }
            }
        }
    }
}


// OpenAI ile

using System.Net.Http.Headers;

class Program
{
    static async Task Main(string[] args)
    {
        string apiKey = "--- key gelecek ---";
        string audioFilePath = "audio1.mp3";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var form = new MultipartFormDataContent();

            var audioContent = new ByteArrayContent(File.ReadAllBytes(audioFilePath));
            audioContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/mpeg");
            form.Add(audioContent, "file", Path.GetFileName(audioFilePath));
            form.Add(new StringContent("whisper-1"), "model");

            Console.WriteLine("Ses Dosyası İşleniyor, Lütfen Bekleyiniz......");

            var response = await client.PostAsync("https://api.openai.com/v1/audio/transcriptions", form);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Transkript: ");
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine($"Hata: {response.StatusCode}");
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }
    }
}