using PRO.DataStructure;
using PRO.Disk.Scene;
using PRO.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PRO
{
    /// <summary>
    /// ����ʵ���࣬����Ϸ����ʱ�洢������������
    /// </summary>
    public class SceneEntity
    {
        public SceneCatalog sceneCatalog { get; private set; }
        public SceneEntity(SceneCatalog sceneCatalog)
        {
            this.sceneCatalog = sceneCatalog;
        }
        #region ��ȡ�Ѿ�ʵ�����Ķ���
        private CrossList<Block> BlockInRAM = new CrossList<Block>();
        private CrossList<BackgroundBlock> BackgroundInRAM = new CrossList<BackgroundBlock>();
        /// <summary>
        /// key��guid  value��building
        /// </summary>
        public Dictionary<string, BuildingBase> BuildingInRAM = new Dictionary<string, BuildingBase>();

        public Block GetBlock(Vector2Int blockPos)
        {
            return BlockInRAM[blockPos];
        }
        public BackgroundBlock GetBackground(Vector2Int blockPos)
        {
            return BackgroundInRAM[blockPos];
        }
        public BuildingBase GetBuilding(string guid)
        {
            return BuildingInRAM[guid];
        }
        #endregion

        #region �Ӵ����м����뱣��
        public void LoadBlockData(Vector2Int blockPos)
        {
            if (JsonTool.LoadText($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\block.txt", out string blockText)
                && JsonTool.LoadText($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\background.txt", out string backgroundText))
            {
                Block block = CreateBlock(blockPos);
                BackgroundBlock background = CreateBackground(blockPos);
                BlockToDiskEx.ToRAM(blockText, block, this);
                BlockToDiskEx.ToRAM(backgroundText, background, this);
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
            JsonTool.StoreText($@"{path}\block.txt", BlockToDiskEx.ToDisk(GetBlock(blockPos), this));
            JsonTool.StoreText($@"{path}\background.txt", BlockToDiskEx.ToDisk(GetBackground(blockPos), this));
        }

        public void LoadBuilding(string guid)
        {
            if (JsonTool.LoadText($@"{sceneCatalog.directoryInfo}\Building\{guid}.txt", out string buildingText) &&
                sceneCatalog.buildingTypeDic.TryGetValue(guid, out Type type))
            {

                BuildingBase building = (BuildingBase)Activator.CreateInstance(type, guid);
                building.Deserialize(buildingText, buildingText.Length);
                BuildingInRAM.Add(guid, building);
            }
            else
            {
                Log.Print($"�޷����ؽ���{guid}�����ܽ����ļ�������", Color.red);
            }
        }
        public void SaveBuilding(string guid)
        {
            JsonTool.StoreText($@"{sceneCatalog.directoryInfo}\Building\{guid}.txt", GetBuilding(guid).Serialize());
        }
        #endregion


        public void Unload()
        {

        }
        #region ��������Ŀ���Ϸ����
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
        #endregion
    }
}