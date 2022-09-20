using System.Collections.Generic;
using UnityEngine;

public class AreaEffect : MonoBehaviour {
    public AreaEffectType areaEffectType;

    private float timer = 0;

    private List<Enemy> enemiesInArea;

    void Start() {
        enemiesInArea = new List<Enemy>();

        if (areaEffectType.snapToGround) {
            // Ground level should be defined by the level in the future via global variable
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    void Update() {
        timer += Time.deltaTime;

        if (timer >= areaEffectType.duration) {
            foreach (Enemy enemy in enemiesInArea) {
                enemy.RemoveStatusEffect(areaEffectType.statusEffect);
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(TagConstants.ENEMY)) {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            if (enemy != null) {
                enemiesInArea.Add(enemy);

                enemy.ApplyStatusEffect(areaEffectType.statusEffect);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(TagConstants.ENEMY)) {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            if (enemy != null) {
                enemiesInArea.Remove(enemy);

                enemy.RemoveStatusEffect(areaEffectType.statusEffect);
            }
        }
    }
}
