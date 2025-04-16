using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCon : MonoBehaviour
{
    public static SceneCon Instance;

    public List<string> sceneNames = new List<string>();

    void Awake()
    {
        // �̱��� + DontDestroy
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSceneNames();
        }
        else
        {
            Destroy(gameObject); // �ߺ� ����
        }
    }

    void LoadSceneNames()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        sceneNames.Clear();

        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            sceneNames.Add(sceneName);
        }

        foreach (string name in sceneNames)
        {
            Debug.Log("��ϵ� ��: " + name);
        }
    }
    // �� ��ȯ �޼���
    public void LoadScene(string SceneToGo)
    {
        SceneManager.LoadScene(SceneToGo);
    }
}
