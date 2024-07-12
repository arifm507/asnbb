using static System.Formats.Asn1.AsnWriter;
using System.Security.Cryptography;
using System.Text;

namespace AllamaShibliQuiz.Helpers
{
    public static class StringExtension
    {
        public static string EncryptString(this string clearText)
        {
            string EncryptionKey = "MAKVKKKBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            string encryptedText = string.Empty;
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptedText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptedText;
        }


    }
}
