using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Warcaby_Server.Network;

namespace Warcaby_Server
{
    public class WarcabyServer
    {
        private TcpListener _tcpListener;
        private Thread _thread;
        private bool _running = true;

        public WarcabyServer(int port)
        {
            try
            {
                DatabaseManager.OpenSqlConnection();
                _tcpListener = new TcpListener(IPAddress.Any,  port);
                _thread = new Thread(ThreadStart);
                MatchmakingManager.Start();
                _thread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                DatabaseManager.CloseSqlConnection();
                throw;
            }
        }

        public void StopServer()
        {
            MatchmakingManager.Stop();
            _running = false;
            _tcpListener.Stop();
        }

        private void ThreadStart()
        {
            Console.WriteLine("Listening thread started");
            
            try
            {
                _tcpListener.Start();

                while (_running)
                {
                    ClientConnection clientConnection = new ClientConnection(_tcpListener.AcceptTcpClient());
                    Console.WriteLine("Client connected!");
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
                StopServer();
            }
            
            Console.WriteLine("Listening thread ended");
        }
    }
}