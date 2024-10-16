namespace Runpay.UserIdentification.DataAccess.IdentifyDoc;

public partial class Request
{
    public int Id { get; set; }

    public string PosCode { get; set; } = null!;

    public string DocType { get; set; } = null!;

    public Guid Guid { get; set; }

    public DateTime DateAdd { get; set; }

    public bool IsRegistered { get; set; }

    public int? UserId { get; set; }

    public string? CodeData { get; set; }

    public int? RegistrationId { get; set; }

    public bool IsLocked { get; set; }

    public DateTime? DateRegistration { get; set; }

    public DateTime? DateEdit { get; set; }

    public string? Note { get; set; }

    public int? UserEdit { get; set; }

    public bool IsEdited { get; set; }

    public int? RegistrationTypeId { get; set; }

    public int? BillTypeId { get; set; }

    public DateTime? SyncDateWithEmoney { get; set; }

    public int? AttractionTypeId { get; set; }

    public string? FromIp { get; set; }

    public bool IsDeclined { get; set; }

    public bool? IsConfirmed { get; set; }

    public int? RequestStatusId { get; set; }

    public int? SiadocumentId { get; set; }

    public string? SiaDocumentNumber { get; set; }

    public string? Phone { get; set; }

    public bool NeedElevateRegistrationType { get; set; }

    public long? OuterId { get; set; }

    public virtual User? User { get; set; }
}
