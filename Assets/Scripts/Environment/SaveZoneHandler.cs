using UnityEngine;

public class SaveZoneHandler {

    public static SaveZone zone;
    public static DamageBarrier barrier;


    public static void RespawnAtLastZone(GameObject player) {
        player.transform.position = zone.transform.position;
        barrier.ResetPosition();

    }

    public static void SetSafeZone(SaveZone saveZone) {
        zone = saveZone;
    }
}
