using PRO;
using UnityEngine;

namespace PRO
{
    public class CameraControl : MonoBehaviour
    {
        public Camera Camera { get; private set; }
        public void Start()
        {
            Camera = GetComponent<Camera>();

        }

        public void Update()
        {
            float one = Pixel.Size;
            Vector3 v = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                v.y += one;
            }
            if (Input.GetKey(KeyCode.A))
            {
                v.x -= one;
            }
            if (Input.GetKey(KeyCode.S))
            {
                v.y -= one;
            }
            if (Input.GetKey(KeyCode.D))
            {
                v.x += one;
            }
            Camera.transform.position += v;
            if (Input.GetKey(KeyCode.Mouse4))
            {
                float axis = Input.GetAxis("Mouse ScrollWheel");
                if (axis > 0)
                    Camera.orthographicSize -= 0.2f;
                else if (axis < 0)
                    Camera.orthographicSize += 0.2f;

            }

        }
    }
}
