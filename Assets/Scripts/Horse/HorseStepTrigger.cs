using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class HorseStepTrigger : MonoBehaviour
{
    //[SerializeField] private float distance;
    //private NavMeshAgent agent;

    //private void Awake()
    //{
    //    agent = GetComponentInParent<NavMeshAgent>();
    //}

    //// Update is called once per frame
    //void FixedUpdate()
    //{
    //    if (agent.velocity.sqrMagnitude > 80)
    //    {
    //        Ray ray = new Ray(this.transform.position, -this.transform.right);

    //        if (Physics.Raycast(ray, out RaycastHit hit, distance, LayerMask.GetMask("HorseTrack")))
    //        {
    //            if (hit.collider != null)
    //            {
    //                ParticleSystem particle = ParticleManager.Instance.PlayParticle(transform.position);

    //                StartCoroutine(IDeactiveParticle(particle.gameObject, particle.main.duration));
    //            }
    //        }
    //    }
    //}
    //IEnumerator IDeactiveParticle(GameObject _object, float _time)
    //{
    //    yield return new WaitForSeconds(_time);
    //    _object.SetActive(false);
    //}
}
