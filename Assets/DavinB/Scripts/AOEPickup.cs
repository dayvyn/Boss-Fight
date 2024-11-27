using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System;

namespace DavinB
{
    public class AOEPickup : MonoBehaviour
    {
        private NavMeshAgent aoeNavMesh;
        private GameObject floor;
        private Transform centerPoint;
        private Collider aoeCollider;
        private bool followingPlayer = false;
        private CharacterMovement player;

        [SerializeField] float range;
        [SerializeField] List<Transform> points = new List<Transform>();
        [SerializeField] float radius;


        private void OnEnable()
        {
            aoeNavMesh = GetComponent<NavMeshAgent>();
            floor = FindAnyObjectByType<NavMeshSurface>().gameObject;
            centerPoint = floor.transform;
            aoeCollider = GetComponent<Collider>();
            StartCoroutine(CheckingCaughtPlayer());
        }
        private void Update()
        {
            if (!followingPlayer)
            {
                if(aoeNavMesh.remainingDistance <= aoeNavMesh.stoppingDistance)
                {
                    Vector3 point;
                    if (RandomPoint(centerPoint.position, range, out point))
                    {
                        aoeNavMesh.SetDestination(point);
                    }
                }
            }
            else
            {
                aoeNavMesh.SetDestination(player.transform.position);
            }
        }

        bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
            result = Vector3.zero;
            return false;
        }

        IEnumerator CheckingCaughtPlayer()
        {
            yield return new WaitForSeconds(4f);
            Collider[] collisions = Physics.OverlapCapsule(points[0].position, points[1].position, radius);
            foreach (Collider collider in collisions)
            {
                if (collider.gameObject.layer == 7 && player != null)
                {
                    if (!player.pickedUpAlready)
                        try
                        {
                            player.PickedUp.Invoke();
                        }
                        catch (NullReferenceException e)
                        {
                            Destroy(this.gameObject);
                        }
                }
            }
            Destroy(this.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 7)
            {
                followingPlayer = true;
                player = other.gameObject.GetComponent<CharacterMovement>();
            }
        }

    }
}