using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MVC_projekt_Peole_kutsumine.Models
{
    public class GuestContext: DbContext
    {
        public DbSet<Guest> Guests { get; set; }
    }
}