using UnityEngine;

namespace NullCheckerRuntime
{
    public class EnergyManager : MonoBehaviour
    {
        #region Exposed
        [SerializeField]
        private bool _isEnergy;

        [SerializeField]
        private float _energyAmount;

        [SerializeField]
        private GameObject _objectDrawer;

        [SerializeField]
        private Transform _transform;

        [SerializeField]
        private EnergyManager _energyManager;

        [SerializeField]
        private BoxCollider _boxCollider;

        [SerializeField]
        private Material _material;

        [SerializeField]
        private Mesh _mesh;

        [SerializeField]
        private AudioClip _audioClip;

        #endregion
    }
}