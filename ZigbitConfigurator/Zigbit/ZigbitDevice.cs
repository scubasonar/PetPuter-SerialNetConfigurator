using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace ZigbitConfigurator.Zigbit
{
    //!< Zigbit device running SerialNet firmware for simple and easy to setup zigbee mesh networks
    class ZigbitDevice
    {
        //! Connection status enumeration
        public enum ZigbitStatus
        {
            NO_USB = 0,  //!< Starting state, serial port closed
            NO_ZIGBIT = 1, //!< FTDI or other usb->TTL detected but no device yet
            NO_NETWORK = 2, //!< Port and device fine, but not connected to a wireless network
            OK_NETWORK = 3
        }; //!< Network joined and good to go
        

        SerialPort sPort; //!< raw serial port we talk to zigbit device through
        ZigbitConfig config; //!< config for the device with network settings etc
        ZigbitParser parser; //!< parser to handle incoming commands

        
        ZigbitStatus _DeviceStatus = ZigbitStatus.NO_USB;

        public event EventHandler<ZigbitConfigEventArgs> ConfigChanged; //!< UI can attach to this and know when our configuration changes


        public ZigbitDevice()
        {
            config = new ZigbitConfig();           
            sPort = new SerialPort(config.port, config.baud);

            sPort.Open();

            parser = new ZigbitParser(sPort);
            parser.ConfigReceived += new EventHandler<ZigbitConfigEventArgs>(parser_ConfigReceived); //!< parser will bubble up a config if it gets a full one from AT%H command                          
        }

        void parser_ConfigReceived(object sender, ZigbitConfigEventArgs e)
        {
            this.config = e.ZigbitConfig;
            if (ConfigChanged != null)
                ConfigChanged(this, new ZigbitConfigEventArgs(this.config)); //!< bubble up the config changed event so the UI can subscribe too.
        }

  

        public void readConfig()
        {
            sPort.Write("AT%H\r");
        }


        //! Broadcast a message out across the network
        public void Write(String data)
        {
            if (_DeviceStatus != ZigbitStatus.OK_NETWORK)
                throw (new Exception("Cannot write to zigbee network, not joined"));
            else
                sPort.Write("ATDU\r" + data + '\r');
        }

        //! Write a message to a specific node on the network
        public void Write(String data, int address)
        {
            if (_DeviceStatus != ZigbitStatus.OK_NETWORK)
                throw (new Exception("Cannot write to zigbee network, not joined"));
            else
                sPort.Write("ATD" + address + '\r' + data + '\r');
        }

    }
}
