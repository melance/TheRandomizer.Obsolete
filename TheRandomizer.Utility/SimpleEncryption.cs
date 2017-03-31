using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheRandomizer.Utility
{
    using System;
    using System.Data;
    using System.Security.Cryptography;
    using System.IO;
    using System.Text;

    public class SimpleEncryption
    {
        private readonly Random _random;
        private readonly byte[] _key;
        private readonly AesManaged _aes;
        private readonly UTF8Encoding _encoder;

        public SimpleEncryption() : this(new byte[] { 182, 225, 186, 099, 080, 073, 095, 209, 060, 013,
                                                      097, 171, 013, 115, 152, 192, 036, 133, 211, 239,
                                                      093, 170, 243, 044, 026, 096, 254, 226, 103, 195,
                                                      093, 095 })
        {
        }

        public SimpleEncryption(byte[] key)
        {
            _random = new Random();
            _aes = new AesManaged();
            _encoder = new UTF8Encoding();
            _key = key;
        }

        public string Encrypt(string unencrypted)
        {
            var vector = new byte[16];
            _random.NextBytes(vector);
            var cryptogram = vector.Concat(Encrypt(_encoder.GetBytes(unencrypted), vector));
            return Convert.ToBase64String(cryptogram.ToArray());
        }

        public string Decrypt(string encrypted)
        {
            var cryptogram = Convert.FromBase64String(encrypted);
            if (cryptogram.Length < 17)
            {
                throw new ArgumentException("Not a valid encrypted string", "encrypted");
            }

            var vector = cryptogram.Take(16).ToArray();
            var buffer = cryptogram.Skip(16).ToArray();
            return _encoder.GetString(Decrypt(buffer, vector));
        }

        private byte[] Encrypt(byte[] buffer, byte[] vector)
        {
            var encryptor = _aes.CreateEncryptor(_key, vector);
            return Transform(buffer, encryptor);
        }

        private byte[] Decrypt(byte[] buffer, byte[] vector)
        {
            var decryptor = _aes.CreateDecryptor(_key, vector);
            return Transform(buffer, decryptor);
        }

        private byte[] Transform(byte[] buffer, ICryptoTransform transform)
        {
            var stream = new MemoryStream();
            using (var cs = new CryptoStream(stream, transform, CryptoStreamMode.Write))
            {
                cs.Write(buffer, 0, buffer.Length);
            }

            return stream.ToArray();
        }
    }
}