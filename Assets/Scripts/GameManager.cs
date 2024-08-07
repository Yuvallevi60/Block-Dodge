using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject block;
    public Transform spawnPoint;
    float maxX = 2.25f;
    float spawnRate = 1;
    float spawnRateMin = 0.1f;
    float spawnRateDecrease = 0.1f;

    bool isPaused = true;


    public GameObject TitleScreen;
    public GameObject PlayScreen;
    TextMeshProUGUI scoreText;

    int score = 0;
    int bestScore;


    private void Start()
    {
        scoreText = PlayScreen.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        LoadBestScore();
        TitleScreen.transform.Find("BestScore").transform.Find("ScoreValue").GetComponent<TextMeshProUGUI>().text = bestScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
            Exit();
    }

    public void StartGame()
    {
        TitleScreen.SetActive(false);
        PlayScreen.SetActive(true);
        isPaused = false;
        InvokeRepeating("SpawnBlock", 0.5f, spawnRate);
    }


    private void SpawnBlock()
    {
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.x = Random.Range(-maxX, maxX);
        Instantiate(block, spawnPos, Quaternion.identity);
    }

    public void IncreaseScore()
    { 
        score++;
        scoreText.text = score.ToString();

        if (spawnRate > spawnRateMin && score % 5 == 0)
            AdjustSpawnRate();
    }

    private void AdjustSpawnRate()
    {
        spawnRate -= spawnRateDecrease;

        CancelInvoke("SpawnBlock");
        InvokeRepeating("SpawnBlock", spawnRate, spawnRate);
    }





    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player
#endif
    }


    public void GameOver()
    {
        if (score > bestScore)
            SaveBestScore();
        SceneManager.LoadScene("Game");
    }



    [System.Serializable]
    class SaveData
    {
        public int bestScore;
    }

    private void SaveBestScore()
    {
        SaveData data = new SaveData();
        data.bestScore = score;

        string json = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    private void LoadBestScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            bestScore = data.bestScore;
        }
        else
            bestScore = 0;
    }
}

