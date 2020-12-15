using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
[RequireComponent (typeof (Trajectory))]

public class Player : MonoBehaviour {

    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public bool toggleAim;
    //Esses três parâmetros determinam todos os outros
    public float timeToJumpPeak = .5f;
    public float moveSpeed = 10f;
    private float bounceParam = 1f;
    [Range(-90f,90f)]
    public float jumpAngle = 0;
    public float jumpDistance = 5f;
    //Esses caras derivam dos de cima. 
    public float jumpGravity = 0;
    public float standartGravity = -20f;
    public float gravity;
    public Vector3 lastPosition;
    
    Vector2 velocity;
    float lastInput;
	Controller2D controller;
    Trajectory trajectory;

    public enum State{
        Running,
        Aiming,
        Jumping,
        Falling,
        Idle
    }

    State playerState = State.Idle;

	void Start() {
        //Instancia classe controller
		controller = GetComponent<Controller2D> ();
        trajectory = GetComponent<Trajectory>();
        //calculo da gravidade e altura do pulo apartir da altura e duração do pulo
        // gravity = (-2 * jumpHeight)/(timeToJumpPeak * timeToJumpPeak);
        // jumpVelocity = 2 * jumpHeight / timeToJumpPeak;
        gravity = standartGravity;
	}

    private void Update() {
        
        CheckJump();
        CheckCollisions();

        // print(playerState);
    }

    private void FixedUpdate() {
        
        velocity.y += gravity * Time.fixedDeltaTime;
        CheckStates();
        CheckJumpAngle();
    }

    void CheckCollisions(){
        Vector2 input = new Vector2();
        if(controller.collisions.bellow){
            if(!(playerState == State.Jumping)){
                input = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
               
                velocity.y = -velocity.y/100;
                gravity = standartGravity;
                jumpDistance = 5f;
                // jumpHeight = 4;
                if(playerState == State.Falling){
                    playerState = State.Running;
                }
            }else{
               
                velocity.y = -velocity.y * bounceParam;
                jumpDistance -= Vector3.Distance(transform.position, lastPosition);
                lastPosition = transform.position;
                jumpAngle = 0;
            }
        }else{
            if(playerState != State.Jumping){
                if(velocity.y < -.1){
                    playerState = State.Falling;
                }
            }
        }
        if(controller.collisions.above){
            velocity.y = -velocity.y * bounceParam;
            jumpDistance -= Vector3.Distance(transform.position, lastPosition);
            lastPosition = transform.position;
        }
        if((controller.collisions.right || controller.collisions.left)){
            velocity.x = -velocity.x * bounceParam;
            jumpDistance -= Vector3.Distance(transform.position, lastPosition);
            lastPosition = transform.position;
        }
    }

    void CheckJump(){
    
       if(Input.GetKeyUp(KeyCode.Space) && controller.collisions.bellow){
            //Pula para direção do direcional
            playerState = State.Jumping;

            velocity.y = (Mathf.Cos(jumpAngle * Mathf.Deg2Rad) * jumpDistance)/timeToJumpPeak;
            // velocity.y = jumpVelocity;
            // jumpVelocity = jumpVelocityConst; 
            velocity.x = (Mathf.Sin(jumpAngle * Mathf.Deg2Rad) * jumpDistance)/timeToJumpPeak;

            lastPosition = transform.position;
        }
    }
   

    void CheckJumpAngle(){
        //Se o botão de espaço for soltado AND estiver em contato com o solo

         //Se space foi apertado AND o player está no chão
        if((Input.GetKey(KeyCode.Space) && controller.collisions.bellow) || toggleAim){
            //Desenha a trajetória e não se move.
            if(Input.GetKey(KeyCode.D)){
                jumpAngle = jumpAngle + (70 * Time.fixedDeltaTime);
            }else if(Input.GetKey(KeyCode.A)){
                jumpAngle = jumpAngle - (70 * Time.fixedDeltaTime);
            }
            playerState = State.Aiming;
            
            trajectory.PhysicsData(jumpGravity, standartGravity, timeToJumpPeak);
            trajectory.Draw(jumpAngle, jumpDistance);
        }else{
            trajectory.DrawPoints.Clear();
        }
    }

    void CheckStates(){
        Vector2 input = new Vector2();

        Vector2 oldVelocity = velocity;
        //O jogador não pode se movimentar durante o pulo, porém se ele estiver no ar por outra razão
        // que não seja o pulo, então pode se movimentar.
        if(playerState != State.Jumping){
            input = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
        }
        
        if(playerState == State.Running){
            //Caso contrário, se não está pulando:
            //Calcula velocidade em x e move o personagem
            velocity.x = input.x * moveSpeed;
            Vector2 deltaPos = (oldVelocity + velocity) * 0.5f * Time.fixedDeltaTime;
            controller.Move(deltaPos);
            animator.SetFloat("isRunning", Mathf.Abs(velocity.x));
            if (input.x < 0){
                spriteRenderer.flipX = true;
            }
            if(input.x > 0){
                spriteRenderer.flipX = false;
            }
        }
        if(playerState == State.Falling){
             velocity.x = input.x * moveSpeed;
            Vector2 deltaPos = (oldVelocity + velocity) * 0.5f * Time.fixedDeltaTime;
            controller.Move(deltaPos);
            
        }

        //Se estiver pulando
        if(playerState == State.Jumping){
            //Movimenta o personagem
            Vector2 deltaPos = (oldVelocity + velocity) * 0.5f * Time.fixedDeltaTime;
            controller.Move(deltaPos);

            gravity = jumpGravity;

            if(Vector3.Distance(transform.position, lastPosition) >= jumpDistance){
                gravity = standartGravity;
                velocity.y = velocity.y / 3;
                velocity.x = velocity.x / 5;
                playerState = State.Falling;
            }
            //Se colidir com algo horizontalmente, inverte a velocidade em X e multiplica por um fator
            //Esse fator será variado conforme o tipo de obstáculo, e será detectado dinâmicamente.
        }
    }


    //Desenha a trajetória, precisa de melhoras.
    // void DrawPath(){
        //acho que esse draw point ta errado Delano :D n sei como arrumar mb
     
        // print("--------");
        // print(jumpAngle);
        // print(Mathf.Sin(jumpAngle * Mathf.Deg2Rad));
        // print(Mathf.Cos(jumpAngle * Mathf.Deg2Rad));

         
    // }

    //void DrawPath() {
	
        // Vector2 drawPoint2 = new Vector2(transform.position.x + Mathf.Sin(jumpAngle * Mathf.Deg2Rad) * jumpDistance, transform.position.y + Mathf.Cos(jumpAngle * Mathf.Deg2Rad) * jumpDistance);
        // Debug.DrawLine(transform.position,  drawPoint2, Color.blue); 
	}
   
    // sin(60) = o/h
    // sin(60)*h = o
    // cos(60)*h = a  
