# zombies

My favorite part of this project was creating the scoreboard using free tools. The way I did this was I pre-made a Google sheets form, have the game download the data, find the top ten entries, compares the values and at the end (when the player eventually dies) their score is examined to see if the player is elligible to enter their name in the top ten.

This was an exercise in working with APIs. Here's how I did it:

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
