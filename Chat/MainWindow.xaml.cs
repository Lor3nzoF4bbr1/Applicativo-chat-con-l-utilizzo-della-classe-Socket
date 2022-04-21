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
        IPAddress remoteSelezionato;
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

                lstMessaggi.Items.Add(from + ": " + messaggio);
            }
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IPEndPoint remote_endpoint = new IPEndPoint(remoteSelezionato, int.Parse(txtPorta.Text));
                byte[] messaggio = Encoding.UTF8.GetBytes(txtMessaggio.Text);

                lstMessaggi.Items.Add("You: " + txtMessaggio.Text);

                socket.SendTo(messaggio, remote_endpoint);

                txtMessaggio.Text = "";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBroadcast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach(string s in lstContatti.Items)
                {
                    string[] elementi = s.Split(' ');
                    int index = elementi.Length - 1; //dato che il conteggio parte da 0
                    string ip = elementi[index];

                    IPAddress remote = IPAddress.Parse(ip);

                    IPEndPoint remote_endpoint = new IPEndPoint(remote, int.Parse(txtPorta.Text));
                    byte[] messaggio = Encoding.UTF8.GetBytes(txtMessaggio.Text);

                    socket.SendTo(messaggio, remote_endpoint);
                }

                lstMessaggi.Items.Add("(Broadcast) You: " + txtMessaggio.Text);

                txtMessaggio.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSalva_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IPAddress controllo = IPAddress.Parse(txtIP.Text); //serve solo per controllare che l'ip inserito sia corretto

                string contatto = txtNome.Text + " IP: " + txtIP.Text;
                lstContatti.Items.Add(contatto);

                txtNome.Text = "";
                txtIP.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lstContatti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string riga = lstContatti.SelectedItem.ToString();
                string[] elementi = riga.Split(' ');
                int index = elementi.Length - 1; //dato che il conteggio parte da 0
                string ip = elementi[index];

                remoteSelezionato = IPAddress.Parse(ip);

                lstMessaggi.Items.Clear();
                txtMessaggio.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
