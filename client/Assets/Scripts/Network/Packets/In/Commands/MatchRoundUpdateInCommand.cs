using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Match;
using UnityEngine;

namespace Network.Packets.In.Commands
{
    public class MatchRoundUpdateInCommand : InCommand
    {
        private readonly BoardController _boardControllerInstance;
        private MatchController _matchControllerInstance;
        private MatchLogic _matchLogicInstance;
        
        private int _pawnStartPosX, _pawnStartPosY;

        private MoveType[] _moveTypes;
        private int[] _movesTilePosX, _movesTilePosY;
        private int[] _movesEnemyTilePosX, _movesEnemyTilePosY;
        private bool _playerTurnChanged;

        public MatchRoundUpdateInCommand(MatchLogic matchLogicInstance, BoardController boardControllerInstance)
        {
            _boardControllerInstance = boardControllerInstance;
            _matchLogicInstance = matchLogicInstance;
            _matchControllerInstance = _boardControllerInstance.matchControllerInstance;
        }

        public override void FillDataFromStream(NetworkStream networkStream)
        {
            _pawnStartPosX = ReadShort(networkStream);
            _pawnStartPosY = ReadShort(networkStream);
            int movesNumber = ReadShort(networkStream);

            _moveTypes = new MoveType[movesNumber];
            _movesTilePosX = new int[movesNumber];
            _movesTilePosY = new int[movesNumber];
            _movesEnemyTilePosX = new int[movesNumber];
            _movesEnemyTilePosY = new int[movesNumber];

            for (int i = 0; i < movesNumber; i++)
            {
                _moveTypes[i] = (MoveType) ReadShort(networkStream);
                _movesTilePosX[i] = ReadShort(networkStream);
                _movesTilePosY[i] = ReadShort(networkStream);

                if (_moveTypes[i] == MoveType.Attack)
                {
                    _movesEnemyTilePosX[i] = ReadShort(networkStream);
                    _movesEnemyTilePosY[i] = ReadShort(networkStream);
                }
            }
            
            _playerTurnChanged = ReadShort(networkStream) == 1;
        }

        public override void Execute()
        {
            var movesToExecute = new List<PawnMove>();
            
            if(_boardControllerInstance.TryGetTile(new Vector2Int(_pawnStartPosX, _pawnStartPosY), out var pawnTile) == false)
                throw new Exception("!!!!!!!!!!");
            
            for (int i = 0; i < _moveTypes.Length; i++)
            {
                if(_boardControllerInstance.TryGetTile(new Vector2Int(_movesTilePosX[i], _movesTilePosY[i]), out var tile) == false)
                    throw new Exception("!!!!!!!!!!");

                Tile enemyTile = null;
                if(_moveTypes[i] == MoveType.Attack && !_boardControllerInstance.TryGetTile(new Vector2Int(_movesEnemyTilePosX[i], _movesEnemyTilePosY[i]), out enemyTile))
                    throw new Exception("!!!!!!!!!!");

                PawnMove pawnMove = new PawnMove(tile, _moveTypes[i]);
                pawnMove.PawnTile = pawnTile;
                pawnMove.EnemyTile = enemyTile;
                
                movesToExecute.Add(pawnMove);
                
                enemyTile?.RemovePawn();
                
            }
            
            if(!pawnTile.Pawn.playerPawn)
                pawnTile.Pawn.ExecuteMoves(movesToExecute.ToArray());
            
            movesToExecute.Last().Tile.SetPawn(pawnTile.Pawn);
            pawnTile.RemovePawn();

            if (_playerTurnChanged)
            {
                _matchControllerInstance._playerTurn = !_matchControllerInstance._playerTurn;
                
                if(_matchControllerInstance._playerTurn)
                    _matchLogicInstance.onYourTurn.Invoke();
                else
                    _matchLogicInstance.onOpponentTurn.Invoke();
            }
        }
    }
}