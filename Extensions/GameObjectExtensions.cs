namespace GRPP.Extensions;

using ProjectMER.Features.Objects;
using UnityEngine;

public static class GameObjectExtensions
{
    extension(GameObject gameObject)
    {
        public bool IsMER => gameObject.GetComponentInParent<SchematicObject>() != null;
    }
}