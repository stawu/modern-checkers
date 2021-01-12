using Match;

namespace Network.Packets.Out.Commands
{
    public class PawnMovesOutCommand : OutCommand
    {
        public PawnMovesOutCommand(PawnMove[] moves) : base(11)
        {
            InsertValue((short)moves[0].PawnTile.BoardPosition.x);
            InsertValue((short)moves[0].PawnTile.BoardPosition.y);
            InsertValue((short)moves.Length);
            
            foreach (var move in moves)
            {
                InsertValue((short)move.MoveType);
                InsertValue((short)move.Tile.BoardPosition.x);
                InsertValue((short)move.Tile.BoardPosition.y);

                if (move.MoveType == MoveType.Attack)
                {
                    InsertValue((short)move.EnemyTile.BoardPosition.x);
                    InsertValue((short)move.EnemyTile.BoardPosition.y);
                }
            }
        }
    }
}