using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ziv
{
    public class Manager : MonoBehaviour
    {
        //variables
        public string player_prefab;
        public Transform[] Spawn_points;

        //Monobehaviour Methods
        private void Start()
        {
            Spawn();
        }
        public void Spawn()
        {
            Transform Spawn_point = Spawn_points[Random.Range(0, Spawn_points.Length-1)];
            PhotonNetwork.Instantiate(player_prefab, Spawn_point.position, Spawn_point.rotation);
        }
    }


}
