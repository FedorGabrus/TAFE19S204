using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace StartFinance.Models
{
    class PersonalInfo
    {
        private DateTime dob;

        [PrimaryKey, AutoIncrement]
        public int PersonalID
        {
            set; get;
        }

        [NotNull]
        public string FirstName
        {
            set; get;
        }

        [NotNull]
        public string LastName
        {
            set; get;
        }

        [NotNull]
        public DateTime DOB
        {
            set; get;
        }

        [NotNull]
        public string Gender
        {
            set; get;
        }

        public string EmailAddress
        {
            set; get;
        }

        public string MobilePhone
        {
            set; get;
        }

        public string FormatFOB
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(DOB.Day);
                sb.Append("/");
                sb.Append(DOB.Month);
                sb.Append("/");
                sb.Append(DOB.Year);
                return sb.ToString();
            }
        }
    }
}
