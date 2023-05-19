using UnityEngine;

public class JumpExecutor : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] Transform _camera;
    [SerializeField] Transform _jumpTarget;
    public float jumpValue = 5;
    private Vector3 _moveDirection = new Vector3(0, 0, 0);

    public void Jump()
    {
        _moveDirection = _jumpTarget.position - _camera.position;
        _moveDirection.y = jumpValue;
    }

    void Update(){
        if(_moveDirection.y > 0)
        {
            _moveDirection.y += Physics.gravity.y * Time.deltaTime;
            if(characterController != null)
            {
                characterController.Move(_moveDirection * Time.deltaTime);
            }
        }
        else
        {
            _moveDirection.y = 0;
        }
    }
}