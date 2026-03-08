using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading;

namespace UdpChat;

public class UdpChatClient
{
    private int Port;                                                      // NU e const
    private const string MulticastAddress = "239.0.0.222";
     
    private UdpClient udpClient;                                          //socketu udp
    private IPEndPoint multicastEndPoint;                                 //endpoint de ip + port
    private bool running = true;

    public UdpChatClient()
    {
        Console.Write("Introdu portul pentru acest client: ");
        Port = int.Parse(Console.ReadLine());

        udpClient = new UdpClient();                                        //creez socketu UDP                                           1
        udpClient.ExclusiveAddressUse = false;                              //mai multe socketuri pot utiliza acelasi port/mai multi clienti pe acelasi port 

        udpClient.Client.SetSocketOption(
            SocketOptionLevel.Socket,
            SocketOptionName.ReuseAddress,                                 //permite reutilizarea aceluiasi port
            true);

        udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, Port));        //atasez/ dau bind la socket pe portul introdus                            1

        IPAddress multicastIP = IPAddress.Parse(MulticastAddress);          //da convert la string in IPAddress
        udpClient.JoinMulticastGroup(multicastIP);                         //4,5aici clientu intra in grup mtcst, da regula sistemului sa primeasca pachetele trimise pe grup multicast

        multicastEndPoint = new IPEndPoint(multicastIP, Port);             //4creez endpointul de destinatie pentru mesajele generale 
    }

    public void Start()
    {
        Console.WriteLine("UDP Chat pornit.");
        Console.WriteLine("Canal general multicast: " + MulticastAddress);
        Console.WriteLine("Pentru mesaj privat: /pm <IP> <PORT> <mesaj>");
        Console.WriteLine("Pentru iesire: /exit");

        Thread receiveThread = new Thread(ReceiveMessages);
        receiveThread.Start();

        while (running)
        {
            try
            {
                string? input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                    continue;

                if (input.ToLower() == "/exit")
                {
                    running = false;
                    udpClient.DropMulticastGroup(IPAddress.Parse(MulticastAddress));
                    udpClient.Close();
                    break;
                }

                if (input.StartsWith("/pm "))
                {
                    SendPrivateMessage(input);
                }
                else
                {
                    SendGeneralMessage(input);
                }
            }
            catch (SocketException ex)                                                                         //6 
            {
                Console.WriteLine("Eroare Socket: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare generala: " + ex.Message);
            }
        }
    }

    private void SendGeneralMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);                     //transform text in bytes
        udpClient.Send(data, data.Length, multicastEndPoint);             // trimit pe adresa:port, toti din grup primesc      1          4
    }

    private void SendPrivateMessage(string input)
    {
        string[] parts = input.Split(' ', 4);                  // parts[0] = /pm parts[1] = IP parts[2] = PORT parts[3] = mesaj

        if (parts.Length < 4)
        {
            Console.WriteLine("Format greșit. /pm <IP> <PORT> <mesaj>");
            return;
        }

        IPAddress targetIP = IPAddress.Parse(parts[1]);
        int targetPort = int.Parse(parts[2]);

        IPEndPoint targetEndPoint = new IPEndPoint(targetIP, targetPort);     //creez endpoint privat

        byte[] data = Encoding.UTF8.GetBytes("[PRIVATE] " + parts[3]);
        udpClient.Send(data, data.Length, targetEndPoint);  //                2   fac unicast, doar acel ip,port specificat in term primeste
    }

    private void ReceiveMessages()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, Port);   

        while (running)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);       //      3  5 blocheaza pana cand vine un pachet, primeste bytes
                string message = Encoding.UTF8.GetString(data);

                Console.WriteLine($"[{remoteEndPoint.Address}:{remoteEndPoint.Port}] {message}");
            }
            catch (SocketException)                                      // 6 daca socketu e inchis in timp ce asteapta, arunc exceptie, o prind ca sa nu dea erori critice
            {
                break;
            }
        }
    }
}


// UDP - unreliable, flooding with UDP trafficking(called DDOS-ing),
// needed in live apps, calls, apps like zoom,â
// youtube live, twitch live, doesnt resend if wasn't received

//TCP - slower protocol than UDP. but a more reliable one
//UDP - based on speed



//orice comunicar3 UDP contine
//Source IP
//Source Port
//Destination IP
//Destination Port