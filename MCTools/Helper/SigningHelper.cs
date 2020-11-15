using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MCTools.Helper
{
    class SigningHelper
    {
        private static bool Verify(byte[] data, byte[] signature)
        {
            X509Certificate2 cert = LoadPublicKey();
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (cert == null)
            {
                throw new ArgumentNullException("publicKey");
            }
            if (signature == null)
            {
                throw new ArgumentNullException("signature");
            }
            var provider = (RSACryptoServiceProvider)cert.PublicKey.Key;
            return provider.VerifyData(data, new SHA1CryptoServiceProvider(), signature);
        }

        private static X509Certificate2 LoadPublicKey()
        {
            return new X509Certificate2(Properties.Resources.cert);
        }

        public static bool VerifyImages(byte[] imageBytes)
        {
            try
            {
                List<byte[]> imageWithSignature = ByteArrayToListByteArray(imageBytes);
                if (imageWithSignature.Count != 2)
                {
                    return false;
                }
                byte[] orignalImages = imageWithSignature[0];
                byte[] signature = imageWithSignature[1];
                if (orignalImages == null || signature == null)
                {
                    return false;
                }
                return Verify(orignalImages, signature);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static List<Bitmap> GetImagesFromEncryptedBytes(byte[] data)
        {
            return ByteArrayToListBitmap(ByteArrayToListByteArray(data)[0]);
        }

        public static List<Bitmap> ByteArrayToListBitmap(byte[] data)
        {
            var mStream = new MemoryStream();
            var binFormatter = new BinaryFormatter();
            mStream.Write(data, 0, data.Length);
            mStream.Position = 0;
            return binFormatter.Deserialize(mStream) as List<Bitmap>;
        }

        public static List<byte[]> ByteArrayToListByteArray(byte[] data)
        {
            var mStream = new MemoryStream();
            var binFormatter = new BinaryFormatter();
            mStream.Write(data, 0, data.Length);
            mStream.Position = 0;
            return binFormatter.Deserialize(mStream) as List<byte[]>;
        }
    }
}
