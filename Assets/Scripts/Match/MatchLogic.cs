using Network.Packets.Out.Commands;
using UnityEngine;
using UnityEngine.Events;

namespace Match
{
    public class MatchLogic : MonoBehaviour
    {
        public UnityEvent onOpponentSentPlayerData;
        public UnityEvent onFirstPlayerTurnSelected;
        public UnityEvent onPlayerIsFirstTurn;
        public UnityEvent onPlayerOpponentIsFirstTurn;
        public UnityEvent onMatchWon;
        public UnityEvent onMatchLost;
        
        private void Start()
        {
            NetworkManager.SendPacket(new MatchSceneLoadedOutCommand());
        }
    }
}
