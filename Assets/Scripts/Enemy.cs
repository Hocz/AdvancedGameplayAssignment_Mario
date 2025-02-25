using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 2.0f;

    [SerializeField] private Rigidbody _rigidbody;

    private bool _isRightDir;

    private void OnEnable()
    {
        _isRightDir = Random.value > 0.5f;
    }

    public void Move()
    {
        float dir;
        if (_isRightDir) dir = 1;
        else dir = -1;


        _rigidbody.velocity = new Vector3(dir * _movementSpeed, 0, 0);

    }
}
