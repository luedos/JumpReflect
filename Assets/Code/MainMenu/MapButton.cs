using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MainMenues
{
    public class MapButton : MonoBehaviour
    {

        public Image MapImage;
        public Text MapNameText;
        public Text MaxScoreText;

        public int levelKey;

        public void UpdateInfos()
        {
            if (!DataManipulators.MainData.GetInstance().myLevelData.ContainsKey(levelKey))
                return;

            DataManipulators.MainData.LevelProps localLP = DataManipulators.MainData.GetInstance().myLevelData[levelKey];

            float ratio = localLP.LevelImage.rect.width / localLP.LevelImage.rect.height;

            if (160f / 90f > ratio)
                MapImage.rectTransform.sizeDelta = new Vector2(90 * ratio, 90);
            else
                MapImage.rectTransform.sizeDelta = new Vector2(160, 160 / ratio);

            MapImage.sprite = localLP.LevelImage;
            MapNameText.text = localLP.LevelName;

            MaxScoreText.text = DataManipulators.MainData.GetInstance().GetMaxScore(
                                        DataManipulators.MainData.GetInstance().GetMainPlayer(),
                                        localLP.LevelName, localLP.LevelHash).ToString();

        }

        public void ButtonClick()
        {
            DataManipulators.MainData.GetInstance().LoadLevel(levelKey);

        }
    }
}