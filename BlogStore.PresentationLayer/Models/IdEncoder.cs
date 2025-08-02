using System;
using System.Text;

namespace BlogStore.PresentationLayer.Models
{
    public static class IdEncoder
    {
        public static string EncodeId(int id)
        {
            var bytes = BitConverter.GetBytes(id);
            return Convert.ToBase64String(bytes).Replace("=", "").Replace("/", "-").Replace("+", "_");
        }

        public static int? DecodeId(string encoded)
        {
            try
            {
                // Base64 string'i tekrar orijinal haline getir
                string base64 = encoded.Replace("-", "/").Replace("_", "+");
                // Base64 string uzunluğu 4'ün katı olmalı
                int mod4 = base64.Length % 4;
                if (mod4 > 0)
                {
                    base64 += new string('=', 4 - mod4);
                }
                var bytes = Convert.FromBase64String(base64);
                return BitConverter.ToInt32(bytes, 0);
            }
            catch
            {
                return null;
            }
        }
    }
} 