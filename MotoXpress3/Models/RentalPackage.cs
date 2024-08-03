using System;
using System.Collections.Generic;

namespace MotoXpress3.Models;

public partial class RentalPackage
{
    public int PackageId { get; set; }

    public string PackageName { get; set; } = null!;

    public string PackageType { get; set; } = null!;
}
