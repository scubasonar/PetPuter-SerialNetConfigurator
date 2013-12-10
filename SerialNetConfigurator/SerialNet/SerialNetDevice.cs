using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace Hydr0Source.PetPuter.SerialNet
{


    //!< SerialNet device running SerialNet firmware for simple and easy to setup zigbee mesh networks
    class SerialNetDevice
    {
        //! Connection status enumeration
        public enum SerialNetDeviceStatus
        {
            NO_USB = 0,  //!< Starting state, serial port closed
            NO_SerialNet = 1, //!< FTDI or other usb->TTL detected but no device yet
            NO_NETWORK = 2, //!< Port and device fine, but not connected to a wireless network
            OK_NETWORK = 3,
            READ_CONFIG,
            WRITE_CONFIG,
            VERIFY_CONFIG,
            UNKNOWN,

        }; //!< Network joined and good to go

        public class DeviceStatusChangedEventArgs : EventArgs
        {

            public DeviceStatusChangedEventArgs(SerialNetDeviceStatus status)
            {
                DeviceStatus = status;
            }

            public SerialNetDeviceStatus DeviceStatus { get; private set; }
        }



        public SerialPort sPort; //!< raw serial port we talk to SerialNet device through
        public SerialNetConfig config; //!< config for the device with network settings etc
        public SerialNetParser parser; //!< parser to handle incoming commands
        public SerialNetProtocol protocol; //!< Protocol with commands etc


        SerialNetDeviceStatus _DeviceStatus = SerialNetDeviceStatus.UNKNOWN;

        public SerialNetDeviceStatus DeviceStatus
        {
            get
            {
                return _DeviceStatus;
            }
            set
            {
                _DeviceStatus = value;
                if (StatusChanged != null)
                    StatusChanged(this, new DeviceStatusChangedEventArgs(value));
            }
        }

        public event EventHandler<SerialNetConfigEventArgs> ConfigChanged; //!< UI can attach to this and know when our configuration changes
        public event EventHandler<DeviceStatusChangedEventArgs> StatusChanged; //!< Let's UI know when we lose connection, connect to network, write config, etc.



        public SerialNetDevice()
        {
            protocol = new SerialNetProtocol();
            config = new SerialNetConfig(protocol);
            sPort = new SerialPort(config.port, config.baud);

            sPort.Open();

            parser = new SerialNetParser(protocol, sPort);
            parser.ConfigReceived += new EventHandler<SerialNetConfigEventArgs>(parser_ConfigReceived); //!< parser will bubble up a config if it gets a full one from AT%H command       

        }

        void parser_ConfigReceived(object sender, SerialNetConfigEventArgs e)
        {
            this.config = e.SerialNetConfig;
            if (ConfigChanged != null)
                ConfigChanged(this, new SerialNetConfigEventArgs(this.config)); //!< bubble up the config changed event so the UI can subscribe too.
        }



        public void readConfig()
        {
            sPort.Write("AT%H\r");
        }

        public void writeConfig(SerialNetConfig configB)
        {
            if (DeviceStatus == SerialNetDeviceStatus.OK_NETWORK)
                sPort.Write("AT+LEAVE\r");

            if (config.role != configB.role)
            {
                sPort.Write("AT+WROLE=" + configB.role.ToString() + '\r');
            }

            if (config.autonet != configB.autonet)
            {
                if (configB.autonet)
                    sPort.Write("AT+WAUTONET=1\r");
                else
                    sPort.Write("AT+WAUTONET=0\r");
            }



            readConfig();
        }

        //! Broadcast a message out across the network
        public void Write(String data)
        {
            if (DeviceStatus != SerialNetDeviceStatus.OK_NETWORK)
                throw (new Exception("Cannot write to zigbee network, not joined"));
            else
                sPort.Write("ATDU\r" + data + '\r');
        }

        //! Write a message to a specific node on the network
        public void Write(String data, int address)
        {
            if (DeviceStatus != SerialNetDeviceStatus.OK_NETWORK)
                throw (new Exception("Cannot write to zigbee network, not joined"));
            else
                sPort.Write("ATD" + address + '\r' + data + '\r');
        }

    }
}
