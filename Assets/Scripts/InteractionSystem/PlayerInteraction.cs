using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviourSingleton<PlayerInteraction>
{
    [Header("Configuration Values")]
    [Space(5f)]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private GameObject _cursor;
    [SerializeField] private Canvas _cursorCanvas;
    private RectTransform _cursorRectTrans;

    [Space(10f)]
    [Header("Input Values")]
    [Space(5f)]
    [SerializeField] private float _dragFollowSpeed = 0.35f;
    public float DragFollowSpeed => _dragFollowSpeed;

    [Space(10f)]
    [Header("Debug Values")]
    [Space(5f)]
    [SerializeField][ReadOnly] private bool _isInteracting = false;
    public bool IsInteracting => _isInteracting;

    [SerializeField][ReadOnly] private Interactable _currentInteractable;
    public Interactable CurrentInteractable
    {
        get => _currentInteractable;

        set
        {
            if (value != _currentInteractable)
            {
                _currentInteractable?.OnInteractHover(false);
                value?.OnInteractHover(true);
            }

            _currentInteractable = value;
        }
    }

    public void SetInteractable(Interactable interactable)
    {
        _currentInteractable = interactable;

        _currentInteractable.OnInteractBegin();

        ((Draggabble)_currentInteractable).ResetOffSet();
    }
    public void ResetInteractable(bool stillInteracting = false)
    {
        Debug.Log($"RESET INTERACTABLE (former: {_currentInteractable?.name})", this);
        _currentInteractable?.OnInteractEnd();
        _isInteracting = stillInteracting;
        _currentInteractable = null;
    }

    public Vector3 MousePosition => _cursor.transform.position;
    [SerializeField][ReadOnly] private Vector3 _mousePosition;


    private void Awake()
    {
        base.SingletonCheck(this, false);
    }

    private void Start()
    {
        Cursor.visible = false;
        _cursorRectTrans = _cursor.GetComponent<RectTransform>();
    }
    private void Update()
    {
        MoveCursor();

        _isInteracting = Mouse.current.leftButton.isPressed;;

        if (_currentInteractable != null) Interact();

        if (!_isInteracting) CheckForInteractable();

        _mousePosition = MousePosition;
    }

    private void MoveCursor()
    {
        Vector2 pos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _cursorCanvas.transform as RectTransform,
            Mouse.current.position.ReadValue(),
            _cursorCanvas.worldCamera,
            out pos
        );

        Vector3 finalPos = pos;
        finalPos.z = _cursorRectTrans.anchoredPosition3D.z;

        _cursorRectTrans.anchoredPosition = pos;
    }

    private void CheckForInteractable()
    {
        Vector3 pos = MousePosition;

        Vector3 cameraPos = _mainCamera.transform.position;

        Ray ray = new Ray(cameraPos, pos - cameraPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 50f))
        {
            Interactable temp = hit.collider.gameObject.GetComponent<Interactable>();

            // Debug.Log($"COLLIDED WITH : {hit.collider?.gameObject.name}");

            if (temp != null)
            {
                CurrentInteractable = temp;
            }
            else CurrentInteractable = null;
        }
        else CurrentInteractable = null;
    }

    private void Interact()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _currentInteractable?.OnInteractBegin();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            _currentInteractable?.OnInteract();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _currentInteractable?.OnInteractEnd();
        }
    }

    private void OnDrawGizmos()
    {
        if (_cursor == null) return;
        
        Vector3 pos = MousePosition;
        Vector3 cameraPos = _mainCamera.transform.position;

        Gizmos.color = _currentInteractable == null ? Color.red : Color.blue;
        Gizmos.DrawRay(cameraPos, (pos - cameraPos) * 50);

        if (_currentInteractable != null) 
            Gizmos.DrawWireSphere(_currentInteractable.transform.position, 1f);

    }
}
