using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Game3Manager : MonoBehaviour
{
    
    [SerializeField] List<GameObject> spawnPoints;
    [SerializeField] List<FoodLevel> foodLevels;
    [SerializeField] FoodLevel targetFoodLevel;
    [SerializeField] int secondsToOutOfScreen;
    [SerializeField] float spawnTime;
    [SerializeField] Game3Level level;
    [SerializeField] TextMeshProUGUI targetCount;
    [SerializeField] TextMeshProUGUI targetTitle;

    //local variables
    private AudioManager audioManager;
    private float _spawnTime;


    private void Awake() {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void Start()
    {
        audioManager.StopAllBGMusic();
        audioManager.Play("bgGame3");
        
        // reset values
        _spawnTime = spawnTime;
        targetFoodLevel.list.Clear();
        for (int i = 0; i < foodLevels[level.currentLevel].list.Count; i++)
        {
            targetFoodLevel.list.Add(foodLevels[level.currentLevel].list[i]);
        }

        targetCount.text = foodLevels[level.currentLevel].list.Count.ToString();
        targetTitle.text = foodLevels[level.currentLevel].name;
    }

    void spawnFood()
    {
        // add dummy and target tags

        var foodLevelIndex = Random.Range(0,foodLevels.Count);
     
        if (targetFoodLevel.list.Count <= 0)
        {
            Debug.Log("WIN");
        }

        var food = Instantiate(foodLevels[foodLevelIndex].list[Random.Range(0,foodLevels[foodLevelIndex].list.Count)], spawnPoints[Random.Range(0,spawnPoints.Count)].transform);
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
        for (int i = 0; i < foodLevels[level.currentLevel].list.Count; i++)
        {
            if (name.ToLower().Contains(foodLevels[level.currentLevel].list[i].gameObject.name.ToLower()))
            {
                //foodLevels[level.currentLevel].list.Remove(foodLevels[level.currentLevel].list[i].gameObject);
                targetFoodLevel.list.Remove(foodLevels[level.currentLevel].list[i].gameObject);
                targetCount.text = targetFoodLevel.list.Count.ToString();
                //Debug.Log(_foodLevels[level.currentLevel].list.Count);
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
