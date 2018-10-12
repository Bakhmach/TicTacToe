using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace TicTacToe.UI
{
    public class CellClickArgs : System.EventArgs
    {
        public readonly int index;
        public CellClickArgs(int n) { index = n; }
    }

    public class CellUI : MonoBehaviour, IPointerClickHandler
    {
        public event System.EventHandler<CellClickArgs> OnClick = delegate { };

        [SerializeField] private Image image;

        public int Index { get; set; }
        public Sprite Icon
        {
            get { return image.sprite; }
            set
            {
                image.sprite = value;
                image.enabled = value != null;
            }
        }

        void Start()
        {
            Icon = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick(this, new CellClickArgs(Index));
        }
    }
}