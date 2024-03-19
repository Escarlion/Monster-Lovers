using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Approach : MonoBehaviour
{
    private KeyCode[] approachKeys = { KeyCode.A, KeyCode.W, KeyCode.S, KeyCode.D };
    private List<KeyCode> keysToPress = new List<KeyCode>();

    bool sequenceRunning = false;
    [SerializeField] private int keysAmount = 5;

    [Header("Mode Selector")]
    [SerializeField] private bool OneErrorMode;
    [SerializeField] private bool BarMode;

    [Header("Bar Mode")]

    [SerializeField] private int targetValue = 100;
    [SerializeField] private int rightValue = 10;
    [SerializeField] private int wrongValue = -20;
    [SerializeField] private int actualValue;


    [Header("Timer")]
    [SerializeField] private float limitTime = 5f;
    [SerializeField] private float timeActuator = 0.5f;

    private void Start()
    {
        if (OneErrorMode && BarMode)
        {
            OneErrorMode = true;
            BarMode = false;
        }
    }

    //Inicia a sequencia
    public void StartSequence()
    {
        sequenceRunning = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().StopPlayerMoviments(sequenceRunning);
        GenerateSequence();

        StartCoroutine(SequenceTimer());

        ViewDebug();

        //necessario limpar a lista eventualmente
        //keysToPress.Clear();

    }

    private void Update()
    {
        //caso a sequencia esteja ativa e tenha algum item na lista de teclas, começa a detectar o que foi pressionado
        if (sequenceRunning && keysToPress.Count > 0)
        {
            //só faz a checagem caso algo seja apertado
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(keysToPress[0]))
                {
                    CorrectKey();
                }
                else
                {
                    IncorrectKey();
                }
            }

        }
        if (Input.GetKeyDown(KeyCode.P)) StartSequence();

    }

    //Caso a tecla correta seja apertada, retira o 1o item da lista, assim deixando a proxima tecla disponivel,
    //caso não tenha nada sobrando na lista, é iniciado a sequencia de vitoria
    private void CorrectKey()
    {
        Debug.Log("Tecla certa");
        keysToPress.RemoveAt(0);

        if (OneErrorMode) if (keysToPress.Count == 0) ApproachResult(true);
        
        //teclas certas acrestam valor na barra, caso alcançe/ultrapasse 100, resulta em vitoria 
        if (BarMode)
        {       
            actualValue += rightValue;

            if (actualValue >= 100)
            {
                ApproachResult(true);
                return;
            }
        }
    }
    //Caso a tecla errada seja pressionada, inicia a sequencia de derrota 
    private void IncorrectKey()
    {
        Debug.Log("Tecla incorreta");

        if(OneErrorMode) ApproachResult(false);

        //teclas incorretas retiram valor da barra, mas não resulta em derrota
        if (BarMode) actualValue += wrongValue;
        
    }

    //determina se foi vitoria ou derrota e limpa as variaveis necessarias 
    private void ApproachResult(bool result)
    {
        if (result) Debug.Log("Você venceu o confronto");
        else Debug.Log("Você perdeu o confronto");

        sequenceRunning = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().StopPlayerMoviments(sequenceRunning);
        keysToPress.Clear();
        StopCoroutine(SequenceTimer());
    }

    //Gera a sequencia aleatoria de teclas que deve ser apertada
    private void GenerateSequence()
    {
        if (OneErrorMode)
        {
            for (int i = 0; i < keysAmount; i++)
            {
                keysToPress.Add(GenerateKey());
            }
        }
        if (BarMode)
        {
            for (int i = 0; i < 50; i++)
            {
                keysToPress.Add(GenerateKey());
            }
        }

    }

    //Gera um numero aleatorio que usa o approachKeys para determinar que tecla vai entrar na sequencia 
    private KeyCode GenerateKey()
    {
        int key = Random.Range(0, 4);

        return approachKeys[key];
    }

    //roda o timer, caso o tempo acabe, resulta em derrota 
    IEnumerator SequenceTimer()
    {
        Debug.Log("Timer iniciado, duração maxima: " + limitTime);

        float actualTime = limitTime;

        while (actualTime > 0)
        {
            yield return new WaitForSeconds(timeActuator);
            actualTime -= timeActuator;
        }

        ApproachResult(false);

        Debug.Log("Tempo encerrado");
    }

    private void ViewDebug()
    {
        //Mostra a sequencia de teclas no Debug
        string allKeys = string.Join(", ", keysToPress);
        Debug.Log("Teclas a serem pressionadas: " + allKeys);
    }
}
