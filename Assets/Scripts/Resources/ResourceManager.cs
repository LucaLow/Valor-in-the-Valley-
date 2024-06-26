using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{

    public int wood;
    public int stone;
    public int food;

    [Space]

    public int defaultWood = 15;
    public int defaultStone = 15;
    public int defaultFood = 10;

    [Space]

    public GameObject _woodDisplay;
    public GameObject _stoneDisplay;
    public GameObject _foodDisplay;

    // Start is called before the first frame update
    void Start()
    {
        // Set the values of each resource to be the default
        wood = defaultWood;
        stone = defaultStone;
        food = defaultFood;
    }

    private void Update()
    {
        _woodDisplay.GetComponent<TextMeshProUGUI>().text = "Wood: " + wood.ToString();
        _stoneDisplay.GetComponent<TextMeshProUGUI>().text = "Stone: " + stone.ToString();
        _foodDisplay.GetComponent<TextMeshProUGUI>().text = "Food: " + food.ToString();
    }

}
