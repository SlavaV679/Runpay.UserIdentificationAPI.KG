using System;
using System.Collections.Generic;

namespace Runpay.UserIdentification.DataAccess.Payments;

public partial class Certificate
{
    public int Id { get; set; }

    /// <summary>
    /// Серийный номер сертификата
    /// </summary>
    public string SerialNumber { get; set; } = null!;

    /// <summary>
    /// поле Subject CN  (кому выдан сертификат)
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Поле Issuer CN (кем выдан)
    /// </summary>
    public string? Issuer { get; set; }

    /// <summary>
    /// Дата выдачи сертификата
    /// </summary>
    public DateTime? DateGiven { get; set; }

    /// <summary>
    /// Дата окончания срока действия сертификата
    /// </summary>
    public DateTime? DateExpire { get; set; }

    /// <summary>
    /// Состояние 1 активный, 0 неактивный
    /// </summary>
    public bool? State { get; set; }

    /// <summary>
    /// 0 -платежи только через payment.asp, 1 платежи и через web и через payment.asp
    /// </summary>
    public bool? InternetPayments { get; set; }

    /// <summary>
    /// Дата добавления записи
    /// </summary>
    public DateTime? DateAdd { get; set; }

    public string? PublicKey { get; set; }

    /// <summary>
    /// Это ссылка на эту же таблицу. Предназначена для того, чтобы когда сертификат закончится, в БД можно было бы указать сертификат, который должен его заменить. Новый сертификат надо заранее подготовить и экспортировать в файл PFX в нужную директорию. Файл должен именоваться SERIAL.pfx
    /// </summary>
    public int? NewCertificateId { get; set; }

    public int UserId { get; set; }
}
