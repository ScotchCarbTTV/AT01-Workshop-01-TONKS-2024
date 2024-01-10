using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTonkMovement : MonoBehaviour
{

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotSpeed = 5f;

    [SerializeField] int actionPoints;
    bool myTurn = false;

    [SerializeField] GameObject playerTonk;

    [SerializeField] GameObject nodeLeft, nodeRight, nodeFront, nodeBack;

    [SerializeField] Vector3 leftPos, rightPos, rearPos;

    [SerializeField] List<GameObject> neighbourNodes = new List<GameObject>();

    [SerializeField] GameObject closestNeighbour;

    //Upkeep: The bot checks if they have any action points and passes to the player if they don't. They check if the player is visible, and find the closest node if the player isn't
    //TurnToFace: The bot turns 90 degrees towards the closest node
    //MoveToNode: The bot moves towards the selected node
    //ShootAtPlayer: The bot takes a shot at the player
    //PlayerTurn: do nothing

    private string[] botStates = { "Upkeep", "TurnToFace", "MoveToNode", "ShootAtPlayer", "PlayerTurn" };
    public enum BotStates { Upkeep, TurnToFace, MoveToNode, ShootAtPlayer, PlayerTurn }
    public BotStates currentState;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.startEnemyTurnEvent += ReplenishActionPoints;

        actionPoints = 0;

        currentState = BotStates.PlayerTurn;
    }

    // Update is called once per frame
    void Update()
    {
        //trigger the method relating to the current 'state'        
        Invoke(botStates[(int)currentState], 0f);
        
    }

    private void Upkeep()
    {
        Debug.Log("Entering upkeep");
        //closestNeighbour = null;

        //check if there are action points
        if (actionPoints < 1)
        {
            Debug.Log("No action points left, passing to player");
            GameManager.startPlayerTurnEvent();
            currentState = BotStates.PlayerTurn;
            return;
        }
        else
        {
            Debug.Log("Checking for vision on player");
            //check if we can see the player
            RaycastHit hit;
            Vector3 adjustedPosition = transform.position + new Vector3(0, 0.5f, 0);
            Vector3 playerDir = playerTonk.transform.position - adjustedPosition;
            Debug.DrawRay(adjustedPosition, playerDir * 10, Color.red, 10f);

            LayerMask mask = LayerMask.GetMask("Default");

            if (Physics.Raycast(adjustedPosition, playerDir, out hit, Mathf.Infinity, mask))
            {
                Debug.Log($"Hit {hit.collider.name}");
                if (hit.collider.gameObject.tag == "Player")
                {
                    Debug.Log("Player spotted, opening fire!");
                    //we can see the player
                    //player spotted, open fire
                    currentState = BotStates.ShootAtPlayer;
                    return;
                }
                else
                {
                    Debug.Log("Unable to see player, moving to engage");
                    //can't see them, need to move
                    FindNeighbouringNodes();
                    //check the neighbouring nodes to see which is closest to the player
                    closestNeighbour = SelectNodeClosestToPlayer();
                    
                    if(closestNeighbour != nodeFront)
                    {
                        Debug.Log("Turning towards chosen destination");
                        leftPos = -transform.right;
                        rightPos = transform.right;                        
                        currentState = BotStates.TurnToFace;
                        return;
                    }
                    else
                    {
                        Debug.Log("Moving towards chosen destination");
                        currentState = BotStates.MoveToNode;
                        return;
                    }

                }
            }
            else
            {

                Debug.Log("Unable to see player, moving to engage");
                //can't see them, need to move
                FindNeighbouringNodes();
                //check the neighbouring nodes to see which is closest to the player
                closestNeighbour = SelectNodeClosestToPlayer();

                if (closestNeighbour != nodeFront)
                {
                    Debug.Log("Turning towards chosen destination");
                    leftPos = -transform.right;
                    rightPos = transform.right;
                    currentState = BotStates.TurnToFace;
                    return;
                }
                else
                {
                    Debug.Log("Moving towards chosen destination");
                    currentState = BotStates.MoveToNode;
                    return;
                }

            }

        }

    }

    private void TurnToFace()
    {

        if(closestNeighbour != nodeFront)
        {
            Debug.Log($"Turning to {closestNeighbour}");
            if (closestNeighbour == nodeLeft)
            {
                float angle = Vector3.Angle(transform.forward, leftPos);
                if (angle > 0)
                {
                    Vector3 newDir = Vector3.RotateTowards(transform.forward, leftPos, rotSpeed * Time.deltaTime, 1f);
                    transform.rotation = Quaternion.LookRotation(newDir);
                }
                else
                {
                    actionPoints--;
                    currentState = BotStates.Upkeep;
                    return;
                }
            }
            else
            {
                float angle = Vector3.Angle(transform.forward, rightPos);
                if (angle > 0)
                {
                    Vector3 newDir = Vector3.RotateTowards(transform.forward, rightPos, rotSpeed * Time.deltaTime, 1f);
                    transform.rotation = Quaternion.LookRotation(newDir);
                }
                else
                {
                    actionPoints--;
                    currentState = BotStates.Upkeep;
                    return;
                }
            }
        }
        else
        {
            Debug.Log($"Turning to {closestNeighbour}");
            float angle = Vector3.Angle(transform.forward, leftPos);           
            if (angle > 0)
            {
                Vector3 newDir = Vector3.RotateTowards(transform.forward, leftPos, rotSpeed * Time.deltaTime, 1f);
                transform.rotation = Quaternion.LookRotation(newDir);
            }
            else
            {
                actionPoints--;
                currentState = BotStates.Upkeep;
                return;
            }
        }
    }

    private void ShootAtPlayer()
    {
        Debug.Log("bang! Bang!");
        actionPoints--;
        currentState = BotStates.Upkeep;
    }

    private void MoveToNode()
    {
        Debug.Log("Entering move state");

        Vector3 adjustedFrontPosition = nodeFront.transform.position + new Vector3(0, -0.5f, 0);
        if (transform.position != adjustedFrontPosition)
        {
            Debug.Log("Moving!");
            transform.position = Vector3.MoveTowards(transform.position, adjustedFrontPosition, moveSpeed * Time.deltaTime);
        }
        else
        {
            actionPoints--;
            Debug.Log("Reached destination!");
            currentState = BotStates.Upkeep;
        }
    }

    private void PlayerTurn()
    {
        Debug.Log("Playeturn, waiting...");
    }

    private void FindNeighbouringNodes()
    {
        neighbourNodes.Clear();

        RaycastHit hit;
        Vector3 adjustedPosition = transform.position + new Vector3(0, 0.5f, 0);
        if (Physics.Raycast(adjustedPosition, transform.forward, out hit))
        {
            if (hit.collider.tag == "Node")
            {
                nodeFront = hit.collider.gameObject;
            }
            else
            {
                nodeFront = null;
            }
        }
        if (Physics.Raycast(adjustedPosition, -transform.forward, out hit))
        {
            if (hit.collider.tag == "Node")
            {
                nodeBack = hit.collider.gameObject;
            }
            else
            {
                nodeBack = null;
            }
        }
        if (Physics.Raycast(adjustedPosition, transform.right, out hit))
        {
            if (hit.collider.tag == "Node")
            {
                nodeRight = hit.collider.gameObject;
            }
            else
            {
                nodeRight = null;
            }
        }
        if (Physics.Raycast(adjustedPosition, -transform.right, out hit))
        {
            if (hit.collider.tag == "Node")
            {
                nodeLeft = hit.collider.gameObject;
            }
            else
            {
                nodeLeft = null;
            }
        }

        neighbourNodes.Add(nodeLeft);
        neighbourNodes.Add(nodeRight);
        neighbourNodes.Add(nodeFront);
        neighbourNodes.Add(nodeBack);
    }

    private GameObject SelectNodeClosestToPlayer()
    {
        float lowestDist = float.MaxValue;

        GameObject closestNeighbour = null;

        foreach(GameObject neighbour in neighbourNodes)
        {
            if (neighbour != null) {
                if (Vector3.Distance(neighbour.transform.position, playerTonk.transform.position) < lowestDist)
                {
                    lowestDist = Vector3.Distance(neighbour.transform.position, playerTonk.transform.position);
                    closestNeighbour = neighbour;
                } 
            }
        }
        return closestNeighbour;
    }

    private void ReplenishActionPoints()
    {
        actionPoints = 3;
        currentState = BotStates.Upkeep;
    }

    private void OnDestroy()
    {
        GameManager.startEnemyTurnEvent -= ReplenishActionPoints;
    }
}
