#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.Linq;
using UnityEngine.UIElements;
using TreeView = UnityEditor.IMGUI.Controls.TreeView;

public static class NetcodeForGameObjectsTools
{
    [MenuItem("Tools/Netcode/NetworkObject Finder")]
    public static void ShowNetworkObjectFinderWindow()
    {
        var editorWindow = EditorWindow.GetWindow<NetworkObjectFinderWindow>();
        editorWindow.titleContent = new GUIContent("NetworkObject Finder");
    }

    public class NetworkObjectFinderWindow : EditorWindow
    {
        private TextField m_GlobalObjectIdHashToFind;

        private void OnEnable()
        {

        }

        private void CreateGUI()
        {
            var root = rootVisualElement;

            var findLabel = new Label("NetworkObjetIdHash:");
            m_GlobalObjectIdHashToFind = new TextField();
            var findButton = new Button(new System.Action(OnButtonClick))
            {
                text = "Find NetworkObject",
            };
            root.Add(findLabel);
            root.Add(m_GlobalObjectIdHashToFind);
            root.Add(findButton);
        }

        private void OnButtonClick()
        {
            var prefabs = AssetDatabase.FindAssets("t:Prefab");

            foreach(var prefab in prefabs)
            {
                var path = AssetDatabase.GUIDToAssetPath(prefab);
                var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var networkObject = gameObject.GetComponent<NetworkObject>();
                if (networkObject != null)
                {
                    if (networkObject.PrefabIdHash.ToString() == m_GlobalObjectIdHashToFind.text)
                    {
                        Selection.activeObject = gameObject;
                        break;
                    }
                }
            }
        }
    }

    [MenuItem("Tools/Netcode/NetworkBehaviour Invocation Order")]
    public static void ShowNetcodeInvocationWindow()
    {
        var editorWindow = EditorWindow.GetWindow<NetcodeInvocationWindow>();
        editorWindow.titleContent = new GUIContent("Invocation Order");
    }

    public class NetcodeInvocationWindow : EditorWindow
    {
        [SerializeField]
        private TreeViewState m_TreeViewState;
        private NetworkObjectTreeView m_TreeView { get; set; }

        private void OnEnable()
        {
            if (m_TreeViewState == null)
            {
                m_TreeViewState = new TreeViewState();
            }
            m_TreeView = new NetworkObjectTreeView(m_TreeViewState);
        }

        private void OnSelectionChange()
        {
            m_TreeView?.UpdateSelected();
        }

        private void OnGUI()
        {
            m_TreeView?.UpdateSelected();
            m_TreeView?.OnGUI(new Rect(0, 0, position.width, position.height));
        }
    }

    public class NetworkObjectTreeView : TreeView
    {
        private GameObject m_CurrentSelection;
        private TreeViewItem m_Root;

        private List<NetworkObject> m_NetworkObjects;

        public NetworkObjectTreeView(TreeViewState treeViewState) : base(treeViewState)
        {
            UpdateSelected(true);
        }

        public void UpdateSelected(bool forceUpdate = false)
        {
            var selectedObject = Selection.activeObject;
            var gameObject = selectedObject as GameObject;
            if (gameObject != m_CurrentSelection || forceUpdate)
            {
                m_CurrentSelection = gameObject;
                Reload();
                ExpandAll();
                Repaint();
            }
        }

        protected override TreeViewItem BuildRoot()
        {
            m_Root = new TreeViewItem { id = 0, depth = -1, displayName = "root" };
            if (m_CurrentSelection == null)
            {
                m_Root.AddChild(new TreeViewItem { id = 1, depth = 0, displayName = "<Not GameObject>" });
                return m_Root;
            }

            m_NetworkObjects = m_CurrentSelection.GetComponentsInChildren<NetworkObject>().ToList();
            if (m_NetworkObjects.Count == 0)
            {
                m_Root.AddChild(new TreeViewItem { id = 1, depth = 0, displayName = "<No NetworkObjects>" });
                return m_Root;
            }

            var networkBehaviours = m_CurrentSelection.GetComponentsInChildren<NetworkBehaviour>();
            if (networkBehaviours.Length == 0)
            {
                m_Root.AddChild(new TreeViewItem { id = 1, depth = 0, displayName = "<No NetworkBehaviours>" });
                return m_Root;
            }
            while (m_NetworkObjects.Count > 0)
            {
                ProcessNetworkObject(m_NetworkObjects[0], 2, 0);
            }

            SetupDepthsFromParentsAndChildren(m_Root);
            return m_Root;
        }

        private int ProcessNetworkObject(NetworkObject networkObject, int currentCount, int currentOrder)
        {
            m_NetworkObjects.Remove(networkObject);
            var gameObject = networkObject.gameObject;
            var networkObjectItem = new NetcodeTreeViewItem<GameObject> { id = 1, depth = 0, InvocationOrder = 0, displayName = $"{gameObject.name} (NetworkObject)", NodeObject = gameObject };
            m_Root.AddChild(networkObjectItem);
            var retVal = AddNetworkBehaviours(networkObjectItem, currentCount, currentOrder);
            retVal = AddChildGameObjects(networkObjectItem, retVal.Item1, retVal.Item2);
            return retVal.Item1;
        }

        private (int, int) AddChildGameObjects(NetcodeTreeViewItem<GameObject> netcodeTreeViewItem, int currentCount, int currentOrder)
        {
            var current = netcodeTreeViewItem.NodeObject;
            var count = currentCount;
            var order = currentOrder;
            var depth = netcodeTreeViewItem.depth + 1;

            if (current.transform.childCount == 0)
            {
                return (count, order);
            }

            for (int i = 0; i < current.transform.childCount; ++i)
            {
                count++;
                var child = current.transform.GetChild(i);
                var childNetworkBehaviours = child.gameObject.GetComponents<NetworkBehaviour>();
                var childNetworkObject = child.GetComponent<NetworkObject>();
                if (childNetworkObject != null)
                {
                    var childrenNetworkBehaviours = child.gameObject.GetComponentsInChildren<NetworkBehaviour>();
                    if (childrenNetworkBehaviours.Length > 0)
                    {
                        count = ProcessNetworkObject(childNetworkObject, count, 0);
                    }
                    else
                    {
                        m_NetworkObjects.Remove(childNetworkObject);
                    }

                    continue;
                }
                if (childNetworkBehaviours.Length == 0)
                {
                    continue;
                }
                var item = new NetcodeTreeViewItem<GameObject> { id = count, depth = depth, InvocationOrder = order, displayName = $"{child.name}", NodeObject = child.gameObject };
                netcodeTreeViewItem.AddChild(item);
                var retVal = AddNetworkBehaviours(item, count, order);
                count = retVal.Item1;
                order = retVal.Item2;
                retVal = AddChildGameObjects(item, count, order);
                count = retVal.Item1;
                order = retVal.Item2;
            }
            return (count, order);
        }

        private (int, int) AddNetworkBehaviours(NetcodeTreeViewItem<GameObject> netcodeTreeViewItem, int currentCount, int currentOrder)
        {
            var networkBehaviours = netcodeTreeViewItem.NodeObject.GetComponents<NetworkBehaviour>();
            var depth = netcodeTreeViewItem.depth + 1;
            var count = currentCount;
            var order = currentOrder;

            if (networkBehaviours.Length == 0)
            {
                return (count, order);
            }
            for (int i = 0; i < networkBehaviours.Length; i++)
            {
                var behaviour = networkBehaviours[i];
                count++;
                var item = new NetcodeTreeViewItem<NetworkBehaviour> { id = count, depth = depth, InvocationOrder = order, displayName = $"[{order}]-{behaviour.GetType().Name}", NodeObject = behaviour };
                netcodeTreeViewItem.AddChild(item);
                order++;
            }
            return (count, order);
        }
    }

    public class NetcodeTreeViewItem<T> : TreeViewItem where T : Object
    {
        public T NodeObject { get; set; }
        public int InvocationOrder { get; set; }
    }
}
#endif
