using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Utilities
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        private readonly string domain;

        public ValidEmailDomainAttribute(string domain)
        {
            this.domain = domain;
        }

        public override bool IsValid(object value)
        {
            string[] strings = value.ToString().Split("@");

            return strings[1].ToUpper() == domain.ToUpper();
        }
    }
}
