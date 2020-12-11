using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

    //Esses três parâmetros determinam todos os outros
    public float jumpHeight = 4f;
    public float timeToJumpPeak = .5f;
    public float moveSpeed = 10f;
    private float bounceParam = 1f;
    [Range(-90f,90f)]
    public float jumpAngle = 0;
    public float jumpDistance = 6f;
    //Esses caras derivam dos de cima. 
    public float gravity;
    public float jumpVelocity;
    public Vector3 lastPosition;

    Vector2 velocity;

    bool jumping;
    float lastInput;
	Controller2D controller;

    public enum State{
        Running,
        Aiming,
        Jumping,
        Falling
    }

    State playerState;

	void Start() {
        //Instancia classe controller
		controller = GetComponent<Controller2D> ();
        //calculo da gravidade e altura do pulo apartir da altura e duração do pulo
        // gravity = (-2 * jumpHeight)/(timeToJumpPeak * timeToJumpPeak);
        // jumpVelocity = 2 * jumpHeight / timeToJumpPeak;
        gravity = -20f;
	}

    private void FixedUpdate() {
        
        Vector2 input = new Vector2();
        velocity.y += gravity * Time.fixedDeltaTime;

        // -----------------------------------------------------------------
        CheckCollisions(input);
        // -------------------------------------------------------------------
        CheckSpaceInput();
        // -----------------------------------------------------------------
        CheckStates(input);
    }

    void CheckCollisions(Vector2 input){
        if(controller.collisions.bellow){
            if(!(playerState == State.Jumping)){
                input = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
                velocity.y = -velocity.y/4;
                gravity = -20f;
                jumpDistance = 6f;
                // jumpHeight = 4;
                if(playerState == State.Jumping || playerState == State.Falling){
                    playerState = State.Running;
                }
            }else{
                velocity.y = -velocity.y * bounceParam;
                jumpDistance -= Vector3.Distance(transform.position, lastPosition);
                lastPosition = transform.position;
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
        if((controller.collisions.right || controller.collisions.left) && playerState == State.Jumping){
            velocity.x = -velocity.x * bounceParam;
            jumpDistance -= Vector3.Distance(transform.position, lastPosition);
            lastPosition = transform.position;
        }
    }
    void CheckSpaceInput(){
        //Se o botão de espaço for soltado AND estiver em contato com o solo
        if(Input.GetKeyUp(KeyCode.Space) && controller.collisions.bellow){
            //Pula para direção do direcional
            playerState = State.Jumping;

            jumpVelocity = (Mathf.Cos(jumpAngle * Mathf.Deg2Rad) * jumpDistance)/timeToJumpPeak;
            velocity.y = jumpVelocity;
            velocity.x = (Mathf.Sin(jumpAngle * Mathf.Deg2Rad) * jumpDistance)/timeToJumpPeak;

            lastPosition = transform.position;
        }

         //Se space foi apertado AND o player está no chão
        if(Input.GetKey(KeyCode.Space) && controller.collisions.bellow){
            //Desenha a trajetória e não se move.
            if(Input.GetKey(KeyCode.D)){
                jumpAngle = jumpAngle + (55 * Time.fixedDeltaTime);
            }else if(Input.GetKey(KeyCode.A)){
                jumpAngle = jumpAngle - (55 * Time.fixedDeltaTime);
            }
            playerState = State.Aiming;
           
            DrawPath();
        }else{
            jumpAngle = 0f;
        }
    }
    void CheckStates(Vector2 input){
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
        }
        if(playerState == State.Falling){
            Vector2 deltaPos = (oldVelocity + velocity) * 0.5f * Time.fixedDeltaTime;
            controller.Move(deltaPos);
        }

        //Se estiver pulando
        if(playerState == State.Jumping){
            //Movimenta o personagem
            Vector2 deltaPos = (oldVelocity + velocity) * 0.5f * Time.fixedDeltaTime;
            controller.Move(deltaPos);

            gravity = 0;

            if(Vector3.Distance(transform.position, lastPosition) >= jumpDistance && gravity == 0){
                
                gravity = -15f;
                velocity.y = velocity.y / 4;
                velocity.x = velocity.x / 3;
                playerState = State.Falling;
            }
            //Se colidir com algo horizontalmente, inverte a velocidade em X e multiplica por um fator
            //Esse fator será variado conforme o tipo de obstáculo, e será detectado dinâmicamente.
        }
    }


    //Desenha a trajetória, precisa de melhoras.
    void DrawPath(){
        Vector2 drawPoint = new Vector2(transform.position.x + Mathf.Sin(jumpAngle * Mathf.Deg2Rad) * jumpDistance, transform.position.y + Mathf.Cos(jumpAngle * Mathf.Deg2Rad) * jumpDistance);
        // print("--------");
        // print(jumpAngle);
        // print(Mathf.Sin(jumpAngle * Mathf.Deg2Rad));
        // print(Mathf.Cos(jumpAngle * Mathf.Deg2Rad));

        Debug.DrawLine(transform.position,  drawPoint, Color.green);  
    }

    // sin(60) = o/h
    // sin(60)*h = o
    // cos(60)*h = a  
}