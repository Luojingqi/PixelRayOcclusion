using PRO.DataStructure;
using PRO.Disk.Scene;
using PRO.Tool;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PRO
{
    /// <summary>
    /// ����ʵ���࣬����Ϸ����ʱ�洢������������
    /// </summary>
    public class SceneEntity
    {
        private CrossList<Block> BlockInRAM = new CrossList<Block>();
        private CrossList<BackgroundBlock> BackgroundInRAM = new CrossList<BackgroundBlock>();
        /// <summary>
        /// key��guid  value��building
        /// </summary>
        private Dictionary<string, Building> BuildingInRAM = new Dictionary<string, Building>();
        public SceneCatalog sceneCatalog { get; private set; }
        public SceneEntity(SceneCatalog sceneCatalog)
        {
            this.sceneCatalog = sceneCatalog;
        }

        public Block GetBlock(Vector2Int blockPos)
        {
            return BlockInRAM[blockPos];
        }
        public BackgroundBlock GetBackground(Vector2Int blockPos)
        {
            return BackgroundInRAM[blockPos];
        }
        public Building GetBuilding(string guid)
        {
            return BuildingInRAM[guid];
        }

        public void LoadBlockData(Vector2Int blockPos)
        {
            if (JsonTool.LoadText($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\block.json", out string blockText)
                && JsonTool.LoadText($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\background.json", out string backgroundText))
            {
                BlockToDisk blockToDisk = JsonTool.ToObject<BlockToDisk>(blockText);
                BackgroundToDisk backgroundToDisk = JsonTool.ToObject<BackgroundToDisk>(backgroundText);
                Block block = CreateBlock(blockPos);
                BackgroundBlock background = CreateBackground(blockPos);
                for (int x = 0; x < Block.Size.x; x++)
                    for (int y = 0; y < Block.Size.y; y++)
                    {
                        block.SetPixel(Pixel.TakeOut(blockToDisk.allPixel[x, y], new(x, y)), false, false);
                        background.SetPixel(Pixel.TakeOut(backgroundToDisk.allPixel[x, y], new(x, y)));
                    }
                block.DrawPixelAsync();
                background.DrawPixelAsync();

                var colliderDataList = GreedyCollider.CreateColliderDataList(block, new(0, 0), new(Block.Size.x - 1, Block.Size.y - 1)); //������ʵ���Խ��ɶ��̴߳���
                GreedyCollider.CreateColliderAction(block, colliderDataList);

            }
            else
            {
                Log.Print($"�޷���������{blockPos}�����������ļ�������", Color.red);
            }
        }

        public void SaveBlockData(Vector2Int blockPos)
        {
            string path = $@"{sceneCatalog.directoryInfo}\Block\{blockPos}";
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
            BlockToDisk blockToDisk = new BlockToDisk(GetBlock(blockPos));
            BackgroundToDisk backgroundToDisk = new BackgroundToDisk(GetBackground(blockPos));
            JsonTool.StoreObject(@$"{path}\block.json", blockToDisk);
            JsonTool.StoreObject($@"{path}\background.json", backgroundToDisk);
        }

        public void LoadBuilding(string guid)
        {
            if (JsonTool.LoadText($@"{sceneCatalog.directoryInfo}\Building\{guid}.json", out string buildingText))
            {
                Building building = JsonTool.ToObject<Building>(buildingText);
                BuildingInRAM.Add(guid, building);
            }
            else
            {
                Log.Print($"�޷����ؽ���{guid}�����ܽ����ļ�������", Color.red);
            }
        }

        public void Unload()
        {

        }
        /// <summary>
        /// ����һ�������Ϸ���壬�ڲ����ص�����Ϊ��
        /// </summary>
        /// <param name="blockPos"></param>
        /// <returns></returns>
        public Block CreateBlock(Vector2Int blockPos)
        {
            var block = Block.TakeOut();
            block.name = $"Block{blockPos}";
            BlockInRAM[blockPos] = block;
            block.transform.position = Block.BlockToWorld(blockPos);
            block.BlockPos = blockPos;
            return block;
        }
        /// <summary>
        /// ����һ����������Ϸ���壬�ڲ����ص�����Ϊ��
        /// </summary>
        /// <param name="blockPos"></param>
        /// <returns></returns>
        public BackgroundBlock CreateBackground(Vector2Int blockPos)
        {
            var back = BackgroundBlock.TakeOut();
            BackgroundInRAM[blockPos] = back;
            back.transform.position = Block.BlockToWorld(blockPos);
            back.transform.parent = GetBlock(blockPos).transform;
            back.BlockPos = blockPos;
            return back;
        }
    }
}