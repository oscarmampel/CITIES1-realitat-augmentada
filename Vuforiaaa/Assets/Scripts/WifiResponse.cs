using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    [Serializable]
    class WIfiElement : IComparable<WIfiElement>
    {
        public string enc;
        public string essid;
        public string rssi;

        int IComparable<WIfiElement>.CompareTo(WIfiElement wifiElement)
        {
            if (wifiElement == null) return 1;

            int outRssi = 0;
            int thisRssi = 0;

            if (Int32.TryParse(wifiElement.rssi, out outRssi))
            {
                if(Int32.TryParse(this.rssi, out thisRssi)){
                    if (outRssi > thisRssi)
                    {
                        return 1;
                    }
                    if (thisRssi > outRssi)
                    {
                        return -1;
                    }

                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return -1;
            }

            return 0;
        }
    }

    [Serializable]
    class ApsClass
    {
        public WIfiElement[] Aps;
    }

    [Serializable]
    class ResultClass
    {
        public ApsClass result;
    }
}
