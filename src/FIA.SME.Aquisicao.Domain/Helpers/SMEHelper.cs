using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace FIA.SME.Aquisicao.Core.Helpers
{
    public static class SMEHelper
    {
        public static DateTime ConvertToSolveUTC(this DateTime date)
        {
            if (date.Hour > 0)
                return date;

            return date.AddHours(5);
        }

        public static string ConvertToBase64(this Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            return Convert.ToBase64String(bytes);
        }

        public static string DescriptionAttr<T>(this T source)
        {
            if (source == null)
                return String.Empty;

            var sourceName = source!.ToString()!;
            FieldInfo? fi = source?.GetType().GetField(sourceName);

            if (fi == null)
                return sourceName;

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi!.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return sourceName;
        }

        public static string FormatCPF(this string cpf)
        {
            if (String.IsNullOrEmpty(cpf))
                return String.Empty;

            return Convert.ToUInt64(cpf.ToOnlyNumbers()).ToString(@"000\.000\.000\-00");
        }

        public static string FormatCNPJ(this string cnpj)
        {
            if (String.IsNullOrEmpty(cnpj))
                return String.Empty;

            return Convert.ToUInt64(cnpj.ToOnlyNumbers()).ToString(@"00\.000\.000/0000\-00");
        }

        public static string FormatPhoneNumber(this string phone)
        {
            if (String.IsNullOrEmpty(phone))
                phone = String.Empty;

            var phoneNumber = phone.ToOnlyNumbers().PadLeft(10, '0');

            if (phoneNumber.Length == 10)
                return long.Parse(phoneNumber).ToString(@"(00) 0000-0000");

            return long.Parse(phoneNumber).ToString(@"(00) 00000-0000");
        }

        public static string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        public static T? GetEnumValueFromDescription<T>(this string description)
        {
            MemberInfo[] fis = typeof(T).GetFields();

            foreach (var fi in fis)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0 && attributes[0].Description == description)
                    return (T)Enum.Parse(typeof(T), fi.Name);
            }

            return default;
        }

        public static string GetFirstWord(this string word)
        {
            if (String.IsNullOrEmpty(word))
                return String.Empty;

            return Regex.Match(word, @"^([\w\-]+)").Value;
        }

        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public static string GetSizeWithSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + GetSizeWithSuffix(-value, decimalPlaces); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", adjustedSize, SizeSuffixes[mag]);
        }

        public static Stream GetStreamFromUrl(this string url)
        {
            byte[] imageData = null;

            using (var wc = new System.Net.WebClient())
                imageData = wc.DownloadData(url);

            return new MemoryStream(imageData);
        }

        public static string GenerateSlug(this string phrase)
        {
            string str = phrase.RemoveDiacritics().ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim(); // cut and trim
            str = Regex.Replace(str, @"\s", "-"); // hyphens
            return str;
        }

        public static bool IsBase64String(this string base64)
        {
            var splittedValue = base64.Split(',');
            var fileBase64 = splittedValue.Length > 1 ? splittedValue[1] : base64;

            Span<byte> buffer = new Span<byte>(new byte[fileBase64.Length]);
            return Convert.TryFromBase64String(fileBase64, buffer, out int bytesParsed);
        }

        public static string RemoveDiacritics(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }

        public static DateTime? SetKindUtc(this DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                return dateTime.Value.SetKindUtc();
            }
            else
            {
                return null;
            }
        }

        public static DateTime SetKindUtc(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc) { return dateTime; }
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        public static DateTime? StringToDateTime(this string value, string[] formats = null)
        {
            if (formats == null)
                formats = new List<string>() {
                                                "dd/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "d/MM/yyyy", "dd/MM/yy", "dd/M/yy", "d/M/yy", "d/MM/yy",
                                                "MM/dd/yyyy", "M/dd/yyyy", "M/d/yyyy", "MM/d/yyyy", "MM/dd/yy", "M/dd/yy", "M/d/yy", "MM/d/yy",
                                                "dd-MM-yyyy", "dd-M-yyyy", "d-M-yyyy", "d-MM-yyyy", "dd-MM-yy", "dd-M-yy", "d-M-yy", "d-MM-yy",
                                                "yyyy-MM-dd", "yyyy-M-dd", "yyyy-M-d", "yyyy-MM-d", "yy-MM-dd", "yy-M-dd", "yy-M-d", "yy-MM-d"
                                            }.ToArray();

            if (DateTime.TryParseExact(value, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var date))
                return date;

            return null;
        }

        public static string ToOnlyNumbers(this string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            return new String(text.Where(Char.IsDigit).ToArray());
        }

        public static string ToTitleCase(this string title)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
        }
    }
}
