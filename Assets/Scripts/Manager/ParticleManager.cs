using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;

    #region Inspector Variables
    [Tooltip("Particles Data")]
    [SerializeField] private Particle[] particles;
    #endregion

    #region Private Variables
    private Dictionary<ParticleType, List<ParticleSystem>> particlesDictionary = new Dictionary<ParticleType, List<ParticleSystem>>();
    #endregion

    #region Unity Methods
    private void Awake()
    {
        Instance = this;
        PoolParticles();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Pool every particle in particles array
    /// </summary>
    private void PoolParticles()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            List<ParticleSystem> pooledParticlesList = new List<ParticleSystem>();
            for (int j = 0; j < particles[i].poolSize; j++)
            {
                ParticleSystem _newParticle = Instantiate(particles[i].particleSystem, transform);
                _newParticle.gameObject.SetActive(false);
                pooledParticlesList.Add(_newParticle);
            }
            particlesDictionary.Add(particles[i].particleType, pooledParticlesList);
        }
    }
    /// <summary>
    /// Increase pool size of particle via particle type
    /// </summary>
    /// <param name="_particleType"></param>
    private void IncreasePoolSize(ParticleType _particleType)
    {
        Particle particle = particles.First(x => x.particleType == _particleType);
        List<ParticleSystem> pooledParticlesList = particlesDictionary[_particleType];
        for (int j = 0; j < particle.poolSize; j++)
        {
            ParticleSystem _newParticle = Instantiate(particle.particleSystem, transform);
            _newParticle.gameObject.SetActive(false);
            pooledParticlesList.Add(_newParticle);
        }
        particlesDictionary[_particleType] = pooledParticlesList;
    }

    /// <summary>
    /// Get particlesystem via particletype
    /// </summary>
    /// <param name="_particleType"></param>
    /// <returns></returns>
    private ParticleSystem GetParticle(ParticleType _particleType)
    {
        for (int i = 0; i < particlesDictionary[_particleType].Count; i++)
        {
            if (!particlesDictionary[_particleType][i].gameObject.activeInHierarchy)
            {
                return particlesDictionary[_particleType][i];
            }
        }
        IncreasePoolSize(_particleType);
        return GetParticle(_particleType);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Play particle via particle type in given position
    /// </summary>
    /// <param name="particleType"></param>
    /// <param name="_vec"></param>
    /// <returns></returns>
    public ParticleSystem PlayParticle(ParticleType particleType, Vector3 _vec)
    {
        ParticleSystem particle = GetParticle(particleType);
        particle.transform.position = _vec;
        particle.gameObject.SetActive(true);
        particle.Play();
        return particle;
    }
    #endregion
}
