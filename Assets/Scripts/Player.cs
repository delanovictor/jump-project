using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

    //Esses três parâmetros determinam todos os outros
    public float jumpHeight = 4;
    public float timeToJumpPeak = .5f;
    public float moveSpeed = 10;
    
    //Esses caras derivam dos de cima. 
    public float gravity;
    public float jumpVelocity;

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

        gravity = -32f;
	}

    private void Update() {
 
        Vector2 input = new Vector2();
        velocity.y += gravity * Time.deltaTime;

        // -----------------------------------------------------------------

        if(controller.collisions.bellow){
            input = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
            velocity.y = -velocity.y/4;
            gravity = -16f;
            // jumpHeight = 4;
            if(playerState == State.Jumping || playerState == State.Falling){
                playerState = State.Running;
            }
        }else{
            if(playerState != State.Jumping){
                if(velocity.y < -1){
                    playerState = State.Falling;
                }
            }
        }

        if(controller.collisions.above){
            velocity.y = -velocity.y;
        }

        if(controller.collisions.right || controller.collisions.left){
            velocity.x = -velocity.x * 0.5f;
        }

        // -------------------------------------------------------------------

          //Se o botão de espaço for soltado AND estiver em contato com o solo
        if(Input.GetKeyUp(KeyCode.Space) && controller.collisions.bellow){
            //Pula para direção do direcional
            jumpVelocity = jumpHeight/timeToJumpPeak;
            playerState = State.Jumping;
            velocity.y = jumpVelocity;
            velocity.x = input.x * moveSpeed;
            if(velocity.x != 0){
                gravity = 0;
            }
        }

         //Se space foi apertado AND o player está no chão
        if(Input.GetKey(KeyCode.Space) && controller.collisions.bellow){
            //Desenha a trajetória e não se move.
            if(Input.GetKeyDown(KeyCode.D)){
                if(jumpHeight > 1){
                    jumpHeight = jumpHeight - 0.5f;
                }
            }else if(Input.GetKeyDown(KeyCode.A)){
                 if(jumpHeight < 5){
                    jumpHeight = jumpHeight + 0.5f;
                }
            }
            print(jumpHeight);
            playerState = State.Aiming;
            if(input.x == 0 && lastInput != 0){
                DrawPath(lastInput);
            }else{
                DrawPath(input.x);
                lastInput = input.x;
            }
        }

        // -----------------------------------------------------------------

        Vector2 oldVelocity = velocity;
        //O jogador não pode se movimentar durante o pulo, porém se ele estiver no ar por outra razão
        // que não seja o pulo, então pode se movimentar.
        if(playerState != State.Jumping){
            input = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
        }
        
        if(playerState == State.Running || playerState == State.Falling){
            //Caso contrário, se não está pulando:
            //Calcula velocidade em x e move o personagem
            velocity.x = input.x * moveSpeed;
            Vector2 deltaPos = (oldVelocity + velocity) * 0.5f * Time.deltaTime;
            controller.Move(deltaPos);
        }

        //Se estiver pulando
        if(playerState == State.Jumping){
            //Movimenta o personagem
            Vector2 deltaPos = (oldVelocity + velocity) * 0.5f * Time.deltaTime;
            controller.Move(deltaPos);

            //Se colidir com algo horizontalmente, inverte a velocidade em X e multiplica por um fator
            //Esse fator será variado conforme o tipo de obstáculo, e será detectado dinâmicamente.
        }
        
        
    }

    //Desenha a trajetória, precisa de melhoras.
    void DrawPath(float direction){
        Vector2 previousDrawPoint = transform.position;
        Vector2 initialVelocity = new Vector2();
        initialVelocity.y += jumpHeight/timeToJumpPeak;
        initialVelocity.x += direction * moveSpeed;

        float steps = 60;
        float timeSteps = (timeToJumpPeak*2)/steps;

        for(int i = 1; i<steps; i++){
            float currentTime = timeSteps * i;
            Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
            Vector2 drawPoint = currentPos + (initialVelocity * currentTime);
            Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
            previousDrawPoint = drawPoint;
        }

    }


}