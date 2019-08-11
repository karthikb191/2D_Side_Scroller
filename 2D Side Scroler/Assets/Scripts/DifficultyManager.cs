using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour {
    public static DifficultyManager Instance;

    public float DifficultyParameter { get {
            return difficultyParameter;
        } }
    private float difficultyParameter = 1;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        GameManager.Instance.ResetGameEvent += ResetDifficulty;
    }
    // Update is called once per frame
    void Update () {
        if (!GameManager.Instance.GameStarted)
            return;

        if (GameManager.Instance.GetTimeElapsed() > 5 * difficultyParameter && difficultyParameter <= 4)
        {
            //Debug.Log(GameManager.Instance.GetTimeElapsed());
            difficultyParameter+=0.001f;
        }
	}

    void ResetDifficulty()
    {
        difficultyParameter = 1;
    }
}
