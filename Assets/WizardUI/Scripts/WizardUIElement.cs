using UnityEngine;

namespace WizardUI
{
    public abstract class WizardUIElement : MonoBehaviour
    {
        public bool Visible
        {
            get => gameObject.activeSelf;
            set
            {
                if (value != Visible)
                {
                    gameObject.SetActive(value);
                    
                    if (value) show();
                    else hide();
                }
            }
        }

        public virtual void OnAppearing()
        {

        }
        public virtual void OnDisappearing()
        {

        }

        void show()
        {
            gameObject.SetActive(true);
            OnAppearing();
        }

        void hide()
        {
            gameObject.SetActive(false);
            OnDisappearing();
        }
    }
}
