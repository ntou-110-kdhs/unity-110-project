using UnityEngine;

namespace DitzelGames.FastIK
{
    public class FastIKLook : MonoBehaviour
    {
        /// <summary>
        /// Look at target
        /// </summary>
        public Transform Target;

        /// <summary>
        /// Initial direction
        /// </summary>
        protected Vector3 StartDirection;

        /// <summary>
        /// Initial Rotation
        /// </summary>
        protected Quaternion StartRotation;

        void Awake()
        {
            if (Target == null)
                return;

            StartDirection = Target.position - transform.position;
            StartRotation = transform.rotation;
           // Debug.Log("Target.position:" + Target.position);
            //Debug.Log("transform.position:" + transform.position);
            //Debug.Log("Target.position - transform.position" + (Target.position - transform.position));
        }

        void LateUpdate()
        {
            if (Target == null)
                return;

            //Debug.Log((Quaternion.FromToRotation(StartDirection, Target.position - transform.position) * StartRotation).eulerAngles);



            //transform.rotation = Quaternion.FromToRotation(StartDirection, Target.position - transform.position) * StartRotation;
            StartDirection = Target.position - transform.position;
            transform.rotation = Quaternion.LookRotation(StartDirection, Vector3.up);// * StartRotation;
        }
    }
}
