using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
namespace ziv
{
    public class Player : MonoBehaviourPunCallbacks
    {

        //variables
        public float speed;

        public float speedModifier;
        public float jumpForce;
        public Camera normalcam;
        public GameObject cameraParent;
        public LayerMask ground;
        public Transform groundDetector;
        public Transform weaponParent;
        private Vector3 weaponParentOrigion;
        private float baseFOV;
        private float sprintFovModifier = 0.001f;
        private Rigidbody rig;
        private float movementcounter;
        private float idlecounter;
        public float max_health;
        private float current_health;
        private Manager manager;
        private Transform ui_healthbar;
        public TextMesh displaynamemodel;
        [Header("UI")]
        private Text timesDiedtxt;
        private Text grouptxt;

        //Methods of unity
        private void Start()
        {


            displaynamemodel.text = "Enemy";
            manager = GameObject.Find("Manager").GetComponent<Manager>();
            current_health = max_health;
            cameraParent.SetActive(photonView.IsMine);
            if (!photonView.IsMine)
            {
                gameObject.layer = 17;
            }
            rig = GetComponent<Rigidbody>();
            if (Camera.main)
            {
                Camera.main.gameObject.SetActive(false);
            }
            baseFOV = normalcam.fieldOfView;
            weaponParentOrigion = weaponParent.localPosition;

            //checks what group you are and gives you atributes based on it
            if (photonView.IsMine)
            {
                displaynamemodel.text = PlayFabAuth.returndisplayname();
                ui_healthbar = GameObject.Find("HUD/Health/Bar").transform;
                timesDiedtxt = GameObject.Find("HUD/TimesDiedText").GetComponent<Text>();
                grouptxt = GameObject.Find("HUD/GroupNameText").GetComponent<Text>();
                if (PlayFabAuth.groupname == "pacifist")
                {
                    speed = speed * 4;

                    
                }
                else if (PlayFabAuth.groupname == "gangmember")
                {
                    speed = speed * 2;
                    jumpForce = jumpForce * 5;

                }
                else if (PlayFabAuth.groupname == "robot")
                {
                    speed = speed /2 ;
                    jumpForce = jumpForce /2;
                    max_health = 500;

                }
                else if (PlayFabAuth.groupname == "admin")
                {
                    speed = speed * 5;
                    jumpForce = jumpForce * 2;
                    max_health = 9000;
 
                }

                Refresh_HealthBar();
            }
        }

        //update function every frame
        public void Update()
        {

            if (!photonView.IsMine)
            {
                return;
            }

            timesDiedtxt.text = "Time Died:" + PlayFabAuth.TimesDied.ToString();
            grouptxt.text = "group is:" + PlayFabAuth.groupname;
            float t_hmove = Input.GetAxisRaw("Horizontal");
            float t_vmove = Input.GetAxisRaw("Vertical");
            bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool jump = Input.GetKey(KeyCode.Space);

            //States Of Movement
            bool isgrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);
            bool isJumping = jump && isgrounded;
            bool isSprinting = sprint && (t_vmove > 0) && !isJumping;

            //Jumping
            if (isJumping)
            {
                rig.AddForce(Vector3.up * jumpForce);
            }

            //headbob

            if(t_hmove==0 && t_vmove == 0)
            {
                HeadBob(idlecounter, 0.025f, 0.025f);
                idlecounter += Time.deltaTime*2f;
            }
            else
            {
                HeadBob(movementcounter, 0.015f, 0.015f);
                movementcounter += Time.deltaTime*6f;
            }
        }
        private void FixedUpdate()
        {

            if (!photonView.IsMine)
            {
                return;
            }
            //Controls
            float t_hmove = Input.GetAxisRaw("Horizontal");
            float t_vmove = Input.GetAxisRaw("Vertical");
            bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool jump = Input.GetKey(KeyCode.Space);


            //States Of Movement
            bool isgrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground );
            bool isJumping = jump && isgrounded;
            bool isSprinting = sprint && (t_vmove > 0) && !isJumping;


            //Movement
            Vector3 t_direction = new Vector3(t_hmove, 0, t_vmove);
            t_direction.Normalize();

            float t_adjustedSpeed = speed;
            if (isSprinting)
            {
                t_adjustedSpeed *= speedModifier;
            }

            Vector3 t_targetVelocity = transform.TransformDirection(t_direction) * t_adjustedSpeed * Time.deltaTime;
            t_targetVelocity.y = rig.velocity.y;
            rig.velocity = t_targetVelocity;


            //FOV CHANGE
            if (isSprinting)
            {
                t_adjustedSpeed *= speedModifier;
                normalcam.fieldOfView = Mathf.Lerp(normalcam.fieldOfView, baseFOV * speedModifier, Time.deltaTime * 2f);
            }
            else
            {
                normalcam.fieldOfView = Mathf.Lerp(normalcam.fieldOfView, baseFOV, Time.deltaTime * 2f);
            }

        }
        void HeadBob(float p_z, float p_x_intensity, float p_y_intensity)
        {
            weaponParent.localPosition = weaponParentOrigion + new Vector3(Mathf.Cos(p_z) * p_y_intensity, Mathf.Sin(p_z*2 ) * p_y_intensity, 0);
        }

        void Refresh_HealthBar()
        {
            float t_health_ratio = (float)current_health / (float)max_health;
            ui_healthbar.localScale = new Vector3(t_health_ratio, 1, 1);
        }

        public void TakeDamage(int p_damage)
        {
            if (photonView.IsMine)
            {
                current_health -= p_damage;
                Refresh_HealthBar();
                Debug.Log(current_health);

                if (current_health <= 0)
                {
                    PhotonNetwork.Destroy(gameObject);
                    manager.Spawn();
                    PlayFabAuth.TimesDied += 1;
                    PlayFabAuth.UpdateTimesDied();
                }
            }
        }


    }
}
