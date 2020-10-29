using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagResolver : MonoBehaviour
{
    public static TagResolver i
    {
        get
        {
            return Singleton<TagResolver>.Instance;
        }
    }
    public GamepadInputManager inputManager;
    public GameManager gameManager;
    public SelectionController selectionController;
    public Inventory inventory;
    public Player player;

    private void Awake()
    {
        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<GamepadInputManager>();
        gameManager = GameObject.FindGameObjectWithTag("death_match").GetComponent<GameManager>();
        selectionController = GameObject.FindGameObjectWithTag("SelectionController").GetComponent<SelectionController>();
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
}
