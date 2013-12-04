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

using ZigbitConfigurator.Zigbit;

namespace ZigbitConfigurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ZigbitDevice radio;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                radio = new ZigbitDevice();
                radio.ConfigChanged += new EventHandler<ZigbitConfigEventArgs>(radio_ConfigChanged);
            }
            catch (Exception ex)
            {
                radio = null;
            }
        }

        //! Parser bubbles up to Zigbit device that bubbles up to here with a nice config
        void radio_ConfigChanged(object sender, ZigbitConfigEventArgs e)
        {
            ZigbitConfig config = e.ZigbitConfig;
            //! update the UI now that we have a nice config!

        }

        private void btn_readCOnfig_Click(object sender, RoutedEventArgs e)
        {
            radio.readConfig();
        }
       
    }
}
