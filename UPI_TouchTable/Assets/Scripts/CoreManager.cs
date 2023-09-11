using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CoreManager : MonoBehaviour
{
    #region Instance
    public static CoreManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    [Header("DATA")]
    [SerializeField] string fName;
    [SerializeField] List<ContentData> all_content = new List<ContentData>();

    [Header("UI")]
    [SerializeField] Button[] mainBTNS;
    [SerializeField] GameObject StartPanel;
    [SerializeField] GameObject AboutPanel;
    [SerializeField] GameObject VideoPanel;
    [SerializeField] CanvasGroup MainStartUI;
    [SerializeField] CanvasGroup ContentUI;
    [SerializeField] RectTransform HomePanel;

    [SerializeField] Image AboutBTN;
    [SerializeField] Image VideoBTN;
    [SerializeField] Sprite[] buttonSprites;

    [Header("Start Panel")]
    [SerializeField] GameObject OpenContentBTNsPanel;
    [SerializeField] TMP_Text selectedTitle;
    [SerializeField] GameObject btnsParent;
    [SerializeField] CanvasGroup ExploreBTN;


    [Header("About Panel")]
    [SerializeField] TMP_Text aboutText;

    [Header("Char Panel")]
    [SerializeField] TMP_Text charText;

    [Header("Video Panel")]
    [SerializeField] VideoPlayer videoPlayer;

    [SerializeField] GameObject SplashScreenVideoObj;

    [SerializeField] AnimationCurve moveXCurve;

    [SerializeField] int lastSelectedIndex;

    public GameState state;

    [SerializeField] GameObject UpiLogo;
    private void Start()
    {
        state = GameState.START;

        GetAllData(fName);
        AssignButtons();
        SplashScreenVideoObj.SetActive(true);
    }

    void HandleStateChange()
    {
        LeanTween.cancel(UpiLogo);

        switch (state)
        {
            case GameState.START:
                {

                    LeanTween.move(UpiLogo.GetComponent<RectTransform>(), Vector3.zero, 0.5f).setEaseInQuad();
                    LeanTween.scale(UpiLogo, Vector3.one, 0.3f).setEaseInQuad();
                    break;
                }
            case GameState.HOMEBTNS:
                {

                    LeanTween.move(UpiLogo.GetComponent<RectTransform>(), Vector3.zero, 0.5f).setEaseInQuad();
                    LeanTween.scale(UpiLogo, Vector3.one, 0.3f).setEaseInQuad();
                    break;
                }
            case GameState.ABOUT:
                {
                    LeanTween.move(UpiLogo.GetComponent<RectTransform>(), new Vector3(0, 480, 0), 0.5f).setEaseInQuad();
                    LeanTween.scale(UpiLogo, Vector3.one * 0.2f, 0.3f).setEaseInQuad();
                    break;
                }
            case GameState.CHAR:
                {
                    LeanTween.move(UpiLogo.GetComponent<RectTransform>(), new Vector3(0, 480, 0), 0.5f).setEaseInQuad();
                    LeanTween.scale(UpiLogo, Vector3.one * 0.2f, 0.3f).setEaseInQuad();
                    break;
                }
            case GameState.VIDEO:
                {
                    LeanTween.move(UpiLogo.GetComponent<RectTransform>(), new Vector3(0, 480, 0), 0.5f).setEaseInQuad();
                    LeanTween.scale(UpiLogo, Vector3.one * 0.2f, 0.3f).setEaseInQuad();
                    break;
                }
        }
    }

    private void GetAllData(string fileName)
    {

#if UNITY_ANDROID
        TextAsset fileContents = (TextAsset)Resources.Load(fileName);
        all_content = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ContentData>>(fileContents.text);

#else
 if (File.Exists(Application.streamingAssetsPath + "/" + fileName))
        {
            string fileContents = File.ReadAllText(Application.streamingAssetsPath + "/" + fileName);
            all_content = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ContentData>>(fileContents);
        }
#endif

    }
    private void AssignButtons()
    {
        for (int i = 0; i < mainBTNS.Length; i++)
        {
            int temp = i;
            mainBTNS[i].transform.GetChild(0).GetComponent<TMP_Text>().text = all_content[i].title;
            mainBTNS[i].onClick.AddListener(() => SelectButton(temp));
        }

    }

    public void LetsExplore()
    {
        SplashScreenVideoObj.SetActive(false);
        state = GameState.HOMEBTNS;
        HandleStateChange();

        LeanTween.cancel(ExploreBTN.gameObject);
        LeanTween.alphaCanvas(ExploreBTN, 0, 0.3f).setOnComplete(() =>
        {
            ExploreBTN.gameObject.SetActive(false);
        });

        LeanTween.cancel(btnsParent);
        LeanTween.move(btnsParent.GetComponent<RectTransform>(), new Vector3(0, btnsParent.GetComponent<RectTransform>().anchoredPosition.y, 0), 1).setEase(moveXCurve);
    }
    public void HomeFromExplore()
    {

        state = GameState.HOMEBTNS;
        HandleStateChange();



        LeanTween.cancel(ExploreBTN.gameObject);

        ExploreBTN.gameObject.SetActive(true);
        LeanTween.alphaCanvas(ExploreBTN, 1, 0.3f);

        LeanTween.cancel(btnsParent);
        LeanTween.move(btnsParent.GetComponent<RectTransform>(), new Vector3(1920, btnsParent.GetComponent<RectTransform>().anchoredPosition.y, 0), 0.1f).setEase(moveXCurve);

        SplashScreenVideoObj.SetActive(true);
    }


    public void SelectButton(int index)
    {
        lastSelectedIndex = index;

        state = GameState.ABOUT;
        HandleStateChange();

        LeanTween.cancel(MainStartUI.gameObject);
        LeanTween.cancel(ContentUI.gameObject);

        LeanTween.alphaCanvas(MainStartUI, 0, 0.5f).setOnComplete(() =>
        {
            MainStartUI.gameObject.SetActive(false);
        });
        ContentUI.gameObject.SetActive(true);
        LeanTween.alphaCanvas(ContentUI, 1, 0.5f);


        VideoPanel.SetActive(false);
        VideoPanel.GetComponent<CanvasGroup>().alpha = 0;

        AboutPanel.SetActive(true);
        AboutPanel.GetComponent<CanvasGroup>().alpha = 1;


        //LeanTween.move(HomePanel, new Vector3(0, 0, 0), 0.5f).setEaseInQuad();

        selectedTitle.text = all_content[lastSelectedIndex].title;


#if UNITY_ANDROID
        string temppath = all_content[index].videoLink.Replace(".mp4", "");
        videoPlayer.clip =  Resources.Load<VideoClip>(temppath);
#else
videoPlayer.url = Application.streamingAssetsPath + "/" + all_content[index].videoLink;

#endif



        videoPlayer.Prepare();

        aboutText.text = all_content[index].about;

        aboutText.rectTransform.sizeDelta = new Vector2(aboutText.rectTransform.sizeDelta.x, aboutText.preferredHeight);


        charText.text = all_content[index].characteristics;
        charText.rectTransform.sizeDelta = new Vector2(charText.rectTransform.sizeDelta.x, charText.preferredHeight);



        OpenContentBTNsPanel.SetActive(true);

        AboutBTN.sprite = buttonSprites[0];
        VideoBTN.sprite = buttonSprites[1];
    }

    public void Home()
    {
        SplashScreenVideoObj.SetActive(true);
        state = GameState.START;
        HandleStateChange();

        videoPlayer.Stop();


        LeanTween.alphaCanvas(VideoPanel.GetComponent<CanvasGroup>(), 0, 0.5f).setOnComplete(() =>
        {
            VideoPanel.SetActive(false);
        });
        LeanTween.alphaCanvas(AboutPanel.GetComponent<CanvasGroup>(), 0, 0.5f).setOnComplete(() =>
        {
            AboutPanel.SetActive(false);
        });

        MainStartUI.gameObject.SetActive(true);
        ExploreBTN.gameObject.SetActive(true);
        ExploreBTN.alpha = 1;

        btnsParent.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920, btnsParent.GetComponent<RectTransform>().anchoredPosition.y);
        LeanTween.alphaCanvas(MainStartUI, 1, 0.5f);
        LeanTween.alphaCanvas(ContentUI, 0, 0.5f).setOnComplete(() =>
        {
            //  HomePanel.anchoredPosition = new Vector2(1920, 0);
            ContentUI.gameObject.SetActive(false);
        });
    }
    public void Back()
    {
        state = GameState.HOMEBTNS;
        HandleStateChange();

        videoPlayer.Stop();

        LeanTween.cancel(VideoPanel.gameObject);
        LeanTween.cancel(AboutPanel.gameObject);


        LeanTween.alphaCanvas(VideoPanel.GetComponent<CanvasGroup>(), 0, 0.5f).setOnComplete(() =>
        {
            VideoPanel.SetActive(false);
        });
        LeanTween.alphaCanvas(AboutPanel.GetComponent<CanvasGroup>(), 0, 0.5f).setOnComplete(() =>
        {
            AboutPanel.SetActive(false);
        });


        ExploreBTN.gameObject.SetActive(false);
        ExploreBTN.alpha = 0;

        LeanTween.cancel(btnsParent);
        LeanTween.move(btnsParent.GetComponent<RectTransform>(), new Vector3(0, btnsParent.GetComponent<RectTransform>().anchoredPosition.y, 0), 1).setEase(moveXCurve);

        LeanTween.cancel(MainStartUI.gameObject);
        LeanTween.cancel(ContentUI.gameObject);

        MainStartUI.gameObject.SetActive(true);
        LeanTween.alphaCanvas(MainStartUI, 1, 0.5f);
        LeanTween.alphaCanvas(ContentUI, 0, 0.5f).setOnComplete(() =>
        {
            //HomePanel.anchoredPosition = new Vector2(1920, 0);
            ContentUI.gameObject.SetActive(false);
        });
    }

    public void OpenAboutPanel()
    {
        state = GameState.ABOUT;
        HandleStateChange();

        videoPlayer.Stop();

        LeanTween.cancel(AboutPanel);

        LeanTween.cancel(VideoPanel);

        AboutBTN.sprite = buttonSprites[0];
        VideoBTN.sprite = buttonSprites[1];

        LeanTween.alphaCanvas(VideoPanel.GetComponent<CanvasGroup>(), 0, 0.5f).setOnComplete(() =>
        {
            VideoPanel.SetActive(false);
        });


        LeanTween.alphaCanvas(AboutPanel.GetComponent<CanvasGroup>(), 1, 0.5f);
        AboutPanel.SetActive(true);
    }



    public void OpenVideoPanel()
    {
        state = GameState.VIDEO;
        HandleStateChange();

        AboutBTN.sprite = buttonSprites[1];
        VideoBTN.sprite = buttonSprites[0];

        videoPlayer.Play();

        LeanTween.cancel(AboutPanel);

        LeanTween.cancel(VideoPanel);

        LeanTween.alphaCanvas(AboutPanel.GetComponent<CanvasGroup>(), 0, 0.5f).setOnComplete(() =>
        {
            AboutPanel.SetActive(false);
        });



        LeanTween.alphaCanvas(VideoPanel.GetComponent<CanvasGroup>(), 1, 0.5f);
        VideoPanel.SetActive(true);
    }

}

[System.Serializable]
public class ContentData
{
    public int index;
    public string title;
    public string about;
    public string characteristics;
    public string videoLink;
}
public enum GameState
{
    START, HOMEBTNS, ABOUT, CHAR, VIDEO
}