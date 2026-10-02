// Anahtarlar koda yazılmıyor, ortam değişkenlerinden okunuyor.
// Directory.Build.props bu dosyayı her projeye link olarak ekliyor; projeler birbirine bağlı değil.
internal static class ApiKeys
{
    // Değişken yoksa ne yapılması gerektiğini yazıp çıkıyorum. Değerin kendisini hiçbir yere yazdırmıyorum.
    public static string Require(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (!string.IsNullOrWhiteSpace(value)) return value.Trim();

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine($"{name} ortam değişkeni tanımlı değil.");
        Console.ResetColor();
        Console.Error.WriteLine($"PowerShell: $env:{name} = \"...\"   ya da kalıcı olarak: setx {name} \"...\"");
        Environment.Exit(1);
        return "";
    }

    // Bölge gibi gizli olmayan ama değişebilen ayarlar için.
    public static string Optional(string name, string fallback)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
    }
}
