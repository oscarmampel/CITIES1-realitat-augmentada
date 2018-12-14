using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiConn
{
    public interface IConnectionCallback
    {
        void Connect();

        void Disconnect();
    }
}
