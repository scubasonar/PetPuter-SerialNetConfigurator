using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hydr0Source.PetPuter.SerialNet
{
    public class SerialNetConfigEventArgs : EventArgs
    {
        public SerialNetConfigEventArgs(SerialNetConfig config)
        {
            SerialNetConfig = config;
        }

        public SerialNetConfig SerialNetConfig { get; private set; }
    }

    public class SerialNetConfig
    {
        public enum NODE_ROLE { COORDINATOR = 0, ROUTER = 1, ENDDEVICE = 2, UNKNOWN = 3 };

        public string port = "COM4";
        public int baud = 38400;

        //! SerialNet config parameters
        public long panID = 0; //!< Extended PAN ID
        public int sPanID = 0; //!< short PAN ID       
        public int channel = 0; //!< current radio channel
        public long chMask = 0; //!< channel mask (should really be a bit string??)        
        public Boolean autonet = false; //!< automatic networking mode enabled        
        public NODE_ROLE role = NODE_ROLE.UNKNOWN; //!< role of this device in the network

        public int src = 0; //!< this devices short network address
        public int sn = 0; //!< this devices long network address
        public int syncPeriod = 0; //!< indirect poll rate (??)
        public int txPWR = 0; //!< transmit power (


        public SerialNetConfig()
        {
        }

        public SerialNetConfig(string[] values)
        {
            if (values[0].Contains("AT%H"))
            {
                ReadFullConfig(values);
            }
        }

        void ReadFullConfig(string[] values)
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
                    role = NODE_ROLE.COORDINATOR;
                    break;
                case 1:
                    role = NODE_ROLE.ROUTER;
                    break;
                case 2:
                    role = NODE_ROLE.ENDDEVICE;
                    break;
                default:
                    role = NODE_ROLE.UNKNOWN;
                    break;
            }

            src = int.Parse(values[10].Split(new char[] { ':' })[1]);
            syncPeriod = int.Parse(values[11].Split(new char[] { ':' })[1]);
            txPWR = int.Parse(values[12].Split(new char[] { ':' })[1]);

            sn = int.Parse(values[25].Split(new char[] { ':' })[1]);            
        }
    }
}
