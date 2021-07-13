using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Network.Packets.Out.Commands;
using UnityEngine;

namespace Match
{
    public class MatchController : MonoBehaviour
    {
        public TrailRenderer TrailRenderer;
        [SerializeField] private BoardController boardControllerInstance;
        [SerializeField] private Color tileColorMouseOver = Color.yellow;
        public bool _playerTurn = true; //todo change to private
        private Pawn _selectedPawn = null;
        private Tile _lastFrameTileAtMousePos = null;
        private PawnMove[] _selectedPawnMoves = new PawnMove[0]; 

        private void Update()
        {
            if (!_playerTurn)
                return;

            var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(cameraRay, out var raycastHit))
            {
                if (boardControllerInstance.TryGetTile(raycastHit.point, out var tileAtMousePosition))
                {
                    if (tileAtMousePosition != _lastFrameTileAtMousePos){
                        if(_lastFrameTileAtMousePos != null)
                            OnMouseExitTile(_lastFrameTileAtMousePos);
                        OnMouseEnterTile(tileAtMousePosition);
                    }
                    
                    OnMoseOverTile(tileAtMousePosition);
                    _lastFrameTileAtMousePos = tileAtMousePosition;
                }
                else
                {
                    if(_lastFrameTileAtMousePos != null)
                        OnMouseExitTile(_lastFrameTileAtMousePos);
                    _lastFrameTileAtMousePos = null;
                }
            }
        }

        private void OnMouseEnterTile(Tile tile)
        {
            var move = _selectedPawnMoves.FirstOrDefault(move => move.Tile == tile);
            if (move != null)
            {
                if (!move.Forbidden)
                {
                    tile.SetTileColor(Color.magenta);

                    TrailRenderer.enabled = true;
                    TrailRenderer.Clear();
                    // TrailRenderer.AddPosition(move.Tile.Transform.position + new Vector3(0, 0.2f));
                    // for (int i = move.PreviousMoveIndex; i > 0; i--)

                    TrailRenderer.AddPosition(move.PawnTile.Transform.position + new Vector3(0, 0.2f));
                    foreach (var pawnMove in GetOrderedPawnMovesToPawnMove(move))
                        TrailRenderer.AddPosition(pawnMove.Tile.Transform.position + new Vector3(0, 0.2f));
                    TrailRenderer.transform.position = move.Tile.Transform.position + new Vector3(0, 0.2f);
                }
            }
            else
                tile.SetTileColor(tileColorMouseOver);
            
            if (tile.TryGetPawn(out var pawnOnTile))
                pawnOnTile.ChangeOutlineToSelection();
        }

        private void OnMoseOverTile(Tile tile)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (tile.TryGetPawn(out var pawnOnTile) && pawnOnTile.playerPawn)
                {
                    foreach (var pawnMove in _selectedPawnMoves)
                        pawnMove.Tile.ResetTileColor();
                    
                    _selectedPawn = pawnOnTile;
                    _selectedPawnMoves = CalculatePossiblePawnMoves(_selectedPawn, tile);
                    bool playerHavePossibleAttack = PlayerHavePossibleAttack();

                    foreach (var move in _selectedPawnMoves)
                    {
                        if (playerHavePossibleAttack && move.MoveType == MoveType.Move)
                        {
                            move.Tile.SetTileColor(Color.black);
                            move.Forbidden = true;
                        }
                        else
                            move.Tile.SetTileColor(Color.red);
                    }
                }

                var moveee = _selectedPawnMoves.FirstOrDefault(move => move.Tile == tile);
                if (moveee != null && !moveee.Forbidden)
                {
                    var orderedPawnMoves = GetOrderedPawnMovesToPawnMove(moveee);
                    _selectedPawn.ExecuteMoves(orderedPawnMoves);
                    NetworkManager.SendPacket(new PawnMovesOutCommand(orderedPawnMoves));
                    //moveee.PawnTile.RemovePawn();
                    //moveee.Tile.SetPawn(_selectedPawn);
                    
                    foreach (var move in _selectedPawnMoves)
                        move.Tile.ResetTileColor();

                    _selectedPawnMoves = new PawnMove[0];
                    _selectedPawn = null;
                    TrailRenderer.enabled = false;
                }
            }
        }
        
        private void OnMouseExitTile(Tile tile)
        {
            PawnMove move = _selectedPawnMoves.FirstOrDefault(move => move.Tile == tile);
            if (move != null)
            {
                TrailRenderer.enabled = false;
                tile.SetTileColor(move.Forbidden ? Color.black : Color.red);
            }
            else
                tile.ResetTileColor();
            
            if (tile.TryGetPawn(out var pawn))
                pawn.ChangeOutlineToDefault();
        }

        private bool PlayerHavePossibleAttack()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (!boardControllerInstance.TryGetTile(new Vector2Int(x, y), out var tile)) 
                        continue;
                    if (!tile.TryGetPawn(out var pawn)) 
                        continue;
                    if(!pawn.playerPawn)
                        continue;

                    PawnMove[] possiblePawnMoves = CalculatePossiblePawnMoves(pawn, tile);
                    if (possiblePawnMoves.Any(move => move.MoveType == MoveType.Attack))
                        return true;
                }
            }

            return false;
        }


        private PawnMove[] CalculatePossiblePawnMoves(Pawn pawn, Tile pawnTile)
        {
            var moves = new List<PawnMove>();
            Vector3[] directions =
            {
                pawn.forwardLook + Vector3.right,
                pawn.forwardLook + Vector3.left,
                -pawn.forwardLook + Vector3.right,
                -pawn.forwardLook + Vector3.left,
            };

            foreach (var direction in directions)
            {
                for (var directionSum = direction; boardControllerInstance.TryGetTile(pawnTile.Transform.position + directionSum, out var tile); directionSum += direction)
                {
                    if (tile.TryGetPawn(out var pawnAtTargetTile))
                    {
                        if(pawnAtTargetTile.playerPawn)
                            break;
                        else
                        {
                            CalculatePossibleAttackMovesAndAddThemTo(moves, pawnTile.Transform.position + directionSum + direction, tile, pawnTile, -1);
                            break;
                        }
                    }
                    else
                    {
                        if(pawn.king == false && Vector3.Dot(direction, pawn.forwardLook) < 0)
                            break;
                        
                        var move = new PawnMove(tile, MoveType.Move);
                        move.PawnTile = pawnTile;
                        moves.Add(move);
                    }

                    if (pawn.king == false)
                        break;
                }
            }

            return moves.ToArray();
        }

        private void CalculatePossibleAttackMovesAndAddThemTo(List<PawnMove> moves, Vector3 startPos, Tile enemyTile, Tile pawnTile, int previousMoveIndex)
        {
            if (!boardControllerInstance.TryGetTile(startPos, out var startTile) || startTile.TryGetPawn(out _)) 
                return;
            
            if(moves.Any(pawnMove => pawnMove.Tile == startTile))
                return;

            var move = new PawnMove(startTile, MoveType.Attack);
            move.PreviousMoveIndex = previousMoveIndex;//moves.Count == 0 ? 0 : moves.Count - 1;
            move.PawnTile = pawnTile;
            move.EnemyTile = enemyTile;
            moves.Add(move);

            Vector3[] directions =
            {
                Vector3.forward + Vector3.right,
                Vector3.forward + Vector3.left,
                Vector3.back + Vector3.right,
                Vector3.back + Vector3.left
            };

            foreach (var direction in directions)
            {
                if (boardControllerInstance.TryGetTile(startPos + direction, out var tile))
                {
                    if (tile.TryGetPawn(out var pawnAtTargetTile) && pawnAtTargetTile.playerPawn == false)
                        CalculatePossibleAttackMovesAndAddThemTo(moves, startPos + direction + direction, tile, pawnTile, moves.Count -1);
                }
            }
        }

        private PawnMove[] GetOrderedPawnMovesToPawnMove(PawnMove targetPawnMove)
        {
            var moves = new List<PawnMove>();
            
            moves.Add(targetPawnMove);
            for (var i = targetPawnMove.PreviousMoveIndex; i != -1; i = _selectedPawnMoves[i].PreviousMoveIndex)
                moves.Add(_selectedPawnMoves[i]);

            moves.Reverse();
            return moves.ToArray();
        }
    }

    public enum MoveType
    {
        Move,
        Attack
    }
    
    public class PawnMove
    {
        public Tile Tile;
        public MoveType MoveType;
        [CanBeNull] public Tile EnemyTile;
        public int PreviousMoveIndex;
        public Tile PreviousTile;
        public Tile PawnTile;
        public bool Forbidden;

        public PawnMove(Tile tile, MoveType moveType)
        {
            Tile = tile;
            MoveType = moveType;
            EnemyTile = null;
            //PawnPosition = Vector3.zero;
            PreviousMoveIndex = -1;
            PreviousTile = null;
            PawnTile = null;
            Forbidden = false;
        }
    }
}