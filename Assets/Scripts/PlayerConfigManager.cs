using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerConfigManager : MonoBehaviour
{
    private List<PlayerConfig> playerConfigs;

    public static PlayerConfigManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        playerConfigs = new List<PlayerConfig>();
        playerConfigs.Add(new PlayerConfig(1, ColorName.BLUE, "Number 1" , 0, true));
        playerConfigs.Add(new PlayerConfig(2, ColorName.RED, "Number 2", 1, true));
        //playerConfigs.Add(new PlayerConfig(3, ColorName.GREEN, "Number 3", 2, true));
        //playerConfigs.Add(new PlayerConfig(4, ColorName.YELLOW, "Number 4", 3, true));
        MenuManager.SetVietnamese();
    }

    public List<PlayerConfig> GetPlayerConfigs()
    {
        return playerConfigs;
    }

    public void OnButtonClick()
    {
        SceneManager.LoadScene(1);
    }
}
