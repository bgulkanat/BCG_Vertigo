using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class BCG_AssetImporter : EditorWindow
{
    private string _targetFolderText = "";
    private string _targetFolderPath = "";
    private string _externalAssetsFolderPath = "";
    private List<string> _changedNamesList = new();
    private List<string> _externalAssetsList = new();
    private ReorderableList _reorderableList;
    private string _findText = "";
    private string _replaceText = "";
    private Vector2 _scrollPosition;
    private bool _targetFolderFoldout = true;
    private bool _externalAssetsFoldout = true;
    private bool _namingFoldout = true;
    private bool _caseSensitive = false;
    private GUIContent _contentHelp;

    [MenuItem("Tools/BCG Asset Importer")]
    public static void ShowWindow()
    {
        var window = GetWindow<BCG_AssetImporter>("BCG Asset Importer");
        window.minSize = new Vector2(400, 360);
        Rect main = EditorGUIUtility.GetMainWindowPosition();
        float centerX = (main.width - 400) / 2 + main.x;
        float centerY = (main.height - 360) / 2 + main.y;
        // Create window in the middle of the screen -- To prevent window to disappear :'D
        window.position = new Rect(centerX, centerY, 400, 360);
    }

    private void OnEnable()
    {
        _contentHelp = EditorGUIUtility.IconContent("_Help");
        SetupReorderableList();
    }
    private void OnGUI()
    {
        GUILayout.MinWidth(300);
        DrawTargetFolderSection();
        DrawExternalAssetsSection();
        DrawNamingSection();
        DrawImportButton();
    }
    private void DrawTargetFolderSection()
    {
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal(GUILayout.Width(250));
        _targetFolderFoldout = EditorGUILayout.Foldout(_targetFolderFoldout, "Internal Target Folder", true, EditorStyles.foldout);
        if (GUILayout.Button(_contentHelp, GUIStyle.none))
        {
            EditorUtility.DisplayDialog("Information", "Choose the Target Folder as a Reference", "OK");
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(4);
        if (_targetFolderFoldout)
        {
            EditorGUILayout.BeginHorizontal();
            _targetFolderText = EditorGUILayout.TextField("Target Folder", _targetFolderText);
            if (GUILayout.Button("...", GUILayout.Width(24)))
            {
                _targetFolderPath = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
                string name = Path.GetFileName(_targetFolderPath);
                if (!string.IsNullOrEmpty(_targetFolderPath))
                {
                    _targetFolderText = _targetFolderPath.Replace(Application.dataPath + "/", "📁 ");
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    private void DrawExternalAssetsSection()
    {
        GUILayout.Space(10);
        GUIContent folderOpened = EditorGUIUtility.IconContent("FolderOpened Icon");
        EditorGUILayout.BeginHorizontal(GUILayout.Width(250));
        _externalAssetsFoldout = EditorGUILayout.Foldout(_externalAssetsFoldout, "External Assets Folder", true, EditorStyles.foldout);
        if (GUILayout.Button(_contentHelp, GUIStyle.none))
        {
            EditorUtility.DisplayDialog("Information", "Choose an External Folder Which Contains New Assets", "OK");
        }
        EditorGUILayout.EndHorizontal();
        if (_externalAssetsFoldout)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.BeginHorizontal();
                _externalAssetsFolderPath = EditorGUILayout.TextField("External Assets Path", _externalAssetsFolderPath);
                GUILayout.Label(folderOpened, GUIStyle.none, GUILayout.Width(20), GUILayout.Height(20));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(10);
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(false));
                _reorderableList.DoLayoutList();
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("Pick Assets Folder", GUILayout.Height(30)))
                {
                    string pathExternal = EditorUtility.OpenFolderPanel("Select External Assets Folder", "", "");
                    if (!string.IsNullOrEmpty(pathExternal))
                    {
                        _externalAssetsFolderPath = pathExternal;
                        LoadExternalAssets();
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }
    }
    private void DrawNamingSection()
    {
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal(GUILayout.Width(85));
        _namingFoldout = EditorGUILayout.Foldout(_namingFoldout, "Naming", true, EditorStyles.foldout);
        if (GUILayout.Button(_contentHelp, GUIStyle.none))
        {
            EditorUtility.DisplayDialog("Information", "Rename Assets In Target Folder -> Their names will match with External Assets", "OK");
        }
        EditorGUILayout.EndHorizontal();
        if (_namingFoldout)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.BeginHorizontal();
                _findText = EditorGUILayout.TextField("Find", _findText);
                GUILayout.Space(10);
                GUI.enabled = true;
                GUILayout.Label("Case-Sensitive", EditorStyles.label, GUILayout.ExpandWidth(false));
                _caseSensitive = GUILayout.Toggle(_caseSensitive, "", EditorStyles.toggle, GUILayout.Width(20));
                EditorGUILayout.EndHorizontal();

                _replaceText = EditorGUILayout.TextField("Replace", _replaceText);
                EditorGUILayout.EndVertical();
            }
        }
    }
    private void DrawImportButton()
    {
        GUILayout.Space(10);
        Color buttonColor = GUI.backgroundColor;
        Color textColor = GUI.contentColor;
        GUI.backgroundColor = new Color32(0x00, 0xff, 0xf6, 255);
        GUI.contentColor = new Color32(0x52, 0xfb, 0xf5, 255);
        if (GUILayout.Button("Import", GUILayout.Height(30)))
        {
            _changedNamesList = RenameObjectsInTargetFolder(_caseSensitive);
            DuplicateTargetFolder();
            Debug.Log("Duplicate button clicked");
        }
        GUI.backgroundColor = buttonColor;
        GUI.contentColor = textColor;
    }
    private void SetupReorderableList()
    {
        _reorderableList = new ReorderableList(_externalAssetsList, typeof(string), true, true, false, false);
        _reorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "External Files: " + (_externalAssetsList.Count > 0 ? _externalAssetsList.Count.ToString() : "Empty"));
        _reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            if (index < 0 || index >= _externalAssetsList.Count) return;
            rect.y += 2;
            rect.height = EditorGUIUtility.singleLineHeight;

            //Zebra Striping - To prevent removing another line mistakenly
            Color backgroundColor = (index % 2 == 0) ? new Color(0.2f, 0.2f, 0.2f, 1f) : new Color(0.25f, 0.25f, 0.25f, 1f);
            EditorGUI.DrawRect(rect, backgroundColor);

            string fileName = Path.GetFileName(_externalAssetsList[index]); //Just item name and its extension
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 30, rect.height), fileName);
            if (GUI.Button(new Rect(rect.x + rect.width - 25, rect.y, 25, rect.height), "X"))
            {
                RemoveItem(index);
            }
        };
    }
    private void RemoveItem(int index)
    {
        if (index >= 0 && index < _externalAssetsList.Count)
        {
            _externalAssetsList.RemoveAt(index);
            SetupReorderableList(); // This line is added to refresh the list after removing an item
            Repaint(); // I add this to prevent GUI error
        }
    }
    private void LoadExternalAssets()
    {
        _externalAssetsList.Clear();
        if (string.IsNullOrEmpty(_externalAssetsFolderPath)) return;

        string[] files = Directory.GetFiles(_externalAssetsFolderPath, "*.*", SearchOption.TopDirectoryOnly);
        
        foreach (string file in files)
        {
            _externalAssetsList.Add(file);
        }
        
        SetupReorderableList();
        Repaint(); // To prevent GUI error
    }
    private void DuplicateTargetFolder()
    {
        if (string.IsNullOrEmpty(_targetFolderPath) || string.IsNullOrEmpty(_findText) || string.IsNullOrEmpty(_replaceText))
        {
            Debug.LogWarning("Target folder or find text is empty!");
            EditorUtility.DisplayDialog("Warning", "You must select target folder and fill naming parameters", "OK");
            return;
        }

        string parentDir = Path.GetDirectoryName(_targetFolderPath);
        string folderName = Path.GetFileName(_targetFolderPath);
        string newFolderName = folderName.Replace(_findText, _replaceText);
        string newFolderPath = Path.Combine(parentDir, newFolderName);
        //If folderName is empty system says "There is already file with same name...." bla bla, I add this check to prevent that error
        if (!folderName.Contains(_findText))
        {
            Debug.LogWarning("Find text '" + _findText + "' not found in folder name '" + folderName + "'!");
            EditorUtility.DisplayDialog("Warning", "The find text must exist in the target folder name", "OK");
            return;
        }
        if (Directory.Exists(newFolderPath))
        {
            Debug.LogWarning("Duplicated folder already exists!");
            return;
        }
        try
        {
            Directory.CreateDirectory(newFolderPath);
            //I compared these two lists: changedNamesList and externalAssetsList
            foreach (string changedName in _changedNamesList)
            {
                Debug.Log(changedName);
                foreach (string assetName in _externalAssetsList)
                {
                    if (Path.GetFileName(assetName) == changedName)
                    {
                        string sourcePath = Path.Combine(_externalAssetsFolderPath, assetName);
                        string destinationPath = Path.Combine(newFolderPath, changedName);

                        File.Copy(sourcePath, destinationPath, true);
                        Debug.Log($"File copied: {sourcePath} -> {destinationPath}");
                    }
                }
            }
            Debug.Log("Folder duplicated successfully: " + newFolderPath);

            //To refresh the new folder and its sub-items in Unity Editor
            string unityPath = "Assets" + newFolderPath.Substring(Application.dataPath.Length);
            AssetDatabase.ImportAsset(unityPath, ImportAssetOptions.ImportRecursive);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error duplicating folder: " + e.Message);
        }
    }
    private List<string> RenameObjectsInTargetFolder(bool useCaseSensitive)
    {
        _changedNamesList.Clear();
        string[] files = Directory.GetFiles(_targetFolderPath, "*.*", SearchOption.TopDirectoryOnly);

        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            string newFileName;
            if (useCaseSensitive)
            {
                newFileName = Regex.Replace(fileName, Regex.Escape(_findText), _replaceText);
            }
            else
            {
                newFileName = Regex.Replace(fileName, Regex.Escape(_findText), _replaceText, RegexOptions.IgnoreCase);
            }
            if (fileName != newFileName)
            {
                _changedNamesList.Add(newFileName);
            }
        }
        return _changedNamesList;
    }
}