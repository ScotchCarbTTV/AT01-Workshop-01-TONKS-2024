using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TonkMovement : MonoBehaviour
{
    [SerializeField] private GameObject frontNode, backNode;
    [SerializeField] Vector3 rightPos = new Vector3();
    [SerializeField] Vector3 leftPos = new Vector3();


    [SerializeField] int actionPoints;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotSpeed = 5f;

    //enum for directions GET USED TO USING THEM BUCKO
    public enum Direction { forward, back, left, right, idle }
    Direction currDirection;

    [SerializeField] bool moving = false;

    private void Awake()
    {
        currDirection = Direction.idle;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.startPlayerTurnEvent += ReplenishActionPoints;

        SetFrontBackNodes();
        SetLeftRightNodes();
        //temporarily just assign 3 action points on game start to player
        actionPoints = 3;
    }



    // Update is called once per frame
    void Update()
    {
        //DEBUG DONT LEAVE THIS IN
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            EndTurn();
        }


        if (!moving)
        {
            if (actionPoints > 0)
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    SetFrontBackNodes();
                    MovementInput(Direction.back);
                    actionPoints--;
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    SetFrontBackNodes();
                    MovementInput(Direction.forward);
                    actionPoints--;
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    SetLeftRightNodes();
                    MovementInput(Direction.left);
                    actionPoints--;
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    SetLeftRightNodes();
                    MovementInput(Direction.right);
                    actionPoints--;
                }
            }
            else
            {
                //pull up the 'No Points remaining! Click to end turn!' prompt
            }
        }
        else
        {
            Movement(currDirection);
        }

    }

    private void Movement(Direction direction)
    {
        //basic movement logic - move either forward or back while forward or back are true and we aren't at the destination. Otherwise accept input.

        switch (direction)
        {
            case Direction.back:
                if (backNode == null)
                {
                    break;
                }
                Vector3 adjustedBackPosition = backNode.transform.position + new Vector3(0, -0.5f, 0);
                if (transform.position != adjustedBackPosition)
                {
                    Debug.Log("Moving!");
                    transform.position = Vector3.MoveTowards(transform.position, adjustedBackPosition, moveSpeed * Time.deltaTime);
                }
                else
                {
                    moving = false;
                    SetLeftRightNodes();
                    currDirection = Direction.idle;
                }
                break;
            case Direction.forward:
                if (frontNode == null)
                {
                    break;
                }
                Vector3 adjustedFrontPosition = frontNode.transform.position + new Vector3(0, -0.5f, 0);
                if (transform.position != adjustedFrontPosition)
                {
                    Debug.Log("Moving!");
                    transform.position = Vector3.MoveTowards(transform.position, adjustedFrontPosition, moveSpeed * Time.deltaTime);
                }
                else
                {
                    moving = false;
                    SetLeftRightNodes();
                    currDirection = Direction.idle;
                }
                break;
            case Direction.left:
                Debug.Log("Turning left");
                float angle1 = Vector3.Angle(transform.forward, leftPos);
                if (angle1 > 0)
                {
                    Vector3 newDir = Vector3.RotateTowards(transform.forward, leftPos, rotSpeed * Time.deltaTime, 1f);
                    transform.rotation = Quaternion.LookRotation(newDir);
                }
                else
                {
                    Debug.Log("Finished turning");
                    moving = false;
                    SetLeftRightNodes();
                    currDirection = Direction.idle;
                    break;
                }
                break;
            case Direction.right:
                Debug.Log("Turning right");
                float angle2 = Vector3.Angle(transform.forward, rightPos);
                if (angle2 > 0f)
                {
                    Vector3 newDir = Vector3.RotateTowards(transform.forward, rightPos, rotSpeed * Time.deltaTime, 1f);
                    transform.rotation = Quaternion.LookRotation(newDir);
                }
                else
                {
                    Debug.Log("Finished turning");
                    moving = false;
                    SetLeftRightNodes();
                    currDirection = Direction.idle;
                    break;
                }
                break;
        }
    }

    //check for node in front and in back method
    private void SetFrontBackNodes()
    {
        //modified position to throw the raycast from
        Vector3 modPosition = transform.position + new Vector3(0, 0.5f, 0);

        RaycastHit hit;
        //Debug.DrawRay(modPosition, transform.forward * 5, Color.red, 10);
        if (Physics.Raycast(modPosition, transform.forward, out hit, 5f))
        {
            if (hit.collider.gameObject.tag == "Node")
            {
                frontNode = hit.collider.gameObject;
            }
            else
            {
                frontNode = null;
                Debug.Log(hit.collider.name);
            }
        }
        if (Physics.Raycast(modPosition, -transform.forward, out hit, 5f))
        {
            if (hit.collider.gameObject.tag == "Node")
            {
                backNode = hit.collider.gameObject;
            }
            else
            {
                backNode = null;
                Debug.Log(hit.collider.name);
            }
        }
    }

    private void SetLeftRightNodes()
    {
        rightPos = transform.right;
        leftPos = -transform.right;
    }

    private void MovementInput(Direction direction)
    {
        switch (direction)
        {
            case Direction.forward:
                //move the player forward
                SetFrontBackNodes();
                if (frontNode != null)
                {
                    moving = true;
                    currDirection = Direction.forward;
                }
                else
                {
                    moving = false;
                }
                break;
            case Direction.back:
                //move the player forward
                SetFrontBackNodes();
                if (backNode != null)
                {
                    moving = true;
                    currDirection = Direction.back;
                }
                else
                {
                    moving = false;
                }
                break;
            case Direction.left:
                moving = true;
                currDirection = Direction.left;
                break;
            case Direction.right:
                moving = true;
                currDirection = Direction.right;
                break;
        }
    }

    private void ReplenishActionPoints()
    {
        actionPoints = 3;
    }

    private void EndTurn()
    {
        if(actionPoints == 0)
        {
            GameManager.startEnemyTurnEvent();
        }
    }

    private void OnDestroy()
    {
        GameManager.startPlayerTurnEvent -= ReplenishActionPoints;
    }

    private void OnDrawGizmos()
    {

    }
}
