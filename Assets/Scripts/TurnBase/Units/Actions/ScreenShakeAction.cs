using System;
using UnityEngine;

namespace TurnBase
{
    public class ScreenShakeAction : MonoBehaviour
    {
        void Start()
        {
            ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
            GrenadeProjectile.OnAnyGrenadeExploded += GrenadeProjectile_OnAnyGrenadeExploded;
        }

        private void GrenadeProjectile_OnAnyGrenadeExploded(object sender, EventArgs e)
        {
            ScreenShake.Instance.Shake(5f);
        }

        private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
        {
            ScreenShake.Instance.Shake();
        }
    }

}