using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
#if UNITY_WSA
using UnityEngine.XR.WSA;
#endif

namespace Assets.Mihiro.Scripts {
    class OriginModifier : MonoBehaviour, IInputClickHandler, INavigationHandler {
        PointerEventData lookData;
        EventSystem eventSystem;
        List<RaycastResult> raycastResultCache = new List<RaycastResult>();

        void Start() {
            eventSystem = EventSystem.current;
            lookData = new PointerEventData(eventSystem);

            InputManager.Instance.AddGlobalListener(gameObject);
        }

        void OnDestroy() {
            if (InputManager.Instance != null) {
                InputManager.Instance.RemoveGlobalListener(gameObject);
            }
        }

        void RefreshLookData() {
            var screenwidth = XRSettings.eyeTextureWidth;
            var screenheight = XRSettings.eyeTextureHeight;

            // 유니티 에디터에서도 돌아가게 하려고 예외처리
            if (screenwidth == 0) {
                screenwidth = Screen.width;
            }
            if (screenheight == 0) {
                screenheight = Screen.height;
            }

            // 화면의 중심으로 고정
            // 홀로렌즈니까 문제 없다
            Vector2 lookPosition;
            lookPosition.x = screenwidth * 0.5f;
            lookPosition.y = screenheight * 0.5f;

            lookData.Reset();
            lookData.delta = Vector2.zero;
            lookData.position = lookPosition;
            lookData.scrollDelta = Vector2.zero;
        }

        List<RaycastResult> RaycastUI() {
            eventSystem.RaycastAll(lookData, raycastResultCache);
            return raycastResultCache;
        }

        bool IsOnUILayer() {
            RefreshLookData();
            var result = RaycastUI();
            if (result.Count > 0) {
                return true;
            }
            return false;
        }

        public void OnInputClicked(InputClickedEventData eventData) {
            if (IsOnUILayer()) {
                return;
            }

            var origin = CustomOrigin.Instance;

            var layerMask = SpatialMappingManager.Instance.LayerMask;
            RaycastHit hit;
            var mgr = GazeManager.Instance;
            if (Physics.Raycast(mgr.GazeOrigin, mgr.GazeNormal, out hit, mgr.MaxGazeCollisionDistance, layerMask)) {
                origin.SetActive(true);

                origin.ReleaseAnchor();
                origin.transform.position = hit.point;
                origin.RetainAnchor();

            } else {
                origin.SetActive(false);
            }
        }

        public void OnNavigationCanceled(NavigationEventData eventData) {
            CustomOrigin.Instance.RetainAnchor();
        }

        public void OnNavigationCompleted(NavigationEventData eventData) {
            CustomOrigin.Instance.RetainAnchor();
        }

        public void OnNavigationStarted(NavigationEventData eventData) {
            CustomOrigin.Instance.ReleaseAnchor();
        }

        public void OnNavigationUpdated(NavigationEventData eventData) {
            var origin = CustomOrigin.Instance;

            var delta = eventData.CumulativeDelta.x;
            var speed = 5.0f;
            var q = Quaternion.AngleAxis(delta * speed, Vector3.up);
            var currRot = origin.transform.rotation;
            var nextRot = currRot * q;
            origin.transform.rotation = nextRot;
        }
    }
}
