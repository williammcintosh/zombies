# zombies

My favorite part of this project was creating the scoreboard using free tools. The way I did this was I pre-made a Google sheets form, have the game download the data, find the top ten entries, compares the values and at the end (when the player eventually dies) their score is examined to see if the player is elligible to enter their name in the top ten.

This was an exercise in working with APIs. Here's how I did it:

Create a Google Sheet, set the sharing to public.

Be sure to add these libraries:
```csharp
using System.IO;
using UnityEngine.Networking;
```

Called from the start() function, immediately downloads the Google Sheet.

```csharp
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
```

the variable 'downloadData' is now set to some value. GetHeightAndWidth() simply gets the size of the Google Sheet.

LoadLeaderList() grabs the information from each contestent from each row, and stores their information into a brand new Leader class object and adds that class object to a list
```csharp
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
```
SortLeaderList() uses bubble sort to sort of the list of Leaders class objects. Only the top ten are considered.

Saving the data was then completed by using a Google Form for the Google Sheet, and then grabbing the information for each component on that sheet.

* To get the components:
    * Get the "entry.12344567" numbers by using a prefilled link:
      1. Go to your Google form
      2. Click the three dots in the top right
      3. Click 'Get Prefilled Link'
      4. Fill in sections
      5. Click "Get link"
      6. Find the "entry.1234567" numbers within
      https://docs.google.com/forms/d/e/1FAIpQLSd9BY7b7Kg34WBJRjuHyb-7CuJ0tYck9JEI2XeMOmGDAYUOKA/viewform?entry.783869678=Willy&entry.44949724=10&entry.1697701526=3/26/2021


```csharp
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
```


