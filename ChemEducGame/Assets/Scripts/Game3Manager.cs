using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Game3Manager : MonoBehaviour
{
    private AudioManager audioManager;
    [SerializeField] List<GameObject> spawnPoints;
    //[SerializeField] GameObject foodPrefab;
    [SerializeField] List<FoodLevel> foodLevels;
    [SerializeField] int secondsToOutOfScreen;
    private float spawnTimeCopy;
    [SerializeField] float spawnTime;
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
        var foodLevel = foodLevels[Random.Range(0,foodLevels.Count)];
        var food = Instantiate(foodLevel.list[Random.Range(0,foodLevel.list.Count)], spawnPoints[Random.Range(0,spawnPoints.Count)].transform);
        if (foodLevel.name.ToLower() == "carbohydrates")
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
