using LocalizationHelper;

namespace Runpay.UserIdentification.Domain.Models
{
    public class TAException : Exception
    {
        private static string descrLocalizationHelper;

        public TAException(int result, string descr, string devMsg = null) : base(descr)
        {
            base.Data["ERROR_CODE"] = result;
            base.Data["ERROR_USR"] = descr;
            base.Data["ERROR_DBG"] = devMsg;
        }

        public TAException(int result, LocalizationResourceKey localizationResourceKey, string devMsg = null, params object[] args)
            : base(descrLocalizationHelper = string.Format(Localization.GetResourceString(localizationResourceKey, LocalizationService.TerminalApi), args))
        {
            base.Data["ERROR_CODE"] = result;
            base.Data["ERROR_USR"] = descrLocalizationHelper;
            base.Data["ERROR_DBG"] = devMsg;
        }

        public TAException(int result, LocalizationResourceKey localizationResourceKey, string devMsg = null, string threeLetterISOLanguageName = null)
            : base(descrLocalizationHelper = Localization.GetResourceString(localizationResourceKey, LocalizationService.TerminalApi, threeLetterISOLanguageName))
        {
            base.Data["ERROR_CODE"] = result;
            base.Data["ERROR_USR"] = descrLocalizationHelper;
            base.Data["ERROR_DBG"] = devMsg;
        }
    }
}
