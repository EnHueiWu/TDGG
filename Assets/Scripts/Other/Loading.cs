using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Loading : MonoBehaviour
{
    public Slider slider;
    [Header("所需要讀取時間")]
    public float readTime = 2f;
    [Header("當前加載時間")]
    public float curProgress;
    [Header("開始遊戲按鈕")]
    public GameObject StartButton;
    [Header("正在載入中文字")]
    public GameObject ChineseLoading;

    void Update()
    {

        curProgress += Time.deltaTime / readTime;
        if (curProgress > 1.0f)
        {
            curProgress = 1;
        }
        OnSliderValueChange(curProgress);
    }

    void OnSliderValueChange(float value)
    {
        slider.value = value;
        if (value >= 1.0)
        {
            ChineseLoading.SetActive(false);
            StartButton.SetActive(true);
        }
    }
    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
