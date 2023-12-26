using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class StaticPage
{
    public int StaticPageId { get; set; }

    public string Name { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string? Content { get; set; }

    public string? PageTitle { get; set; }

    public string? MetaKeyword { get; set; }

    public string? MetaDescription { get; set; }

    public bool IsActive { get; set; }

    public DateTime AddedDate { get; set; }

    public DateTime? ModifyDate { get; set; }

    public string? Ipaddress { get; set; }

    public int? PageSequence { get; set; }
}
