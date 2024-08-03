using System;
using System.Collections.Generic;

namespace MotoXpress3.Models;

public partial class RentalRecord
{
    public int RentalId { get; set; }

    public int BikeId { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime RentalDate { get; set; }

    public DateTime ReturnDate { get; set; }

    public decimal? TotalCost { get; set; }

    public virtual Bike? Bike { get; set; } = null!;

    public virtual AspNetUser? User { get; set; } = null!;
}