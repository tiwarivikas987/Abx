using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace abx_exchange_client;
public static class AbxClient
{
    public static void StartClient()
    {
        string hostname = "127.0.0.1"; // Replace with actual hostname or IP
        int port = 3000;

        using TcpClient client = new TcpClient(hostname, port);
        
        NetworkStream stream = client.GetStream();

        // Step 4: Send Request to Stream All Packets
        byte[] requestPayload = new byte[] { 1, 0 }; // CallType 1, ResendSeq 0
        stream.Write(requestPayload, 0, requestPayload.Length);

        // Step 5: Receive and Parse Response
        List<Packet> packets = new List<Packet>();
        byte[] buffer = new byte[17]; // Each packet is 17 bytes

        while (stream.Read(buffer, 0, buffer.Length) > 0)
        {
            Packet packet = ParsePacket(buffer);
            packets.Add(packet);
        }

        // Step 6: Handle Missing Sequences (not implemented in this basic example)

        // Step 7: Generate JSON Output
        string jsonOutput = JsonSerializer.Serialize(packets);
        System.IO.File.WriteAllText("output.json", jsonOutput);

        Console.WriteLine("Data saved to output.json");

    }


    static Packet ParsePacket(byte[] buffer)
    {
        string symbol = Encoding.ASCII.GetString(buffer, 0, 4);
        char buySellIndicator = (char)buffer[4];
        int quantity = BitConverter.ToInt32(buffer, 5);
        int price = BitConverter.ToInt32(buffer, 9);
        int sequence = BitConverter.ToInt32(buffer, 13);

        return new Packet
        {
            Symbol = symbol,
            BuySellIndicator = buySellIndicator,
            Quantity = quantity,
            Price = price,
            Sequence = sequence
        };
    }
}
