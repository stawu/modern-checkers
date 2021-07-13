using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using Network.Packets;
using UnityEngine;

public static class NetworkManager
{
    public static bool ConnectedToServer => _tcpClient != null && _tcpClient.Connected;

    private static NetworkStream _networkStream;
    private static TcpClient _tcpClient;
    
    public static bool TryInitializeConnectionToServer(string serverIP, int serverPort)
    {
        if (ConnectedToServer)
            return true;
        
        try
        {
            _tcpClient = new TcpClient(serverIP, serverPort);
            _networkStream = _tcpClient.GetStream();
        }
        catch (SocketException e)
        {
            Debug.LogError(e);
            return false;
        }

        return true;
    }

    public static void CloseConnectionToServer()
    {
        if (!ConnectedToServer)
            return;
        
        _tcpClient.Close();
    }

    public static void SendPacket(OutPacket packet)
    {
        var dataByteArray = packet.GetDataAsByteArray();
        _networkStream.Write(dataByteArray, 0, dataByteArray.Length);
    }

    public static void FillPacket(InPacket packet)
    {
        packet.FillDataFromStream(_networkStream);
    }
    
    public static UniTask FillPacketAsync(InPacket packet)
    {
        return UniTask.Run( () => packet.FillDataFromStream(_networkStream));
    }
}
