using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZigbitConfigurator.Zigbit
{
    public class ZigbitConfigEventArgs : EventArgs
    {
        public ZigbitConfigEventArgs(ZigbitConfig config)
        {
            ZigbitConfig = config;
        }

        public ZigbitConfig ZigbitConfig { get; private set; }
    }

    public class ZigbitConfig
    {
        enum RADIO_ROLE { COORDINATOR = 0, ROUTER = 1, ENDDEVICE = 2, UNKNOWN = 3 };

        public string port = "COM4";
        public int baud = 38400;

        //! SerialNet config parameters
        public long panID = 0; //!< Extended PAN ID
        public int sPanID = 0; //!< short PAN ID       
        public int channel = 0; //!< current radio channel
        public long chMask = 0; //!< channel mask (should really be a bit string??)        
        public Boolean autonet = false; //!< automatic networking mode enabled        
        RADIO_ROLE role = RADIO_ROLE.UNKNOWN; //!< role of this device in the network

        public int src = 0; //!< this devices short network address
        public int sn = 0; //!< this devices long network address
        public int syncPeriod = 0; //!< indirect poll rate (??)
        public int txPWR = 0; //!< transmit power (


        public ZigbitConfig()
        {
        }

        public ZigbitConfig(string[] values)
        {
            if (values[0].Contains("AT%H"))
            {
                readFullConfig(values);
            }
        }

        void readFullConfig(string[] values)
        {
            int _role;

            panID = int.Parse(values[1].Split(new char[] { ':' })[1]);
            sPanID = int.Parse(values[2].Split(new char[] { ':' })[1]);
            channel = int.Parse(values[3].Split(new char[] { ':' })[1]);
            chMask = long.Parse(values[4].Split(new char[] { ':' })[1]);
            // 5
            // 6
            if (int.Parse(values[7].Split(new char[] { ':' })[1]) == 1)
                autonet = true;
            else
                autonet = false;
            
            // 8
            _role = int.Parse(values[9].Split(new char[] { ':' })[1]);
            switch(_role)
            {
                case 0:
                    role = RADIO_ROLE.COORDINATOR;
                    break;
                case 1:
                    role = RADIO_ROLE.ROUTER;
                    break;
                case 2:
                    role = RADIO_ROLE.ENDDEVICE;
                    break;
                default:
                    role = RADIO_ROLE.UNKNOWN;
                    break;
            }

            src = int.Parse(values[10].Split(new char[] { ':' })[1]);
            syncPeriod = int.Parse(values[11].Split(new char[] { ':' })[1]);
            txPWR = int.Parse(values[12].Split(new char[] { ':' })[1]);

            sn = int.Parse(values[25].Split(new char[] { ':' })[1]);
            string test = "hello";
        }
    }
}
