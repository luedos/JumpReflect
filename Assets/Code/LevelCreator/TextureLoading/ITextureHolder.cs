using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator
{
    public interface ITextureHolder
    {
        void SetCurrentTexture(TextureButton textureButton);
    }
}