using UnityEngine;
using DependencyInjection;
using Game.Tatedrez.View;
using Game.Tatedrez.Factory;


namespace Game.Tatedrez
{
    public class Provider : MonoBehaviour, IDependencyProvider
    {
        private UINavigationController navigationController = null;
        private IPieceFactory pieceFactory = null;
        private IPlayerFactory playerFactory = null;

        [Provide]
        public UINavigationController NavigationController()
        {
            if(navigationController != null)
            {
                return navigationController;
            }

            navigationController = new UINavigationController();
            return navigationController;
        }

        [Provide]
        public IPieceFactory ProvidePieceFactory()
        {
            if(pieceFactory != null)
            {
                return pieceFactory;
            }

            pieceFactory = new PieceFactory();
            return pieceFactory;
        }

        [Provide]
        public IPlayerFactory ProvidePlayerFactory()
        {
            if(playerFactory != null)
            {
                return playerFactory;
            }

            playerFactory = new PlayerFactory();
            return playerFactory;
        }
    }
}