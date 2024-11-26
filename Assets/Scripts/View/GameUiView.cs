using UnityEngine;
using Game.Tatedrez.Model;
using DependencyInjection;
using UnityEngine.UI;

namespace Game.Tatedrez.View
{
    public class GameUIView : MonoBehaviour
    {
        [Header("Game Over UI")]
        [SerializeField] private GameObject gameScreen;
        [SerializeField] private GameObject winScreen;
        [SerializeField] private Button retryButton;
        [SerializeField] private TMPro.TextMeshProUGUI winnerText;

        [Header("HUD Controller")]
        [SerializeField] private HUDController hudController;

        private UINavigationController navigationController;

        [Inject]
        private void Init(UINavigationController uINavigationController)
        {
            navigationController = uINavigationController;
        }

        private void Start() 
        {
            retryButton.onClick.AddListener(()=> ResetUI());
        }

        public void BindRetryButton(System.Action OnRetry)
        {
            retryButton.onClick.AddListener(() => OnRetry());
        }

        public void ShowWinScreen(Player winner)
        {
            winnerText.text = $"{winner.Color} Wins!";
            navigationController.Push(winScreen);
        }

        public void UpdateHUD(int totalMoves, Player currentPlayer)
        {
            hudController.UpdateHUD(totalMoves, currentPlayer);
        }

        public void ResetUI()
        {
            navigationController.Pop(); 
            hudController.UpdateHUD(0, null); 
        }
    }
}
