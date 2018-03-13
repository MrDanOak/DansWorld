using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansWorld.GameClient.Validation
{
    class EmailValidator : IValidator
    {
        string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!#$%&'*+-/=?^_`{|}~0123456789.@";
        public bool Validate(string s)
        {
            if (s.Length == 0) return false;
            if (s[s.Length - 1] == '.' || s[0] == '.') return false;
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
