using System.Linq;
using UnityEngine;

namespace Match
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private GameObject standableTilePrefab;
        [SerializeField] private GameObject forbiddenTilePrefab;
        [SerializeField] private Vector3 boardPositionOffset = new Vector3(-3.5f, 0, -3.5f);
        public MatchController matchControllerInstance;//todo
        
        private Tile[,] _tiles = new Tile[8,8];
        private Pawn[] _playerPawns;
        private Pawn[] _opponentPawns;

        public bool TryGetTile(Vector3 worldPosition, out Tile tile)
        {
            int x = Mathf.RoundToInt(worldPosition.x - boardPositionOffset.x);
            int y = Mathf.RoundToInt(worldPosition.z - boardPositionOffset.z);

            if (x >= 0 && x < 8 && y >= 0 && y < 8)
            {
                tile = _tiles[x, y];
                return true;
            }

            tile = null;
            return false;
        }
        
        public bool TryGetTile(Vector2Int boardPosition, out Tile tile)
        {
            if (boardPosition.x >= 0 && boardPosition.x < 8 && boardPosition.y >= 0 && boardPosition.y < 8)
            {
                tile = _tiles[boardPosition.x, boardPosition.y];
                return true;
            }

            tile = null;
            return false;
        }
        
        public void ResetPawns(bool playerStarts)
        {
            Camera.main.transform.root.GetComponent<Animator>().SetTrigger(playerStarts ? "RotateRight" : "RotateLeft");//todo
            if (_tiles[0, 0] == null)
            {//todo
                GenerateBoard();
                InstantiatePawnsForPlayer();
            }

            matchControllerInstance._playerTurn = playerStarts;

            var currentPawnIndex = 0;
            for (var y = 0; y < 3; y++)
            {
                for (var x = (y == 1 ? 1 : 0); x < 8; x += 2)
                {
                    _tiles[x, y].SetPawn(playerStarts ? _playerPawns[currentPawnIndex] : _opponentPawns[currentPawnIndex]);
                    _tiles[x, y].Pawn.forwardLook = Vector3.forward;
                    _tiles[x, y].Pawn.MoveTo(_tiles[x, y].Transform.position);
                    currentPawnIndex++;
                }
            }
            
            currentPawnIndex = 0;
            for (var y = 5; y < 8; y++)
            {
                for (var x = (y == 6 ? 0 : 1); x < 8; x += 2)
                {
                    _tiles[x, y].SetPawn(playerStarts ? _opponentPawns[currentPawnIndex] : _playerPawns[currentPawnIndex]);
                    _tiles[x, y].Pawn.forwardLook = Vector3.back;
                    _tiles[x, y].Pawn.MoveTo(_tiles[x, y].Transform.position);
                    currentPawnIndex++;
                }
            }
        }

        public void InstantiatePawnsForOpponent(int[] opponentSelectedSkinsIdsForPawns)
        {
            _opponentPawns = new Pawn[opponentSelectedSkinsIdsForPawns.Length];
            var currentPawnIndex = 0;
            
            foreach (var selectedSkinIdForPawn in opponentSelectedSkinsIdsForPawns)
            {
                _opponentPawns[currentPawnIndex] = Instantiate(SkinsManager.Skins.First(skin => skin.id == selectedSkinIdForPawn).pawnPrefab, new Vector3(4.5f, 0, currentPawnIndex - 5f), Quaternion.identity).GetComponent<Pawn>();
                _opponentPawns[currentPawnIndex].SetAsEnemyPawn();
                currentPawnIndex++;
            }
        }

        private void Awake()
        {
            if (_tiles[0, 0] == null)
            {//todo
                GenerateBoard();
                InstantiatePawnsForPlayer();
            }
        }
        
        private void InstantiatePawnsForPlayer()
        {
            _playerPawns = new Pawn[PlayerDataManager.SelectedSkinsIdsForPawns.Length];
            var currentPawnIndex = 0;
            
            foreach (var selectedSkinIdForPawn in PlayerDataManager.SelectedSkinsIdsForPawns)
            {
                _playerPawns[currentPawnIndex] = Instantiate(SkinsManager.Skins.First(skin => skin.id == selectedSkinIdForPawn).pawnPrefab, new Vector3(-4.5f, 0, currentPawnIndex - 5f), Quaternion.identity).GetComponent<Pawn>();
                currentPawnIndex++;
            }
        }

        
        private void GenerateBoard()
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    _tiles[x, y] = new Tile(new Vector2Int(x, y), Instantiate((y+x) % 2 == 0 ? standableTilePrefab : forbiddenTilePrefab, new Vector3(x, 0, y) + boardPositionOffset, Quaternion.identity).transform);
                    _tiles[x, y].Transform.forward = Vector3.down;
                }
            }
        }
    }
}
