using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

    //Esses três parâmetros determinam todos os outros
    public float jumpHeight = 4;
    public float timeToJumpPeak = .5f;
    public float moveSpeed = 7;
    
    //Esses caras derivam dos de cima. 
    float gravity;
    float jumpVelocity;

    Vector2 velocity;

    bool jumping;
	Controller2D controller;


	void Start() {
        //Instancia classe controller
		controller = GetComponent<Controller2D> ();
        //calculo da gravidade e altura do pulo apartir da altura e duração do pulo
        gravity = (-2 * jumpHeight)/(timeToJumpPeak * timeToJumpPeak);
        jumpVelocity = 2 * jumpHeight / timeToJumpPeak;
	}

    private void Update() {
        Vector2 input = new Vector2();

        //Se houver colisões em cima ou embaixo do personagem:
            // - Pega os inputs do player
            // - Zera sua velocidade em y (será mudado para os diferentes tipos de terreno)
            // - Se o player está no chão, zera a flag de pulo

        if(controller.collisions.above || controller.collisions.bellow){
            input = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
            velocity.y = 0;
            if(controller.collisions.bellow){
                jumping = false;
            }
        }
        //O jogador não pode se movimentar durante o pulo, porém se ele estiver no ar por outra razão
        // que não seja o pulo, então pode se movimentar.
        else if(!jumping){
            input = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
        }

        //Esse oldVelocity é usado para melhor simular a trajetória.
        Vector2 oldVelocity = velocity;

        //Aplica a gravidade nesse frame
        velocity.y += gravity * Time.deltaTime;

        //Se space foi apertado AND o player está no chão
        if(Input.GetKey(KeyCode.Space) && controller.collisions.bellow){
            //Desenha a trajetória e não se move.
            DrawPath(input.x);
        }
        else if(!jumping){
            //Caso contrário, se não está pulando:
            //Calcula velocidade em x e move o personagem
            velocity.x = input.x * moveSpeed;
            Vector2 deltaPos = (oldVelocity + velocity) * 0.5f * Time.deltaTime;
            controller.Move(deltaPos);
        }

        //Se o botão de espaço for soltado AND estiver em contato com o solo
        if(Input.GetKeyUp(KeyCode.Space) && controller.collisions.bellow){
            //Pula para direção do direcional
            jumping = true;
            velocity.y = jumpVelocity;
            velocity.x = input.x * moveSpeed;
        }

        //Se estiver pulando
        if(jumping == true){
            //Movimenta o personagem
            Vector2 deltaPos = (oldVelocity + velocity) * 0.5f * Time.deltaTime;
            controller.Move(deltaPos);

            //Se colidir com algo horizontalmente, inverte a velocidade em X e multiplica por um fator
            //Esse fator será variado conforme o tipo de obstáculo, e será detectado dinâmicamente.
            if(controller.collisions.right || controller.collisions.left){
                velocity.x = -velocity.x * 0.5f;
            }
        }
    }

    //Desenha a trajetória, precisa de melhoras.
    void DrawPath(float direction){
        Vector2 previousDrawPoint = transform.position;
        Vector2 initialVelocity = new Vector2();
        initialVelocity.y += jumpVelocity;
        initialVelocity.x += direction * moveSpeed;

        float steps = 60;
        float timeSteps = (timeToJumpPeak*2)/steps;

        for(int i = 1; i<steps; i++){
            float currentTime = timeSteps * i;
            Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
            Vector2 drawPoint = currentPos + ((initialVelocity * currentTime) + (Vector2.up * gravity * currentTime * currentTime)/2);
            Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
            previousDrawPoint = drawPoint;
        }

    }

}