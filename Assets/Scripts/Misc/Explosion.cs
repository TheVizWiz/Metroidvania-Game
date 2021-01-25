using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class Explosion {

    private static Collider2D[] targets;

    static Explosion() {
        targets = new Collider2D[100]; 
    }

    public static void CreateExplosion(Vector3 origin, float radius, float damage, ElementType elementType, bool chain, LayerMask targetLayers) {
        int numHit = Physics2D.OverlapCircleNonAlloc(origin, radius, targets, targetLayers);


        for (int i = 0; i < numHit; i++) {
            Collider2D collider = targets[i];
            try {
                if (collider.GetComponent<IExplodable>().Explode(damage, elementType)) {
                    
                    if (chain)
                        CreateExplosion(collider.transform.position, radius, damage, elementType, true, targetLayers);
                }
            } catch (MissingComponentException exception) {

            }
        }
        
    }


    public static IEnumerator CreateExplosion(Vector3 origin, float radius, float damage, ElementType elementType, bool chain,
        LayerMask targetLayers, float waitTime) {
        
        yield return new WaitForSeconds(waitTime);
 
        
        int numHit = Physics2D.OverlapCircleNonAlloc(origin, radius, targets, targetLayers);


        for (int i = 0; i < numHit; i++) {
            Collider2D collider = targets[i];
            try {
                if (collider.GetComponent<IExplodable>().Explode(damage, elementType)) {

                    if (chain)
                        GameManager.sceneAnimator.StartCoroutine(
                            CreateExplosion(collider.transform.position, radius, damage, elementType, true,
                                targetLayers, waitTime));
                }
            } catch (MissingComponentException exception) {
                
            }
        }
    }

}
