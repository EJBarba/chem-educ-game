using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Game3Manager : MonoBehaviour
{
    private AudioManager audioManager;
    [SerializeField] List<GameObject> spawnPoints;
    [SerializeField] GameObject foodPrefab;
    [SerializeField] int secondsToOutOfScreen = 2;
    private float spawnTimeCopy = 2f;
    [SerializeField] float spawnTime;
    private void Awake() {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void Start()
    {
        audioManager.StopAllBGMusic();
        audioManager.Play("bgGame3");
        spawnTime = spawnTimeCopy;
    }

    void spawnFood()
    {
        var food = Instantiate(foodPrefab, spawnPoints[Random.Range(0,spawnPoints.Count)].transform);
        food.transform.DOMove(new Vector3(-10,food.transform.position.y,0), secondsToOutOfScreen);
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
