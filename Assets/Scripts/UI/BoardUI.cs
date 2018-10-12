using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TicTacToe.Core;

namespace TicTacToe.UI
{
    public class BoardUI : MonoBehaviour
    {
        public event System.EventHandler<CellClickArgs> OnClick = delegate { };

        [SerializeField] private int columns;
        [SerializeField] private int rows;

        [SerializeField] private GridLayoutGroup grid;
        [SerializeField] private RectTransform contentRect;
        [SerializeField] private CanvasGroup canvasGroup;

        [SerializeField] private CellUI cellPrefab;
        private CellUI[] cells;

        [SerializeField] private Sprite crossSprite;
        [SerializeField] private Sprite noughtSprite;

        public bool Lock
        {
            get; set;
        }

        void Start()
        {
            CreateCells();
        }

        void CreateCells()
        {
            int cellCount = columns * rows;
            float width = contentRect.rect.width - grid.padding.right - grid.padding.left - grid.spacing.x * (columns - 1);
            float height = contentRect.rect.height - grid.padding.top - grid.padding.bottom - grid.spacing.y * (rows - 1);

            grid.cellSize = new Vector2(width / columns, height / rows);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;

            cells = new CellUI[cellCount];
            for (int i = 0; i < cellCount; ++i)
            {
                cells[i] = Instantiate(cellPrefab, contentRect);
                cells[i].Index = i;
                cells[i].OnClick += CellOnClick;
            }
        }

        private void CellOnClick(object sender, CellClickArgs e)
        {
            if(!Lock)
                OnClick(sender, e);
        }

        public void MarkCell(int n, CellSign sign)
        {
            cells[n].Icon = sign == CellSign.None ? null : sign == CellSign.Cross ? crossSprite : noughtSprite;
        }

        public bool IsMarked(int n)
        {
            return cells[n].Icon != null;
        }

        public void Clear()
        {
            for(int i = 0; i < cells.Length; ++i)
            {
                cells[i].Icon = null;
            }
        }
    }
}
