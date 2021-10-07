using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace NullChecker.Runtime
{
    public class EnergyManager : MonoBehaviour
    {
        #region Exposed

        [Header("Values")]        
        [SerializeField]
        private bool _isEnergy;

        [SerializeField]
        private float _energyAmount;


        [Header("Objects")]
        [SerializeField]
        private GameObject _gameObject;

        [SerializeField]
        private Transform _transform;

        [SerializeField]
        private EnergyManager _energyManager;

        [SerializeField]
        private BoxCollider _boxCollider;
        
        [SerializeField]
        private Image _image;


        [Header("Assets")]
        [SerializeField]
        private Material _material;

        [SerializeField]
        private Mesh _mesh;

        [SerializeField]
        private AudioClip _audioClip;


        [Header("Complexes")]
        [SerializeField]
        private SampleStruct _struct;
        
        [SerializeField]
        private SampleClass _class;


        [Header("Collections")]
        [SerializeField]
        private Transform[] _array;
        
        [SerializeField]
        private List<GameObject> _list;

        #endregion


        [System.Serializable]
        public struct SampleStruct
        {
            public Transform structTransform;
        }

        [System.Serializable]
        public class SampleClass
        {
            public GameObject[] classArray;
        }
    }
}