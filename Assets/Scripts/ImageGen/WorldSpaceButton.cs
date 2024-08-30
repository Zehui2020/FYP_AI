using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;
using System.Collections;

public class WorldSpaceButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private BoxCollider2D buttonCollider;
    private ComfyPromptCtr promptCtr;
    [SerializeField] private TextMeshPro promptText;

    private bool _isHovering = false;
    public bool _isDragging = false;
    private Vector2 _pointerDownPosition;
    private float _dragThreshold = 10f;
    private Vector3 _clickOffset;

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Rigidbody2D _jointRB;
    [SerializeField] private HingeJoint2D _hingeJoint;

    public float clickThreshold = 0.3f;
    private float _pointerDownTime;
    private bool _isPointerDown = false;

    [Header("Input Events")]
    public UnityEvent onClick;
    public UnityEvent onPointerDown;
    public UnityEvent onPointerUp;
    public UnityEvent onHoverEnter;
    public UnityEvent onHoverExit;
    public UnityEvent onDrag;
    public UnityEvent onDragEnd;

    [Header("Check Grounded")]
    public UnityEvent onGrounded;
    public UnityEvent onUngrounded;

    private string prompt;

    private void Start()
    {
        _hingeJoint.enabled = false;
    }

    private void Update()
    {
        if (Mathf.Abs(transform.position.x) > 10 || Mathf.Abs(transform.position.y) > 10)
        {
            if (!_isDragging)
            {
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = 0;
            }
        }
    }

    public void SetPrompt(string prompt, ComfyPromptCtr promptCtr)
    {
        this.prompt = prompt;
        this.promptCtr = promptCtr;
        promptText.text = this.prompt;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isHovering)
        {
            _isHovering = true;
            onHoverEnter?.Invoke();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isHovering)
        {
            _isHovering = false;
            onHoverExit?.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 cursorWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.WorldToScreenPoint(transform.position).z));
        cursorWorldPosition.z = transform.position.z;
        _clickOffset = cursorWorldPosition - transform.position;

        _hingeJoint.enabled = true;
        _hingeJoint.transform.position = cursorWorldPosition;

        _pointerDownPosition = eventData.position;
        _isDragging = false;
        onPointerDown?.Invoke();
        _rb.velocity = Vector2.zero;

        _pointerDownTime = Time.time;
        _isPointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isDragging)
        {
            _hingeJoint.enabled = false;
            _isDragging = false;
            onDragEnd?.Invoke();
            onPointerUp?.Invoke();
        }

        if (!_isPointerDown || _isDragging)
        {
            return;
        }

        float duration = Time.time - _pointerDownTime;
        if (duration <= clickThreshold)
            onClick?.Invoke();

        _isPointerDown = false;
    }

    public void OnInitializePotentialDrag(PointerEventData ped)
    {
        ped.useDragThreshold = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Vector2.Distance(eventData.position, _pointerDownPosition) < _dragThreshold)
        {
            return;
        }

        if (!_isDragging)
        {
            _isDragging = true;
            onDrag?.Invoke();
            _rb.velocity = Vector2.zero;

            onUngrounded?.Invoke();
        }
    }

    private void FixedUpdate()
    {
        if (_isDragging)
        {
            Vector3 cursorWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z));
            cursorWorldPosition.z = transform.position.z;

            Vector3 newPosition = cursorWorldPosition - _clickOffset / 10f;

            _jointRB.MovePosition(newPosition);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.collider.CompareTag("Button") && !col.collider.CompareTag("Ground"))
        {
            return;
        }

        onGrounded?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        promptCtr.AddPrompt(prompt);
        gameObject.SetActive(false);
        _hingeJoint.enabled = false;
        _isDragging = false;
    }
}