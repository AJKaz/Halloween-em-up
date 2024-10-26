using UnityEngine;

public abstract class EnemyState
{
    protected float time { get; set; }

    protected Enemy enemy;

    public EnemyState(Enemy e) {
        enemy = e;
    }

    public virtual void OnEnter() {
        time = 0f;
    }

    public virtual void OnUpdate() {
        time += Time.deltaTime;
    }

    public virtual void OnExit() {

    }
}
