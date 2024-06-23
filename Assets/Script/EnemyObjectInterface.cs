using UnityEngine;

public interface EnemyObjectInterface
{
    public void ForcePointerEnter();

    public void ForcePointerExit();

    public void SetEnemyObjectList(GameObject[,] enemyObjectList);

    public string GetObjectType();

    public void SetPoint(int l, int f);

    public int GetLine();

    public int GetField();

    public void AttackStart();

    public void AttackEnd();

    public int GetPower();

    public bool Attack(int power, int type, bool forceAttack);
}
