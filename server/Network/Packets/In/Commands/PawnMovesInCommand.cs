using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Warcaby_Server.Network.Packets.Out.Commands;

namespace Warcaby_Server.Network.Packets.In.Commands
{
    public class PawnMovesInCommand : InCommand
    {
        public enum MoveType
        {
            Move,
            Attack
        };

        public struct PawnMove
        {
            public MoveType MoveType;
            public int TilePosX, TilePosY;
            public int EnemyTilePosX, EnemyTilePosY;
        }
        
        private ClientConnection _playerConnection;
        private PlayerData _playerData;
        private int _pawnStartPosX, _pawnStartPosY; 
        private PawnMove[] _moves;

        public PawnMovesInCommand(ClientConnection playerConnection, PlayerData playerData)
        {
            _playerConnection = playerConnection;
            _playerData = playerData;
        }

        public override void FillDataFromStream(NetworkStream networkStream)
        {
            _pawnStartPosX = ReadShort(networkStream);
            _pawnStartPosY = ReadShort(networkStream);
            _moves = new PawnMove[ReadShort(networkStream)];
            
            for (var i = 0; i < _moves.Length; i++)
            {
                _moves[i].MoveType = (MoveType) ReadShort(networkStream);
                _moves[i].TilePosX = ReadShort(networkStream);
                _moves[i].TilePosY = ReadShort(networkStream);

                if (_moves[i].MoveType == MoveType.Attack)
                {
                    _moves[i].EnemyTilePosX = ReadShort(networkStream);
                    _moves[i].EnemyTilePosY = ReadShort(networkStream);
                }
            }
        }

        public override void Execute()
        {
            Match match = _playerData.TempServerOnlyData.PlayingMatch;
            int playerIndex = match.PlayersNames[0] == _playerData.AccountName ? 0 : 1;
            int opponentIndex = playerIndex == 1 ? 0 : 1;
            
            if(match.CurrentPlayerIndexTurn != playerIndex)
                return;

            var acceptedMoves = new List<PawnMove>();
            
            int startPosX = _pawnStartPosX;
            int startPosY = _pawnStartPosY;
            foreach (var move in _moves)
            {
                if (move.MoveType == MoveType.Move)
                {
                    if (match.TryMakePawnMove(startPosX, startPosY, move.TilePosX, move.TilePosY))
                        acceptedMoves.Add(move);
                    else
                        break;
                }
                else if (move.MoveType == MoveType.Attack)
                {
                    if (match.TryMakePawnKill(startPosX, startPosY, move.EnemyTilePosX, move.EnemyTilePosY, move.TilePosX, move.TilePosY))
                        acceptedMoves.Add(move);
                    else
                        break;
                }

                startPosX = move.TilePosX;
                startPosY = move.TilePosY;
            }
            
            _playerConnection.SendPacket(new MatchRoundUpdateOutCommand(_pawnStartPosX, _pawnStartPosY, acceptedMoves.ToArray(), match.CurrentPlayerIndexTurn != playerIndex)); 
            match.PlayersConnections[opponentIndex].RequestToSendCommand(new MatchRoundUpdateOutCommand(_pawnStartPosX, _pawnStartPosY, acceptedMoves.ToArray(), match.CurrentPlayerIndexTurn != playerIndex));

            if (match.GameEnded)
            {
                _playerConnection.SendPacket(new MatchEndOutCommand(match.CurrentPlayerIndexTurn == playerIndex));
                match.PlayersConnections[opponentIndex].RequestToSendCommand(new MatchEndOutCommand(match.CurrentPlayerIndexTurn == opponentIndex));

                match.PlayersData[opponentIndex].TempServerOnlyData.AcceptedMatchInvitation = false;
                _playerData.TempServerOnlyData.AcceptedMatchInvitation = false;
                
                match.PlayersData[opponentIndex].TempServerOnlyData.PlayingMatch = null;
                _playerData.TempServerOnlyData.PlayingMatch = null;
            }
        }
    }
}