using System.ComponentModel;

namespace Runpay.UserIdentification.Domain.Enums
{
    public enum ErrorCodeEnum
    {
        [Description("Операция выполнена успешно, пользователь с указанными данными идентифицирован")]
        SuccessResultCode = 0,

        [Description("Анкета уже существует с этим номером")]
        ProfileAlreadyExists = 1,

        [Description("Внутренняя ошибка сервера")]
        InternalServerError = -1,

        [Description("Некорректный формат запроса, отсутствуют обязательные параметры")]
        IncorrectRequestFormat = 2,

        [Description("Неверный формат даты рождения")]
        IncorrectDateOfBirth = 3,

        [Description("Подпись не действительна")]
        SignatureIsNotValid = 4,

        [Description("Ошибка сервера")]
        ServerError = 500,
    }
}
