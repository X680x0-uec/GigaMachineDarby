using UnityEngine;
using UnityEditor;

public class ObjectReplacer : EditorWindow
{
    [SerializeField] private GameObject newPrefab;

    [MenuItem("Tools/Object Replacer")]
    static void Init()
    {
        GetWindow<ObjectReplacer>("Replacer");
    }

    void OnGUI()
    {
        GUILayout.Label("オブジェクト置き換えツール（サイズ統一版）", EditorStyles.boldLabel);
        GUILayout.Space(10);

        newPrefab = (GameObject)EditorGUILayout.ObjectField("新しいプレハブ", newPrefab, typeof(GameObject), false);

        GUILayout.Space(20);

        if (GUILayout.Button("選択中のオブジェクトを置き換える"))
        {
            ReplaceSelectedObjects();
        }
    }

    void ReplaceSelectedObjects()
    {
        if (newPrefab == null)
        {
            Debug.LogError("新しいプレハブを指定してください！");
            return;
        }

        foreach (GameObject oldObj in Selection.gameObjects)
        {
            // Undo登録（元に戻せるようにする）
            Undo.RegisterCompleteObjectUndo(oldObj, "Replace Object");

            // 新しいプレハブを生成
            GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(newPrefab);
            Undo.RegisterCreatedObjectUndo(newObj, "Create New Object");

            // 親オブジェクトを引き継ぐ
            newObj.transform.SetParent(oldObj.transform.parent);

            // 位置と回転だけコピー（サイズはコピーしない！）
            newObj.transform.localPosition = oldObj.transform.localPosition;
            newObj.transform.localRotation = oldObj.transform.localRotation;
            
            // ★変更点：サイズは「新しいプレハブの設定」をそのまま使う
            // もし強制的に (1,1,1) にしたい場合は Vector3.one に書き換えてください
            newObj.transform.localScale = newPrefab.transform.localScale;

            // 順序を整える（ヒエラルキー上で元のオブジェクトのすぐ下に来るように）
            newObj.transform.SetSiblingIndex(oldObj.transform.GetSiblingIndex());

            // 元のオブジェクトを削除
            Undo.DestroyObjectImmediate(oldObj);
        }
    }
}