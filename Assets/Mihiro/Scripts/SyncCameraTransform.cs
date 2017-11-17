using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Mihiro.Scripts {
    [RequireComponent(typeof(Camera))]
    public class SyncCameraTransform : MonoBehaviour {
        public Camera cam_tracking;
        public Transform origin;

        Camera cam_render;

        private void Awake() {
            cam_render = GetComponent<Camera>();
        }

        private void OnPreCull() {
            // origin -> tracking camera 변환 계산
            var origin_mat = origin.localToWorldMatrix;
            var origin_invMat = origin_mat.inverse;
            var cam_mat = cam_tracking.transform.localToWorldMatrix;
            var mat = origin_invMat * cam_mat;
            transform.FromMatrixToLocalTransform(mat);

            // https://answers.unity.com/questions/36446/disable-frustum-culling.html
            cam_render.cullingMatrix = Matrix4x4.Ortho(-99999, 99999, -99999, 99999, 0.001f, 99999) *
                            Matrix4x4.Translate(Vector3.forward * -99999 / 2f) *
                            cam_render.worldToCameraMatrix;
        }
    }
}