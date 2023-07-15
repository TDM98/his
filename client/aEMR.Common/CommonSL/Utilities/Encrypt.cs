using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace aEMR.Common.Utilities
{
        public static class EncryptExtension
        {
            private const string SECURITY_KEY = "TREGZ";

            public static string Encrypt(string input,string key,string pass)
            {

                // Test data
                if (input==null || input=="") 
                {
                    return "";
                }
                string data = input;

                byte[] utfdata = UTF8Encoding.UTF8.GetBytes(data);

                byte[] saltBytes = UTF8Encoding.UTF8.GetBytes(key);



                // Our symmetric encryption algorithm

                AesManaged aes = new AesManaged();



                // We're using the PBKDF2 standard for password-based key generation

                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(pass, saltBytes);



                // Setting our parameters

                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;

                aes.KeySize = aes.LegalKeySizes[0].MaxSize;



                aes.Key = rfc.GetBytes(aes.KeySize / 8);

                aes.IV = rfc.GetBytes(aes.BlockSize / 8);



                // Encryption

                ICryptoTransform encryptTransf = aes.CreateEncryptor();



                // Output stream, can be also a FileStream

                MemoryStream encryptStream = new MemoryStream();

                CryptoStream encryptor = new CryptoStream(encryptStream, encryptTransf, CryptoStreamMode.Write);



                encryptor.Write(utfdata, 0, utfdata.Length);

                encryptor.Flush();

                encryptor.Close();



                // Showing our encrypted content

                byte[] encryptBytes = encryptStream.ToArray();

                //string encryptedString = UTF8Encoding.UTF8.GetString(encryptBytes, 0, encryptBytes.Length);

                string encryptedString = Convert.ToBase64String(encryptBytes);

                Debug.WriteLine(encryptedString);



                return encryptedString;

            }



            public static string Decrypt(string base64Input, string key, string pass)
            {
                
                //byte[] encryptBytes = UTF8Encoding.UTF8.GetBytes(input);

                byte[] encryptBytes = Convert.FromBase64String(base64Input);

                byte[] saltBytes = Encoding.UTF8.GetBytes(key);


                // Our symmetric encryption algorithm

                AesManaged aes = new AesManaged();


                // We're using the PBKDF2 standard for password-based key generation

                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(pass, saltBytes);



                // Setting our parameters

                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;

                aes.KeySize = aes.LegalKeySizes[0].MaxSize;



                aes.Key = rfc.GetBytes(aes.KeySize / 8);

                aes.IV = rfc.GetBytes(aes.BlockSize / 8);



                // Now, decryption

                ICryptoTransform decryptTrans = aes.CreateDecryptor();



                // Output stream, can be also a FileStream
               

                MemoryStream decryptStream = new MemoryStream();

                CryptoStream decryptor = new CryptoStream(decryptStream, decryptTrans, CryptoStreamMode.Write);

                try
                {
                    decryptor.Write(encryptBytes, 0, encryptBytes.Length);

                    decryptor.Flush();

                    decryptor.Close();
                }
                catch (Exception ex) 
                {                
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }


            // Showing our decrypted content

            byte[] decryptBytes = decryptStream.ToArray();

                string decryptedString = UTF8Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);

                Debug.WriteLine(decryptedString);

                return decryptedString;

            }

        }


}
