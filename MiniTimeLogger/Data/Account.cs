using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MiniTimeLogger.Support.ExceptionHandling;

namespace MiniTimeLogger.Data
{
    public static class Account
    {
        public static bool LoggedIn { get; private set; }

        public static bool Login(string username, string password)
        {
            LogDebug($"Account::[static]{GetCaller()}");
            LogGenericError(new NotImplementedException("Account::[static]Login(<username>, <password>)"));
            return true;
        }
    }
}
