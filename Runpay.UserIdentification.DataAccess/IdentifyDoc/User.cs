using System;
using System.Collections.Generic;

namespace Runpay.UserIdentification.DataAccess.IdentifyDoc;

public partial class User
{
    public int Id { get; set; }

    public string LastName { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string Idnp { get; set; } = null!;

    public bool UserFormExists { get; set; }

    public string? Email { get; set; }

    public string? Bills { get; set; }

    public int? TypeId { get; set; }

    public DateTime? UserFormDate { get; set; }

    public bool IsResident { get; set; }

    public string? ZipCode { get; set; }

    public int? IpsUserId { get; set; }

    public bool IpsUserState { get; set; }

    public virtual Document? Document { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
