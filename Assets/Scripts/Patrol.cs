using UnityEditor;
using UnityEngine;

public class Patrol : MonoBehaviour {
    [SerializeField]
    Vector3[] wayPoints;

    [SerializeField]
    private float speed = 2f;

    private int _currentWaypointIndex = 0;

    private void Start() {
        if (wayPoints != null && wayPoints.Length > 0) {
            transform.position = wayPoints[0];
        }
    }

    private void Update() {
        if (wayPoints == null || wayPoints.Length == 0) return;

        // Mover hacia el waypoint actual
        Vector3 direction = (wayPoints[_currentWaypointIndex] - transform.position).normalized;
        transform.position += direction * (speed * Time.deltaTime);

        // Verificar si llegó al waypoint actual
        if (Vector3.Distance(transform.position, wayPoints[_currentWaypointIndex]) < 0.1f) {
            // Cambiar al siguiente waypoint (circular)
            _currentWaypointIndex = (_currentWaypointIndex + 1) % wayPoints.Length;
        }
    }

    private void OnDrawGizmos() {
        if (wayPoints != null && wayPoints.Length > 0) {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < wayPoints.Length; i++) {
                Gizmos.DrawSphere(wayPoints[i], 1);
                if (i == wayPoints.Length - 1) {
                    Gizmos.DrawLine(wayPoints[i], wayPoints[0]);
                } else {
                    Gizmos.DrawLine(wayPoints[i], wayPoints[i + 1]);
                }
            }
        }
    }
}
