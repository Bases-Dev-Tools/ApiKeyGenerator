using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.ComponentModel.Design;

namespace OnBaseAPIKeyGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            start:
            Console.WriteLine("- Select a function below to begin ----------------");
            Console.WriteLine("     Press 1 to create an API Key.");
            Console.WriteLine("     Press 2 to decrypt an API Key.");
            var selectChar = Console.ReadKey(true);
            switch (selectChar.Key)
            {
                case ConsoleKey.D1:
                    Selection1();
                    Console.Clear();
                    goto start;
                case ConsoleKey.D2:
                    Selection2();
                    Console.Clear();
                    goto start;
                default:
                    Console.Clear();
                    goto start;
            }
        }

        private static void Selection1()
        {
            Console.WriteLine("Enter your Username:");
            string usr = Console.ReadLine();
            string pwd = MaskPassword();
            string apikey = CreateApiKey(usr, pwd);
            Console.WriteLine("Copy the API Key and hit any key to return to the main menu.");
            Console.WriteLine(apikey);
            Console.ReadLine();
        }
        private static void Selection2()
        {
            Console.WriteLine("Enter the API Key");
            string key = Console.ReadLine();
            string[] pair = DecryptApiKey(key);
            Console.WriteLine($"Username: {pair[0]}");
            Console.WriteLine($"Password: {pair[1]}");
            Console.WriteLine("Hit any key to return to the main menu.");
            Console.ReadLine();
        }
        private static string MaskPassword()
        {
            bool finished = false;
            Console.WriteLine("Please Enter your password, when finished press enter.");
            string password = string.Empty;
            while (!finished)
            {
                var chr = Console.ReadKey(true);
                if (chr.Key == ConsoleKey.Enter)
                    finished = true;
                else
                {
                    password = password + chr.KeyChar.ToString();
                    Console.Write("*");
                }

            }
            Console.WriteLine();
            return password;
        }

        //Replace this Key with your own 64 character key
        private static string _key = "RTCQSWOWR3VG1YQnrDe+VUv+ZXFF+gfFs2c4FtZTduc=";
        private static string CreateApiKey(string username, string password)
        {
            string combo = $"{username}:::{password}";
            return AuthTools.EncryptString(_key, combo);
        }
        private static string[] DecryptApiKey(string apiKey)
        {
            string combo = AuthTools.DecryptString(_key, apiKey);
            string[] seperators = { ":::" };
            return combo.Split(seperators, StringSplitOptions.None);
        }
    }   
    public static class AuthTools
    {
        public static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }
        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }        
    }

}
