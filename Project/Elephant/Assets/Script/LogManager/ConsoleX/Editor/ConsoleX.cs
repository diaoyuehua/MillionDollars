using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Text.RegularExpressions;

public class ConsoleTreeView : TreeView
{
    private TreeViewItem root;
    private bool collapse = false;
    private string searchingString = "";
    private Regex reg = null;

    public ConsoleTreeView(bool col) : base(new TreeViewState())
    {
        showAlternatingRowBackgrounds = true;
        showBorder = true;
        rowHeight = 32;

        collapse = col;

        root = new TreeViewItem(-1, -1);
        root.children = new List<TreeViewItem>();
    }

    public void SetCollapse(bool col)
    {
        if (col != collapse)
        {
            collapse = col;
        }
    }

    public void SetCount(int count)
    {
        int childrenCount = root.children.Count;
        if(childrenCount < count)
        {
            for (int i = childrenCount; i < count; ++i)
            {
                root.children.Add(new TreeViewItem(i, 0));
            }
            int first;
            int last;
            GetFirstAndLastVisibleRows(out first, out last);
            if(childrenCount - last <= 1)
            {
                state.scrollPos = new Vector2(0, float.MaxValue);
            }

            Reload();
        }
        else if(childrenCount > count)
        {
            int diff = childrenCount - count;
            root.children.RemoveRange(childrenCount - diff, diff);
            SetSelection(new List<int>());
            Reload();
        }
    }

    public void SetSearch(string ss)
    {
        if(string.IsNullOrWhiteSpace(ss))
        {
            reg = null;
        }
        else if(searchingString != ss)
        {
            searchingString = ss;
            reg = new Regex(searchingString);
        }
    }

    protected override TreeViewItem BuildRoot()
    {
        return root;
    }

    protected override bool CanMultiSelect(TreeViewItem item)
    {
        return false;
    }

    protected override void RowGUI(RowGUIArgs args)
    {
        int row = args.row;
        object[] arg = new object[4] {row, 2, 0, "" };
        ConsoleX.getLinesAndModeFromEntryInternalMethod.Invoke(null, arg);

        int mode = (int)arg[2];
        string text = arg[3].ToString();

        if (reg!=null)
        {
            if(reg.IsMatch(text))
            {
                GUI.contentColor = Color.cyan;
            }
            else
            {
                GUI.contentColor = Color.gray;
            }
        }
        else
        {
            GUI.contentColor = Color.white;
        }

        EditorGUI.DrawRect(new Rect(args.rowRect.x+4, args.rowRect.y+12, 8, 8), ConsoleX.GetColorByMode(mode));
        EditorGUI.LabelField(new Rect(args.rowRect.x+16, args.rowRect.y, args.rowRect.width-16, args.rowRect.height), text);
        if (collapse)
        {
            int count = (int)ConsoleX.getEntryCountMethod.Invoke(null, new object[1] { row });
            EditorGUI.LabelField(new Rect(args.rowRect.width-48, args.rowRect.y+8, 40, 16), count > 999 ? "999+" : count.ToString(), EditorStyles.miniButton);
        }

        GUI.contentColor = Color.white;
    }

    protected override void DoubleClickedItem(int id)
    {
        ConsoleX.rowGotDoubleClickedMethod.Invoke(null, new object[1] { id });
    }
}

public class ConsoleX : EditorWindow
{
    [MenuItem("Window/ConsoleX")]
    public static void CreateDialog()
    {
        EditorWindow.GetWindow(typeof(ConsoleX));
    }

    public enum XMode
    {
        Error = 1 << 0,
        Assert = 1 << 1,
        Log = 1 << 2,
        Fatal = 1 << 4,
        DontPreprocessCondition = 1 << 5,
        AssetImportError = 1 << 6,
        AssetImportWarning = 1 << 7,
        ScriptingError = 1 << 8,
        ScriptingWarning = 1 << 9,
        ScriptingLog = 1 << 10,
        ScriptCompileError = 1 << 11,
        ScriptCompileWarning = 1 << 12,
        StickyError = 1 << 13,
        MayIgnoreLineNumber = 1 << 14,
        ReportBug = 1 << 15,
        DisplayPreviousErrorInStatusBar = 1 << 16,
        ScriptingException = 1 << 17,
        DontExtractStacktrace = 1 << 18,
        ShouldClearOnPlay = 1 << 19,
        GraphCompileError = 1 << 20,
        ScriptingAssertion = 1 << 21,
        VisualScriptingError = 1 << 22
    };

    public enum XConsoleFlags
    {
        Collapse = 1 << 0,
        ClearOnPlay = 1 << 1,
        ErrorPause = 1 << 2,
        Verbose = 1 << 3,
        StopForAssert = 1 << 4,
        StopForError = 1 << 5,
        Autoscroll = 1 << 6,
        LogLevelLog = 1 << 7,
        LogLevelWarning = 1 << 8,
        LogLevelError = 1 << 9,
        ShowTimestamp = 1 << 10,
    };

    public static Type logEntriesType = null;
    public static MethodInfo getCountMethod = null;
    public static MethodInfo getCountsByTypeMethod = null;
    public static MethodInfo startGettingEntriesMethod = null;
    public static MethodInfo endGettingEntriesMethod = null;
    public static MethodInfo clearMethod = null;
    public static MethodInfo setConsoleFlagMethod = null;
    public static PropertyInfo consoleFlagProperty = null;
    public static MethodInfo getEntryInternalMethod = null;
    public static MethodInfo getEntryCountMethod = null;
    public static MethodInfo getLinesAndModeFromEntryInternalMethod = null;
    public static MethodInfo rowGotDoubleClickedMethod = null;
    public static Type logEntryType = null;
    public static FieldInfo conditionInfo = null;
    public static FieldInfo modeInfo = null;

    public static SearchField searchBar = null;
    public static ConsoleTreeView consoleTreeView = null;

    public static bool searching = false;
    public static string searchingStr = null;
    public static string searchStr = null;

    public ConsoleX()
    {
        logEntriesType = typeof(EditorWindow).Assembly.GetType("UnityEditor.LogEntries", true);
        getCountMethod = logEntriesType.GetMethod("GetCount", BindingFlags.Static | BindingFlags.Public);
        getCountsByTypeMethod = logEntriesType.GetMethod("GetCountsByType", BindingFlags.Static | BindingFlags.Public);
        startGettingEntriesMethod = logEntriesType.GetMethod("StartGettingEntries", BindingFlags.Static | BindingFlags.Public);
        endGettingEntriesMethod = logEntriesType.GetMethod("EndGettingEntries", BindingFlags.Static | BindingFlags.Public);
        clearMethod = logEntriesType.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
        setConsoleFlagMethod = logEntriesType.GetMethod("SetConsoleFlag", BindingFlags.Static | BindingFlags.Public);
        consoleFlagProperty = logEntriesType.GetProperty("consoleFlags", BindingFlags.Static | BindingFlags.Public);
        getEntryInternalMethod = logEntriesType.GetMethod("GetEntryInternal", BindingFlags.Static | BindingFlags.Public);
        getEntryCountMethod = logEntriesType.GetMethod("GetEntryCount", BindingFlags.Static | BindingFlags.Public);
        getLinesAndModeFromEntryInternalMethod = logEntriesType.GetMethod("GetLinesAndModeFromEntryInternal", BindingFlags.Static | BindingFlags.Public);
        rowGotDoubleClickedMethod = logEntriesType.GetMethod("RowGotDoubleClicked", BindingFlags.Static | BindingFlags.Public);
        logEntryType = typeof(EditorWindow).Assembly.GetType("UnityEditor.LogEntry");
        conditionInfo = logEntryType.GetField("condition", BindingFlags.Instance | BindingFlags.Public);
        modeInfo = logEntryType.GetField("mode", BindingFlags.Instance | BindingFlags.Public);

        searching = false;
        searchingStr = "";
        searchStr = "";
    }

    public static bool HasMode(int mode, XMode modeToCheck) { return (mode & (int)modeToCheck) != 0; }

    public static bool HasFlag(XConsoleFlags flags, int consoleFlag) { return (consoleFlag & (int)flags) != 0; }

    public static Color GetColorByMode(int mode)
    {
        if (HasMode(mode, XMode.Fatal | XMode.Assert |
                    XMode.Error | XMode.ScriptingError |
                    XMode.AssetImportError | XMode.ScriptCompileError |
                    XMode.GraphCompileError | XMode.ScriptingAssertion))
        {
            return Color.red;
        }
        else if(HasMode(mode, XMode.ScriptCompileWarning | XMode.ScriptingWarning | XMode.AssetImportWarning))
        {
            return Color.yellow;
        }
        else if(HasMode(mode, XMode.Log | XMode.ScriptingLog))
        {
            return Color.white;
        }
        else
        {
            return Color.white;
        }
    }

    void OnEnable()
    {
        int consoleFlag = (int)consoleFlagProperty.GetValue(null, new object[0]);
        bool col = HasFlag(XConsoleFlags.Collapse, consoleFlag);

        searchBar = new SearchField();
        consoleTreeView = new ConsoleTreeView(col);
        consoleTreeView.Reload();
    }

    void OnGUI()
    {
        //record origin color
        Color originColor = GUI.color;

        //toolbar
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        //clear button
        if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
        {
            clearMethod.Invoke(null, new object[0]);
            consoleTreeView.SetSelection(new List<int>());
        }

        //space
        EditorGUILayout.Space();

        //collapse
        int consoleFlag = (int)consoleFlagProperty.GetValue(null, new object[0]);
        bool oldCollapse = HasFlag(XConsoleFlags.Collapse, consoleFlag);
        bool newCollapse = EditorGUILayout.Toggle(oldCollapse, EditorStyles.toggle, GUILayout.MaxWidth(16));
        if(oldCollapse != newCollapse)
        {
            setConsoleFlagMethod.Invoke(null, new object[2] { (int)XConsoleFlags.Collapse, newCollapse });
            consoleTreeView.SetCollapse(newCollapse);
            consoleTreeView.SetSelection(new List<int>());
        }
        EditorGUILayout.LabelField("Collapse", GUILayout.MaxWidth(64));
        GUI.color = originColor;

        //clear on pause
        bool oldClearOnPlay = HasFlag(XConsoleFlags.ClearOnPlay, consoleFlag);
        bool newClearOnPlay = EditorGUILayout.Toggle(oldClearOnPlay, EditorStyles.toggle, GUILayout.MaxWidth(16));
        if(oldClearOnPlay != newClearOnPlay)
        {
            setConsoleFlagMethod.Invoke(null, new object[2] { (int)XConsoleFlags.ClearOnPlay, newClearOnPlay });
        }
        EditorGUILayout.LabelField("ClearOnPlay", GUILayout.MaxWidth(80));
        GUI.color = originColor;

        //error pause
        bool oldErrorPause = HasFlag(XConsoleFlags.ErrorPause, consoleFlag);
        bool newErrorPause = EditorGUILayout.Toggle(oldErrorPause, EditorStyles.toggle, GUILayout.MaxWidth(16));
        if (oldErrorPause != newErrorPause)
        {
            setConsoleFlagMethod.Invoke(null, new object[2] { (int)XConsoleFlags.ErrorPause, newErrorPause });
        }
        EditorGUILayout.LabelField("ErrorPause", GUILayout.MaxWidth(80));
        GUI.color = originColor;

        //flexible space
        GUILayout.FlexibleSpace();

        //search
        searchStr = searchBar.OnToolbarGUI(searchStr);
        consoleTreeView.SetSearch(searchStr);

        GUI.color = originColor;

        //flexible space
        GUILayout.FlexibleSpace();

        //log count
        object[] args = new object[3];
        getCountsByTypeMethod.Invoke(null, args);

        EditorGUI.BeginChangeCheck();

        GUI.color = Color.white;
        bool flagLogLevelLog = EditorGUILayout.Toggle(HasFlag(XConsoleFlags.LogLevelLog, consoleFlag), EditorStyles.toggle, GUILayout.MaxWidth(8));
        
        EditorGUILayout.LabelField((int)args[2] > 999 ? "999+" : args[2].ToString(), EditorStyles.whiteLabel, GUILayout.MaxWidth(32));

        GUI.color = Color.yellow;
        bool flagLogLevelWarning = EditorGUILayout.Toggle(HasFlag(XConsoleFlags.LogLevelWarning, consoleFlag), EditorStyles.toggle, GUILayout.MaxWidth(8));
        
        EditorGUILayout.LabelField((int)args[1] > 999 ? "999+" : args[1].ToString(), EditorStyles.whiteLabel, GUILayout.MaxWidth(32));

        GUI.color = Color.red;
        bool flagLogLevelError = EditorGUILayout.Toggle(HasFlag(XConsoleFlags.LogLevelError, consoleFlag), EditorStyles.toggle, GUILayout.MaxWidth(8));
        
        EditorGUILayout.LabelField((int)args[0] > 999 ? "999+" : args[0].ToString(), EditorStyles.whiteLabel, GUILayout.MaxWidth(32));

        if(EditorGUI.EndChangeCheck())
        {
            consoleTreeView.SetSelection(new List<int>());
        }

        setConsoleFlagMethod.Invoke(null, new object[2] { (int)XConsoleFlags.LogLevelLog, flagLogLevelLog });
        setConsoleFlagMethod.Invoke(null, new object[2] { (int)XConsoleFlags.LogLevelWarning, flagLogLevelWarning });
        setConsoleFlagMethod.Invoke(null, new object[2] { (int)XConsoleFlags.LogLevelError, flagLogLevelError });

        GUI.color = originColor;

        EditorGUILayout.EndHorizontal();

        //log
        startGettingEntriesMethod.Invoke(null, new object[0]);

        int count = (int)getCountMethod.Invoke(null, new object[0] { });

        consoleTreeView.SetCount(count);
        consoleTreeView.OnGUI(new Rect(0, 17, position.width, position.height-80));

        IList<int> selectList = consoleTreeView.GetSelection();
        string text = "";
        if (selectList.Count > 0)
        {
            var logEntry = System.Activator.CreateInstance(logEntryType);
            getEntryInternalMethod.Invoke(null, new object[2] { selectList[0], logEntry });
            text = conditionInfo.GetValue(logEntry).ToString();
        }
        EditorGUI.SelectableLabel(new Rect(0, position.height - 63, position.width, 64), text, EditorStyles.whiteLabel);
        endGettingEntriesMethod.Invoke(null, new object[0]);

        GUI.color = originColor;
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
}
