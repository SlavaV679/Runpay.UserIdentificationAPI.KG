using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;

namespace Runpay.UserIdentification.BisinessLogic.Helpers
{
    public class RSAHelper
    {
        //private static Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Проверка подписанной строки
        /// </summary>
        public static bool VerifySignature(string originalMessage, Encoding encoding, string signedMessage, string certFileName)
        {
            try
            {
                if (!File.Exists(certFileName))
                {
                    throw new FileNotFoundException(string.Format("File [{0}] not found", certFileName));
                }

                X509Certificate2 cert = new X509Certificate2(certFileName);

                using (RSA rsa = cert.GetRSAPublicKey())
                {

                    byte[] data = encoding.GetBytes(originalMessage);
                    byte[] signaturedata = Convert.FromBase64String(signedMessage);

                    return rsa.VerifyData(data, signaturedata, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                }
            }
            catch (Exception ex)
            {
                //_log.LogError("VerifySignature error. ", ex);
                return false;
            }
        }
    }
}
