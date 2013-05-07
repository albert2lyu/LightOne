using System;
using System.Linq;

namespace Bee
{
    public class ParseException : ApplicationException {
        public ParseException(string format, params string[] args)
            : base(string.Format(format, args)) {

        }
    }
}
