using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Card : MonoBehaviour
{
    public enum DragDirection
    {
        Up,
        Down,
        Right,
        Left
    }

    const int StackCheckDenoter = 0;

    //Settings

    public int indexInStack = 0;

    public bool isAnchor = false;
    public bool isDebugging = false;

    public float moveDuration = 0.5f;
    public float directionAngle = 45;
    public float swipeDistance = 1.5f;
    public float horizontalMoveAmount = 1f;
    public float verticalMoveAmount = 1.5f;
    public float offset = 0.1f;
    public float colliderOnHoldMultiplier = 1.2f;
    public float currentMultiplier = 0;
    public float moveFxDuration = 0.5f;
    public float minimizeTweenDuration = 0.2f;

    public string colorTag = "white";
    // Connections
    public GameObject band;
    public GameObject fxGameObject;
    public GameObject swipeFxGameObject;
    public GameObject windEffects;
    public GameObject confettiEffect;

    public Transform childCard;
    public Transform cardParent;

    public DOTweenAnimation wrongMoveTween;
    public DOTweenAnimation waveTween;

    Collider col;
    BoxCollider boxCol;
    Card card;
    // State Variables
    Vector3 mousePosition;
    Vector3 dragDirection;
    Vector3[] colliderSize;

    public bool canSwipe = false;
    public bool isMoving = false;
    bool isComplete = false;
    bool isDomino = false;

    DragDirection dominoDirection;
    // Start is called before the first frame update
	void Awake()
	{
		InitConnections();
	}
    void Start()
    {
        InitState();
    }
    void InitConnections()
    {
        colliderSize = new Vector3[2];
        col = GetComponent<Collider>();
        boxCol = GetComponent<BoxCollider>();
    }
    void InitState()
    {
        colorTag = tag;

        if (isAnchor)
        {
            band.SetActive(true);
        }

        colliderSize[0] = boxCol.size;
        colliderSize[1] = boxCol.size * colliderOnHoldMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDebugging)
            DrawDebugRays();

        if(childCard != null && !isComplete)
        {
            if(childCard.localPosition.y != -offset && !card.isMoving)
            {
                childCard.localPosition = new Vector3(childCard.localPosition.x, -offset, childCard.localPosition.z);
                if (CompareTag("Red"))
                {
                    Debug.Log("NANANANANA");
                }
            }
        }

        currentMultiplier = boxCol.size.x / colliderSize[0].x;
    }

    private void OnMouseDown()
    {
        dragDirection = Vector3.zero;
    }

    private void OnMouseDrag()
    {
        if (canSwipe)
        {
            mousePosition = Input.mousePosition;
            mousePosition.z = -Camera.main.transform.position.z;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            

            if (Vector3.Distance(transform.position, mousePosition) > swipeDistance && canSwipe)
            {
                dragDirection = mousePosition - transform.position;
                EventManager.DisableSwipingEvent();

                if (CheckMovementAvailability(GetSwipeDirection()))
                {
                    EventManager.CardSwipedEvent();
                    EventManager.DisableSwipingEvent();
                }
                else
                {
                    //Cant move to target space
                    EventManager.DisableCollidersEvent();
                    DisableSwiping();
                    DisableWindInChildren();
                    wrongMoveTween.DORestart();                    
                }
            }
        }
    }

    private void OnMouseUp()
    {
        EventManager.EnableSwipingEvent();
    }

    
    void DrawDebugRays()
    {
        Vector3 dirVec = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (90 - directionAngle)), Mathf.Sin(Mathf.Deg2Rad * (90 - directionAngle)), 0);
        Debug.DrawRay(transform.position, dirVec * 200, Color.blue);
        dirVec = -dirVec;
        Debug.DrawRay(transform.position, dirVec * 200, Color.blue);

        dirVec = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (90 + directionAngle)), Mathf.Sin(Mathf.Deg2Rad * (directionAngle + 90)), 0);
        Debug.DrawRay(transform.position, dirVec * 200, Color.blue);
        dirVec = -dirVec;
        Debug.DrawRay(transform.position, dirVec * 200, Color.blue);

        if (dragDirection != Vector3.zero)
        {
            Debug.DrawRay(transform.position, mousePosition * 200, Color.green);
        }
    }

    private DragDirection GetSwipeDirection()
    {
        if (dragDirection.x >= 0 && dragDirection.y >= 0)
        {
            
            if (Vector3.Angle(transform.right, dragDirection) >= directionAngle)
            {
                return DragDirection.Up;
            }

            return DragDirection.Right;
        }
        else if(dragDirection.x >= 0)
        {
            
            if (Vector3.Angle(transform.right, dragDirection) >= directionAngle)
            {
                return DragDirection.Down;
            }

            return DragDirection.Right;
        }
        else if(dragDirection.x < 0 && dragDirection.y >= 0)
        {
            
            if(Vector3.Angle(transform.right, dragDirection) <= directionAngle + 90)
            {
                return DragDirection.Up;
            }

            return DragDirection.Left;
        }
        else
        {
            
            if (Vector3.Angle(transform.right, dragDirection) <= directionAngle + 90)
            {
                return DragDirection.Down;
            }

            return DragDirection.Left;
        }
    }

    bool CheckMovementAvailability(DragDirection dir, bool spaceAllowed = true)
    {
        RaycastHit hit;
        if(dir == DragDirection.Up)
        {
            Debug.DrawRay(transform.position, transform.up * verticalMoveAmount, Color.red, 10);
            if(Physics.Raycast(transform.position, transform.up, out hit, verticalMoveAmount))//returns true if ray hits a collider.
            {
                //Target space is occupied
                if (hit.collider.CompareTag(tag) && hit.collider.GetComponent<Card>().indexInStack > indexInStack && !isAnchor)
                {
                    MoveToCard(dir);
                    return true;
                }                
            }
            else if(spaceAllowed)
            {
                //Target space is empty
                return MoveToSpace(dir);
            }
            else
            {
                return false;
            }
        }


        else if (dir == DragDirection.Down)
        {
            Debug.DrawRay(transform.position, -transform.up * 100, Color.red, 10);
            if (Physics.Raycast(transform.position, -transform.up, out hit, verticalMoveAmount))//returns true if ray hits a collider.
            {
                //Target space is occupied
                if (hit.collider.CompareTag(tag) && hit.collider.GetComponent<Card>().indexInStack > indexInStack && !isAnchor)
                {
                    MoveToCard(dir);
                    return true;
                }
            }
            else if(spaceAllowed)
            {
                //Target space is empty
                return MoveToSpace(dir);
            }
            else
            {
                return false;
            }
        }


        else if(dir == DragDirection.Right)
        {
            Debug.DrawRay(transform.position, transform.right * 100, Color.red, 10);
            if (Physics.Raycast(transform.position, transform.right, out hit, horizontalMoveAmount))//returns true if ray hits a collider.
            {
                //Target space is occupied
                if (hit.collider.CompareTag(tag) && hit.collider.GetComponent<Card>().indexInStack > indexInStack && !isAnchor)
                {
                    MoveToCard(dir);
                    return true;
                }
            }
            else if(spaceAllowed)
            {
                //Target space is empty
                return MoveToSpace(dir);
            }
            else
            {
                return false;
            }
        }


        else
        {
            Debug.DrawRay(transform.position, -transform.right * 100, Color.red, 10);
            if (Physics.Raycast(transform.position, -transform.right, out hit, horizontalMoveAmount))//returns true if ray hits a collider.
            {
                //Target space is occupied
                if (hit.collider.CompareTag(tag) && hit.collider.GetComponent<Card>().indexInStack > indexInStack && !isAnchor)
                {
                    MoveToCard(dir);
                    return true;
                }
            }
            else if(spaceAllowed)
            {
                //Target space is empty
                return MoveToSpace(dir);
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    void MoveToCard(DragDirection direction)
    {
        isMoving = true;

        if(direction == DragDirection.Up)
        {
            transform.DOMoveY(transform.position.y + verticalMoveAmount, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                isMoving = false;
                if(isDomino)
                    DominoEffect(direction);
            });
        }

        else if(direction == DragDirection.Down)
        {
            transform.DOMoveY(transform.position.y - verticalMoveAmount, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                isMoving = false;
                if (isDomino)
                    DominoEffect(direction);
            });
        }

        else if(direction == DragDirection.Right)
        {
            transform.DOMoveX(transform.position.x + horizontalMoveAmount, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                isMoving = false;
                if (isDomino)
                    DominoEffect(direction);
            });
        }

        else//Move left
        {
            transform.DOMoveX(transform.position.x - horizontalMoveAmount, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                isMoving = false;
                if (isDomino)
                    DominoEffect(direction);
            });
        }
    }

    bool MoveToSpace(DragDirection direction)
    {
        if(direction == DragDirection.Up)
        {
            if(childCard != null)
            {
                //In a stack
                card.MoveFromUnder(direction);
                childCard.parent = cardParent;
                childCard = null;
                return true;
            }
            return false;
        }


        else if (direction == DragDirection.Down)
        {
            if (childCard != null)
            {
                //In a stack
                card.MoveFromUnder(direction);
                childCard.parent = cardParent;
                childCard = null;
                return true;
            }
            return false;
        }


        else if (direction == DragDirection.Right)
        {
            if (childCard != null)
            {
                //In a stack
                card.MoveFromUnder(direction);
                childCard.parent = cardParent;
                childCard = null;
                return true;
            }
            return false;
        }


        else
        {
            if (childCard != null)
            {
                //In a stack
                card.MoveFromUnder(direction);
                childCard.parent = cardParent;
                childCard = null;
                return true;
            }
            return false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tag))
        {
            if (isMoving)
            {
                col.enabled = false;
            }
            else
            {                
                //PlayMoveFx();
                AssignChildCard(other.transform);
                Invoke(nameof(CallStackChecker), moveDuration);
            }
        }
    }

    public void AssignChildCard(Transform cardTransform)
    {
        if(childCard == null)
        {
            childCard = cardTransform;
            card = childCard.GetComponent<Card>();
            childCard.parent = transform;
            childCard.position = new Vector3(childCard.position.x, childCard.position.y, transform.position.z + offset);
            if(indexInStack - card.indexInStack == 1)
            {
                PlayMoveFx();
            }
        }
        else
        {
            if(childCard != null)
            {
                card.AssignChildCard(cardTransform);
            }
            else
            {
                
            }
        }
    }

    public void DominoEffect(DragDirection direction)
    {
        if (transform.parent != cardParent)
        {
            transform.parent.TryGetComponent<Card>(out Card cardP);
            if (cardP != null)
            {
                cardP.DominoEffect(direction);
            }
        }
        else
        {
            if (CheckMovementAvailability(direction, false))
            {
                EventManager.DisableSwipingEvent();
            }
            else
            {
                EventManager.EnableSwipingEvent();
            }

        }
    }

    public void MoveFromUnder(DragDirection direction)
    {
        if(direction == DragDirection.Up)
        {
            transform.DOMoveY(transform.position.y + verticalMoveAmount, moveDuration).OnComplete(() =>
            {
                windEffects.SetActive(false);
                transform.position = new Vector3(transform.position.x, transform.position.y + offset, 0);
                col.enabled = true;
                windEffects.SetActive(true);
            });
        }
        else if(direction == DragDirection.Down)
        {
            transform.DOMoveY(transform.position.y - verticalMoveAmount, moveDuration).OnComplete(() =>
            {
                windEffects.SetActive(false);
                transform.position = new Vector3(transform.position.x, transform.position.y + offset, 0);
                col.enabled = true;
                windEffects.SetActive(true);
            });
        }
        else if(direction == DragDirection.Right)
        {
            transform.DOMoveX(transform.position.x + horizontalMoveAmount, moveDuration).OnComplete(() =>
            {
                windEffects.SetActive(false);
                transform.position = new Vector3(transform.position.x, transform.position.y + offset, 0);
                col.enabled = true;
                windEffects.SetActive(true);
            });
        }
        else if(direction == DragDirection.Left)
        {
            transform.DOMoveX(transform.position.x - horizontalMoveAmount, moveDuration).OnComplete(() =>
            {
                windEffects.SetActive(false);
                transform.position = new Vector3(transform.position.x, transform.position.y + offset, 0);
                col.enabled = true;
                windEffects.SetActive(true);
            });
        }
    }

    void CallStackChecker()
    {
        CheckIfStackIsComplete();
    }

    public void CheckIfStackIsComplete(int index = StackCheckDenoter, bool hasAnchor = false)
    {
        //0 to denote it got called on trigger
        if (isAnchor)
        {
            //Starts from anchor
            if(childCard != null)
            {
                card.CheckIfStackIsComplete(indexInStack, isAnchor);
            }
        }

        else
        {
            if (hasAnchor)
            {
                //Starts from anchor
                if(index - indexInStack == 1)
                {
                    //Is stacked correctly
                    if(childCard != null)
                    {
                        card.CheckIfStackIsComplete(indexInStack, hasAnchor);
                    }
                    else if(indexInStack == 1)
                    {
                        //.StackIsCompleteEvent(tag);
                        PlayMexicanWave();
                    }
                    else
                    {
                        EventManager.StackIsIncompleteEvent();
                    }
                }

                else
                {
                    EventManager.StackIsWrongEvent();
                }
            }

            else
            {
                //Doesn't have an anchor
                if(index == StackCheckDenoter)
                {
                    //Is the top card
                    if(childCard != null)
                    {
                        card.CheckIfStackIsComplete(indexInStack, hasAnchor);
                    }
                }

                else
                {
                    if (index - indexInStack == 1)
                    {
                        //Is stacked correctly
                        if (childCard != null)
                        {
                            card.CheckIfStackIsComplete(indexInStack, hasAnchor);
                        }
                        else
                        {
                            EventManager.StackIsIncompleteEvent();
                        }
                    }
                    else
                    {
                        EventManager.StackIsWrongEvent();
                    }
                }
                
            }
        }
        
    }

    private void OnEnable()
    {
        EventManagerInit();
        if(isAnchor)
            EventManager.AnchorEnabledEvent();
    }

    void EventManagerInit()
    {
        EventManager.EnableSwiping += EnableSwiping;
        EventManager.OnLevelStarted += EnableSwiping;;
        EventManager.DisableSwiping += DisableSwiping;
        EventManager.CardConfig += CardConfig;
        EventManager.FixRotation += FixRotation;
        EventManager.OnSensitivityChanged += OnSensitivityChanged;
        EventManager.DisableColliders += DisableCollider;
        EventManager.EnableColliders += EnableCollider;
        if (isAnchor)
        {
            EventManager.StackIsComplete += PlayCompleteFx;
        }
        else
        {
            EventManager.StackIsComplete += MakeComplete;
        }
    }

    void EnableSwiping()
    {
        if (!isComplete)
        {
            canSwipe = true;
        }

        boxCol.size = colliderSize[1];
    }

    void DisableSwiping()
    {
        canSwipe = false;

        boxCol.size = colliderSize[0];
    }

    void PlayCompleteFx(string cardTag)
    {
        if (CompareTag(cardTag))
        {
            fxGameObject.SetActive(true);
            DisableSwiping();
            isComplete = true;
            MinimizeStack();
            Debug.Log("Called minimizer");
        }
    }

    void MakeComplete(string cardTag)
    {
        if (CompareTag(cardTag))
        {
            isComplete = true;
            DisableSwiping();
        }
    }

    void PlayMoveFx()
    {
        swipeFxGameObject.SetActive(true);
        Invoke(nameof(StopMoveFx), moveFxDuration);
    }

    void StopMoveFx()
    {
        swipeFxGameObject.SetActive(false);
    }

    void CardConfig()
    {
        isDomino = true;
    }

    void FixRotation()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        if (transform.parent != cardParent)
            transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
    }

    public void OnWrongMoveTweenComplete()
    {
        EnableSwiping();
        EventManager.FixRotationEvent();
        EventManager.EnableCollidersEvent();
        EnableWindInChildren();
    }
    
    void OnSensitivityChanged(float sensitivity)
    {
        swipeDistance = sensitivity;
    }

    void DisableCollider()
    {
        col.enabled = false;
    }

    void EnableCollider()
    {
        if (transform.parent == cardParent)
        {
            col.enabled = true;
        }
        
    }

    public void PlayMexicanWave()
    {
        windEffects.SetActive(false);
        waveTween.DORestart();
    }

    public void OnWaveTweenEnd()
    {
        if(transform.parent != cardParent)
        {
            Card pCard = transform.parent.GetComponent<Card>();
            pCard.PlayMexicanWave();
        }
        else if(transform.parent == cardParent)
        {
            PlayConfettiEffect();
        }
        else
        {
            return;
        }
    }
    

    void PlayConfettiEffect()
    {
        if(confettiEffect != null)
        {
            confettiEffect?.SetActive(true);
            Invoke(nameof(InvokeCompletionEvent), confettiEffect.GetComponent<ParticleSystem>().main.duration);
        }
        else
        {
            InvokeCompletionEvent();
        }
    }

    void InvokeCompletionEvent()
    {
        EventManager.StackIsCompleteEvent(tag);
    }

    public void DisableWindInChildren()
    {
        windEffects.SetActive(false);

        if(childCard != null)
        {
            card.DisableWindInChildren();
        }
    }

    public void EnableWindInChildren()
    {
        windEffects.SetActive(true);

        if(childCard != null)
        {
            card.EnableWindInChildren();
        }
    }

    public void MinimizeStack()
    {
        if (childCard != null)
        {
            if (!isAnchor)
            {
                transform.DOLocalMoveY(0, minimizeTweenDuration).SetEase(Ease.Linear).OnComplete(() =>
                {
                    card.MinimizeStack();
                });
            }
            else
            {
                card.MinimizeStack();
            }
        }
        else
        {
            transform.DOLocalMoveY(0, minimizeTweenDuration).SetEase(Ease.Linear);
        }
    }

    public bool CheckIfMoveIsRight(int index)
    {
        return false;
    }
}

