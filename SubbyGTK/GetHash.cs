using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SubbyGTK
{
    internal class MovieHasher
    {
        public static string GetHash(string filename)
        {
            return ToHexadecimal(ComputeMovieHash(filename));
        }

        public static long GetByteSize(string filename)
        {
            var f = new FileInfo(filename);
            return f.Length;
        }

        private static byte[] ComputeMovieHash(string filename)
        {
            byte[] result;
            using (Stream input = File.OpenRead(filename))
            {
                result = ComputeMovieHash(input);
            }
            return result;
        }

        private static byte[] ComputeMovieHash(Stream input)
        {
            long streamsize = input.Length;
            long lhash = streamsize;

            long i = 0;
            var buffer = new byte[sizeof (long)];
            while (i < 65536/sizeof (long) && (input.Read(buffer, 0, sizeof (long)) > 0))
            {
                i++;
                lhash += BitConverter.ToInt64(buffer, 0);
            }

            input.Position = Math.Max(0, streamsize - 65536);
            i = 0;
            while (i < 65536/sizeof (long) && (input.Read(buffer, 0, sizeof (long)) > 0))
            {
                i++;
                lhash += BitConverter.ToInt64(buffer, 0);
            }
            input.Close();
            byte[] result = BitConverter.GetBytes(lhash);
            Array.Reverse(result);
            return result;
        }

        private static string ToHexadecimal(byte[] bytes)
        {
            var hexBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                hexBuilder.Append(bytes[i].ToString("x2"));
            }
            return hexBuilder.ToString();
        }

        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}