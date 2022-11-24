using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Instantiate(foodPrefab, spawnPoints[0].transform);
    }
}
