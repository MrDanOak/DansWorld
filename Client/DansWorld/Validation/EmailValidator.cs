using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansWorld.GameClient.Validation
{
    /// <summary>
    /// This class is used to validate email addresses
    /// inherits from validator to ensure a validate method is called.
    /// This also means loops of foreach(IValidator) can be used to speed up validation process.
    /// </summary>
    class EmailValidator : IValidator
    {
        //a list of accepted characters in an email
        string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!#$%&'*+-/=?^_`{|}~0123456789.@";
        public bool Validate(string s)
        {
            //if its empty, not valid
            if (s.Length == 0) return false;
            //if the first or last character is a period, also an invalid email
            if (s[s.Length - 1] == '.' || s[0] == '.') return false;
            //testing to see if the email has an @ symbol and a period after the at if so, it's valid
            bool hasAt = false;
            bool hasPeriodAfterAt = false;
            for (int i = 0; i < s.Length; i++)
            {
                if (!valid.Contains(s[i])) return false;
                if (hasAt && s[i] == '@') return false;
                if (s[i] == '@') hasAt = true;
                if (hasAt && !hasPeriodAfterAt && s[i] == '.') hasPeriodAfterAt = true;
            }
            return (hasAt && hasPeriodAfterAt);
        }
    }
}
