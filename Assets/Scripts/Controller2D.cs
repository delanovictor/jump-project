using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour {

	const float skinWidth = .015f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;
	public float maxClimbingAngle = 80;
	public LayerMask collisionMask;
	float horizontalRaySpacing;
	float verticalRaySpacing;
	public CollisionInfo collisions;
	public bool onAir;

	BoxCollider2D bc2d;
	RaycastOrigins raycastOrigins;
	

	// Função chamada no script do player, recebe uma velocidade, checa se há colisões e move o objeto.
	public void Move(Vector2 velocity){
		UpdateRaycastOrigins ();
		collisions.Reset();
		if(velocity.x != 0){
			HorizontalCollisions(ref velocity);
		}
		if(velocity.y != 0){
			VerticalCollisions(ref velocity);
		}
		onAir = !(collisions.bellow && collisions.above && collisions.bellow && collisions.above);
		
		transform.Translate(velocity);
	}
	
	void Start() {
		bc2d = GetComponent<BoxCollider2D> ();
		CalculateRaySpacing ();
	}

// ----------------------------------------------
// --------------- RAY CAST ---------------------
// ----------------------------------------------

	//Calcula a distancia entre os rays, baseando-se na quantidade deles e no tamanho do box collider
	void CalculateRaySpacing() {
		Bounds bounds = bc2d.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	//Calcula as posições rays importantes
	void UpdateRaycastOrigins() {
		Bounds bounds = bc2d.bounds;
		bounds.Expand (skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	// Armazena as posições dos rays importantes
	struct RaycastOrigins {
		
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}


// ----------------------------------------------
	//Desenha os rays verticalmente e horizontalmente, caso eles acertem algo seta as variaveis do contruct collisions
	void VerticalCollisions(ref Vector2 velocity){
		float directionY = Mathf.Sign(velocity.y);
		float rayLenght = Mathf.Abs(velocity.y) + skinWidth;
		for (int i = 0; i < verticalRayCount; i ++) {
			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLenght, collisionMask);
			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLenght ,Color.red);


			if(hit){
				// float angle = Vector2.Angle(hit.normal, Vector2.up);
				// print("Angulo da Colisão na Vertical: " + angle);
				
				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLenght = hit.distance;

				collisions.bellow = (directionY == -1);
				collisions.above = (directionY == 1);
			}
		}
	}

	void HorizontalCollisions(ref Vector2 velocity){
		float directionX = Mathf.Sign(velocity.x);
		float rayLenght = Mathf.Abs(velocity.x) + skinWidth;
		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLenght, collisionMask);
			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLenght ,Color.red);
			if(hit){
				float angle = Vector2.Angle(hit.normal, Vector2.up);
		
				velocity.x = (hit.distance - skinWidth) * directionX;
				rayLenght = hit.distance;

				collisions.left = directionX == -1;
				collisions.right = directionX == 1;
			}
		}
	}

// --------------------------------------------------
	public struct CollisionInfo{
		public bool above, bellow, left, right;
		public void Reset(){
			above = bellow = false;
			right = left = false;
		}
	}	
}