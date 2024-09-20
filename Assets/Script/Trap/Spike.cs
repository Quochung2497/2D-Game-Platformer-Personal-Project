using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    
    private void OnCollisionEnter2D(Collision2D _other)
    {
        string layerName = LayerMask.LayerToName(_other.collider.gameObject.layer);
        if (layerName == "Player")
        {
            StartCoroutine(RespawnPoint());
        }
        else if (layerName == "Attackable")
        {
            Enemy enemyController = _other.collider.GetComponent<Enemy>();
            enemyController.TrapHit(10f);
        }
    }

    IEnumerator RespawnPoint()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        PlayerController.Instance.pState.cutscenes = true;
        PlayerController.Instance.pState.Invincible = true;
        PlayerController.Instance.rb.velocity = Vector2.zero;
        Time.timeScale = 0f;
        StartCoroutine(UIManager.Instance.sceneFader.Fade(ScreenFader.FadeDirection.In));
        PlayerController.Instance.TakeDamage(1);
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1;
        PlayerController.Instance.transform.position = GlobalController.instance.platformingRespawnPoint;
        StartCoroutine(UIManager.Instance.sceneFader.Fade(ScreenFader.FadeDirection.Out));
        yield return new WaitForSecondsRealtime(UIManager.Instance.sceneFader.fadeTime);
        PlayerController.Instance.pState.cutscenes = false;
        PlayerController.Instance.pState.Invincible = false;
    }
}
