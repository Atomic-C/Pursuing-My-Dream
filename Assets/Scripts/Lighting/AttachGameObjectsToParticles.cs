using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that attach game object that contains a light source, into the particle system of the object (each particle now emits light)
/// Work around for using lights in particle system in 2D
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class AttachGameObjectsToParticles : MonoBehaviour
{
    /// <summary>
    /// The game object containing the light source
    /// </summary>
    public GameObject lightPrefab;

    /// <summary>
    /// The object particle system
    /// </summary>
    private ParticleSystem _particleSystem;

    /// <summary>
    /// List that will hold all the instances of the light containing game object
    /// </summary>
    private List<GameObject> _lightInstances = new List<GameObject>();

    /// <summary>
    /// Array of particles
    /// </summary>
    private ParticleSystem.Particle[] _particles;

    // Start is called before the first frame update
    /// <summary>
    /// Chache the necessary variables and initialize the array of particles with the max particles ammount in the particle system
    /// </summary>
    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
    }

    // Update is called once per frame
    /// <summary>
    /// Activate the game object with the light source in the same position of each particles, if they are being simulated (active), if not, deactivate the game object for reuse
    /// </summary>
    void LateUpdate()
    {
        int count = _particleSystem.GetParticles(_particles);

        while (_lightInstances.Count < count)
            _lightInstances.Add(Instantiate(lightPrefab, _particleSystem.transform));

        bool worldSpace = _particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World;
        for (int i = 0; i < _lightInstances.Count; i++)
        {
            if (i < count)
            {
                if (worldSpace)
                    _lightInstances[i].transform.position = _particles[i].position;
                else
                    _lightInstances[i].transform.localPosition = _particles[i].position;
                _lightInstances[i].SetActive(true);
            }
            else
            {
                _lightInstances[i].SetActive(false);
            }
        }
    }
}
