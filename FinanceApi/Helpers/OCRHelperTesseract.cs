using System.Drawing;
using System.Text.RegularExpressions;
using FinanceApi.Extensions;
using FinanceApi.Models;
using Tesseract;

namespace FinanceApi.Helpers;

internal class OCRHelper
{
    private const string _languageFilePath = @"./Assets/tessdata";

    internal (Movement[], CurrencyConversion[]) Process(IEnumerable<IFormFile> files, FinanceDb db, Module module, DateTime referenceDate, DateTimeKind dateTimeKind = DateTimeKind.Unspecified)
    {
        var movementResults = new List<Movement>();
        var currencyConversionResults = new List<CurrencyConversion>();

        foreach (var file in files)
        {
            var (movements, currencyConversions) = this.Process(file, db, module, referenceDate, dateTimeKind);
            movementResults.AddRange(movements);
            currencyConversionResults.AddRange(currencyConversions);
        }

        return (movementResults.ToArray(), currencyConversionResults.ToArray());
    }

    internal (Movement[], CurrencyConversion[]) Process(IFormFile file, FinanceDb db, Module module, DateTime referenceDate, DateTimeKind dateTimeKind = DateTimeKind.Unspecified)
    {
        this.DocumentProcessGuard();

        byte[] byteArray = new byte[0];
        using (var imageStream = new MemoryStream())
        {
            file.CopyTo(imageStream);

            var testArray = imageStream.ToArray();

            using (var adjustedImageStream = this.AdjustImage(imageStream))
            {
                byteArray = adjustedImageStream.ToArray();
                return this.Process(byteArray, db, module, referenceDate, dateTimeKind);
            }
        }
    }

    internal (Movement[], CurrencyConversion[]) Process(byte[] bytes, FinanceDb db, Module module, DateTime referenceDate, DateTimeKind dateTimeKind = DateTimeKind.Unspecified)
    {
        this.DocumentProcessGuard();

        var movements = new List<Movement>();
        var currencyConversions = new List<CurrencyConversion>();

        using (var image = Tesseract.Pix.LoadFromMemory(bytes))
        {
            using (var engine = new TesseractEngine(_languageFilePath, "spa", EngineMode.Default))
            {
                engine.SetVariable("user_defined_dpi", "300");
                using (var page = engine.Process(image))
                {
                    string[] text = page.GetText().Split("\n")
                        .Where(o => !string.IsNullOrWhiteSpace(o))
                        .SelectMany(o => o.Split("\t"))
                        .ToArray();

                    text = text.Skip(2).ToArray();
                    DateTime localReferenceDate = referenceDate.Duplicate();
                    var currencies = db.Currency.ToArray();

                    for (var i = 0; i + 1 < text.Length; i = i + 2)
                    {
                        string entry = text[i];
                        string dateAndConversion = text[i + 1];
                        var (movement, currencyConversion) = this.BuildMovement(module, entry, dateAndConversion, referenceDate, currencies);
                        movements.Add(movement);
                        if (currencyConversion != null) currencyConversions.Add(currencyConversion);
                    }

                    Console.WriteLine($"Texto \n {text}");
                }
            }
        }

        return (movements.ToArray(), currencyConversions.ToArray());
    }

    private (Movement, CurrencyConversion?) BuildMovement(Module module, string content, string dateAndConversion, DateTime referenceDate, Currency[] currencies)
    {
        string pattern = @"(.+\s+)([\+\-]*\s*[\d\,\.]*\s+)([a-zA-Z]+)";
        Match match = Regex.Match(content, pattern);

        string entity = match.Groups[1].Value.Trim().TrimStart('.').Trim();
        decimal amount;
        decimal.TryParse(match.Groups[2].Value.Trim(), out amount);
        string currencyName = match.Groups[3].Value.Trim();

        string localPattern = @"(\d+)(\sde\s)([a-zA-Z]+)(\s*)([\=z\s]*)(\s*)([\d,\s]*)([a-zA-Z]*)";
        match = Regex.Match(dateAndConversion, localPattern);

        string[] dateValues = match.Groups[1].Value.Trim().TrimStart('.').Trim().Split(" de ");
        short dateDay;
        short.TryParse(match.Groups[1].Value.Trim(), out dateDay);
        int dateMonth = DateHelpers.GetMonthNumber(match.Groups[3].Value);
        int dateYear = referenceDate.Year + (referenceDate > DateTime.Now ? -1 : 0);
        var date = new DateTime(dateYear, dateMonth, dateDay, 0, 0, 0, referenceDate.Kind);

        var movementCurrency = this.GetCurrency(currencies, currencyName);

        var movement = new Movement()
        {
            ModuleId = module.Id,
            Module = module,
            TimeStamp = date,
            Concept1 = entity,
            Concept2 = string.Empty,
            Ammount = amount,
            Total = null,
            Currency = movementCurrency
        };

        var amountLocalCurrencyStr = match.Groups[7].Value.Trim();
        string localCurrencyName = match.Groups[8].Value.Trim();

        CurrencyConversion currencyConversion = null;
        if (!string.IsNullOrWhiteSpace(amountLocalCurrencyStr) && !string.IsNullOrWhiteSpace(localCurrencyName))
        {
            decimal amountLocalCurrency = 0;
            if (!string.IsNullOrWhiteSpace(amountLocalCurrencyStr))
                decimal.TryParse(amountLocalCurrencyStr, out amountLocalCurrency);

            var conversionCurrency = this.GetCurrency(currencies, localCurrencyName);

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
        if (!System.IO.Directory.Exists(_languageFilePath)) throw new FileNotFoundException("Language file not found");
    }

    public MemoryStream AdjustImage(MemoryStream stream)
    {
        return RGBToGrayScale(stream);
    }

    private MemoryStream RGBToGrayScale(MemoryStream stream)
    {
        using (Bitmap image = new Bitmap(stream))
        {
            var mostUsedColor = GetMostUsedColor(image);
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color originalColor = image.GetPixel(x, y);
                    var newColor = ApplyHighContrast(originalColor, mostUsedColor);
                    // newColor = ApplyGreyScale(newColor);
                    image.SetPixel(x, y, newColor);
                }
            }

            var result = new MemoryStream();
            image.Save(result, System.Drawing.Imaging.ImageFormat.Png);
            return result;
        }
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

    private Color ApplyHighContrast(Color color, Color mostUsedColor)
    {
        /* R 29,V 38, A 50 */
        if (color.R == mostUsedColor.R && color.G == mostUsedColor.G && color.B == mostUsedColor.B) return Color.FromArgb(255, 255, 0);

        return Color.FromArgb(0, 0, 0);
    }

}
