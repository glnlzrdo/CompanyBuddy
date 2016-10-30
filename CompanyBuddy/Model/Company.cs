using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace CompanyBuddy.Model
{
    public class Company
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CutOff { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public Company()
        {

        }

        public Company(string _companyName, string _address, string _phone, string _cutOff)
        {
            CompanyName = _companyName;
            Address = _address;
            Phone = _phone;
            CutOff = _cutOff;
            
        }

    }
}
