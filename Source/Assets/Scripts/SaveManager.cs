using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

public static class SaveManager 
{
    private static HashSet<string> gameProgress = new HashSet<string>();
    private static XmlSerializableCounters counters = new XmlSerializableCounters();
    public static Vector2 playerSpawnPoint;
    public static float playerLust = 0;
    public static int currentLevelSceneCode = 1;
    public static string SessionStartTime = "";

    public static string participantID = "";
    private static int playSession = 0;
    private static XmlSerializer hashSetSerializer = new XmlSerializer(typeof(HashSet<string>));
    private static XmlSerializer counterSerializer = new XmlSerializer(typeof(XmlSerializableCounters));



    public static void SaveToPrefs()
    {
        PlayerPrefs.DeleteAll();
        string xml;

        StringWriter stringWriter = new StringWriter();
        hashSetSerializer.Serialize(stringWriter, gameProgress);
        xml = stringWriter.ToString();
        PlayerPrefs.SetString("gameProgress", xml);

        stringWriter = new StringWriter();
        counterSerializer.Serialize(stringWriter, counters);
        xml = stringWriter.ToString();
        PlayerPrefs.SetString("counters", xml);

        PlayerPrefs.SetFloat("PlayerPosX", playerSpawnPoint.x);
        PlayerPrefs.SetFloat("PlayerPosY", playerSpawnPoint.y);
        PlayerPrefs.SetFloat("PlayerLust", playerLust);
        PlayerPrefs.SetInt("CurrLevel", currentLevelSceneCode);
        PlayerPrefs.SetInt("PlaySession", playSession);
        PlayerPrefs.SetString("ParticipantID", participantID);

    }

    public static void LoadFromPrefs()
    {
        string xml = PlayerPrefs.GetString("counters");
        StringReader stringReader;
        if (xml != "")
        {
            stringReader = new StringReader(xml);
            counters = (XmlSerializableCounters)counterSerializer.Deserialize(stringReader);
        }

        xml = PlayerPrefs.GetString("gameProgress");
        if (xml != "")
        {
            stringReader = new StringReader(xml);
            gameProgress = (HashSet<string>)hashSetSerializer.Deserialize(stringReader);
        }
        float xPos = PlayerPrefs.GetFloat("PlayerPosX", playerSpawnPoint.x);
        float yPos = PlayerPrefs.GetFloat("PlayerPosY", playerSpawnPoint.y);
        playerSpawnPoint = new Vector2(xPos, yPos);
        playerLust = PlayerPrefs.GetFloat("PlayerLust", playerLust);
        currentLevelSceneCode = PlayerPrefs.GetInt("CurrLevel", currentLevelSceneCode);
        playSession = PlayerPrefs.GetInt("PlaySession")+1;
        participantID = PlayerPrefs.GetString("ParticipantID");

    }

    public static void Clear()
    {
        gameProgress.Clear();
        counters.Clear();
        playerLust = 0f;
        currentLevelSceneCode = 1;
        playSession = 1;
        playerSpawnPoint = Vector2.zero;
        SessionStartTime = System.DateTime.Now.ToString("dd/mm/yyyy HH:mm:ss");
    }

    public static void AddEvent(string eventCode)
    {
        gameProgress.Add(eventCode);
        Debug.Log("code:" + eventCode);
    }

    public static bool CheckCondition(string eventCode)
    {
        if(eventCode[0] == '!')
            return !gameProgress.Contains(eventCode.Remove(0,1));
        return gameProgress.Contains(eventCode);
    }

    public static void IncrementCounter(string counterCode)
    {
        Debug.Log("code:" + counterCode);
        if (counters.ContainsKey(counterCode))
        {
            counters[counterCode]++;
        }
        else
        {
            counters.Add(counterCode, 1);
        }
    }

    public static void ClearLevelData(string LevelCode)
    {
        string[] eventCodes = new string[gameProgress.Count];
        gameProgress.CopyTo(eventCodes);
        foreach(string eventCode in eventCodes)
        {
            if (eventCode.StartsWith(LevelCode)) gameProgress.Remove(eventCode);
        }
        string[] counterCodes = new string[counters.Count];
        counters.Keys.CopyTo(counterCodes,0);
        foreach (string key in counterCodes)
        {
            if (key.StartsWith(LevelCode))
            {
                counters.Remove(key);
            }
        }
    }

    public static IEnumerator PostLevelData(string LevelCode)
    {
        string[] eventCodes = new string[gameProgress.Count];
        gameProgress.CopyTo(eventCodes);
        StringBuilder eventSb = new StringBuilder();
        StringBuilder counterSb = new StringBuilder();

        foreach (string eventCode in eventCodes)
        {
            if (eventCode.StartsWith(LevelCode)) eventSb.Append(eventCode+"&");
        }

        string[] counterCodes = new string[counters.Count];
        counters.Keys.CopyTo(counterCodes, 0);
        foreach (string key in counterCodes)
        {
            if (key.StartsWith(LevelCode))
            {
                counterSb.Append(key + "=" + counters[key] + "&");
            }
        }

        WWWForm form = new WWWForm();
        form.AddField("usp", "pp_url");
        form.AddField("entry.757403998", participantID);
        form.AddField("entry.31532034", playSession.ToString("N0"));
        form.AddField("entry.409668991", SessionStartTime);
        form.AddField("entry.939852653", eventSb.ToString());
        form.AddField("entry.1253151178", counterSb.ToString());

        UnityWebRequest www = UnityWebRequest.Post("https://docs.google.com/forms/d/e/1FAIpQLScZVqtRd9KqSziQED4lfWONsP_-H8y6nsVcaNdJq09TCpr94w/formResponse", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }

    public static int GetCounter(string counterCode)
    {
        int total = 0;
        foreach (string key in counters.Keys)
        {
            if (key.Contains(counterCode))
            {
                total += counters[key];
            }
        }

        return total;
    }
   
}
