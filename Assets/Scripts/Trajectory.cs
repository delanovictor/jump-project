using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    public float step = 50;
    public List<Vector3> DrawPoints = new List<Vector3>();
    // float jumpAngle;
    // float jumpDistance;
    // Vector2 initialVelocity;

    float timeToJumpPeak;
    float jumpGravity;
    float standartGravity;
    // Start is called before the first frame update

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PhysicsData(float _jumpGravity, float _standartGravity, float _timeToJumpPeak){
        jumpGravity = _jumpGravity;
        standartGravity = _standartGravity;
        timeToJumpPeak = _timeToJumpPeak; 
    }

    public void Draw(float jumpAngle, float jumpDistance){
        DrawPoints.Clear();

        Vector3 drawPoint = transform.position;
        Vector3 previousDrawPoint = transform.position;
        Vector2 initialVelocity = new Vector2();

        float startTime = 0;;
        float pathDistance = 0;
        float simulationTime = 0;
        int i = 0;

        initialVelocity.y = (Mathf.Cos(jumpAngle * Mathf.Deg2Rad) * jumpDistance)/timeToJumpPeak;
        initialVelocity.x = (Mathf.Sin(jumpAngle * Mathf.Deg2Rad) * jumpDistance)/timeToJumpPeak;


        while(Vector3.Distance(transform.position,drawPoint) < jumpDistance){
			simulationTime = i / step;
            Vector3 displacement = new Vector3();

            displacement.x = initialVelocity.x * simulationTime;
            displacement.y = initialVelocity.y * simulationTime + (jumpGravity * simulationTime * simulationTime / 2f);
			drawPoint = transform.position + displacement;
			// Debug.DrawLine (previousDrawPoint, drawPoint, Color.green);
            DrawPoints.Add(drawPoint);
            pathDistance += Vector3.Distance(previousDrawPoint,drawPoint);
            previousDrawPoint = drawPoint;  
            i++;
		}

        // float fy = (initialVelocity.y * simulationTime + (jumpGravity * simulationTime * simulationTime / 2f));
        // float gy = (initialVelocity.y * simulationTime + (standartGravity * simulationTime * simulationTime / 2f));
        // float initialHeight = Mathf.Abs(fy - gy);

        // startTime = simulationTime;
        // simulationTime = 0;

        // for (i = 1; i <= step; i++) {
		// 	simulationTime = i / step;
        //     simulationTime += startTime;
        //     Vector3 displacement = new Vector3();

        //     displacement.x = initialVelocity.x * simulationTime;
        //     displacement.y = initialVelocity.y * simulationTime + (standartGravity * simulationTime * simulationTime / 2f) + initialHeight;
		// 	drawPoint = transform.position + displacement;
        //     DrawPoints.Add(drawPoint);
		// 	// Debug.DrawLine (previousDrawPoint, drawPoint, Color.green);
		// 	previousDrawPoint = drawPoint;
	
    	// }
    }

    private void OnDrawGizmos() {
        foreach(Vector3 point in DrawPoints){
            Gizmos.DrawSphere(point, 0.03f);
        }
    
    }
}
