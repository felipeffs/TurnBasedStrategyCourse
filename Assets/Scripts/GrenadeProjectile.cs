using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GrenadeProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyGrenadeExploded;

    [SerializeField] private Transform grenadeExplodeVfxPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;

    private Vector3 targetPosition;
    private Action onGrenadeBehaviourComplete;
    private float totalDistance;
    private Vector3 positionXZ;
    private bool isDone = false;
    private float timer;

    private void Update()
    {
        if (!isDone)
        {
            MoveInArc();


            float reachedTargetDistance = .2f;
            if (Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance)
            {
                DoDamage();

                OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);

                DoExplodeVFX();

                trailRenderer.transform.parent = null;

                gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;

                isDone = true;
                timer = .5f;
            }
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Destroy(gameObject);
            onGrenadeBehaviourComplete();
        }
    }

    private void MoveInArc()
    {
        Vector3 moveDirection = (targetPosition - positionXZ).normalized;

        float moveSpeed = 15f;
        positionXZ += moveDirection * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - (distance / totalDistance);

        float maxHeight = totalDistance / 4f;
        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);
    }

    private void DoDamage()
    {
        float damageRadius = 4f;
        Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent<Unit>(out Unit targetUnit))
            {
                targetUnit.Damage(30);
            }
            if (collider.TryGetComponent<DestructibleCrate>(out DestructibleCrate destructibleCrate))
            {
                destructibleCrate.Damage();
            }
        }
    }

    private void DoExplodeVFX()
    {
        Instantiate(grenadeExplodeVfxPrefab, targetPosition + Vector3.up, Quaternion.identity);
    }

    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviourComplete)
    {
        this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }
}
