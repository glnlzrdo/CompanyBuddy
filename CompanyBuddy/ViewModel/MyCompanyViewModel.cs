using CompanyBuddy.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CompanyBuddy.ViewModel
{
    class MyCompanyViewModel
    {
        SQLiteConnection dbConn;

        //Create Table 
        public async Task<bool> onCreate(string DB_PATH)
        {
            try
            {
                if (!CheckFileExists(DB_PATH).Result)
                {
                    using (dbConn = new SQLiteConnection(DB_PATH))
                    {
                        dbConn.CreateTable<Company>();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private async Task<bool> CheckFileExists(string fileName)
        {
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Update Company
        public void UpdateCompany(Company newCompany)
        {
            var dbConn = new SQLiteConnection(App.DB_PATH);
            var myCompanyDetails = dbConn.Table<Company>().Where(c => c.Id == 1).FirstOrDefault();
            myCompanyDetails.CompanyName = newCompany.CompanyName;
            myCompanyDetails.Address = newCompany.Address;
            myCompanyDetails.Phone = newCompany.Phone;
            myCompanyDetails.CutOff = newCompany.CutOff;
            dbConn.RunInTransaction(() => { dbConn.Update(myCompanyDetails); });
        }

        //Read Company for editing
        public Company AccessMyCompany()
        {
            var dbConn = new SQLiteConnection(App.DB_PATH);
            var myCompanyDetails = dbConn.Table<Company>().Where(c => c.Id == 1).FirstOrDefault();
            return myCompanyDetails;
        }

        //Read or Initialize Company
        public List<Company> ReadCompany()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<Company> myCompany = dbConn.Table<Company>().ToList<Company>();
                if (myCompany.Count == 0)
                {
                    Company companyInfo = new Company("MyCompanyName", "#000 Street, Town, City, State", "00-000-0000", "15-30");
                    dbConn.RunInTransaction(() => { dbConn.Insert(companyInfo); });
                    myCompany.Add(companyInfo);
                }
                return myCompany;
            }
        }

        
    }
}
