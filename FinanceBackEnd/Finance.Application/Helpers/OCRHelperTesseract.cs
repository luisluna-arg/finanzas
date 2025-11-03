using System.Drawing;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using Finance.Domain.Models;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;
using Tesseract;

namespace Finance.Helpers;

[SupportedOSPlatform("windows")]
public class OcrHelper
{
    private const string LanguageFilePath = @"./Assets/tessdata";

    public (Movement[] Movements, CurrencyConversion[] CurrencyConversions) Process(IEnumerable<IFormFile> files, FinanceDbContext db, AppModule appModule, DateTime referenceDate, DateTimeKind dateTimeKind = DateTimeKind.Unspecified)
    {
        var movementResults = new List<Movement>();
        var currencyConversionResults = new List<CurrencyConversion>();

        foreach (var file in files)
        {
            var (movements, currencyConversions) = Process(file, db, appModule, referenceDate, dateTimeKind);
            movementResults.AddRange(movements);
            currencyConversionResults.AddRange(currencyConversions);
        }

        return (movementResults.ToArray(), currencyConversionResults.ToArray());
    }

    public (Movement[] Movements, CurrencyConversion[] CurrencyConversion) Process(IFormFile file, FinanceDbContext db, AppModule appModule, DateTime referenceDate, DateTimeKind dateTimeKind = DateTimeKind.Unspecified)
    {
        DocumentProcessGuard();

        byte[] byteArray;
        using (var imageStream = new MemoryStream())
        {
            file.CopyTo(imageStream);

            using (var adjustedImageStream = AdjustImage(imageStream))
            {
                byteArray = adjustedImageStream.ToArray();
                return Process(byteArray, db, appModule, referenceDate, dateTimeKind);
            }
        }
    }

    public (Movement[] Movements, CurrencyConversion[] CurrencyConversions) Process(byte[] bytes, FinanceDbContext db, AppModule appModule, DateTime referenceDate, DateTimeKind dateTimeKind = DateTimeKind.Unspecified)
    {
        DocumentProcessGuard();

        var movements = new List<Movement>();
        var currencyConversions = new List<CurrencyConversion>();

        using (var image = Tesseract.Pix.LoadFromMemory(bytes))
        {
            using (var engine = new TesseractEngine(LanguageFilePath, "spa", EngineMode.Default))
            {
                engine.SetVariable("user_defined_dpi", "300");
                using (var page = engine.Process(image))
                {
                    string[] text = page.GetText().Split("\n")
                        .Where(o => !string.IsNullOrWhiteSpace(o))
                        .SelectMany(o => o.Split("\t"))
                        .ToArray();

                    text = text.Skip(2).ToArray();
                    var currencies = db.Currency.ToArray();

                    for (var i = 0; i + 1 < text.Length; i = i + 2)
                    {
                        string entry = text[i];
                        string dateAndConversion = text[i + 1];
                        var (movement, currencyConversion) = BuildMovement(appModule, entry, dateAndConversion, referenceDate, currencies);
                        movements.Add(movement);
                        if (currencyConversion != null) currencyConversions.Add(currencyConversion);
                    }

                    Console.WriteLine($"Texto \n {text}");
                }
            }
        }

        return (movements.ToArray(), currencyConversions.ToArray());
    }

    public string[] CitricCaptureToImage(IEnumerable<IFormFile> files)
    {
        DocumentProcessGuard();

        var result = new List<string>();

        using (var engine = new TesseractEngine(LanguageFilePath, "spa", EngineMode.Default))
        {
            engine.SetVariable("user_defined_dpi", "300");

            foreach (var file in files)
            {
                result.Add(file.FileName);
                result.Add(string.Empty);
                using (var imageStream = new MemoryStream())
                {
                    file.CopyTo(imageStream);

                    using (var adjustedImageStream = AdjustImage(imageStream))
                    {
                        using (var image = Tesseract.Pix.LoadFromMemory(adjustedImageStream.ToArray()))
                        {
                            using (var page = engine.Process(image))
                            {
                                if (page == null) continue;

                                var imageText = page.GetText();
                                var entries = imageText.Split("\n\n").Skip(4).ToArray();

                                int index = Array.IndexOf(entries, entries.FirstOrDefault(s => new char[] { '+', '-' }.Any(c => s.StartsWith(c))));

                                var concepts = entries.Take(index).SelectMany(s => s.Split("\n")).ToArray();
                                var values = entries.Skip(index).SelectMany(s => s.Split("\n")).ToArray();

                                var stringBuilder = new StringBuilder();
                                var iConcept = 0;
                                var iVal = 0;

                                Func<string, string> numberFormatter = (str) => str.Replace("+ ", string.Empty).Replace("- ", "-").Replace(".", string.Empty).Replace(",", ".").Replace("= ", string.Empty).Replace(" ", "\t");

                                while (iConcept < concepts.Length && iVal < values.Length)
                                {
                                    var concept = concepts[iConcept];
                                    var dateStr = concepts[iConcept + 1];

                                    // How to set year when the entry is from previous year, also how determine if it's from the immediate year or older
                                    var date = DateTime.Parse(dateStr);
                                    dateStr = date.ToString("MM/dd/yyyy");
                                    var value = values[iVal];
                                    stringBuilder.Append(dateStr).Append("\t");
                                    stringBuilder.Append(concept).Append("\t");
                                    stringBuilder.Append(numberFormatter(value)).Append("\t");
                                    if (!value.Contains("ARS")) stringBuilder.Append(numberFormatter(values[++iVal]));
                                    iConcept += 2;
                                    iVal++;
                                    result.Add(stringBuilder.ToString());
                                    stringBuilder.Clear();
                                }
                            }
                        }
                    }
                }

                result.Add("\n============================\n");
            }
        }

        return result.ToArray();
    }

    public MemoryStream AdjustImage(MemoryStream stream)
    {
        try
        {
            using (Bitmap image = new Bitmap(stream))
            {
                var colorsToReplace = new Color[]
                {
                    GetMostUsedColor(image)
                };

                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        Color originalColor = image.GetPixel(x, y);
                        var newColor = ApplyHighContrast(originalColor, colorsToReplace);
                        newColor = ApplyGreyScale(newColor);
                        image.SetPixel(x, y, newColor);
                    }
                }

                var result = new MemoryStream();
                image.Save(result, System.Drawing.Imaging.ImageFormat.Png);
                return result;
            }
        }
        catch (ArgumentException ex)
        {
            // Manejar la excepción
            Console.WriteLine($"Se produjo una excepción al crear la imagen Bitmap: {ex.Message}");
            throw; // Opcional: volver a lanzar la excepción
        }
    }

    private (Movement Movement, CurrencyConversion? CurrencyConversion) BuildMovement(AppModule appModule, string content, string dateAndConversion, DateTime referenceDate, Currency[] currencies)
    {
        string pattern = @"(.+\s+)([\+\-]*\s*[\d\,\.]*\s+)([a-zA-Z]+)";
        Match match = Regex.Match(content, pattern);

        string entity = match.Groups[1].Value.Trim().TrimStart('.').Trim();
        decimal amount;
        decimal.TryParse(match.Groups[2].Value.Trim(), out amount);
        string currencyName = match.Groups[3].Value.Trim();

        string localPattern = @"(\d+)(\sde\s)([a-zA-Z]+)(\s*)([\=z\s]*)(\s*)([\d,\s]*)([a-zA-Z]*)";
        match = Regex.Match(dateAndConversion, localPattern);

        short dateDay;
        short.TryParse(match.Groups[1].Value.Trim(), out dateDay);
        int dateMonth = DateTimeHelper.GetMonthNumber(match.Groups[3].Value);
        int dateYear = referenceDate.Year + (referenceDate > DateTime.Now ? -1 : 0);
        var date = new DateTime(dateYear, dateMonth, dateDay, 0, 0, 0, referenceDate.Kind);

        var movementCurrency = GetCurrency(currencies, currencyName);

        var movement = new Movement()
        {
            AppModuleId = appModule.Id,
            AppModule = appModule,
            TimeStamp = date,
            CreatedAt = DateTime.UtcNow,
            Concept1 = entity,
            Concept2 = string.Empty,
            Amount = amount,
            Total = null,
            Currency = movementCurrency
        };

        var amountLocalCurrencyStr = match.Groups[7].Value.Trim();
        string localCurrencyName = match.Groups[8].Value.Trim();

        CurrencyConversion? currencyConversion = null;
        if (!string.IsNullOrWhiteSpace(amountLocalCurrencyStr) && !string.IsNullOrWhiteSpace(localCurrencyName))
        {
            decimal amountLocalCurrency = 0;
            decimal.TryParse(amountLocalCurrencyStr, out amountLocalCurrency);

            var conversionCurrency = GetCurrency(currencies, localCurrencyName);

            currencyConversion = new CurrencyConversion()
            {
                Movement = movement,
                Amount = amountLocalCurrency,
                Currency = conversionCurrency
            };
        }

        return (movement, currencyConversion);
    }

    private Currency GetCurrency(Currency[] currencies, string currency)
    {
        var currencyEntity = currencies.FirstOrDefault(o => o.ShortName == currency);
        if (currencyEntity == null) throw new Exception($"Currency not found: {currency}");
        return currencyEntity;
    }

    private void DocumentProcessGuard()
    {
        if (!System.IO.Directory.Exists(LanguageFilePath)) throw new FileNotFoundException("Language file not found");
    }

    private Color GetMostUsedColor(Bitmap image)
    {
        Dictionary<string, int> colors = new Dictionary<string, int>();
        for (int x = 0; x < image.Width; x++)
        {
            for (int y = 0; y < image.Height; y++)
            {
                Color originalColor = image.GetPixel(x, y);
                var key = string.Join("_", originalColor.R, originalColor.G, originalColor.B);
                if (!colors.ContainsKey(key)) colors.Add(key, 1);
                else colors[key]++;
            }
        }

        string[] colorValues = colors.OrderByDescending(c => c.Value).First().Key.Split('_');

        return Color.FromArgb(int.Parse(colorValues[0]), int.Parse(colorValues[1]), int.Parse(colorValues[2]));
    }

    private Color ApplyGreyScale(Color c)
    {
        int red = (byte)(c.R * 0.299);
        int green = (byte)(c.G * 0.587);
        int blue = (byte)(c.B * 0.114);
        return Color.FromArgb(red + green + blue, red + green + blue, red + green + blue);
    }

    private Color ApplyHighContrast(Color color, Color[] colorsToReplace)
    {
        if (colorsToReplace.Any(c => color.R == c.R && color.G == c.G && color.B == c.B)) return Color.FromArgb(255, 255, 0);
        if (color.B < 90 && color.B > 45 && color.G < color.B && color.R < color.G) return Color.FromArgb(255, 255, 0);

        return Color.FromArgb(0, 0, 0);
    }
}
