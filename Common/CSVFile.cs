using Common.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class CSVFile
    {
        public static string FindFile(string filename)
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            FileInfo[] files;

            while ((files = dir.GetFiles(filename)).Length == 0 && dir.Parent != null)
                dir = dir.Parent;

            if (files.Length == 1)
                return Path.Combine(dir.FullName, filename);
            else
                return null;
        }

        public static IEnumerable<Customer> ReadFile(string filename)
        {
            StreamReader sr = new StreamReader(filename);

            var fieldsline = sr.ReadLine();

            var fields = fieldsline.Split(';');

            var mapping = new Dictionary<string, string>
            {
                { "KUNDENR_ANONYMISERT", "CustomerId" },
                { "VALID_FROM_DTTM",     "ValidFrom" },
                { "KUNDE_POSTNR",        "PostCode" },
                { "KUNDE_POSTSTED",      "City" },
                { "KUNDEANSVARLIG_ID",   "ClerkId" },
                { "BANK_ID",             "BankId" },
                { "SAMTYKKE_FORSIKRING", "ConsentInsurance" },
                { "SAMTYKKE_BANK",       "ConsentBank" }
            };

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                var parts = line.Split(';');
                var customer = new Customer();

                for (int i = 0; i < parts.Length; i++)
                {
                    PropertyInfo pi = typeof(Customer).GetProperty(mapping[fields[i]]);
                    if (pi.PropertyType == typeof(string))
                        pi.SetValue(customer, parts[i]);
                    else if (pi.PropertyType == typeof(DateTime))
                        pi.SetValue(customer, ConvertDate(parts[i]));

                }

                yield return customer;
            }

            sr.Close();
        }

        public static DateTime ConvertDate(string v)
        {
            return DateTime.ParseExact(v, "ddMMMyyyy h:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}