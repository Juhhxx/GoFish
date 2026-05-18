using UnityEngine;
using UnityEngine.Events;

public class Button : Interactable
{
    public UnityEvent OnButtonClickDown;
    public UnityEvent OnButtonClickUp;
    public UnityEvent OnButtonHoverEnter;
    public UnityEvent OnButtonHoverExit;

    private void Start()
    {
        Draggabble drag = GetComponentInParent<Draggabble>();

        if (drag != null)
        {
            InteractBegin   += drag.OnInteractBegin;
            Interact        += drag.OnInteract;
            InteractEnd     += drag.OnInteractEnd;
        }

        InteractBegin   += OnButtonClickDown.Invoke;
        InteractEnd     += OnButtonClickUp.Invoke;
        InteractHover   += (onOff) => {

            if (onOff) OnButtonHoverEnter?.Invoke();
            else OnButtonHoverExit?.Invoke();
        };
    }
    
}
