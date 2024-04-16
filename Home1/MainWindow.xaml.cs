using System.Net.Sockets;
using System.Net;
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

namespace client_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string address = "127.0.0.1";
        private int port = 8080;
        private IPEndPoint ipPoint;
        private IPEndPoint remotePoint;
        private UdpClient client;

        public MainWindow()
        {
            InitializeComponent();
            ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
            remotePoint = new IPEndPoint(IPAddress.Any, 0);
            client = new UdpClient();
        }

        private void Search_btn(object sender, RoutedEventArgs e)
        {
            string index = street_index.Text;
            byte[] data = Encoding.Unicode.GetBytes(index);

            client.Send(data, data.Length, ipPoint); // Відправити дані

            string requestInfo = $"Sent request: {index}";
            ListBoxItem item = new ListBoxItem();
            item.Content = requestInfo;
            listBox.Items.Add(item);

            try
            {
                data = client.Receive(ref remotePoint); // Отримати відповідь
                string response = Encoding.Unicode.GetString(data);
                string responseInfo = $"Received response: {response}";
                ListBoxItem responseItem = new ListBoxItem();
                responseItem.Content = responseInfo;
                listBox.Items.Add(responseItem);
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Error receiving response: " + ex.Message);
            }
        }
    }


}