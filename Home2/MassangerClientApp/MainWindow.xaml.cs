using ClientMessage;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MassangerClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
   
        IPEndPoint serverPoint;
        UdpClient client;
        ObservableCollection<MessageInfo> messages;
        string name;

        public MainWindow()
        {
            InitializeComponent();
            string serverAddress = ConfigurationManager.AppSettings["serverAddress"];
            short serverPort = short.Parse(ConfigurationManager.AppSettings["serverPort"]);
            serverPoint = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
            client = new UdpClient();
            messages = new ObservableCollection<MessageInfo>();
            this.DataContext = messages;

        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {

            string message = JsonSerializer.Serialize<MessageInfo>(new MessageInfo(msgTextBox.Text, name));
            msgTextBox.Text = "";
            SendMessage(message);

        }
        
        private void msgTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter) { 
                SendButton_Click(sender, e);
            }
        }

        private void LeaveButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage("<$leave>");
        }

        private void JoinButton_Click(object sender, RoutedEventArgs e)
        {
            name = nameClient.Text;
            nameClient.Visibility = Visibility.Hidden;
            this.Title = name;
            SendMessage("<$join>");
            Listener();
        }
        private async void SendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            await client.SendAsync(data, data.Length, serverPoint);
        }
        private async void Listener()
        {
            while (true)
            {
                var data = await client.ReceiveAsync();
                MessageInfo message = JsonSerializer.Deserialize<MessageInfo>(Encoding.Unicode.GetString(data.Buffer));
                messages.Add(message);
            }
        }

        
    }
   
}