using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace ZigbitConfigurator.Zigbit
{
    class ZigbitDevice
    {
        public SerialPort sPort;
        public ZigbitConfig config;
        public ZigbitParser parser;

        public ZigbitDevice()
        {
            config = new ZigbitConfig();           
            sPort = new SerialPort(config.port, config.baud);
            sPort.Open();

            parser = new ZigbitParser(sPort);
        }

        public void readConfig()
        {
            sPort.Write("AT%H\r");
        }
    }
}
