using System;
using UnityEngine;
using UnityEngine.Localization.Metadata;

namespace MaouSuika.Core
{
    [Serializable]
    [Metadata(AllowedTypes = MetadataType.Locale)]
    public class LocalePresentationMetadata : IMetadata
    {
        [SerializeField] private Sprite icon;

        public Sprite Icon => icon;
    }
}