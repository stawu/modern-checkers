using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Warcaby_Server.Network;
using Warcaby_Server.Network.Packets.Out.Commands;

namespace Warcaby_Server
{
    public static class MatchmakingManager
    {
        private static ConcurrentDictionary<string, ClientConnection> _competitiveQueue = new();

        private static ConcurrentDictionary<int, Match> _matches = new();

        private static Thread _thread = new Thread(ThreadStart);
        private static bool _running;

        public static void Start()
        {
            _running = true;
            _thread.Start();
        }

        public static void Stop()
        {
            _running = false;
        }
        
        public static void AddPlayerToQueue(string playerName, ClientConnection connection)
        {
            _competitiveQueue.TryAdd(playerName, connection);
        }
        
        public static void RemovePlayerFromQueue(string playerDataAccountName)
        {
            _competitiveQueue.TryRemove(playerDataAccountName, out _);
        }
        
        public static Match TryGetMatch(int matchId)
        {
            return _matches.TryGetValue(matchId, out var match) ? match : null;
        }

        private static void ThreadStart()
        {
            while (_running)
            {
                if (_competitiveQueue.Count < 2)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                var (playerName, playerConnection) = _competitiveQueue.First();
                if (TryFindOpponentForPlayer(playerName, out var opponent) == false)
                    continue;

                if (!playerConnection.Connected)
                {
                    _competitiveQueue.TryRemove(playerName, out _);
                    continue;
                }
                if (!opponent.Value.Connected)
                {
                    _competitiveQueue.TryRemove(opponent.Key, out _);
                    continue;
                }

                var matchId = CreateNewMatchAndReturnItsId(playerConnection, opponent.Value);

                try
                {
                    playerConnection.SendPacket(new MatchFoundOutCommand(matchId));
                    opponent.Value.SendPacket(new MatchFoundOutCommand(matchId));
                }
                catch (Exception e) { Console.WriteLine(e); }
                
                _competitiveQueue.TryRemove(playerName, out _);
                _competitiveQueue.TryRemove(opponent.Key, out _);
                
            }
        }

        private static bool TryFindOpponentForPlayer(string playerName, out KeyValuePair<string, ClientConnection> opponent)
        {
            opponent = _competitiveQueue.FirstOrDefault(pair => pair.Key != playerName);
            
            //return true if opponent was found otherwise false
            return !opponent.Equals(default(KeyValuePair<string, ClientConnection>));
        }
        
        private static int CreateNewMatchAndReturnItsId(ClientConnection firstPlayerConnection, ClientConnection secondPlayerConnection)
        {
            int generatedMatchId;
            do
            { 
                generatedMatchId = new Random().Next();
            } while (_matches.TryAdd(generatedMatchId, new Match(firstPlayerConnection, secondPlayerConnection)) == false);

            return generatedMatchId;
        }
    }
}