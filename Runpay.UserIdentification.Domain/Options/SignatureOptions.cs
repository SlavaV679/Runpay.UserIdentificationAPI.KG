namespace Runpay.UserIdentification.Domain.Options
{
    public class SignatureOptions
    {
        /// <summary>
        /// Path to public certificate
        /// </summary>
        public string PublicCertPath { get; set; }

        /// <summary>
        /// Is need to validate signature
        /// </summary>
        public bool ValidateSignature { get; set; }
    }
}
