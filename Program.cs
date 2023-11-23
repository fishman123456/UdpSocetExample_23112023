using System.Net.Sockets;
using System.Net;
using System.Text;

namespace UdpSocetExample_23112023
{
    class program
    {
        // UdpSender - выполняет отправку сообщения через Udp-сокет
        static void UdpSender(string senderIpStr, int senderPort, string receiverIpStr, int receiverPort)
        {
            // 1. Создать udp-сокет
            Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // 2. Подготовим удаленный endpoint
            IPEndPoint receiverEndPoint = new IPEndPoint(IPAddress.Parse(receiverIpStr), receiverPort);
            IPEndPoint senderEndPoint = new IPEndPoint(IPAddress.Parse(senderIpStr), senderPort);
            Console.WriteLine($"sender> sender on {senderEndPoint}; receiver on {receiverEndPoint}");

            // 3. Свяжем отправителя с сокетом прослушивания сообщений
            sender.Bind(senderEndPoint);

            // 4. Отправим сообщение
            string message = $"udp message, created at {DateTime.Now}";
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            sender.SendTo(buffer, receiverEndPoint);
            Console.WriteLine($"sender> отправлено сообщение '{message}'");

            // 4. получим ответное сообщение
            buffer = new byte[1024];
            int bytesRead = sender.Receive(buffer);
            message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"sender> получено сообщение '{message}'");
        }

        // UdpReceiver - выполняет получение сообщения через Udp-сокет
        static void UdpReceiver(string senderIpStr, int senderPort, string receiverIpStr, int receiverPort)
        {
            // 1. Создать udp-сокет
            Socket receiver = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // 2. Подготовим локальный endpoint-ы
            IPEndPoint receiverEndPoint = new IPEndPoint(IPAddress.Parse(receiverIpStr), receiverPort);
            IPEndPoint senderEndPoint = new IPEndPoint(IPAddress.Parse(senderIpStr), senderPort);
            Console.WriteLine($"receiver> sender on {senderEndPoint}; receiver on {receiverEndPoint}");

            // 3. Свяжем сокет с endpoint-ом прослушивания
            receiver.Bind(receiverEndPoint);

            // 4. получить сообщение
            byte[] buffer = new byte[1024];
            int bytesRead = receiver.Receive(buffer);
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"receiver> получено сообщение '{message}'");

            // 5. отправить ответное сообщение
            message = $"second message";
            buffer = Encoding.UTF8.GetBytes(message);
            receiver.SendTo(buffer, senderEndPoint);
            Console.WriteLine($"receiver> отправлено сообщение '{message}'");
        }

        static void Main(string[] args)
        {
            string senderIpStr = "127.0.0.1";
            int senderPort = 1024;
            string receiverIpStr = "127.0.0.1";
            int receiverPort = 1025;

            Thread receiverThread = new Thread(() => UdpReceiver(senderIpStr, senderPort, receiverIpStr, receiverPort));
            Thread senderThread = new Thread(() => UdpSender(senderIpStr, senderPort, receiverIpStr, receiverPort));

            receiverThread.Start();
            senderThread.Start();

            receiverThread.Join();
            senderThread.Join();
        }
    }
}