using System;

namespace UdpChat;

class Program
{
    static void Main()
    {
        UdpChatClient client = new UdpChatClient();
        client.Start();
    }
}