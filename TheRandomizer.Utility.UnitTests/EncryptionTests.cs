using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheRandomizer.Utility;
using System.Diagnostics;

namespace TheRandomizer.Utility.UnitTests
{
    [TestClass]
    public class EncryptionTests
    {
        [TestMethod]
        public void EncryptionTest()
        {
            const string HELLO_WORLD = "Hello World!";
            var encrypt = new SimpleEncryption();
            var encryptedValue = encrypt.Encrypt(HELLO_WORLD);
            var decryptedValue = encrypt.Decrypt(encryptedValue);
            Debug.WriteLine($"Encrypted Value: {encryptedValue}");
            Debug.WriteLine($"Decrypted Value: {decryptedValue}");
            Assert.AreEqual(HELLO_WORLD, decryptedValue);
        }

        [TestMethod]
        public void EncryptString()
        {
            var encrypt = new SimpleEncryption();
            var encryptedValue = encrypt.Encrypt("fypGSznFBZNjDX-tiBKuD3Ox");
            
            Debug.WriteLine($"Encrypted Value: {encryptedValue}");
            Debug.WriteLine($"Decrypted Value: {encrypt.Decrypt(encryptedValue)}");
        }
    }
}
