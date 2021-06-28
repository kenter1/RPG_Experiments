using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using EasyBuildSystem.Features.Scripts.Editor.Inspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


namespace EasyBuildSystem.Addons.CircularMenu.Scripts.Editor
{
    [CustomEditor(typeof(CircularMenu))]
    public class CircularMenuInspector : UnityEditor.Editor
    {
        #region Fields

        private CircularMenu Target;

        private string CategoryName;

        private bool GeneralFoldout;
        private bool UIFoldout;
        private bool InputsFoldout;
        private bool CategoriesFoldout;
        private bool AnimatorFoldout;

        #endregion Fields

        #region Methods

        private void OnEnable()
        {
            Target = (CircularMenu)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region Circular Menu General

            InspectorStyles.DrawSectionLabel("Circular Menu - General");

            GeneralFoldout = EditorGUILayout.Foldout(GeneralFoldout, "General Settings", true);

            if (GeneralFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("DisableComponentsWhenShown"), new GUIContent("Disable Components When Shown :", ""), true);
            }

            #endregion Circular Menu General

            #region Circular Menu UI

            InspectorStyles.DrawSectionLabel("Circular Menu - UI");

            UIFoldout = EditorGUILayout.Foldout(UIFoldout, "UI Settings", true);

            if (UIFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Selection"), new GUIContent("Selection :", ""));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("SelectionIcon"), new GUIContent("Selection Icon :", ""));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SelectionIconPreserveAspect"), new GUIContent("Selection Icon Preserve Aspect :", ""));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("SelectionText"), new GUIContent("Selection Name :", ""));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SelectionDescription"), new GUIContent("Selection Description :", ""));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("ButtonNormalColor"), new GUIContent("Selection Icon Normal Color :", ""));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ButtonHoverColor"), new GUIContent("Selection Icon Hover Color :", ""));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ButtonPressedColor"), new GUIContent("Selection Icon Pressed Color :", ""));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("CircularButton"), new GUIContent("Button Prefab :", ""));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ButtonImagePreserveAspect"), new GUIContent("Button Image Preserve Aspect :", ""));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ButtonSpacing"), new GUIContent("Button Spacing :", ""));
            }

            #endregion Circular Menu UI

            #region Circular Menu Input

            InspectorStyles.DrawSectionLabel("Circular Menu - Inputs");

            InputsFoldout = EditorGUILayout.Foldout(InputsFoldout, "Inputs Settings", true);

            if (InputsFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Controller"), new GUIContent("Input Controller Type :", ""));
            }

            #endregion Circular Menu Input

            #region Circular Menu Categories

            InspectorStyles.DrawSectionLabel("Circular Menu - Categories");

            CategoriesFoldout = EditorGUILayout.Foldout(CategoriesFoldout, "Categories Settings", true);

            if (CategoriesFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("DefaultCategoryIndex"), new GUIContent("Default Category Index :", ""));

                GUI.color = Color.black / 4f;
                GUILayout.BeginVertical("helpBox");
                GUI.color = Color.white;

                for (int i = 0; i < Target.Categories.Count; i++)
                {
                    GUI.color = Color.black / 4f;
                    GUILayout.BeginVertical("helpBox");
                    GUI.color = Color.white;

                    Target.Categories[i].Name = EditorGUILayout.TextField("Category Name :", Target.Categories[i].Name);
                    Target.Categories[i].Content = (GameObject)EditorGUILayout.ObjectField("Category Content :", Target.Categories[i].Content, typeof(GameObject), true);

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(13);
                    SerializedProperty ButtonsProperty = serializedObject.FindProperty("Categories.Array.data[" + i + "].Buttons");

                    EditorGUILayout.PropertyField(ButtonsProperty, true);
                    GUILayout.EndHorizontal();

                    if (GUILayout.Button("Convert Current Piece Collection To Buttons"))
                    {
                        if (BuildManager.Instance != null)
                        {
                            if (BuildManager.Instance.Pieces != null)
                            {
                                for (int x = 0; x < BuildManager.Instance.Pieces.Count; x++)
                                {
                                    Target.Categories[i].Buttons.Add(new CircularButtonData()
                                    {
                                        Icon = BuildManager.Instance.Pieces[x].Icon,
                                        Order = x,
                                        Text = BuildManager.Instance.Pieces[x].Name,
                                        Description = BuildManager.Instance.Pieces[x].Description,
                                        Action = new UnityEvent()
                                    });
                                }

                                EditorUtility.SetDirty(target);
                            }
                        }
                    }

                    if (GUILayout.Button("Remove Category"))
                    {
                        if (Target.transform.Find(Target.Categories[i].Name) != null)
                            DestroyImmediate(Target.transform.Find(Target.Categories[i].Name).gameObject);

                        Target.Categories.RemoveAt(i);
                    }

                    GUILayout.EndVertical();
                }

                GUILayout.EndVertical();

                GUI.color = Color.black / 4f;
                GUILayout.BeginVertical("helpBox");
                GUI.color = Color.white;

                CategoryName = EditorGUILayout.TextField("Category Name :", CategoryName);

                if (GUILayout.Button("Add New Category"))
                {
                    GameObject NewContent = new GameObject(CategoryName);
                    NewContent.transform.SetParent(Target.transform, false);
                    NewContent.transform.localPosition = Vector3.zero;
                    Target.Categories.Add(new CircularMenu.UICustomCategory() { Name = CategoryName, Content = NewContent });
                    CategoryName = string.Empty;
                }

                GUILayout.EndVertical();
            }

            #endregion Circular Menu Categories

            #region Circular Menu Animator

            InspectorStyles.DrawSectionLabel("Circular Menu - Animator");

            AnimatorFoldout = EditorGUILayout.Foldout(AnimatorFoldout, "Animator Settings", true);

            if (AnimatorFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Animator"), new GUIContent("Circular Animator :", ""));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowStateName"), new GUIContent("Circular Show State Name :", ""));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("HideStateName"), new GUIContent("Circular Hide State Name :", ""));
            }

            #endregion Circular Menu Animator

            serializedObject.ApplyModifiedProperties();
        }

        #endregion Methods
    }
}
