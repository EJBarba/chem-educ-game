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

    //local variables
    private AudioManager audioManager;
    private float _spawnTime;
    private List<FoodLevel> _foodLevels;



    private void Awake() {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void Start()
    {
        audioManager.StopAllBGMusic();
        audioManager.Play("bgGame3");

        // reset values
        _spawnTime = spawnTime;
        _foodLevels = foodLevels;
    }

    void spawnFood()
    {
        // add dummy and target tags

        var foodLevelIndex = Random.Range(0,_foodLevels.Count);
     
        if ( _foodLevels[foodLevelIndex].list.Count <=0)
        {
            Debug.Log("WIN");
        }

        var food = Instantiate(_foodLevels[foodLevelIndex].list[Random.Range(0,_foodLevels[foodLevelIndex].list.Count)], spawnPoints[Random.Range(0,spawnPoints.Count)].transform);
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

    public void TargetHit(string name)
    {
        for (int i = 0; i < _foodLevels[level.currentLevel].list.Count; i++)
        {
            if (name.ToLower().Contains(_foodLevels[level.currentLevel].list[i].gameObject.name.ToLower()))
            {
                _foodLevels[level.currentLevel].list.Remove(_foodLevels[level.currentLevel].list[i].gameObject);
                Debug.Log(_foodLevels[level.currentLevel].list.Count);
                break;
            }
        }


    }

    public void PauseAnimations()
    {
        DOTween.PauseAll();
    }

    public void ResumeAnimations()
    {
        DOTween.PlayAll();
    }

    private void Update() 
    {
        _spawnTime -= Time.deltaTime;
        if (_spawnTime <= 0f)
        {
            spawnFood();
            _spawnTime = spawnTime;
        }
    }
}
