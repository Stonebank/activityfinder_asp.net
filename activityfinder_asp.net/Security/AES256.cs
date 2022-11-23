
using System.Security.Cryptography;
using System.Text;

namespace activityfinder_asp.net.Security
{
    public class AES256
    {

        public static string Encrypt(string password)
        {
            var salt = Encoding.UTF8.GetBytes(Constant.PASSWORD_SALT);

            using (var algo = Aes.Create())
            {
                using (var encrypt = algo.CreateEncryptor(salt, algo.IV))
                {
                    using (var msEncryption = new MemoryStream())
                    {
                        using (var cEncrypt = new CryptoStream(msEncryption, encrypt, CryptoStreamMode.Write))
                        {
                            using (var sEncrypt = new StreamWriter(cEncrypt))
                            {
                                sEncrypt.Write(password);
                            }
                            var aesIv = algo.IV;
                            var content = msEncryption.ToArray();
                            var result = new byte[aesIv.Length + content.Length];
                            Buffer.BlockCopy(aesIv, 0, result, 0, aesIv.Length);
                            Buffer.BlockCopy(content, 0, result, aesIv.Length, content.Length);
                            return Convert.ToBase64String(result);
                        }
                    }
                }
            }

        }

        public static string Decrypt(string password)
        {
            var cipher = Convert.FromBase64String(password);
            var aesIv = new byte[16];
            var b_cipher = new byte[16];

            Buffer.BlockCopy(cipher, 0, aesIv, 0, aesIv.Length);
            Buffer.BlockCopy(cipher, aesIv.Length, b_cipher, 0, aesIv.Length);

            var salt = Encoding.UTF8.GetBytes(Constant.PASSWORD_SALT);

            using (var aesAlgo = Aes.Create())
            {
                using (var decryptor = aesAlgo.CreateDecryptor(salt, aesIv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(b_cipher))
                    {
                        using (var cDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sDecrypt = new StreamReader(cDecrypt))
                            {
                                result = sDecrypt.ReadToEnd();
                            }
                        }
                    }
                    return result;
                }
            }
        }

    }
}
