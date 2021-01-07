using Network.Packets.Out.Commands;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace LoggedInScene
{
    public class PlayLogic : MonoBehaviour
    {
        public UnityEvent onMatchFound;
        public UnityEvent onMatchRejectedByOpponent;

        private int _lastFoundedMatchId;
        
        public void SearchCompetitiveMatch()
        {
            Debug.Log("Searching for competitive match!");
        
            NetworkManager.SendPacket(new PlayCompetitiveRequestOutCommand());
        }

        public void MarkMatchAsFounded(int matchId)
        {
            _lastFoundedMatchId = matchId;
            onMatchFound.Invoke();
        }

        public void AcceptLastFoundedMatchInvitation()
        {
            NetworkManager.SendPacket(new MatchFoundResponseOutCommand(MatchFoundResponseOutCommand.ResponseType.Accept, _lastFoundedMatchId));
        }
        
        public void RejectLastFoundedMatchInvitation()
        {
            NetworkManager.SendPacket(new MatchFoundResponseOutCommand(MatchFoundResponseOutCommand.ResponseType.Reject, _lastFoundedMatchId));
        }

        public void StartMatch()
        {
            SceneManager.LoadScene("Match", LoadSceneMode.Single);
        }
    }
}
