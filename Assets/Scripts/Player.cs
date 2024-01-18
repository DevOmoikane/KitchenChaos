using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent {

    public static Player Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Player already exists!");
            return;
        }
        Instance = this;
    }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }
    public event EventHandler OnPickedObject;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float slerpSpeed = 20f;
    [SerializeField] private float playerRadius = 0.7f;
    [SerializeField] private float playerHeight = 2.0f;
    [SerializeField] private float interactionDistance = 2.0f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private bool isWalking = false;
    private Vector3 lastInteractDirection;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e) {
        if (selectedCounter != null) {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
        if (selectedCounter != null) {
            selectedCounter.Interact(this);
        }
    }

    private void Update() {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking() {
        return isWalking;
    }

    private Vector3 GetMoveDir() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        return new Vector3(inputVector.x, 0, inputVector.y);
    }

    private void HandleInteractions() {
        Vector3 moveDir = GetMoveDir();
        if (moveDir != Vector3.zero) {
            lastInteractDirection = moveDir;
        }
        if (Physics.Raycast(transform.position, lastInteractDirection, 
            out RaycastHit raycastHit, interactionDistance, countersLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out BaseCounter counter)) {
                if (counter != selectedCounter) {
                    SetSelectedCounter(counter);
                }
            } else {
                SetSelectedCounter(null);
            }
        } else {
            SetSelectedCounter(null);
        }
    }

    private void SetSelectedCounter(BaseCounter selectedCounter) {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = selectedCounter
        });
    }

    private void HandleMovement() {
        Vector3 moveDir = GetMoveDir();
        float moveDistance = moveSpeed * Time.deltaTime;
        Vector3 capsulePoint1 = transform.position + Vector3.up * playerRadius;
        Vector3 capsulePoint2 = transform.position + Vector3.up * (playerHeight - playerRadius);
        bool canMove = !Physics.CapsuleCast(capsulePoint1, capsulePoint2,
            playerRadius, moveDir, moveDistance);
        isWalking = moveDir != Vector3.zero;
        if (!canMove) {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0);
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(capsulePoint1, capsulePoint2,
                playerRadius, moveDirX, moveDistance);
            if (canMove) {
                moveDir = moveDirX;
            } else {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z);
                canMove = moveDir.z != 0 && !Physics.CapsuleCast(capsulePoint1, capsulePoint2,
                    playerRadius, moveDirZ, moveDistance);
                if (canMove) {
                    moveDir = moveDirZ;
                }
            }
        }
        if (canMove) {
            transform.position += moveDir * moveDistance;
        }
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * slerpSpeed);
    }

    public Transform GetKitchenObjectFollowTransform() {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null) {
            OnPickedObject?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }

    public void ClearKitchenObject() {
        kitchenObject = null;
    }

    public bool HasKitchenObject() {
        return kitchenObject != null;
    }
}
