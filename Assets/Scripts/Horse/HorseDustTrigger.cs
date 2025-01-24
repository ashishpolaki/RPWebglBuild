using System.Collections;
using UnityEngine;

namespace HorseRace
{
    public class HorseDustTrigger : MonoBehaviour
    {
        [SerializeField] private Transform rightLeg;
        [SerializeField] private Transform leftLeg;

        public void PlayLeftLegDustEffect()
        {
            if(ParticleManager.Instance == null)
            {
                return;
            }

            ParticleSystem particle = ParticleManager.Instance.PlayParticle(ParticleType.HorseDustCloud,leftLeg.position);
            StartCoroutine(IDeactiveParticle(particle.gameObject, particle.main.duration));
        }
        public void PlayRightLegDustEffect()
        {
            if (ParticleManager.Instance == null)
            {
                return;
            }
            ParticleSystem particle = ParticleManager.Instance.PlayParticle(ParticleType.HorseDustCloud, rightLeg.position);
            StartCoroutine(IDeactiveParticle(particle.gameObject, particle.main.duration));
        }

        IEnumerator IDeactiveParticle(GameObject _object, float _time)
        {
            yield return new WaitForSeconds(_time);
            _object.SetActive(false);
        }
    }
}
