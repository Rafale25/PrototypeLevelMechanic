using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// enum SDFColliderType {
//     cube,
//     sphere,
// };

public class Player : MonoBehaviour
{
    [SerializeField] Transform _playerCamera;
    Transform _player;

    void Start()
    {
        _player = this.GetComponent<Transform>();

    }

    void Update()
    {

    }


    void OnDrawGizmos()
    {
        Vector3 position = _playerCamera.position + _playerCamera.forward * 30f;
        Quaternion rotation = _playerCamera.rotation;

        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.Translate(position) * Matrix4x4.Rotate(rotation);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(10.0f, 10.0f, 60f));
    }
}
