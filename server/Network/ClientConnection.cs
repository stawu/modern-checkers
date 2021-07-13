using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using Warcaby_Server.Network.Packets;
using Warcaby_Server.Network.Packets.In;
using Warcaby_Server.Network.Packets.In.Commands;
using Warcaby_Server.Network.Packets.Out;
using Warcaby_Server.Network.Packets.Out.Commands;

namespace Warcaby_Server.Network
{
    public class ClientConnection : IClientThreadProxy
    {
        public bool Connected => _tcpClient.Connected;
        public PlayerData PlayerData;

        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private Thread _thread;
        private bool _commandListening = true;
        private readonly ConcurrentQueue<OutCommand> _requestedCommandsToSendFromOtherThreads = new();

        public ClientConnection(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _thread = new Thread(ThreadStart);
            _thread.Start();
        }
        
        public void SendPacket(OutPacket packet)
        {
            var dataByteArray = packet.GetDataAsByteArray();
            _networkStream.Write(dataByteArray, 0, dataByteArray.Length);
        }

        public void EndCommandListening()
        {
            _commandListening = false;
        }

        private void ThreadStart()
        {
            try
            {
                Console.WriteLine("Client connection thread started");
                _networkStream = _tcpClient.GetStream();

                PlayerData = new PlayerData();

                while (PlayerData.TempServerOnlyData.LoggedIn == false)
                {
                    var loginRequestPacket = new LoginRequestInPacket();
                    loginRequestPacket.FillDataFromStream(_networkStream);
                    Console.WriteLine("loginRequestPacket: " + loginRequestPacket.Login + ", " + loginRequestPacket.Password);

                    if (DatabaseManager.AccountCredentialsCorrect(loginRequestPacket.Login, loginRequestPacket.Password))
                    {
                        SendPacket(new LoginResponseOutPacket(LoginResponseOutPacket.LoginStatus.Successful, "OK"));
                        PlayerData.TempServerOnlyData.LoggedIn = true;
                        PlayerData.AccountName = loginRequestPacket.Login;

                        SendPacket(new SkinOffersOutPacket(DatabaseManager.GetSkinOffersFromDatabase()));
                        
                        DatabaseManager.GetPlayerDataFromDatabaseAndInsertInto(ref PlayerData);
                        if ((DateTime.Now - PlayerData.LastLoginDate).Days >= 1)
                            PlayerData.DailyRewardClaimed = false;
                        
                        PlayerData.LastLoginDate = DateTime.Now;

                        SendPacket(new PlayerDataOutPacket(PlayerData));

                        
                        var commandTypePacket = new CommandTypeInPacket();
                        while (_commandListening)
                        {
                            Thread.Sleep(250);
                            while (_requestedCommandsToSendFromOtherThreads.TryDequeue(out var requestedCommand))
                                SendPacket(requestedCommand);
                            
                            if (_networkStream.DataAvailable)
                            {
                                commandTypePacket.FillDataFromStream(_networkStream);
                                InCommand inCommand = commandTypePacket.CommandId switch
                                {
                                    0 => new BuySkinRequestInCommand(this, PlayerData),
                                    1 => new SelectSkinRequestInCommand(this, PlayerData),
                                    //2 => new UpdatePlayerDataInCommand(), <-- CLIENT
                                    3 => new PlayCompetitiveRequestInCommand(PlayerData, this),
                                    //4 => new MatchFoundInCommand(), <-- CLIENT
                                    5 => new MatchFoundResponseInCommand(PlayerData),
                                    //6 => new StartMatchInCommand(), <-- CLIENT
                                    //7 => new MatchRejectedInCommand(), <-- CLIENT
                                    8 => new MatchSceneLoadedInCommand(this, PlayerData),
                                    // 9 => new OpponentPlayerDataInCommand(matchLogicInstance), <-- CLIENT
                                    // 10 => new MatchRoundStartInCommand(matchLogicInstance), <-- CLIENT
                                    11 => new PawnMovesInCommand(this, PlayerData),
                                    //12 => new MatchRoundUpdateInCommand(_boardControllerInstance), <-- CLIENT
                                    //13 => new MatchEndInCommand(_matchLogicInstance), <-- CLIENT
                                    14 => new LogoutRequestInCommand(this),
                                    //15 => new LogoutInCommand(), <-- CLIENT
                                    16 => new CancelCompetitiveRequestInCommand(PlayerData),
                                    17 => new ClaimDailyRewardInCommand(this, PlayerData),
                                    18 => new SurrenderRequestInCommand(this, PlayerData),
                                    _ => null
                                };

                                inCommand.FillDataFromStream(_networkStream);
                                inCommand.Execute();
                            }
                        }
                        
                    }
                    else
                        SendPacket(new LoginResponseOutPacket(LoginResponseOutPacket.LoginStatus.Negative, "Wrong user name or password!"));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try
            {
                PlayerData.TempServerOnlyData.LoggedIn = false;
                SendPacket(new LogoutOutCommand());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try
            {
                Match match = PlayerData.TempServerOnlyData.PlayingMatch;
                if (match != null)
                {
                    int playerIndex = match.PlayersNames[0] == PlayerData.AccountName ? 0 : 1;
                    int opponentIndex = playerIndex == 1 ? 0 : 1;
                    match.PlayersConnections[opponentIndex].RequestToSendCommand(new MatchEndOutCommand(true));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            Console.WriteLine("Client connection thread ended");
            _tcpClient.Close();
            _networkStream.Close();
        }

        //Called from other threads
        public void RequestToSendCommand(OutCommand command)
        {
            _requestedCommandsToSendFromOtherThreads.Enqueue(command);
        }
    }
}