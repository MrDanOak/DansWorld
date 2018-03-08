using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansWorld.GameClient.Validation
{
    public interface IValidator
    {
        bool Validate(string s);
    }
}
