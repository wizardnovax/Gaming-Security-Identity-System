using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
namespace ziv
{
    public class Look : MonoBehaviourPunCallbacks
    {
        
        ///Variables
        public static bool CursorLocked = true;
        public Transform player;
        public Transform cams;
        public Transform weapon;
        public GameObject eyes;
        public float xSensitive;
        public float ySensitive;
        public float maxAngle;
        private Quaternion camCenter;
        
        //MonoBehavior Methods
        void Start()
        {
            //when script starts
            camCenter = cams.localRotation;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            eyes.SetActive(!photonView.IsMine);
        }

        void Update()
        {
            //every frame script runs 
            if (!photonView.IsMine)
            {
                return;
            }
            SetY();
            SetX();
            UpdateCursorLock();
        }
        
        //Other Methods
        void SetY()
        {
            float t_input = Input.GetAxis("Mouse Y") * ySensitive * Time.deltaTime;
            Quaternion t_adj = Quaternion.AngleAxis(t_input, -Vector3.right);
            Quaternion t_delta = cams.localRotation * t_adj;
            if(Quaternion.Angle(camCenter, t_delta) < maxAngle)
            {
                cams.localRotation = t_delta;
                weapon.localRotation = t_delta;
            }

            weapon.rotation = cams.rotation;
        }

        void SetX()
        {
            float t_input = Input.GetAxis("Mouse X") * xSensitive * Time.deltaTime;
            Quaternion t_adj = Quaternion.AngleAxis(t_input, Vector3.up);
            Quaternion t_delta = player.localRotation * t_adj;
            player.localRotation = t_delta;

        }
        void UpdateCursorLock()
        {
            if (CursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CursorLocked = false;
                }
            }

            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CursorLocked = true;
                }
            }
        }
    }
}
