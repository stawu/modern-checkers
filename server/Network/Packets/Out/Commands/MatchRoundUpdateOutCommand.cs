using Warcaby_Server.Network.Packets.In.Commands;

namespace Warcaby_Server.Network.Packets.Out.Commands
{
    public class MatchRoundUpdateOutCommand : OutCommand
    {
        public MatchRoundUpdateOutCommand(int pawnStartPosX, int pawnStartPosY, PawnMovesInCommand.PawnMove[] moves, bool playerTurnChanged) : base(12)
        {
            InsertValue((short)pawnStartPosX);
            InsertValue((short)pawnStartPosY);
            InsertValue((short)moves.Length);

            foreach (var move in moves)
            {
                InsertValue((short)move.MoveType);
                InsertValue((short)move.TilePosX);
                InsertValue((short)move.TilePosY);

                if (move.MoveType == PawnMovesInCommand.MoveType.Attack)
                {
                    InsertValue((short)move.EnemyTilePosX);
                    InsertValue((short)move.EnemyTilePosY);
                }
            }

            InsertValue((short) (playerTurnChanged ? 1 : 0));
        }
    }
}