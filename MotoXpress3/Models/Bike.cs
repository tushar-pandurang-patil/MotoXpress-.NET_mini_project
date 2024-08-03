using System;
using System.Collections.Generic;

namespace MotoXpress3.Models;

public partial class Bike
{
    public int BikeId { get; set; }

    public string BikeName { get; set; } = null!;

    public string BikeType { get; set; } = null!;

    public int PerDayRental { get; set; }

    public bool Availability { get; set; }

    public string BikeImage { get; set; } = null!;

    public virtual ICollection<RentalRecord> RentalRecords { get; set; } = new List<RentalRecord>();
}
