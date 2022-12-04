using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Game3Manager : MonoBehaviour
{
    
    [SerializeField] List<GameObject> spawnPoints;
    [SerializeField] List<FoodLevel> foodLevels;
    [SerializeField] int secondsToOutOfScreen;
    [SerializeField] float spawnTime;
    [SerializeField] Game3Level level;
    private AudioManager audioManager;
    private float spawnTimeCopy;
    private void Awake() {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void Start()
    {
        audioManager.StopAllBGMusic();
        audioManager.Play("bgGame3");
        spawnTimeCopy = spawnTime;
    }

    void spawnFood()
    {
        // add dummy and target tags
        var foodLevelIndex = Random.Range(0,foodLevels.Count);
        var foodLevel = foodLevels[foodLevelIndex];
        var food = Instantiate(foodLevel.list[Random.Range(0,foodLevel.list.Count)], spawnPoints[Random.Range(0,spawnPoints.Count)].transform);
        if (foodLevelIndex == level.currentLevel)
        {
            food.tag = "Target";
        }

        food.transform.DOMove(new Vector3(-20,food.transform.position.y,0), secondsToOutOfScreen)
        .OnComplete(() => 
        {
            Destroy(food);
        });
    }

    private void Update() 
    {
        spawnTimeCopy -= Time.deltaTime;
        if (spawnTimeCopy <= 0f)
        {
            spawnFood();
            spawnTimeCopy = spawnTime;
        }
    }
}
