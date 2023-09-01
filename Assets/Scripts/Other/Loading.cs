using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Loading : MonoBehaviour
{
    public Slider slider;
    [Header("�һݭnŪ���ɶ�")]
    public float readTime = 2f;
    [Header("��e�[���ɶ�")]
    public float curProgress;
    [Header("�}�l�C�����s")]
    public GameObject StartButton;
    [Header("���b���J����r")]
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
