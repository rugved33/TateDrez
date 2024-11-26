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

            this.pieceFactory = new PieceFactory();
            return pieceFactory;
        }
    }
}