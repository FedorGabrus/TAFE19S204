using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartFinance.Models
{

    public class Appointment
    {

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [NotNull, Unique]
        public string Name { get; set; }

        [NotNull]
        public string DateOfAppt { get; set; }

        [NotNull]
        public string TimeOfAppt { get; set; }

    }
}
