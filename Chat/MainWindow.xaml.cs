using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;
using System.Windows.Threading;

namespace Chat
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket socket = null;
        DispatcherTimer dTimer=null;
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                //internetwork=ipv4, dgram=datagram
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                IPAddress local_address = IPAddress.Any;
                IPEndPoint local_endpoint = new IPEndPoint(local_address.MapToIPv4(), 65000);

                socket.Bind(local_endpoint);

                socket.Blocking = false;
                socket.EnableBroadcast = true;

                dTimer = new DispatcherTimer();
                dTimer.Tick += new EventHandler (aggiornamento_dTimer); //cosa fare
                dTimer.Interval += new TimeSpan(0, 0, 0, 0, 250); //ogni quanto vado a leggere il socket 250ms
                dTimer.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void aggiornamento_dTimer (object sender, EventArgs e)
        {
            int nBytes = 0;

            if ((nBytes = socket.Available) > 0)
            {
                //RICEZIONI DEI CARATTERI IN ATTESA
                byte[] buffer = new byte[nBytes];

                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                nBytes = socket.ReceiveFrom(buffer, ref remoteEndPoint);
                string from = ((IPEndPoint)remoteEndPoint).Address.ToString();
                string messaggio = Encoding.UTF8.GetString(buffer, 0, nBytes);

                lstRoba.Items.Add(from + ": " + messaggio);
            }
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IPAddress remote = IPAddress.Parse(txtIP.Text);
                IPEndPoint remote_endpoint = new IPEndPoint(remote, int.Parse(txtPorta.Text));
                byte[] messaggio = Encoding.UTF8.GetBytes(txtMessaggio.Text);
                socket.SendTo(messaggio, remote_endpoint);

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
