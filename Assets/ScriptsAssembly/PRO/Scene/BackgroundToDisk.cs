﻿using PRO.DataStructure;
using UnityEngine;

namespace PRO.Disk.Scene
{
    /// <summary>
    /// 背景块数据存储到磁盘与从磁盘中加载的类
    /// </summary>
    public class BackgroundToDisk
    {
        public Vector2Int blockPos;
        public PixelToDisk[,] allPixel;
        public BackgroundToDisk() { }
        public BackgroundToDisk(BackgroundBlock block)
        {
            blockPos = block.BlockPos;
            allPixel = new PixelToDisk[Block.Size.x, Block.Size.y];
            for (int y = 0; y < Block.Size.y; y++)
                for (int x = 0; x < Block.Size.x; x++)
                    allPixel[x, y] = new PixelToDisk(block.GetPixel(new Vector2Byte(x, y)));
        }
    }
}
