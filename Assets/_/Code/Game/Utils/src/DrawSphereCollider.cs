using UnityEngine;

namespace Utils.Runtime
{
    public class DrawSphereCollider : MonoBehaviour
    {
        #region Unity API

        private void Awake()
        {
            
        }

        private void OnDrawGizmos()
        {
            _colliderRadius = GetComponent<SphereCollider>().radius;
            Gizmos.color = Color.lightPink;
            Gizmos.DrawWireSphere(gameObject.transform.position, _colliderRadius);
        }

        #endregion
        
        
        #region Private and Protected

        private float _colliderRadius;

        #endregion
    }
}
