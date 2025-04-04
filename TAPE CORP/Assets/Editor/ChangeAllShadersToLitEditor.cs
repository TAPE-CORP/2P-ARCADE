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
//                Debug.LogError("��Ƽ������ ���� �Ҵ����ּ���.");
//                return;
//            }

//            ApplyMaterialToAll();
//        }
//    }

//    private void ApplyMaterialToAll()
//    {
//        int count = 0;

//        // SpriteRenderer�� ����
//        foreach (var renderer in FindObjectsOfType<SpriteRenderer>())
//        {
//            Undo.RecordObject(renderer, "Apply Material");
//            renderer.sharedMaterial = materialToApply;
//            count++;
//        }

//        // TilemapRenderer�� ����
//        foreach (var tileRenderer in FindObjectsOfType<TilemapRenderer>())
//        {
//            Undo.RecordObject(tileRenderer, "Apply Material");
//            tileRenderer.material = materialToApply;
//            count++;
//        }

//        Debug.Log($"��Ƽ���� {materialToApply.name} ��(��) {count}���� ������Ʈ�� ����Ǿ����ϴ�.");
//    }
//}
