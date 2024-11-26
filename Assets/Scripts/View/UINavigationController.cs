using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tatedrez.View
{
    public class UINavigationController
    {
        public bool popUpEnabled;
        public delegate void NavigationEventCompleted();
        private Stack<GameObject> navigationStack = new Stack<GameObject>();

        public void Push(GameObject popup, bool newFlow = true, bool shouldActivateImmediately = true)
        {
            if (newFlow)
            {
                Pop();
            }
            else
            {
                GameObject popupToClose = navigationStack.Peek();
                DeactivatePopup(popupToClose);
            }

            navigationStack.Push(popup);
            if (!popUpEnabled) { popUpEnabled = true; }

            if (shouldActivateImmediately)
            {
                ActivatePopup(popup);
            }
        }

        public void Pop()
        {
            if (navigationStack.Count > 0)
            {
                GameObject popupToClose = navigationStack.Pop();
                DeactivatePopup(popupToClose);
            }
        }

        private void ActivatePopup(GameObject popup)
        {
            popup.SetActive(true);
        }

        private void DeactivatePopup(GameObject popup)
        {
            popup.SetActive(false);

            if (navigationStack.Count > 0)
            {
                GameObject popupToOpen = navigationStack.Peek();
                ActivatePopup(popupToOpen);
            }
            else
            {
                popUpEnabled = false;
            }
        }
    }
}
