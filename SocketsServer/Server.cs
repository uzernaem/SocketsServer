using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Sockets
{
    public partial class frmMain : Form
    {
        private Socket ClientSock;                      // клиентский сокет
        private TcpListener Listener;                   // сокет сервера
        private UdpClient UServer;
        Thread b;
        private List<Thread> Threads = new List<Thread>();      // список потоков приложения (кроме родительского)
        private bool _continue = true;                          // флаг, указывающий продолжается ли работа с сокетами   
        IPAddress IP;
        private Dictionary<string, TcpClient> Clients = new Dictionary<string, TcpClient>();

        // конструктор формы
        public frmMain()
        {
            InitializeComponent();

            UServer = new UdpClient(8888);
            UServer.EnableBroadcast = true;

            b = new Thread(BcastResponse);
            b.Start();

            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());    // информация об IP-адресах и имени машины, на которой запущено приложение
            IP = hostEntry.AddressList[0];                        // IP-адрес, который будет указан при создании сокета
            int Port = 1010;                                                // порт, который будет указан при создании сокета

            // определяем IP-адрес машины в формате IPv4
            foreach (IPAddress address in hostEntry.AddressList)
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    IP = address;
                    break;
                }

            // вывод IP-адреса машины и номера порта в заголовок формы, чтобы можно было его использовать для ввода имени в форме клиента, запущенного на другом вычислительном узле
            this.Text += "     " + IP.ToString() + "  :  " + Port.ToString();

            // создаем серверный сокет (Listener для приема заявок от клиентских сокетов)
            Listener = new TcpListener(IP, Port);
            Listener.Start();

            // создаем и запускаем поток, выполняющий обслуживание серверного сокета
            Threads.Clear();
            Threads.Add(new Thread(ReceiveMessage));
            Threads[Threads.Count - 1].Start();
        }

        private void BcastResponse()
        {
            while (true)
            {
                var ClientEp = new IPEndPoint(IPAddress.Any, 0);
                var ClientRequestData = UServer.Receive(ref ClientEp);
                var ClientRequest = Encoding.ASCII.GetString(ClientRequestData);
                var ResponseData = Encoding.ASCII.GetBytes(IP.ToString());
                UServer.Send(ResponseData, ResponseData.Length, ClientEp);
            }
        }

        // работа с клиентскими сокетами

        private void ReceiveMessage()
        {
            // входим в бесконечный цикл для работы с клиентскими сокетом
            while (_continue)
            {
                ClientSock = Listener.AcceptSocket();           // получаем ссылку на очередной клиентский сокет
                Threads.Add(new Thread(ReadMessages));          // создаем и запускаем поток, обслуживающий конкретный клиентский сокет
                Threads[Threads.Count - 1].Start(ClientSock);
            }
        }

        // получение сообщений от конкретного клиента
        private void ReadMessages(object ClientSock)
        {
            string msg = "";        // полученное сообщение
            // входим в бесконечный цикл для работы с клиентским сокетом
            while (_continue)
            {
                byte[] buff = new byte[1024];                           // буфер прочитанных из сокета байтов
                ((Socket)ClientSock).Receive(buff);                     // получаем последовательность байтов из сокета в буфер buff
                msg = Encoding.Unicode.GetString(buff).Replace("\0", "");     // выполняем преобразование байтов в последовательность символов
                if (!Regex.IsMatch(msg, @" >> "))
                {
                    if (Regex.IsMatch(msg, @"\w_logout"))
                    {
                        Clients.Remove(msg.Replace("_logout", ""));
                        userList.Invoke((MethodInvoker)delegate
                        {
                            userList.Items.Remove(msg.Replace("_logout", ""));
                        });
                    }
                    else if (!Clients.ContainsKey(msg))
                    {
                        var x = ((Socket)ClientSock).RemoteEndPoint.ToString();
                        var IP = IPAddress.Parse(Regex.Match(x, @".+(?=:)").Value);
                        var port = Int32.Parse(x.Replace(Regex.Match(x, @".+(?=:)").Value + ":", ""));
                        Clients.Add(msg, new TcpClient());
                        ReturnConnect(msg, IP);

                        userList.Invoke((MethodInvoker)delegate
                        {
                            userList.Items.Add(msg);
                        });
                    }
                }
                else if (msg != "")
                {
                    rtbMessages.Invoke((MethodInvoker)delegate
                    {

                        rtbMessages.Text += "\n >> " + msg;             // выводим полученное сообщение на форму
                    });
                    string client = Regex.Match(msg, @"\w+(?= >>)").Value;
                    //ReturnConnect(client);
                    ReturnMessages(msg.Trim('\0'));
                    //SendMessage(msg + " <<", client);
                }
                //Thread.Sleep(500);
            }
        }

        private void ReturnMessages(string msg)
        {
            foreach (string client in Clients.Keys)
            {
                SendMessage(msg, client);
            }
        }

        private void ReturnConnect(string client, IPAddress IP)
        {
            if (!Clients[client].Connected)
            {
                int Port = 1011;                                // номер порта, через который выполняется обмен сообщениями
                Clients[client].Connect(IP, Port);                       // подключение к серверному сокету
            }
        }

        private void SendMessage(string msg, string client)
        {
            byte[] buff = Encoding.Unicode.GetBytes(msg);   // выполняем преобразование сообщения (вместе с идентификатором машины) в последовательность байт
            Stream stm = Clients[client].GetStream();                                                    // получаем файловый поток клиентского сокета
            stm.Write(buff, 0, buff.Length);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _continue = false;      // сообщаем, что работа с сокетами завершена

            // завершаем все потоки
            foreach (Thread t in Threads)
            {
                t.Abort();
                t.Join(500);
            }

            // закрываем клиентский сокет
            if (ClientSock != null)
                ClientSock.Close();

            // приостанавливаем "прослушивание" серверного сокета
            if (Listener != null)
                Listener.Stop();
        }
    }
}