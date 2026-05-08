using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;

public static class FontSetupHelper
{
    const string FONT_PATH      = "Assets/Fonts/malgun.ttf";
    const string SDF_ASSET_PATH = "Assets/Fonts/malgun_SDF.asset";

    [MenuItem("FakeSoldier/Setup Korean Font + Apply All Scenes")]
    public static void SetupKoreanFont()
    {
        // 1. 기존 깨진 에셋 삭제
        if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(SDF_ASSET_PATH) != null)
        {
            AssetDatabase.DeleteAsset(SDF_ASSET_PATH);
            AssetDatabase.Refresh();
        }

        // 2. 소스 폰트 임포트
        AssetDatabase.ImportAsset(FONT_PATH, ImportAssetOptions.ForceSynchronousImport);
        var sourceFont = AssetDatabase.LoadAssetAtPath<Font>(FONT_PATH);
        if (sourceFont == null) { Debug.LogError("malgun.ttf 로드 실패"); return; }

        // 3. TMP 폰트 에셋 생성
        var tmpFont = TMP_FontAsset.CreateFontAsset(sourceFont);
        tmpFont.atlasPopulationMode = AtlasPopulationMode.Dynamic;
        tmpFont.name = "Malgun Gothic SDF";

        // 4. 메인 에셋 저장
        AssetDatabase.CreateAsset(tmpFont, SDF_ASSET_PATH);

        // 5. 서브 에셋 저장 (아틀라스 텍스처, 머티리얼)
        if (tmpFont.atlasTextures != null)
        {
            for (int i = 0; i < tmpFont.atlasTextures.Length; i++)
            {
                var tex = tmpFont.atlasTextures[i];
                if (tex != null)
                {
                    tex.name = "Malgun Gothic SDF Atlas";
                    AssetDatabase.AddObjectToAsset(tex, tmpFont);
                }
            }
        }
        if (tmpFont.material != null)
        {
            tmpFont.material.name = "Malgun Gothic SDF Material";
            AssetDatabase.AddObjectToAsset(tmpFont.material, tmpFont);
        }

        EditorUtility.SetDirty(tmpFont);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 다시 로드 (저장 후 참조 갱신)
        tmpFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(SDF_ASSET_PATH);
        if (tmpFont == null) { Debug.LogError("TMP 폰트 에셋 저장 실패"); return; }

        // 6. TMP 기본 폰트 설정
        var settings = TMP_Settings.instance;
        if (settings != null)
        {
            var settingsSO = new SerializedObject(settings);
            settingsSO.Update();
            var defaultFontProp = settingsSO.FindProperty("m_defaultFontAsset");
            if (defaultFontProp != null)
            {
                defaultFontProp.objectReferenceValue = tmpFont;
                settingsSO.ApplyModifiedProperties();
                Debug.Log("TMP Default Font → 맑은 고딕 SDF 설정 완료");
            }
        }

        // 7. 모든 씬에 폰트 적용
        string[] scenePaths = {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/Stage_01.unity", "Assets/Scenes/Stage_02.unity",
            "Assets/Scenes/Stage_03.unity", "Assets/Scenes/Stage_04.unity",
            "Assets/Scenes/Stage_05.unity",
            "Assets/Scenes/Ending_Bad.unity", "Assets/Scenes/Ending_Normal.unity",
            "Assets/Scenes/Ending_True.unity", "Assets/Scenes/Credit.unity"
        };

        foreach (var path in scenePaths)
        {
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            ApplyFontToScene(scene, tmpFont);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"폰트 적용: {path}");
        }

        Debug.Log("한국어 폰트 전체 적용 완료!");
    }

    static void ApplyFontToScene(Scene scene, TMP_FontAsset font)
    {
        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var tmp in root.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                var so = new SerializedObject(tmp);
                so.Update();
                var fontProp = so.FindProperty("m_fontAsset");
                if (fontProp != null)
                {
                    fontProp.objectReferenceValue = font;
                    so.ApplyModifiedProperties();
                }
            }
        }
    }
}
