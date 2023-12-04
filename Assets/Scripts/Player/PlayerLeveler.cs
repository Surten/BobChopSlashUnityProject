using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLeveler : MonoBehaviour
{
    public event Action onLevelUpEvent;
    public int expLevelScalingLinear = 5;

    private int playerLevel = 0;
    private int expForNextLevel = 0;
    private int currentExp = 0;

    public Slider expSlider;

    #region Singleton
    private static PlayerLeveler mInstance;

    private void Awake()
    {
        if (mInstance != null)
            Debug.Log("More than one instance of Inventory");
        mInstance = this;
    }
    public static PlayerLeveler Instance
    {
        get
        {
            return mInstance;
        }
    }
    #endregion

    private void Start()
    {
        expForNextLevel = expLevelScalingLinear;
        expSlider.minValue = 0;
        expSlider.maxValue = expForNextLevel;
        expSlider.value = 0;
    }

    public void AddExp(int exp)
    {
        currentExp += exp;
        if (currentExp >= expForNextLevel)
            LevelUpPlayer();
        expSlider.value = currentExp;
    }

    private void LevelUpPlayer()
    {
        playerLevel++;
        currentExp -= expForNextLevel;

        expForNextLevel += expLevelScalingLinear;
        expSlider.maxValue = expForNextLevel;

        onLevelUpEvent?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            AddExp(1);
        }
    }

}
