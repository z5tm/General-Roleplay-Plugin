namespace GRPP.API.Features;

using System.Collections.Generic;
using UnityEngine;

public static class FrozenPlayerManager
{
    public static List<ExPlayer> FrozenPlayers { get; set; } = [];
    
    extension(ExPlayer player)
    {
        public void ToggleFrozenPlayer(bool? freeze = null)
        {
            freeze ??= !FrozenPlayers.Contains(player);
            
            if ((bool)freeze)
                FrozenPlayers.Add(player);
            else
                FrozenPlayers.Remove(player);
            
            player.FreezePlayer((bool)freeze);
        }
        
        private void FreezePlayer(bool freeze)
        {
            if (!player.GameObject.TryGetComponent<CharacterController>(out var characterController))
                return;
            
            characterController.enabled = !freeze;
            if (!player.GameObject.TryGetComponent<Rigidbody>(out var rigidbody))
                rigidbody = player.GameObject.AddComponent<Rigidbody>();
            
            rigidbody.isKinematic = freeze;
            rigidbody.useGravity = !freeze;
            
            if (freeze)
                return;
            
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }
}