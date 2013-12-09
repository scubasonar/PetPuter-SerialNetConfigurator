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

        public SerialNetProtocol protocol;

        public string port = "COM4";
        public int baud = 38400;

        //! SerialNet config parameters
        public long panIDLong = 0; //!< Extended PAN ID
        public int panID = 0; //!< short PAN ID       
        public int channel = 0; //!< current radio channel
        public long chMask = 0; //!< channel mask (should really be a bit string??)        
        public Boolean autonet = false; //!< automatic networking mode enabled        
        public NODE_ROLE role = NODE_ROLE.UNKNOWN; //!< role of this device in the network

        public int src = 0; //!< this devices short network address
        public int sn = 0; //!< this devices long network address
        public int syncPeriod = 0; //!< indirect poll rate (??)
        public int txPWR = 0; //!< transmit power 
        public int childrenCount = 0;
        public List<int> childrenIds = new List<int>();

        public SerialNetConfig()
        {
            String test = "";
        }

        public SerialNetConfig(SerialNetProtocol _protocol)
        {
            protocol = _protocol;
        }

        public SerialNetConfig(SerialNetProtocol _protocol, string[] values)
        {
            protocol = _protocol;

            if (values[0].Contains("AT%H"))
            {
                ReadFullConfig(values);
            }
        }

        void ReadFullConfig(string[] values)
        {
            int _role;
            string command = "", data = "";
            int i = 0;
            int val = 0;

            char[] d = new char[] { ':' };

            if (values[i].Contains("AT%H"))
                i++;

            //! We can't expect the values to come in a set order because some won't show up depending on the tag config
            for (; i < values.Length; i++)
            {
                string[] fields = values[i].Split(d);
                if (!values[i].Contains(':'))
                    continue;

                if (protocol.configValueDict.TryGetValue(fields[0], out val))
                {
                    switch (val)
                    {
                        case (int)SerialNetProtocol.CFG_COMMANDS.WPANID:
                            int.TryParse(fields[1], out panID);                            
                            break;
                        case (int)SerialNetProtocol.CFG_COMMANDS.WCHAN:
                            int.TryParse(fields[1], out channel);                           
                            break;
                        case (int)SerialNetProtocol.CFG_COMMANDS.WCHMASK:
                            long.TryParse(fields[1], out chMask);                           
                            break;
                        case (int)SerialNetProtocol.CFG_COMMANDS.WCHPAGE:
                            //!< not used yet D:
                            break;
                        case (int)SerialNetProtocol.CFG_COMMANDS.WAUTONET:
                            int _autonet = 0;
                            int.TryParse(fields[1], out _autonet);                            
                            autonet = (_autonet == 1);
                            break;
                        case (int)SerialNetProtocol.CFG_COMMANDS.WROLE:
                            int.TryParse(fields[1], out _role);                            
                            switch (_role)
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
                            break;
                        case (int)SerialNetProtocol.CFG_COMMANDS.GSN:
                            int.TryParse(fields[1], out sn);                            
                            break;
                        case (int)SerialNetProtocol.CFG_COMMANDS.WNWKPANID:
                            long.TryParse(fields[1], out panIDLong);
                            break;
                    } 
                }
            }      
        }
    }
}
