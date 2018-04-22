using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using LitJson;
using System;
using System.IO;

public class Ranking : MonoBehaviour {
    public InputField PlayerNameInputField;     // 用于获取输入名字的InputField控件.
    public InputField PlayerScoreInputField;    // 用于获取输入分数的InputField控件.
    public Button SubmitButton;                 // 用于获取提交输入的Button控件

    public GameObject PrefabShow;               // 显示排行榜上单个记录的Prefab
    public Image ParentsShow;                   // 显示排行榜上所有记录的的父物体

    
	// Use this for initialization
	void Start () {
        // 为 Button 控件注册点击的监听事件.
        // onClick：按下按钮时触发的UnityEvent.
        // AddListener（）：将一个非持久侦听器添加到UnityEvent.
        SubmitButton.onClick.AddListener(SubmitInfo); 
	}
    /// <summary>
    /// 提交输入信息的函数.
    /// </summary>
    public void SubmitInfo()
    {
        if (PlayerNameInputField.text == "" || 
            PlayerScoreInputField.text == "" || 
            (PlayerNameInputField.text == "" && PlayerScoreInputField.text == ""))
        {
            // 获取 Json 数据并排序.
            DictionarySort(GetJsonData()); 
            return;
        }
        PlayerInfo player = new PlayerInfo();
        player.PlayerName = PlayerNameInputField.text;
        player.PlayerScore = int.Parse(PlayerScoreInputField.text);
        Debug.Log("名字：" + player.PlayerName);
        Debug.Log("分数：" + player.PlayerScore);
        // 将输入的 名字和分数信息 转换成 Json数据 并以 字符串 的形式返回.
        string PlayerResultData = ResultToJson(player.PlayerName, player.PlayerScore);
        // 保存字符串.
        SaveString(PlayerResultData);
        // 获取所有的Json数据并排序.
        DictionarySort(GetJsonData());
    }
    /// <summary>
    /// 将输入结果转换成Json数据
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Score"></param>
    /// <returns></returns>
    public string ResultToJson(string Name, int Score)
    {
        StringBuilder strb = new StringBuilder();
        JsonWriter WriteData = new JsonWriter(strb);
        // 开始写入
        WriteData.WriteObjectStart();
        // 写入名字
        WriteData.WritePropertyName("Name");
        WriteData.Write(Name + "|" + DateTime.Now.ToShortTimeString());
        // 写入分数
        WriteData.WritePropertyName("Score");
        WriteData.Write(Score);
        // 写入结束
        WriteData.WriteObjectEnd();
        return strb.ToString(); 
    }

    /// <summary>
    /// 将Json格式的数据保存到Json.txt文件
    /// </summary>
    /// <param name="str"></param>
    private void SaveString(string str)
    {
        FileInfo file = new FileInfo(Application.dataPath + "/Json.txt");
        StreamWriter sw = null;
        if (file.Exists)
        {
            sw = file.AppendText();
        }
        else
        {
            sw = file.CreateText();
        }
        sw.WriteLine(str);
        sw.Close();
    }

    /// <summary>
    /// 获取Json数据
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, int> GetJsonData()
    {
        FileStream filestream = new FileStream(Application.dataPath + "/Json.txt", FileMode.Open);
        Dictionary<string, int> jsonData = new Dictionary<string, int>();
        if (filestream.CanRead)
        {
            StreamReader streamreader = new StreamReader(filestream);
            string jsonstring;
            while ((jsonstring = streamreader.ReadLine()) != null)
            {
                JsonData data = JsonMapper.ToObject(jsonstring);
                jsonData.Add(data["Name"].ToString(), int.Parse(data["Score"].ToString()));
            }
        }
        return jsonData;
    }

    private void DictionarySort(Dictionary<string, int> dictionary)
    {
        if (dictionary.Count > 0)
        {
            List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>(dictionary);
            list.Sort(delegate (KeyValuePair<string, int> s1, KeyValuePair<string, int> s2) {
                return s2.Value.CompareTo(s1.Value);
            });
            dictionary.Clear();
            float i = 1f, r = 1f, g = 1f, b = 0f;
            foreach (KeyValuePair<string, int> kvp in list)
            {
                if (i <= 3)
                {
                    string[] key = kvp.Key.Split('|');
                    GameObject gameobj = Instantiate(PrefabShow, ParentsShow.transform.position-i*(new Vector3(0f,50f,0f)),
                        Quaternion.identity) as GameObject;
                    gameobj.transform.SetParent(ParentsShow.transform);

                    r -= 0.2f;
                    g -= 0.2f;
                    b -= 0.2f;
                    Debug.Log(r + g + b);
                    Text[] Children = gameobj.GetComponentsInChildren<Text>();
                    Children[0].color = new Color(r, g, b); 
                    Children[1].color = new Color(r, g, b);
                    Children[2].color = new Color(r, g, b);
                    Children[3].color = new Color(r, g, b);

                    Children[0].text = (i++).ToString();
                    Children[1].text = key[0];
                    Children[2].text = kvp.Value.ToString(); 
                    Children[3].text = key[1];  
                } 
                else
                {
                    string[] Key = kvp.Key.Split('|');
                    GameObject gameobj = Instantiate(PrefabShow, ParentsShow.transform.position-i * (new Vector3(0f, 50f, 0f)),
                        Quaternion.identity) as GameObject;
                    gameobj.transform.SetParent(ParentsShow.transform);
                    Text[] Children = gameobj.GetComponentsInChildren<Text>();
                    Children[0].text = (i++).ToString();
                    Children[1].text = Key[0];
                    Children[2].text = kvp.Value.ToString();
                    Children[3].text = Key[1];
                }   
            }
        }
    }
}
