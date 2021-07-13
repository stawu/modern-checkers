using Warcaby_Server.Network;

namespace Warcaby_Server
{
    public class Match
    {
        private class Pawn
        {
            public int PlayerId { get; private set; }
            public bool King;

            public Pawn(int playerId)
            {
                PlayerId = playerId;
                King = false;
            }
        }
        
        private class Tile
        {
            public Pawn Pawn = null;
        };
        
        public int CurrentPlayerIndexTurn;
        public bool GameEnded = false;
        public IClientThreadProxy[] PlayersConnections { get; private set; }
        public string[] PlayersNames { get; private set; }
        public PlayerData[] PlayersData { get; }
        public bool[] PlayersReadyFlag { get; }

        private Tile[,] _gameBoard = new Tile[8, 8];

        public Match(ClientConnection firstPlayerConnection, ClientConnection secondPlayerConnection)
        {
            CurrentPlayerIndexTurn = -1;
            PlayersConnections = new IClientThreadProxy[] { firstPlayerConnection, secondPlayerConnection };
            PlayersNames = new string[] { firstPlayerConnection.PlayerData.AccountName, secondPlayerConnection.PlayerData.AccountName };
            PlayersData = new PlayerData[] { firstPlayerConnection.PlayerData, secondPlayerConnection.PlayerData };
            PlayersReadyFlag = new bool[] {false, false};
        }
        
        public void ResetPawns(int startingPlayerIndex)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                    _gameBoard[i, j] = new Tile();
            }

            int secondPlayerIndex = startingPlayerIndex == 1 ? 0 : 1;
            
            for (var y = 0; y < 3; y++)
            {
                for (var x = (y == 1 ? 1 : 0); x < 8; x += 2)
                    _gameBoard[x, y].Pawn = new Pawn(startingPlayerIndex);
            }
            
            for (var y = 5; y < 8; y++)
            {
                for (var x = (y == 6 ? 0 : 1); x < 8; x += 2)
                    _gameBoard[x, y].Pawn = new Pawn(secondPlayerIndex);
            }
        }
        
        public bool TryMakePawnMove(int xPawnPos, int yPawnPos, int xMovePos, int yMovePos)
        {
            if (xMovePos < 0 || yPawnPos < 0 || xMovePos >= 8 || yMovePos >= 8)
                return false;

            if (PlayerHaveAttackPossibility(CurrentPlayerIndexTurn))
                return false;
            
            //todo move validation
            _gameBoard[xMovePos, yMovePos].Pawn = _gameBoard[xPawnPos, yPawnPos].Pawn;
            _gameBoard[xPawnPos, yPawnPos].Pawn = null;
            
            CurrentPlayerIndexTurn = CurrentPlayerIndexTurn == 1 ? 0 : 1;

            return true;
        }

        public bool TryMakePawnKill(int startPosX, int startPosY, int moveEnemyTilePosX, int moveEnemyTilePosY, int moveTilePosX, int moveTilePosY)
        {
            if (startPosX < 0 || startPosY < 0 || moveEnemyTilePosX >= 8 || moveEnemyTilePosY >= 8 || moveTilePosX < 0 || moveTilePosY >= 8)
                return false;

            //todo
            _gameBoard[moveTilePosX, moveTilePosY].Pawn = _gameBoard[startPosX, startPosY].Pawn;
            _gameBoard[startPosX, startPosY].Pawn = null;
            _gameBoard[moveEnemyTilePosX, moveEnemyTilePosY].Pawn = null;
            
            if(!PlayerHaveAttackPossibility(CurrentPlayerIndexTurn))
                CurrentPlayerIndexTurn = CurrentPlayerIndexTurn == 1 ? 0 : 1;


            int firstPlayerPawnCounter = 0;
            int secondPlayerPawnCounter = 0;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (_gameBoard[x, y].Pawn?.PlayerId == 0)
                        firstPlayerPawnCounter++;
                    else if (_gameBoard[x, y].Pawn?.PlayerId == 1)
                        secondPlayerPawnCounter++;
                }
            }

            if (firstPlayerPawnCounter == 0 || secondPlayerPawnCounter == 0)
                GameEnded = true;
            
            return true;
        }

        private bool PlayerHaveAttackPossibility(int playerIndex)
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (_gameBoard[x, y].Pawn?.PlayerId != playerIndex)
                        continue;

                    if (PawnHaveAttackPossibility(x, y))
                        return true;
                }
            }

            return false;
        }

        private bool TryGetGameBoardValue(int x, int y, out Tile value)
        {
            if (x < 0 || y < 0 || x >= 8 || y >= 8)
            {
                value = null;
                return false;
            }

            value = _gameBoard[x, y];
            return true;
        }

        private bool PawnHaveAttackPossibility(int pawnPosX, int pawnPosY)
        {
            int enemyIndex = _gameBoard[pawnPosX, pawnPosY].Pawn.PlayerId == 0 ? 1 : 0;
            var directions = new[] { (1, 1), (-1, 1), (1, -1), (-1, -1) };

            foreach (var (directionX, directionY) in directions)
            {
                if (TryGetGameBoardValue(pawnPosX + directionX, pawnPosY + directionY, out var tile))
                {
                    if(tile.Pawn == null || tile.Pawn.PlayerId != enemyIndex)
                        continue;

                    if (TryGetGameBoardValue(pawnPosX + directionX * 2, pawnPosY + directionY * 2,
                        out var behindEnemyTile))
                    {
                        if (behindEnemyTile.Pawn == null)
                            return true;
                    }
                        
                }
            }

            return false;
        }
    }
}