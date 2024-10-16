using Runpay.UserIdentification.Domain.Enums;
using Runpay.UserIdentification.Domain.Extensions;

namespace Runpay.UserIdentification.Domain.Models
{
    public class IdentifyException : Exception
    {
        /// <summary>
        /// Код ошибки
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        /// Сообщение ошибки для пользователя
        /// </summary>
        public string ErrorUserMessage { get; set; }
        /// <summary>
        /// Сообщение ошибки для разработчика
        /// </summary>
        public string ErrorDebugMessage { get; set; }

        public IdentifyException(int result, string descr, string devMsg = null) : base(descr)
        {
            ErrorCode = result;
            ErrorUserMessage = descr;
            ErrorDebugMessage = devMsg;
        }

        public IdentifyException(ErrorCodeEnum errorCode)
        {
            ErrorCode = (int)errorCode;
            ErrorUserMessage = errorCode.GetDescriptionByEnumValue();
            //ErrorDBG = devMsg;
        }

        public IdentifyException(ErrorCodeEnum errorCode, string parameters)
        {
            ErrorCode = (int)errorCode;
            ErrorUserMessage = errorCode.GetDescriptionByEnumValue() + ": " + parameters;
            //ErrorDBG = devMsg;
        }
    }
}
