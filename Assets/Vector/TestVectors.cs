using System;
using UnityEngine;

namespace Vector
{
    [Serializable]
    public class VectorInfo
    {
        [NonSerialized]
        public string Name;
        
        public MyVector3 From = new MyVector3(0, 0, 0);
        public MyVector3 To = new MyVector3(0, 2, 3);
        public MyVector3 Vector;

        public bool Normalize;
    }

    public class TestVectors : MonoBehaviour
    {
        public float Thickness = 5f;
        public VectorInfo A;
        public VectorInfo B;
        public bool Sum;

        private void OnValidate()
        {
            A.Name = "A";
            B.Name = "B";
            DebugVector(A);
            DebugVector(B);
            
            if (Sum)
            {
                var sumVectors = A.Vector + B.Vector;
                Debug.Log("Sum A and B :" + sumVectors);
            }
        }

        public void DebugVector(VectorInfo vectorInfo)
        {
            Debug.Log($"\n  VECTOR {vectorInfo.Name}:");
            Debug.Log(("White :" + vectorInfo.Vector));
            Debug.Log("Magnitude :" + vectorInfo.Vector.Magnitude());
            if(vectorInfo.Normalize)
                Debug.Log("Normalize green :" + (vectorInfo.Vector).Normalize());
        }

        private void OnDrawGizmos()
        {
            A.Vector = A.To - A.From;
            B.Vector = B.To - B.From;
        }
    }
}
