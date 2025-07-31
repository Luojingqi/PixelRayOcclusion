using PRO;
using PRO.Tool;
using UnityEngine;
using static PRO.BottomBagM;
namespace PRO
{
    public class BottomBagC : UIChildControllerBase
    {
        public override UIChildViewBase View => view;
        private BottomBagV view = new BottomBagV();


        private int MaxColumn => view.MaxColumn;

        private int Row
        {
            get { return view.Row; }
            set
            {
                if (value < RowMinMax.x || value > RowMinMax.y) return;
                view.Row = value;
            }
        }
        public Vector2Int RowMinMax = new Vector2Int(1, 4);

        public GridObjectGroup[] GridGroups;

        public override void Init()
        {
            base.Init();
            GridObject.InitPool(view.GridObject);
            view.EnlargeButton.onClick.AddListener(() => { Row++; });
            view.NarrowButton.onClick.AddListener(() => { Row--; });
            GridGroups = new GridObjectGroup[RowMinMax.y];
            int maxColumn = MaxColumn;
            for (int i = 0; i < GridGroups.Length; i++)
            {
                var group = new GridObjectGroup(i, maxColumn);
                GridGroups[i] = group;
                for (int j = 0; j < maxColumn; j++)
                {
                    group.Grids[j].transform.SetParent(view.GridLayout.transform);
                    group.Grids[j].Number.text = $"{i}__{j}";
                    group.Grids[j].Name.text = null;
                }
            }
        }
        public void ResetGridObjectGroup()
        {
            for (int i = 0; i < RowMinMax.y; i++)
            {
                var group = GridGroups[i]; 
                for (int j = 0; j < MaxColumn; j++)
                {
                    group.Grids[j].transform.SetParent(view.GridLayout.transform);
                    group.Grids[j].Number.text = $"{i}__{j}";
                    group.Grids[j].Name.text = null;
                }
            }
        }
        public void SetRole(Role role)
        {
            ResetGridObjectGroup();
            int r = 0;
            int c = 0;
            //foreach (var operate in role.AllCanUseOperate.Values)
            //{
            //    var grid = GridGroups[r].Grids[c++];
            //    if (c >= GridGroups[r].Grids.Length)
            //    {
            //        r++;
            //        c = 0;
            //    }
            //    operate.BuildUI(grid);
            //}
        }
    }
}