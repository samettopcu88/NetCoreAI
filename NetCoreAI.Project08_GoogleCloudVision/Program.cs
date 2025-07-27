using Google.Cloud.Vision.V1;
class Program
{
    static void Main(string[] args)
    {
        Console.Write("Resim yolunu giriniz:");
        string imagePath = Console.ReadLine();
        Console.WriteLine();

        string credentialPath = @"--- servis json dosyası gelecek ---";
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

        try
        {
            var client = ImageAnnotatorClient.Create();
            var image = Image.FromFile(imagePath);
            var response = client.DetectText(image);
            Console.WriteLine("Resimdeki Metin:");
            Console.WriteLine();
            foreach (var annotination in response)
            {
                if (!string.IsNullOrEmpty(annotination.Description))
                {
                    Console.WriteLine(annotination.Description);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Bir hata oluştu {ex.Message}");
        }
    }
}