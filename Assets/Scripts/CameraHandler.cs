using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hyeur
{
    public class CameraHandler : MonoBehaviour
    {
        public Transform targetTransform;
        public Transform cameraTransform;
        public Transform cameraPivotTransform;
        private Transform myTransform;
        private Vector3 cameraTransformPosition;

        private Vector3 cameraFollowVelocity = Vector3.zero;
        private LayerMask ignoreLayers;

        public static CameraHandler inst;

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;
        public float defaultPosition;
        public float lookAngle;
        public float pivotAngle;
        public float minimumPivot = -35;
        public float maximumPivot = 35;

        public float cameraSphereRadius = 0.2f;

        public float cameraCollisionOffset = 0.2f;
        public float minimumCollisionOffset = 0.2f;

        private float targetPosition;


        public void Awake(){
            inst = this;
            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        }

        public void FollowTarget(float dt){ 
            Vector3 targetPosition = Vector3.SmoothDamp
            (myTransform.position, targetTransform.position, ref cameraFollowVelocity, dt / followSpeed);
            myTransform.position= targetPosition;

            HandleCameraCollisions(dt);
        }

        public void HandleCameraRotation(float dt, float mouseXInput, float mouseYInput){
            float targetLookAngle = Mathf.Lerp(lookAngle, lookAngle + (mouseXInput * lookSpeed) / dt,0.1f);
            lookAngle =  targetLookAngle;
            float targetPivotAngle = Mathf.Lerp(pivotAngle, pivotAngle - (mouseYInput *  pivotSpeed) / dt,0.1f);
            pivotAngle = targetPivotAngle;
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot,maximumPivot);

            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            myTransform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.x = pivotAngle;

            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targetRotation;

        }

        private void HandleCameraCollisions(float dt) {
            targetPosition = defaultPosition;
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast(cameraPivotTransform.position,cameraSphereRadius, direction,out hit, Mathf.Abs(targetPosition), ignoreLayers)) {
                float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPosition = -(dis - cameraCollisionOffset);
            }

            if (Mathf.Abs(targetPosition) < minimumCollisionOffset) {
                targetPosition = minimumCollisionOffset;
            }

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z,targetPosition, dt / 0.2f);
            cameraTransform.localPosition = cameraTransformPosition;
        }
    }

}
