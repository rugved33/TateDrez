using UnityEngine;
using TMPro;
using Game.Tatedrez.Model;
using System.Collections;

namespace Game.Tatedrez.View
{
    public class HUDController : MonoBehaviour
    {
        [Header("HUD Elements")]
        [SerializeField] private TextMeshProUGUI currentPlayerText;
        [SerializeField] private TextMeshProUGUI totalMovesHUD;

        [SerializeField] private TextMeshProUGUI feedbackText;

        [Header("Feedback Settings")]
        [SerializeField] private float feedbackDuration = 2f;

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
        public void ShowFeedback(string message)
        {
            feedbackText.text = message;
            feedbackText.gameObject.SetActive(true);
            StartCoroutine(HideFeedbackAfterDelay());
        }

        private IEnumerator HideFeedbackAfterDelay()
        {
            yield return new WaitForSeconds(feedbackDuration);
            feedbackText.gameObject.SetActive(false);
        }
    }
}
