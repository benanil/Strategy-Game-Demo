using UnityEngine;

namespace PanteonGames
{
    [RequireComponent(typeof(Camera))]
    public class CameraMovement : MonoBehaviour
    {
        static CameraMovement instance;
        public float speed = 4;
        public Vector2 MinPosition = new Vector2(-5, -5);
        public Vector2 MaxPosition = new Vector2(10, 10);

        Vector3 oldPosition;

        Camera _camera;

        public HoveredChecker productionMenu, InformationMenu;

        private void Start()
        {
            instance = this;
            _camera = GetComponent<Camera>();
        }

        public static bool IsUIHovering()
        {
            return instance.productionMenu.Hovered == true || instance.InformationMenu.Hovered == true;
        }

        private void Update()
        {
            Vector3 screenToWorldPoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            if (IsUIHovering() == false)
            {
                _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - Input.mouseScrollDelta.y, 1, 6);
            }

            // right click for drag camera
            if (Input.GetMouseButton(1))
            {
                Vector3 newPosition = transform.position - ((screenToWorldPoint - oldPosition) * speed * Time.deltaTime);
                // we don't want to go infinity with camera
                newPosition.x = Mathf.Clamp(newPosition.x, MinPosition.x, MaxPosition.x);
                newPosition.y = Mathf.Clamp(newPosition.y, MinPosition.y, MaxPosition.y);
                transform.position = newPosition;
            }
            
            oldPosition = screenToWorldPoint;
        }
    }
}

