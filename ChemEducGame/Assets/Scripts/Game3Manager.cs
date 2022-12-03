using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Game3Manager : MonoBehaviour
{
    private AudioManager audioManager;
    [SerializeField] List<GameObject> spawnPoints;
    [SerializeField] GameObject foodPrefab;
    private void Awake() {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void Start()
    {
        audioManager.StopAllBGMusic();
        audioManager.Play("bgGame3");
        spawnFood();
    }

    void spawnFood()
    {
        var food = Instantiate(foodPrefab, spawnPoints[0].transform);
        food.transform.DOMove(new Vector3(10,0,0), 2);

    }
}
