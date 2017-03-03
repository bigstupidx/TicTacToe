using UnityEngine;

public class DestroyWhenParticleDone : MonoBehaviour {
    
	void Start () {
        Invoke("DestroyThis", GetComponent<ParticleSystem>().main.duration);
	}
	
    private void DestroyThis() {
        Destroy(gameObject);
    }
	
}
