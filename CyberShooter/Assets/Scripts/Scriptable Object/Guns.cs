using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ziv
{
    [CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
    public class Guns : ScriptableObject
    {
        public string name;
        public int damage;
        public float aimSpeed;
        public GameObject prefab;
    }
}

