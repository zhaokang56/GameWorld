using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;
/// <summary>
/// 数字华容道
/// </summary>
public class numberCtrl : MonoBehaviour
{
    [SerializeField]
    public Transform numberBg;
    Transform currentEmpty;
    List<int> rightList = new List<int>();
    List<int> numberList = new List<int>();
     int space = 20;
    public Text lvText;
    public Text useTimeText;
    public Text useStepText;
    public Button restartBtn;
    public Button backBtn;
    public Button overBackBtn;
    public Button overNewBtn;
    public Transform overPanel;
    int lineCount = 4;
    bool isPlaying ;
    float useTime ;
    int useStep;
    float oldTime ;
    string currentTime;
    string userName;
    public void SetLineCount(int count)
    {
        lineCount=count;
        GetData();

    }
    void GetData()
    {
		Init();
      
        numberList = GetRandoms(lineCount * lineCount, lineCount * lineCount + 1);
        numberBg.GetComponent<RectTransform>().sizeDelta = new Vector2(lineCount * 120+20, lineCount * 120+20);
        if (lineCount * lineCount < numberBg.childCount)
        {
            for (int i = lineCount * lineCount; i < numberBg.childCount; i++)
            {
                numberBg.GetChild(i).gameObject.SetActive(false);
            }
        }
        else if (lineCount * lineCount > numberBg.childCount)
        {
            for (int i = numberBg.childCount; i < lineCount * lineCount; i++)
            {
                GameObject item = Instantiate(numberBg.GetChild(0).gameObject);
                item.transform.SetParent(numberBg, false);
            }
        }
        for (int i = 0; i < numberList.Count; i++)
        {
            if (numberBg.GetChild(i).gameObject.activeSelf == false)
            {
                numberBg.GetChild(i).gameObject.SetActive(true);
            }
            if (numberList[i] == numberList.Count)
            {
                ChangeToEmpty(numberBg.GetChild(i));
            }
            else
            {
                numberBg.GetChild(i).Find("Text").GetComponent<Text>().text = (numberList[i]).ToString();
            }
            Transform objTrans = numberBg.GetChild(i);
			objTrans.GetComponent<Button>().onClick.RemoveAllListeners();
            objTrans.GetComponent<Button>().onClick.AddListener(() => { OnButtonClick(objTrans); });
            if (numberList[i] == (i + 1))
            {
                rightList.Add(numberList[i]);
            }
        }
    }
    private void Start()
    {
        userName= GetRandomString(5,true,false,false,false,"");
		restartBtn.onClick.AddListener(() => {   GetData(); });
		backBtn.onClick.AddListener(() => {  Destroy(gameObject); });
        overBackBtn.onClick.AddListener(() => {  Destroy(gameObject); });
        overNewBtn.onClick.AddListener(() => {    overPanel.gameObject.SetActive(false);  GetData(); });
    }
    private void Update() {
        if (isPlaying)
        {
                useTime +=Time.deltaTime;
            if (useTime-oldTime>=1)
            {
                oldTime=useTime;
                currentTime = FormatTime((int)useTime);
                useTimeText.text = string.Format("游戏耗时\n{0}", currentTime);
            }
        }
    }
    string FormatTime(int totalSeconds)
    {
        int hours = totalSeconds / 3600;
        string hh = hours < 10 ? "0" + hours : hours.ToString();
        int minutes = (totalSeconds - hours * 3600) / 60;
        string mm = minutes < 10f ? "0" + minutes : minutes.ToString();
        int seconds = totalSeconds - hours * 3600 - minutes * 60;
        string ss = seconds < 10 ? "0" + seconds : seconds.ToString();
        return string.Format("{0:D2}:{1:D2}:{2:D2}", hh, mm, ss);
    }
    void OnButtonClick(Transform clickTrans)
    {
        float distance = Vector3.Distance(clickTrans.localPosition, currentEmpty.localPosition);
        if (distance <= (clickTrans.GetComponent<RectTransform>().rect.width + space) * 1.1f)
        {
             useStep+=1;
             useStepText.text= string.Format("目前步数\n{0}步", useStep);  
            int changeIndex = int.Parse(clickTrans.Find("Text").GetComponent<Text>().text);
            string changeText = clickTrans.Find("Text").GetComponent<Text>().text;
            if (changeIndex== currentEmpty.GetSiblingIndex()+1)
            {
                rightList.Add(changeIndex);
            }
            else
            {
                if (rightList.Contains(changeIndex))
                {
                    rightList.Remove(changeIndex);
                }
            }
            ChatClient.Instance.SendMessage(string.Format("{0}:将数字{1}挪到第{2}格",userName,changeIndex,currentEmpty.GetSiblingIndex()+1));
            currentEmpty.Find("Text").GetComponent<Text>().text = changeText;
            currentEmpty.GetComponent<Button>().enabled = true;
            currentEmpty.GetComponent<Image>().color = Color.white;
            ChangeToEmpty(clickTrans);
            if (rightList.Count >= numberList.Count - 1)
            {
                Debug.Log("游戏已完成");
                isPlaying=false;
                overPanel.gameObject.SetActive(true);
            }
        }



    }
	void Init()
	{
		if (currentEmpty!=null)
		{
			currentEmpty.Find("Text").GetComponent<Text>().text = "";
			currentEmpty.GetComponent<Button>().enabled = true;
			currentEmpty.GetComponent<Image>().color = Color.white;
			 currentEmpty = null;
		}
        useStep=0;
        useStepText.text= string.Format("目前步数\n{0}步", useStep);  
		useTime = 0;
        currentTime = "00:00:00";
        lvText.text = string.Format("游戏等级\n{0}*{1}", lineCount, lineCount);
        isPlaying = true;
        useTimeText.text = string.Format("游戏耗时\n{0}", currentTime);
        rightList.Clear();
        numberList.Clear();
	}
    void ChangeToEmpty(Transform clickTrans)
    {
        currentEmpty = clickTrans;
        currentEmpty.Find("Text").GetComponent<Text>().text = "";
        currentEmpty.GetComponent<Button>().enabled = false;
        currentEmpty.GetComponent<Image>().color = Color.red;
    }
    List<int> GetRandoms(int sum, int max)
    {
        List<int> randomList = new List<int>();
        int j = 0;
        //空数字所在行数
        int emptyPosLine = 0;
        System.Random rm = new System.Random();
        for (int i = 0; randomList.Count < sum; i++)
        {
            //返回一个小于所指定最大值的非负随机数
            int nValue = rm.Next(max);
            //containsValue(object value)   是否包含特定值
            if (!randomList.Contains(nValue) && nValue != 0)
            {
                if (nValue == sum)
                {
                    emptyPosLine = (int)(Math.Floor(j / (Math.Sqrt(sum)))) + 1;
                }
                //把键和值添加到hashtable
                randomList.Add(nValue);
                j++;
            }

        }
        bool allCountIsOdd = IsOdd(randomList.Count);
        bool inverseNumberIsOdd = IsOdd(GetInverseNumberCount(randomList));
        bool emptyLineIsOdd = IsOdd(emptyPosLine);
        Debug.Log(string.Format("总数是奇数还是偶数{0},空格所在行数{1},逆序数为奇数还是偶数{2}", allCountIsOdd, inverseNumberIsOdd, emptyLineIsOdd));
        if (allCountIsOdd)
        {
            //若格子列数为奇数，则逆序数必须为偶数
            if (inverseNumberIsOdd == false)
            {
                Debug.Log("格子列数为奇数，逆序数为偶数返回");
                return randomList;
            }
            else
            {
                Debug.Log("格子列数为奇数，逆序数为奇数重新计算");
                return GetRandoms(sum, max);
            }
        }
        else
        {
            //若格子列数为偶数，且逆序数为偶数
            if (inverseNumberIsOdd == false)
            {
                if (emptyLineIsOdd == false)
                {
                    Debug.Log("格子列数为偶数，逆序数为偶数，空格所在行为偶数，返回");
                    return randomList;
                }
                else
                {
                    Debug.Log("格子列数为偶数，逆序数为偶数，空格所在行为奇数，重新计算");
                    return GetRandoms(sum, max);
                }
            }
            //若格子列数为偶数，且逆序数为奇数
            else
            {
                if (emptyLineIsOdd)
                {
                    Debug.Log("格子列数为偶数，逆序数为奇数，空格所在行为奇数，返回");
                    return randomList;
                }
                else
                {
                    Debug.Log("格子列数为偶数，逆序数为奇数，空格所在行为偶数，重新计算");
                    return GetRandoms(sum, max);
                }
            }
        }
        // return randomList;
    }
    ///是不是奇数
    private bool IsOdd(int num)
    {
        return (num & 1) == 1;
    }
    ///获取逆序数
    int GetInverseNumberCount(List<int> numList)
    {
        int count = 0;

        for (int i = 0; i < numList.Count - 1; i++)
        {
            for (int j = i + 1; j < numList.Count; j++)
            {
                if (numList[i] > numList[j])
                    count++;
            }
        }
        return count;
    }
    //随机生成字符串
     string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
           System. Random r = new System. Random(BitConverter.ToInt32(b, 0));
            string s = 　　"用户", str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        } 
}
