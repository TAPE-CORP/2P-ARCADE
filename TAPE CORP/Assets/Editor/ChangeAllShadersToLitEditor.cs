//using UnityEngine;
//using UnityEditor;
//using UnityEngine.Tilemaps;

//public class ChangeAllShadersToLitEditor : EditorWindow
//{
//    private Material materialToApply;

//    [MenuItem("Tools/Apply Material To All Renderers")]
//    public static void ShowWindow()
//    {
//        GetWindow<ChangeAllShadersToLitEditor>("Apply Material");
//    }

//    private void OnGUI()
//    {
//        GUILayout.Label("Apply Material To All Sprite/Tilemap Renderers", EditorStyles.boldLabel);
//        materialToApply = (Material)EditorGUILayout.ObjectField("Material", materialToApply, typeof(Material), false);

//        if (GUILayout.Button("Apply To All In Scene"))
//        {
//            if (materialToApply == null)
//            {
//                Debug.LogError("머티리얼을 먼저 할당해주세요.");
//                return;
//            }

//            ApplyMaterialToAll();
//        }
//    }

//    private void ApplyMaterialToAll()
//    {
//        int count = 0;

//        // SpriteRenderer에 적용
//        foreach (var renderer in FindObjectsOfType<SpriteRenderer>())
//        {
//            Undo.RecordObject(renderer, "Apply Material");
//            renderer.sharedMaterial = materialToApply;
//            count++;
//        }

//        // TilemapRenderer에 적용
//        foreach (var tileRenderer in FindObjectsOfType<TilemapRenderer>())
//        {
//            Undo.RecordObject(tileRenderer, "Apply Material");
//            tileRenderer.material = materialToApply;
//            count++;
//        }

//        Debug.Log($"머티리얼 {materialToApply.name} 이(가) {count}개의 오브젝트에 적용되었습니다.");
//    }
//}
