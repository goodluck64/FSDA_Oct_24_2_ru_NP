using System.IO;
using System.Net;
using System.Net.Sockets;
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
using System.Timers;
using Timer = System.Timers.Timer;

namespace TCPChat;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private TcpClient _tcpClient;
    private StreamReader? _reader;
    private StreamWriter? _writer;
    private SynchronizationContext _synchronizationContext;
    private Timer _timer;

    public MainWindow()
    {
        InitializeComponent();
        
        _tcpClient = new TcpClient();
        _synchronizationContext = SynchronizationContext.Current!;

        _timer = new Timer
        {
            Interval = 5 * 1000,
            AutoReset = true,
            Enabled = true
        };

        _timer.Elapsed += (sender, args) => { TryConnectAndListen(); };

        _timer.Start();
    }

    private void SendButton_OnClick(object sender, RoutedEventArgs e)
    {
        _writer!.WriteLine($"[{DateTime.Now}][{Environment.UserName}]: {MessageTextBox.Text}");
        _writer.Flush();

        MessageTextBox.Text = string.Empty;
    }

    private void TryConnectAndListen()
    {
        try
        {
            _tcpClient.Connect(IPEndPoint.Parse("172.20.208.162:8989"));

            var networkStream = _tcpClient.GetStream();

            _reader = new StreamReader(networkStream);
            _writer = new StreamWriter(networkStream);

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var message = _reader.ReadLine();

                    _synchronizationContext.Send(_ =>
                    {
                        if (message is not null)
                        {
                            MessagesListBox.Items.Add(message);
                            MessagesListBox.ScrollIntoView(message);
                        }
                        
                    }, null);
                }
            }, TaskCreationOptions.LongRunning);

            _timer.Stop();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}