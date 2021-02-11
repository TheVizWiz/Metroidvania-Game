using UnityEngine;

namespace Interfaces {
    
    public interface ICarryable {
        void Pickup(Transform carrierTransform, Vector3 offset);
        void Release();
    }

    public interface ICarrier {

        Transform GetCarrierTransform();
    }

    public interface IThrowable {
        void Throw();
    }

    public interface IBreakable {
        void Break();
    }

    public interface IStrikable {
        bool Strike(float damage, ElementType element);
    }

    public interface ISlashable {
        bool Slash(float damage, float throwForce, float weakenAmount, float weakenTime, ElementType element);
    }

    public interface IExplodable {
        //returns if the item dies from the explosion or not
        bool Explode(float damage, ElementType element);
    }

    public interface IProgressBar {
        void SetCurrentValue(float newVal, bool checkBounds);

        float GetCurrentValue();
    }

    public interface IAnimatedUI {
        void Show();

        void Hide();
    }

    public interface IInteractable {
        public void Interact();
    }
    

}