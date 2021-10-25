using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameState { Init, Running, Win, Loss};

public class GameManager : MonoBehaviour
{
    // GUI
    [Header("Heads Up Display")]
    public TextMeshProUGUI healthText;
    public RectTransform fuelBar;
    public TextMeshProUGUI gameOverText;

    //public GameObject[] walls;

    GameState state;
    PlayerController player;

    // gui variables
    private float barWidth;
    float MAX_WIDTH;

    // properties
    public GameState State { get => state; set => state = value; }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        // Initialize jetpack fuel bar
        MAX_WIDTH = fuelBar.rect.width;
        barWidth = MAX_WIDTH / player.maxFuel * player.Fuel;
        fuelBar.sizeDelta = new Vector2(barWidth, fuelBar.rect.height);

        state = GameState.Running;
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = player.Health.ToString();

        // update the jetpack fuel bar
        barWidth = MAX_WIDTH / player.maxFuel * player.Fuel;
        fuelBar.sizeDelta = new Vector2(barWidth, fuelBar.rect.height);
    }

    public void Die()
    {
        gameOverText.gameObject.SetActive(true);
        state = GameState.Loss;
        Time.timeScale = 0;
    }

}
