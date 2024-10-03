using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using CodeMonkey.Utils;

public class HighscoreTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
    [SerializeField] private TMP_InputField playerNameInput;
    private List<Transform> highscoreEntryTransformList;

    private void Awake()
    {
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null)
        {
            // There's no stored table, initialize
            Debug.Log("Initializing table with default values...");
            AddHighscoreEntry();
            // Reload
            jsonString = PlayerPrefs.GetString("highscoreTable");
            highscores = JsonUtility.FromJson<Highscores>(jsonString);
        }

        // Sort entry list by Score
        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
            {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    // Swap
                    HighscoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }

        highscoreEntryTransformList = new List<Transform>();
        int maxEntries = Mathf.Min(highscores.highscoreEntryList.Count, 10);
        for (int i = 0; i < maxEntries; i++)
        {
            CreateHighscoreEntryTransform(highscores.highscoreEntryList[i], entryContainer, highscoreEntryTransformList);
        }
    }

    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 45f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank)
        {
            default:
                rankString = rank + "TH"; break;

            case 1: rankString = "1ST"; break;
            case 2: rankString = "2ND"; break;
            case 3: rankString = "3RD"; break;
        }

        entryTransform.Find("posText").GetComponent<Text>().text = rankString;

        int score = highscoreEntry.score;

        entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

        string name = highscoreEntry.name;
        entryTransform.Find("nameText").GetComponent<Text>().text = name;

        // Set background visible odds and evens, easier to read
        entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);

        // Highlight First
        if (rank == 1)
        {
            entryTransform.Find("posText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("scoreText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("nameText").GetComponent<Text>().color = Color.green;
        }

        // Set tropy
        switch (rank)
        {
            default:
                entryTransform.Find("trophy").gameObject.SetActive(false);
                break;
            case 1:
                entryTransform.Find("trophy").GetComponent<Image>().color = UtilsClass.GetColorFromString("FFD200");
                break;
            case 2:
                entryTransform.Find("trophy").GetComponent<Image>().color = UtilsClass.GetColorFromString("C6C6C6");
                break;
            case 3:
                entryTransform.Find("trophy").GetComponent<Image>().color = UtilsClass.GetColorFromString("B76F56");
                break;

        }

        transformList.Add(entryTransform);
    }
    public void ClearHighscoreTable()
    {
        // Xóa dữ liệu bảng điểm cao trong PlayerPrefs
        PlayerPrefs.DeleteKey("highscoreTable");
        PlayerPrefs.Save();

        // Cập nhật lại UI để hiển thị bảng điểm trống
        UpdateUI();
    }
    public void AddHighscoreEntry()
    {
        // Lấy điểm từ global.instance
        int score = GlobalController.instance.playerScore;

        // Lấy tên người chơi từ TMP.InputField
        string name = playerNameInput.text;

        // Kiểm tra xem tên người chơi có trống hay không
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("Player name cannot be empty!");
            return;
        }

        // Tạo mới một HighscoreEntry
        HighscoreEntry newEntry = new HighscoreEntry { score = score, name = name };

        // Tải dữ liệu điểm cao đã lưu
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null)
        {
            // Nếu chưa có bảng điểm, khởi tạo
            highscores = new Highscores
            {
                highscoreEntryList = new List<HighscoreEntry>()
            };
        }

        // Thêm mục mới vào bảng điểm
        highscores.highscoreEntryList.Add(newEntry);

        // Sắp xếp lại danh sách theo điểm số giảm dần
        highscores.highscoreEntryList.Sort((x, y) => y.score.CompareTo(x.score));

        // Giới hạn danh sách tối đa 10 phần tử
        if (highscores.highscoreEntryList.Count > 10)
        {
            highscores.highscoreEntryList.RemoveRange(10, highscores.highscoreEntryList.Count - 10);
        }

        // Lưu lại bảng điểm đã cập nhật
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();

        // Cập nhật lại UI với bảng điểm mới
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Xóa tất cả các entry cũ
        foreach (Transform child in entryContainer)
        {
            if (child == entryTemplate) continue;
            Destroy(child.gameObject);
        }

        // Tải lại dữ liệu và cập nhật UI
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null)
        {
            Debug.LogWarning("No highscore data found.");
            return;
        }

        // Sắp xếp lại danh sách theo điểm số giảm dần
        highscores.highscoreEntryList.Sort((x, y) => y.score.CompareTo(x.score));

        highscoreEntryTransformList.Clear();

        // Tạo lại các entry dựa trên danh sách điểm cao mới
        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            CreateHighscoreEntryTransform(highscores.highscoreEntryList[i], entryContainer, highscoreEntryTransformList);
        }
    }

    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
    }

    /*
     * Represents a single High score entry
     * */
    [System.Serializable]
    private class HighscoreEntry
    {
        public int score;
        public string name;
    }
}
