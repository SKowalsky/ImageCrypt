using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Security.Cryptography;
using System.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace ImageCrypt
{
    public class Crypto
    {

        public static string Encrypt(string toEncrypt, string key)
        {
            try
            {
                var keyHash = GetMD5Hash(key);
                var toDecryptBuffer = CryptographicBuffer.ConvertStringToBinary(toEncrypt, BinaryStringEncoding.Utf8);
                var aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
                var symetricKey = aes.CreateSymmetricKey(keyHash);
                var buffEncrypted = CryptographicEngine.Encrypt(symetricKey, toDecryptBuffer, null);
                var strEncrypted = CryptographicBuffer.EncodeToBase64String(buffEncrypted);
                return strEncrypted;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string Decrypt(string cipherString, string key)
        {
            try
            {
                var keyHash = GetMD5Hash(key);
                IBuffer toDecryptBuffer = CryptographicBuffer.DecodeFromBase64String(cipherString);
                SymmetricKeyAlgorithmProvider aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
                var symetricKey = aes.CreateSymmetricKey(keyHash);
                var buffDecrypted = CryptographicEngine.Decrypt(symetricKey, toDecryptBuffer, null);
                string strDecrypted = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buffDecrypted);
                return strDecrypted;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static IBuffer GetMD5Hash(string key)
        {
            IBuffer buffUtf8Msg = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            IBuffer buffHash = objAlgProv.HashData(buffUtf8Msg);
            if (buffHash.Length != objAlgProv.HashLength)
            {
                throw new Exception("Error: Failed to create hash value");
            }
            return buffHash;
        }

    }
}
