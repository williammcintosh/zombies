using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using TMPro;
using System;
using UnityEngine.UI;

public class LeaderBoardScript : MonoBehaviour
{
    public Button submitButton;
    public int height, width, leaderListCount;
    private const string url = "https://docs.google.com/spreadsheets/d/1PDP1Xq4nIllxEK-y-QkGAwNwNa9K1DYjJGWTmhgxwCY/export?format=csv";
    public string basePath;
    public TextMeshProUGUI [] leaderBoardList = new TextMeshProUGUI [10];
    public List<LeaderScript> leadersList = new List<LeaderScript>();
    public TMP_InputField inputName;
    public GameObject inputField;
    public GameObject soldier;
    // Start is called before the first frame update
    void Start()
    {
        basePath = Path.Combine(Application.streamingAssetsPath, "ZombieSurvivalLeadersBoard.csv");
        StartCoroutine(DownloadData());
        inputField.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
       //FOR TESTING
       if (Input.GetKeyDown(KeyCode.Space)) {
           PrintLeaderList();
       }
    }
    public IEnumerator DownloadData()
    {
        yield return new WaitForSeconds(1f);
        string downloadData = null;
        for (int i = 0; i < 10 && downloadData == null; i++) {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) {
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError) {
                    //Debug.Log("Ahh man, it done screwed up!");
                } else {
                    downloadData = webRequest.downloadHandler.text;
                }
                webRequest.Dispose();
            }
            yield return new WaitForSeconds(5f);
        }
        GetHeightAndWidth(downloadData);
        yield return new WaitForSeconds(1f);
        LoadLeaderList(downloadData);
        yield return new WaitForSeconds(1f);
        SortLeaderList();
        yield return null;
    }
    public void GetHeightAndWidth(string data)
    {
        width = height = 0;
        string[] rows = data.Split('\n');
        foreach(string row in rows)
        {
            ++height;
            string[] columns = row.Split(',');
            foreach (string column in columns) {
                //Only counts for the first row
                if (height == 1)
                    ++width;
            }
        }
    }
    public void LoadLeaderBoard()
    {
        for (int i = 0; i < leaderBoardList.Length; i++) {
            string nextName = leadersList[i].leaderName;
            string nextScore = leadersList[i].score.ToString();
            string nextDate = leadersList[i].date;
            leaderBoardList[i].text = nextName + "  " + nextScore + "  " + nextDate;
        }
    }
    public void LoadLeaderList(string data)
    {
        string[] rows = data.Split('\n');
        for (int i = 1; i < height; i++) {
            if (rows[i].ToString() != null) {
                string[] words = rows[i].Split(',');
                string newName = words[1];
                string newScore = words[2];
                string newDate = words[3];
                LeaderScript nextLeader = new LeaderScript(newName, newScore, newDate);
                leadersList.Add(nextLeader);
            }
        }
    }
    public void SortLeaderList()
    {
        //PrintLeaderList();
        for (int i = leadersList.Count-1; i >0; i--) {
            for (int j = i-1; j>=0; j--) {
                if (leadersList[i].score > leadersList[j].score) {
                    LeaderScript copy = new LeaderScript(leadersList[j]);
                    leadersList.Insert(i+1, copy);
                    leadersList.RemoveAt(j);
                }
            }
        }
        //PrintLeaderList();
    }
    public void InsertNewLeader(int newScore)
    {
        LoadLeaderBoard();
        if (PlayerDidQualify(newScore)) {
            inputField.SetActive(true);
        }
    }
    public bool PlayerDidQualify(int newScore)
    {
        for (int i = 0; i < leaderBoardList.Length; i++) {
            if (leadersList[i].score < newScore) {
                return true;
            }
        }
        return false;
    }
    public void PrintLeaderList()
    {
        Debug.Log("----------------------------------");
        for (int i = 0; i < leadersList.Count; i++) {
            string pName = leadersList[i].leaderName;
            string pScore = leadersList[i].score.ToString();
            string pDate = leadersList[i].date;
            Debug.Log("Leader["+i+"] "+pName + " " + pScore + " " + pDate);
        }
    }
    public void Send()
    {
        if (inputName.GetComponent<TMP_InputField>().text != null && inputName.GetComponent<TMP_InputField>().text != "") {
            string newName = inputName.GetComponent<TMP_InputField>().text;
            string newScore = soldier.GetComponent<SoldierScript>().counter.ToString();
            DateTime localDate = System.DateTime.Now;
            //Just grabs the date portion of the call
            string [] newDate = localDate.ToString().Split(' ');
            LeaderScript newLeader = new LeaderScript(newName, newScore, newDate[0]);
            leadersList.Add(newLeader);
            SortLeaderList();
            LoadLeaderBoard();
            submitButton.interactable = false;
            StartCoroutine(WriteLeaderBoard(newLeader));
        }
    }
    /*
      Get the "entry.12344567" numbers by using a prefilled link:
      1. Go to your Google form
      2. Click the three dots in the top right
      3. Click 'Get Prefilled Link'
      4. Fill in sections
      5. Click "Get link"
      6. Find the "entry.1234567" numbers within
      https://docs.google.com/forms/d/e/1FAIpQLSd9BY7b7Kg34WBJRjuHyb-7CuJ0tYck9JEI2XeMOmGDAYUOKA/viewform?entry.783869678=Willy&entry.44949724=10&entry.1697701526=3/26/2021
    */
    public IEnumerator WriteLeaderBoard(LeaderScript newLeader)
    {
        string formURL =    "https://docs.google.com/forms/d/e/1FAIpQLSd9BY7b7Kg34WBJRjuHyb-7CuJ0tYck9JEI2XeMOmGDAYUOKA/formResponse";
        string newName = newLeader.leaderName;
        string newScore = newLeader.score.ToString();
        string newDate = newLeader.date.ToString();
        WWWForm form = new WWWForm();
        form.AddField("entry.783869678", newName);
        form.AddField("entry.44949724", newScore);
        form.AddField("entry.1697701526", newDate);
        byte[] rawData = form.data;
        UnityWebRequest webUploadRequest = UnityWebRequest.Post(formURL, form);
        yield return webUploadRequest.SendWebRequest();
    }
}
