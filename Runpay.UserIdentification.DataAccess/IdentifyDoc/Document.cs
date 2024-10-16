using System;
using System.Collections.Generic;

namespace Runpay.UserIdentification.DataAccess.IdentifyDoc;

public partial class Document
{
    public int? TypeId { get; set; }

    public string? SerialNumber { get; set; }

    public DateTime? GivenDate { get; set; }

    public DateTime? ValidityDate { get; set; }

    public string? IssuedBy { get; set; }

    public string? Address { get; set; }

    public string? Country { get; set; }

    public DateTime? BirthDate { get; set; }

    public int Id { get; set; }

    public int? RegionId { get; set; }

    public bool? Gender { get; set; }

    public string? AddressResidence { get; set; }

    public string? Beneficiar { get; set; }

    public int? WorkTypeId { get; set; }

    public string? WorkPosition { get; set; }

    public string? WorkPlace { get; set; }

    public bool IsPul { get; set; }

    public string? AfilliatedParties { get; set; }

    public string? AssociatedMembers { get; set; }

    public string? FamilyMembers { get; set; }

    public int? OperationTypeId { get; set; }

    public string? WorkTypeOther { get; set; }

    public string? OperationTypeOther { get; set; }

    public string? Inn { get; set; }

    public string? Snils { get; set; }

    public string? BirthPlace { get; set; }

    public bool IsChanged { get; set; }

    public DateTime? DateChange { get; set; }

    public virtual User IdNavigation { get; set; } = null!;
}
