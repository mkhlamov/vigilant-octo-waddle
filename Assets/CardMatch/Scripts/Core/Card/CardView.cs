using UnityEngine;
using UnityEngine.UI;

namespace CardMatch.Card
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] 
        private Image cardImage;
        
        [SerializeField]
        private Button cardButton;
        
        public void Initialize(Color color)
        {
            cardImage.color = color;
        }
    }
}