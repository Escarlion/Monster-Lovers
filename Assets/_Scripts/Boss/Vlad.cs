using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vlad : Boss
{
    protected Player player;

    int atk1 = 0, atk2 = 0, atk3 = 0, atk4 = 0;

    private void Start()
    {
        stageCount = 2;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        FillConditionalAttacks();
    }
    private void FixedUpdate()
    {
        if (!isAttacking) PrepareAtack();
    }

    //preenche quais os ataques disponiveis para cada estagio
    //MUDAR: colocar está logica quando o boss morrer e no start para economizar memoria
    protected override void FillAttackList(int actualStage)
    {
        attackList.Clear();
        if (actualStage == 1)
        {
            attackList.Add(nameof(Attack01));
            attackList.Add(nameof(Attack02));
            attackList.Add(nameof(Attack03));
        }
        else if (actualStage == 2)
        {
            attackList.Add(nameof(Attack02));
            attackList.Add(nameof(Attack03));
            attackList.Add(nameof(Attack04));
        }
        else Debug.Log("estado não encontrado");
    }
    //Adicionar aqui os ataques que precisam cumprir condições especificas para serem executados
    protected override void FillConditionalAttacks()
    {
        conditionalAttacks.Add(nameof(Attack03));
    }

    //Checa se a condição para certo ataque foi cumprida ou não
    protected override bool CheckConditions(string attack)
    {
        if (attack == nameof(Attack03) && BossDistance(player.transform, 2f)) return true;
        else return false;
    }

    //escolhe um ataque aleatorio
    protected override string GetAttack()
    {
        FillAttackList(actualStage);

        int i = Random.Range(0, attackList.Count);

        return attackList[i];
    }

    //Realiza o atk
    protected override void PrepareAtack()
    {
        if (isDead == true || isAttacking) return;

        string attack = GetAttack();

        if (conditionalAttacks.Contains(attack))
        {
            if (CheckConditions(attack) == false)
            {
                Debug.Log("Condições do ataque " + attack + " falharam.");
                return;
            }
        }
        isAttacking = true;
        Invoke(attack, attackWaitTime);

        /*Debug.Log("---Lista de atks---\nEstado:" + actualStage + " intervalo entre os atk: " + attackWaitTime + 
            "\natk1: " + atk1 + "\natk2: " + atk2 + "\natk3: " + atk3 + "\natk4: " + atk4);*/

        Invoke(nameof(EndAttack), 1.5f);
    }

    private void Attack01()
    {
        atk1++;
        Debug.Log("Attack01");
    }
    private void Attack02()
    {
        atk2++;
        Debug.Log("Attack02");
    }
    private void Attack03()
    {
        atk3++;
        Debug.Log("Attack03");
    }
    private void Attack04()
    {
        atk4++;
        Debug.Log("Attack04");
    }

}
