using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using Hydr0Source.PetPuter.SerialNet;

namespace Hydr0Source.PetPuter.SerialNetConfigurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        delegate void UpdateRadioConfig(object sender, SerialNetConfigEventArgs e);
        UpdateRadioConfig UpdateRadioConfigHandler;

        SerialNetDevice radio;

        public MainWindow()
        {
            InitializeComponent();
            RoleComboBox.SelectedIndex = (int)SerialNetConfig.NODE_ROLE.UNKNOWN;
            UpdateRadioConfigHandler = radio_ConfigChanged;

            try
            {
                radio = new SerialNetDevice();
                radio.ConfigChanged += new EventHandler<SerialNetConfigEventArgs>(radio_ConfigChangedSafe);
                radio.StatusChanged += new EventHandler<SerialNetDevice.DeviceStatusChangedEventArgs>(radio_StatusChanged);
                
            }
            catch (Exception ex)
            {
                radio = null;
            }
        }

        void radio_StatusChanged(object sender, SerialNetDevice.DeviceStatusChangedEventArgs e)
        {
            switch (e.DeviceStatus)
            {
                case SerialNetDevice.SerialNetDeviceStatus.UNKNOWN:
                    StatusLabel.Content = "Unknown status";
                    break;
                case SerialNetDevice.SerialNetDeviceStatus.NO_USB:
                    StatusLabel.Content = "Please plug device into PC and connect";
                    break;
                case SerialNetDevice.SerialNetDeviceStatus.NO_SerialNet:
                    StatusLabel.Content = "USB port OK, no PetPuter detected";
                    break;
                case SerialNetDevice.SerialNetDeviceStatus.NO_NETWORK:
                    StatusLabel.Content = "PetPuter OK, not connected to network";
                    break;
                case SerialNetDevice.SerialNetDeviceStatus.OK_NETWORK:
                    StatusLabel.Content = "PetPuter online";
                    break;
                case SerialNetDevice.SerialNetDeviceStatus.READ_CONFIG:
                    StatusLabel.Content = "Reading device config";
                    break;
                case SerialNetDevice.SerialNetDeviceStatus.WRITE_CONFIG:
                    StatusLabel.Content = "Writing device config";
                    break;
            }
        }
        

        void radio_ConfigChangedSafe(object sender, SerialNetConfigEventArgs e)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, UpdateRadioConfigHandler, this, e);
        }

        //! We update the UI here to reflect the device configuration. 
        void radio_ConfigChanged(object sender, SerialNetConfigEventArgs e)
        {
            SerialNetConfig config = e.SerialNetConfig;
            //! update the UI now that we have a nice config!
            String hello = "";

       
            RoleComboBox.SelectedIndex = (int)config.role;
            
            if (config.autonet)
                AutoNetworkComboBox.SelectedIndex = 0;
            else
                AutoNetworkComboBox.SelectedIndex = 1;

            PANIDTextBox.Text = config.panIDLong.ToString(); 
            SPANIDTextBox.Text = config.panID.ToString();
            ChannelMaskTextBox.Text = config.chMask.ToString();
            ChannelTextBox.Text = config.channel.ToString();
            ShortAddressTextBox.Text = config.src.ToString();
            SNTextBox.Text = config.sn.ToString();
            TXPowerTextBox.Text = config.sn.ToString();
            //! Update the UI to match the config            
        }

        //! Generate a SerialNetConfig object based on the current UI fields 
        //! Used before write config to collect user entries
        SerialNetConfig UItoConfig()
        {            
            List<TextBox> invalid = new List<TextBox>();
            SerialNetConfig finalConfig = new SerialNetConfig();

            if(!long.TryParse(PANIDTextBox.Text,       out finalConfig.panIDLong))
                invalid.Add(PANIDTextBox);
            if(!int.TryParse(SPANIDTextBox.Text,       out finalConfig.panID))
                invalid.Add(PANIDTextBox);
            if(!long.TryParse(ChannelMaskTextBox.Text, out finalConfig.chMask))
                invalid.Add(ChannelMaskTextBox);

            finalConfig.role = (SerialNetConfig.NODE_ROLE)RoleComboBox.SelectedIndex;
            
            foreach(TextBox t in invalid)
            {
                t.Background = Brushes.Red;
            }

            return finalConfig;
        }

        private void btn_readConfig_Click(object sender, RoutedEventArgs e)
        {
            if (radio == null)
                MessageBox.Show("Please connect a device first");
            else
                radio.readConfig();
        }

        private void btn_writeConfig_Click(object sender, RoutedEventArgs e)
        {
            if (radio == null)
                MessageBox.Show("Please connect a device first");
            else
                radio.writeConfig(UItoConfig());
        }

  
       
    }
}
