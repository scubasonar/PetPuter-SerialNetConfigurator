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

using ZigbitConfigurator.Zigbit;

namespace ZigbitConfigurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        delegate void UpdateRadioConfig(object sender, ZigbitConfigEventArgs e);
        UpdateRadioConfig UpdateRadioConfigHandler;

        ZigbitDevice radio;

        public MainWindow()
        {
            InitializeComponent();
            RoleComboBox.SelectedIndex = (int)ZigbitConfig.RADIO_ROLE.UNKNOWN;
            UpdateRadioConfigHandler = radio_ConfigChanged;

            try
            {
                radio = new ZigbitDevice();
                radio.ConfigChanged += new EventHandler<ZigbitConfigEventArgs>(radio_ConfigChangedSafe);
            }
            catch (Exception ex)
            {
                radio = null;
            }
        }

        void radio_ConfigChangedSafe(object sender, ZigbitConfigEventArgs e)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, UpdateRadioConfigHandler, this, e);
        }

        //! We update the UI here to reflect the device configuration. 
        void radio_ConfigChanged(object sender, ZigbitConfigEventArgs e)
        {
            ZigbitConfig config = e.ZigbitConfig;
            //! update the UI now that we have a nice config!
            String hello = "";

       
            RoleComboBox.SelectedIndex = (int)config.role;
            
            if (config.autonet)
                AutoNetworkComboBox.SelectedIndex = 0;
            else
                AutoNetworkComboBox.SelectedIndex = 1;

            PANIDTextBox.Text = config.panID.ToString(); 
            SPANIDTextBox.Text = config.sPanID.ToString();
            ChannelMaskTextBox.Text = config.chMask.ToString();
            ChannelTextBox.Text = config.channel.ToString();
            ShortAddressTextBox.Text = config.src.ToString();
            SNTextBox.Text = config.sn.ToString();
            TXPowerTextBox.Text = config.sn.ToString();
            //! Update the UI to match the config            
        }

        //! Generate a ZigbitConfig object based on the current UI fields 
        //! Used before write config to collect user entries
        ZigbitConfig UItoConfig()
        {
            ZigbitConfig finalConfig = new ZigbitConfig();
            long.TryParse(PANIDTextBox.Text,       out finalConfig.panID);
            int.TryParse(SPANIDTextBox.Text,       out finalConfig.sPanID);
            long.TryParse(ChannelMaskTextBox.Text, out finalConfig.chMask);

            return finalConfig;
        }

        private void btn_readCOnfig_Click(object sender, RoutedEventArgs e)
        {
            radio.readConfig();
        }
       
    }
}
