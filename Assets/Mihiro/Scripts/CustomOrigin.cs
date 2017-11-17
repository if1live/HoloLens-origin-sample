using UnityEngine;
#if UNITY_WSA
using UnityEngine.XR.WSA;
#endif


namespace Assets.Mihiro.Scripts {
    public class CustomOrigin : MonoBehaviour {
        public static CustomOrigin Instance { get; internal set; }

        public void SetActive(bool b) {
            gameObject.SetActive(b);
        }

        public void ReleaseAnchor() {
#if UNITY_WSA
            var prev = GetComponent<WorldAnchor>();
            if (prev != null) {
                DestroyImmediate(prev);
            }
#endif
        }

        public void RetainAnchor() {
#if UNITY_WSA
            gameObject.AddComponent<WorldAnchor>();
#endif
        }

        void Awake() {
            Debug.Assert(Instance == null);
            Instance = this;
        }

        void OnDestroy() {
            Debug.Assert(Instance == this);
            Instance = null;
        }
    }
}
