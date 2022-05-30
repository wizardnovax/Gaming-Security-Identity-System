using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
namespace ziv
{
    public class Weapon : MonoBehaviourPunCallbacks
    {
        //variables
        public Guns[] loadout;
        public Transform weaponParent;
        private GameObject currentWeapon;
        private int currentIndex;
        public GameObject bulletholePrefab;
        public LayerMask canbeshot;
        private int x = 0;

        string groupname = PlayFabAuth.groupname;

        // MonoBehaviour function
        void Update()
        {
            //checks player controls for gun
            if (photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha1) && groupname!="pacafist") { photonView.RPC("Equip", RpcTarget.All, 0); x=0; }
            if (photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha2) && groupname!="pacafist" && groupname!="gangmember"){ photonView.RPC("Equip", RpcTarget.All, 1); x = 1; }
            if (photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha3) && groupname == "admin") { photonView.RPC("Equip", RpcTarget.All, 2); x = 2; }
            if (currentWeapon != null)
            {
                if (photonView.IsMine)
                {
                    Aim(Input.GetMouseButton(1));

                    if (Input.GetMouseButtonDown(0))
                    {
                        photonView.RPC("Shoot", RpcTarget.All,  x);
                    }
                }

            }
        }

        [PunRPC]
        void Equip(int p_ind)
        {
            if(currentWeapon != null)
            {
                Destroy(currentWeapon);
            }
            currentIndex = p_ind;
            GameObject t_newequipment = Instantiate(loadout[p_ind].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
            t_newequipment.transform.localPosition = Vector3.zero;
            t_newequipment.transform.localEulerAngles = Vector3.zero;
            currentWeapon = t_newequipment;
        }

        void Aim(bool IsAming)
        {
            Transform t_anchor = currentWeapon.transform.Find("Anchor");    
            Transform t_states_ads = currentWeapon.transform.Find("States/ADS");
            Transform t_states_hip = currentWeapon.transform.Find("States/Hip");
            if (IsAming)
            {
                t_anchor.position = Vector3.Lerp(t_states_ads.position,t_anchor.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
            }
            else
            {
                t_anchor.position = Vector3.Lerp(t_states_hip.position, t_anchor.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
            }
        }

        [PunRPC]
        void Shoot(int p_ind) 
        {
            Transform t_spawn = transform.Find("Camera");
            currentIndex = p_ind;
            RaycastHit t_hit = new RaycastHit();
            if(Physics.Raycast(t_spawn.position, t_spawn.forward, out t_hit, 1000f, canbeshot))
            {
                GameObject t_newHole = Instantiate(bulletholePrefab, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity) as GameObject;
                t_newHole.transform.LookAt((t_hit.point + t_hit.normal));
                Destroy(t_newHole, 5f);

                if (photonView.IsMine)
                {
                    if (t_hit.collider.gameObject.layer == 17)
                    {
                        Debug.Log(loadout[currentIndex].name);
                        t_hit.collider.gameObject.GetPhotonView().RPC("TakeDamagep", RpcTarget.All, loadout[currentIndex].damage);
                    }

                }
            }
        }

        [PunRPC]
        private void TakeDamagep(int p_damage)
        {
            GetComponent<Player>().TakeDamage(p_damage);
        }


    }
}