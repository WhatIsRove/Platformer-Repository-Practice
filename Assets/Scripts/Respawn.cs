using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawn : MonoBehaviour
{
    [SerializeField] private float respawnTime = 3f;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(RespawnOnDelay());
    }

    private IEnumerator RespawnOnDelay()
    {
        yield return new WaitForSeconds(respawnTime);
        SceneManager.LoadScene("SampleScene");
    }
}
