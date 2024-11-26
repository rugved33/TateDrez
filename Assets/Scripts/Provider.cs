using UnityEngine;
using DependencyInjection;
using Game.Tatedrez.View;


namespace Game.Tatedrez
{
    public class Provider : MonoBehaviour, IDependencyProvider
    {
        private UINavigationController navigationController = null;
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
    }
}