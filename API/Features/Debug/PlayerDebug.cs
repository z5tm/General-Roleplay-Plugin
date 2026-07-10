namespace GRPP.API.Features.Debug;

using UnityEngine;

public static class PlayerDebug
{
    public static string[] GetPlayerInfo(ExPlayer player)
    {
        var stringList = Exiled.API.Features.Pools.ListPool<string>.Pool.Get();
        var iterator = 0;
        foreach (var component in player.GameObject.GetComponents<Component>())
        {
            iterator++;
            
            stringList.Add(string.Join("\n ", component.tag) + $" (iteration {iterator}, component.tag)\n" +
            string.Join("\n ", component.GetType()) + $" (iteration {iterator}, component's type\n" +
            string.Join("\n ", component.GetType().FullName) + $" (iteration {iterator}, component's full name)\n" +
            string.Join("\n ", component.GetType().AssemblyQualifiedName) + $" (iteration {iterator}, component's assembly qualified name)\n" +
            string.Join("\n ", component.GetType().Attributes) + $" (iteration {iterator}, component's attributes)\n");
        }
        
        return Exiled.API.Features.Pools.ListPool<string>.Pool.ToArrayReturn(stringList);
    }
}