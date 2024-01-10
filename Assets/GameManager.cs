using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//will handle the player's and AI's turns mostly
//opening/closing menus etc etc
//mosly through events

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Only one game manager should be in the scene!");
            gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public delegate void StartPlayerTurn();
    public static StartPlayerTurn startPlayerTurnEvent;

    public delegate void StartEnemyTurn();
    public static StartEnemyTurn startEnemyTurnEvent;

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
