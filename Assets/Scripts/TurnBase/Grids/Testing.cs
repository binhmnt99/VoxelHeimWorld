using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class Testing : MonoBehaviour
    {
        [SerializeField] private Unit unit;
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                MyClass myClass = new MyClass(6);
                myClass.Testing<string>("testing");
            }
        }
    }

    public class MyClass
    {
        private int i;

        public MyClass(int i)
        {
            this.i = i;
            Debug.Log(i);
        }

        public void Testing<T>(T t)
        {
            Debug.Log(t);
        }
    }
}
