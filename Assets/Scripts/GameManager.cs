using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public int totalEnemies;
    public float delayStart;
    public float spawnInterval;
    public int numberOfPowerUp;
}

public class GameManager : MonoBehaviour
{
    public List<Wave> waves;
    public GameObject enemyPrefab;
    public GameObject[] powerUps;
    public Transform[] spawnPoints;
    public Transform powerUpSpawnArea;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI countdownText;

    public int currentWave;
    public int enemyKillCount;

    private float surviveTime;
    private string savePath;

    private Coroutine waveCoroutine;

    private void Start()
    {
        savePath =
            Application.persistentDataPath + "/save.json";
        LoadGame();

        StartCoroutine(WaveControl());
    }

    private IEnumerator WaveControl()
    {
        for (int i = 0; i < waves.Count; i++)
        {
            int currentWave = i + 1;

            if (waveCoroutine != null)
            {
                StopCoroutine(waveCoroutine);
            }

            yield return StartCoroutine(WaveSpawn(i));
        }
    }

    private IEnumerator WaveSpawn(int waveId)
    {
        Wave currentWave = waves[waveId];

        float time = currentWave.delayStart;

        while (time > 0)
        {
            countdownText.text = time.ToString("F1") + "s before next wave";
            yield return new WaitForSeconds(0.1f);
            time -= 0.1f;
        }

        countdownText.text = "";
        waveText.text = "Wave " + (waveId + 1);

        for (int i = 0; i < currentWave.numberOfPowerUp; i++)
        {
            int random = Random.Range(0, powerUps.Length);
            float spawnRange = powerUpSpawnArea.localScale.x * 5f;

            Vector3 randomPosition = new Vector3(
                Random.Range(-spawnRange, spawnRange),
                transform.position.y + 0.5f,
                Random.Range(-spawnRange, spawnRange));

            Instantiate(
                powerUps[random],
                powerUpSpawnArea.position + randomPosition,
                Quaternion.identity);
        }

        for (int i = 0; i < currentWave.totalEnemies ; i++)
        {
            int random = Random.Range(0, spawnPoints.Length);
            Instantiate(
                enemyPrefab,
                spawnPoints[random].position + Vector3.up * 0.5f,
                Quaternion.identity);

            yield return new WaitForSeconds(currentWave.spawnInterval);
        }
    }
    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.wave = currentWave;
        data.killCount = enemyKillCount;
        data.surviveTime = surviveTime;

        string json =
            JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log("Saved");
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);

            SaveData data =
                JsonUtility.FromJson<SaveData>(json);

            currentWave = data.wave;
            enemyKillCount = data.killCount;
            surviveTime = data.surviveTime;

            Debug.Log("Loaded");

        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
