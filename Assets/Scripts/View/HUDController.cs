using UnityEngine;
using TMPro;
using Game.Tatedrez.Model;

namespace Game.Tatedrez.View
{
    public class HUDController : MonoBehaviour
    {
        [Header("HUD Elements")]
        [SerializeField] private TextMeshProUGUI currentPlayerText;
        [SerializeField] private TextMeshProUGUI totalMovesHUD;

        public void UpdateHUD(int totalMoves, Player currentPlayer)
        {
            totalMovesHUD.text = $"Total Moves: {totalMoves}";

            if(currentPlayer != null)
            {
                currentPlayerText.text = $"Current Player: {currentPlayer.Color}";

                if(currentPlayer.Color == PlayerColor.Black)
                {
                    currentPlayerText.color = Color.black;
                }
                else
                {
                    currentPlayerText.color = Color.white;
                }
            }
        }
    }
}
