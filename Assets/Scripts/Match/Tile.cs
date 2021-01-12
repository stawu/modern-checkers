using JetBrains.Annotations;
using UnityEngine;

namespace Match
{
    public class Tile
    {
        public Vector2Int BoardPosition { get; private set; }
        public Transform Transform { get; private set; }
        [CanBeNull] public Pawn Pawn { get; private set; }

        private Material _material;
        private Color _defaultColor;

        public Tile(Vector2Int boardPosition, Transform transform)
        {
            BoardPosition = boardPosition;
            Transform = transform;
            _material = transform.GetComponent<MeshRenderer>().material;
            _defaultColor = _material.color;
        }

        public bool TryGetPawn(out Pawn pawn)
        {
            pawn = Pawn;
            return Pawn != null;
        }
        
        public void SetPawn(Pawn pawn)
        {
            Pawn = pawn;
        }

        public void RemovePawn()
        {
            SetPawn(null);
        }

        public void SetTileColor(Color color)
        {
            _material.color = color;
        }

        public void ResetTileColor()
        {
            _material.color = _defaultColor;
        }
    }
}