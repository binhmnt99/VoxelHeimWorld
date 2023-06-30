using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class MapData : MonoBehaviour
    {
        public static MapData Instance { get; private set; }
        [SerializeField] private List<GameObject> mapList;
        private List<Vector3> mapPositionList;
        private float minX, minZ;
        private float maxX, maxZ;
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            MapData.Instance = this;
            mapPositionList = new List<Vector3>();
            //MapDataHandle();
        }





        //Call MapDataHandle first at GridSystemHex
        public void MapDataHandle()
        {

            Debug.Log(mapPositionList.Count);
        }

        public int GetAbsoluteValue(float number)
        {
            if (number < 0)
            {
                return Mathf.RoundToInt(-number);
            }
            else
            {
                return Mathf.RoundToInt(number);
            }
        }

        public int GetLengthOfX()
        {
            return GetAbsoluteValue(minX) + GetAbsoluteValue(maxX);
        }

        public int GetLengthOfY()
        {
            return GetAbsoluteValue(minZ) + GetAbsoluteValue(maxZ);
        }

        public List<Vector3> GetMapPositionList()
        {
            return mapPositionList;
        }
    }
}

