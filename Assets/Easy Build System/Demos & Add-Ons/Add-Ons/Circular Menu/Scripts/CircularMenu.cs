using EasyBuildSystem.Features.Scripts.Core.Base.Builder;
using EasyBuildSystem.Features.Scripts.Core.Base.Builder.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.XR;
#endif
using UnityEngine.UI;

namespace EasyBuildSystem.Addons.CircularMenu.Scripts
{
    public class CircularMenu : MonoBehaviour
    {
        #region Fields

        public static CircularMenu Instance;

        [Serializable]
        public class UICustomCategory
        {
            public string Name;
            public GameObject Content;
            public List<CircularButtonData> Buttons = new List<CircularButtonData>();
            public List<CircularButton> InstancedButtons = new List<CircularButton>();
        }

        public enum ControllerType
        {
            KeyboardAndMouse,
            Android,
            Gamepad,
            XR
        }

        public ControllerType Controller = ControllerType.KeyboardAndMouse;

        public int DefaultCategoryIndex;

        public List<UICustomCategory> Categories = new List<UICustomCategory>();

        public MonoBehaviour[] DisableComponentsWhenShown;

        public Image Selection;
        public Image SelectionIcon;
        public bool SelectionIconPreserveAspect = true;
        public Text SelectionText;
        public Text SelectionDescription;
        public Color ButtonNormalColor;
        public Color ButtonHoverColor;
        public Color ButtonPressedColor;

        public CircularButton CircularButton;
        public float ButtonSpacing = 160f;
        public bool ButtonImagePreserveAspect = true;

        public Animator Animator;
        public string ShowStateName;
        public string HideStateName;

        [HideInInspector]
        public GameObject SelectedButton;

        [HideInInspector]
        public UICustomCategory CurrentCategory;

        public bool IsActive = false;

        private readonly List<float> ButtonsRotation = new List<float>();
        private int Elements;
        private float Fill;
        private float CurrentRotation;

        #endregion

        #region Methods

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            for (int i = 0; i < Categories.Count; i++)
            {
                if (Categories[i].Content != null)
                {
                    Categories[i].Buttons = Categories[i].Buttons.OrderBy(o => o.Order).ToList();

                    for (int x = 0; x < Categories[i].Buttons.Count; x++)
                    {
                        CircularButton Button = Instantiate(CircularButton, Categories[i].Content.transform);

                        if (Button.Icon != null)
                            Button.Icon.preserveAspect = ButtonImagePreserveAspect;

                        Button.Init(Categories[i].Buttons[x].Text, Categories[i].Buttons[x].Description, Categories[i].Buttons[x].Icon, Categories[i].Buttons[x].Action);
                        Categories[i].InstancedButtons.Add(Button);
                    }

                    Categories[i].InstancedButtons = Categories[i].Content.GetComponentsInChildren<CircularButton>(true).ToList();
                }
            }

            ChangeCategory(Categories[0].Name);

            #if ENABLE_INPUT_SYSTEM
            BuilderInput.Instance.userInteraface.CircularMenu.performed += ctx => { Show(); };
            BuilderInput.Instance.userInteraface.CircularMenu.canceled += ctx => { Hide(); };
            #endif
        }

        private void Update()
        {
            if (!Application.isPlaying)
                return;

#if ENABLE_INPUT_SYSTEM
            if (BuilderInput.Instance.userInteraface.Cancel.triggered)
                BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
#else

            if (Input.GetKeyDown(KeyCode.Tab))
            { 
                if (!IsActive) 
                    Show();
                else 
                    Hide();
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
                BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
#endif

            if (!IsActive)
                return;

            Selection.fillAmount = Mathf.Lerp(Selection.fillAmount, Fill, .2f);

            #if ENABLE_INPUT_SYSTEM
            if (Controller == ControllerType.Gamepad)
            {
                Vector2 InputAxis = BuilderInput.Instance.userInteraface.Select.ReadValue<Vector2>();

                if (Mathf.Abs(InputAxis.x) > 0.25f || Mathf.Abs(InputAxis.y) > 0.25f)
                    CurrentRotation = Mathf.Atan2(InputAxis.x, InputAxis.y) * 57.29578f;
            }
            else if (Controller == ControllerType.XR)
            {
                UnityEngine.XR.InputDevice Device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
                Device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 InputAxis);

                if (Mathf.Abs(InputAxis.x) > 0.25f || Mathf.Abs(InputAxis.y) > 0.25f)
                    CurrentRotation = Mathf.Atan2(InputAxis.x, InputAxis.y) * 57.29578f;
            }
            else if (Controller == ControllerType.KeyboardAndMouse || Controller == ControllerType.Android)
            {
                Vector3 BoundsScreen = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);
                Vector3 RelativeBounds = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0f) - BoundsScreen;
                CurrentRotation = Mathf.Atan2(RelativeBounds.x, RelativeBounds.y) * 57.29578f;
            }
            #else
            Vector3 BoundsScreen = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);
            Vector3 RelativeBounds = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f) - BoundsScreen;
            CurrentRotation = Mathf.Atan2(RelativeBounds.x, RelativeBounds.y) * 57.29578f;
            #endif

            if (CurrentRotation < 0f)
                CurrentRotation += 360f;

            _ = -(CurrentRotation - Selection.fillAmount * 360f / 2f);

            float Average = 9999;

            GameObject Nearest = null;

            for (int i = 0; i < Elements; i++)
            {
                GameObject InstancedButton = CurrentCategory.InstancedButtons[i].gameObject;
                InstancedButton.transform.localScale = Vector3.one;
                float Rotation = Convert.ToSingle(InstancedButton.name);

                if (Mathf.Abs(Rotation - CurrentRotation) < Average)
                {
                    Nearest = InstancedButton;
                    Average = Mathf.Abs(Rotation - CurrentRotation);
                }
            }

            SelectedButton = Nearest;
            float CursorRotation = -(Convert.ToSingle(SelectedButton.name) - Selection.fillAmount * 360f / 2f);
            Selection.transform.localRotation = Quaternion.Slerp(Selection.transform.localRotation, Quaternion.Euler(0, 0, CursorRotation), 15f * Time.deltaTime);

            for (int i = 0; i < Elements; i++)
            {
                CircularButton Button = CurrentCategory.InstancedButtons[i].GetComponent<CircularButton>();

                if (Button.gameObject != SelectedButton)
                    Button.Icon.color = Color.Lerp(Button.Icon.color, ButtonNormalColor, 15f * Time.deltaTime);
                else
                    Button.Icon.color = Color.Lerp(Button.Icon.color, ButtonHoverColor, 15f * Time.deltaTime);
            }

            SelectionIcon.sprite = SelectedButton.GetComponent<CircularButton>().Icon.sprite;
            SelectionIcon.preserveAspect = SelectionIconPreserveAspect;
            SelectionText.text = SelectedButton.GetComponent<CircularButton>().Text;
            SelectionDescription.text = SelectedButton.GetComponent<CircularButton>().Description;

#if ENABLE_INPUT_SYSTEM
            if (BuilderInput.Instance.userInteraface.Validate.triggered)
            {
                if (SelectedButton.GetComponent<CircularButton>().GetComponent<Animator>() != null)
                    SelectedButton.GetComponent<CircularButton>().GetComponent<Animator>().Play("Button Press");

                SelectedButton.GetComponent<CircularButton>().Action.Invoke();
            }
#else
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (SelectedButton.GetComponent<CircularButton>().GetComponent<Animator>() != null)
                    SelectedButton.GetComponent<CircularButton>().GetComponent<Animator>().Play("Button Press");

                SelectedButton.GetComponent<CircularButton>().Action.Invoke();
            }
#endif
        }

        private void RefreshButtons()
        {
            Elements = CurrentCategory.InstancedButtons.Count;

            if (Elements > 0)
            {
                Fill = 1f / (float)Elements;

                float FillRadius = Fill * 360f;
                float LastRotation = 0;

                for (int i = 0; i < Elements; i++)
                {
                    GameObject Temp = CurrentCategory.InstancedButtons[i].gameObject;

                    float Rotate = LastRotation + FillRadius / 2;
                    LastRotation = Rotate + FillRadius / 2;

                    Temp.transform.localPosition = new Vector2(ButtonSpacing * Mathf.Cos((Rotate - 90) * Mathf.Deg2Rad), -ButtonSpacing * Mathf.Sin((Rotate - 90) * Mathf.Deg2Rad));
                    Temp.transform.localScale = Vector3.one;

                    if (Rotate > 360)
                        Rotate -= 360;

                    Temp.name = Rotate.ToString();

                    ButtonsRotation.Add(Rotate);
                }
            }
        }

        /// <summary>
        /// This method allows to change of category by name.
        /// </summary>
        public void ChangeCategory(string name)
        {
            DefaultCategoryIndex = Categories.ToList().FindIndex(entry => entry.Content.name == name);

            if (DefaultCategoryIndex == -1)
                return;

            CurrentCategory = Categories[DefaultCategoryIndex];

            for (int i = 0; i < Categories.Count; i++)
            {
                if (Categories[i].Content != null)
                {
                    if (i != DefaultCategoryIndex)
                        Categories[i].Content.SetActive(false);
                    else
                        Categories[i].Content.SetActive(true);
                }
            }

            RefreshButtons();
        }

        /// <summary>
        /// This method allows to change mode.
        /// </summary>
        public void ChangeMode(string modeName)
        {
            Hide();

            BuilderBehaviour.Instance.ChangeMode(modeName);
        }

        /// <summary>
        /// This method allows to pass in placement mode with a piece name.
        /// </summary>
        public void ChangePiece(string name)
        {
            Hide();

            BuilderBehaviour.Instance.ChangeMode(BuildMode.None);

            BuilderBehaviour.Instance.SelectPrefab(BuildManager.Instance.GetPieceByName(name));
            BuilderBehaviour.Instance.ChangeMode(BuildMode.Placement);
        }

        /// <summary>
        /// This method allows to show the circular menu.
        /// </summary>
        protected void Show()
        {
            Animator.CrossFade(ShowStateName, 0.1f);

            for (int i = 0; i < DisableComponentsWhenShown.Length; i++)
                DisableComponentsWhenShown[i].enabled = false;

            if (Controller != ControllerType.XR)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            IsActive = true;
        }

        /// <summary>
        /// This method allows to close the circular menu.
        /// </summary>
        protected void Hide()
        {
            Animator.CrossFade(HideStateName, 0.1f);

            for (int i = 0; i < DisableComponentsWhenShown.Length; i++)
                DisableComponentsWhenShown[i].enabled = true;

            if (Controller != ControllerType.XR)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            IsActive = false;
        }

#endregion
    }
}