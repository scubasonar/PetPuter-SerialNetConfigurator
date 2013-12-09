using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace Hydr0Source.PetPuter.SerialNet
{
    class SerialNetParser
    {
        //public event EventHandler<String> DataReceived; //!< event wraps around the SerialNet data transfer functions ATDU ATDx etc to treat the SerialNet like a normal serialport
        //public event EventHandler<String> ChildJoined;
        //public event EventHandler<String> ChildLost;
        public event EventHandler<SerialNetConfigEventArgs> ConfigReceived;

        public SerialNetProtocol protocol;

        public const int BUFFER_SIZE = 3000; //!< size of our receive buffer, at%H gives back a lot of data.
        public DateTime lastRX; //!< time for the last data we received for timeouts etc

        byte[] buf = new byte[BUFFER_SIZE]; // !< our buffer for incoming data to be parsed
        int index = 0; //!< where we are as far as filling the buffer goes

        //!< SerialNet parser needs a serialport to attach to
        public SerialNetParser(SerialNetProtocol _protocol, SerialPort port)
        {
            protocol = _protocol;
            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
        }

        //!< when data comes in we see if there is anything goon it in then call the general parser
        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lastRX = DateTime.Now;

            SerialPort port = sender as SerialPort;
            int count = port.BytesToRead;   
            byte[] incoming = new byte[count];
            String text;

            if (index + count < BUFFER_SIZE)
            {
                port.Read(buf, index, count);
                index += count;
            }
            else
            {
                port.Read(buf, index, BUFFER_SIZE - index);
                index = BUFFER_SIZE;
            }
            
             text = UTF8Encoding.UTF8.GetString(buf);
            {
                if (text.Contains("OK"))
                    parseIncoming(text);
            }

            
        }

        
        //! reset the parser to begin anew
        public void reset()
        {
        }

        void parseIncoming(String text)
        {
            string[] values;
            
            text = text.Replace('\r',' ');
            values = text.Split(new char[] { '\n' });
            String test = values[0];
            
            switch (values[0].TrimEnd(new char[]{' '}))
            {
                case "AT%H":
                    SerialNetConfig config = new SerialNetConfig(protocol, values);
                    if (config != null)
                        OnConfigReceived(config);
                    break;
            }

        }

        public virtual void OnConfigReceived(SerialNetConfig config)
        {
            if(ConfigReceived != null)
            {
                ConfigReceived(this, new SerialNetConfigEventArgs(config));
            }
        }
    }
}
