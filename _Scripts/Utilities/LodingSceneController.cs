using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * File     : LodingSceneController.cs
 * Desc     : 화면 전환 시 로딩 화면
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */
public class LodingSceneController : MonoBehaviour
{
    private static string _nextScene;

    [SerializeField]
    private Image _background;
    [SerializeField]
    private Sprite[] _backgroundImage;
    private int _arrayIndex;
    [SerializeField]
    private Image _progressBar;

    public static void LoadScene(string sceneName)
    {
        _nextScene = sceneName;
        SceneManager.LoadScene(EnumTypes.SceneName.LoadingScene.ToString());
    }

    void Start()
    {
        _arrayIndex = Random.Range(0, _backgroundImage.Length);
        _background.sprite = _backgroundImage[_arrayIndex];
        StartCoroutine(LoadSceneProgress());
    }

    private IEnumerator LoadSceneProgress()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(_nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;

        while (!op.isDone)
        {
            yield return null;

            if (op.progress < 0.9f)
            {
                _progressBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                _progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);

                if (_progressBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
