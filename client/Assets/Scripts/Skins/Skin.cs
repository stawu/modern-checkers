using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skins
{
    [CreateAssetMenu(fileName = "New Skin", menuName = "Game skin")]
    public class Skin : ScriptableObject
    {
        public int id;
        public string skinName;
        public Sprite image;
        public GameObject pawnPrefab;
    }
}
