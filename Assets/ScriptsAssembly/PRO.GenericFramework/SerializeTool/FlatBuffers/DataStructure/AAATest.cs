using Google.FlatBuffers;
using MyGame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
namespace PRO
{
    public class AAATest : MonoBehaviour
    {
        [Button]
        public void B()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            //�������л��ֽ����黺�棬��������ʱ��С�ᷭ��
            FlatBufferBuilder builder = new FlatBufferBuilder(1024);
            //������Щ�����ǵĳ�������
            List<Vector2> posArray = new List<Vector2>() { new Vector2(1.3f, 4.2f), new Vector2(5.3f, 8.4f), new Vector2(10.11f, 31.24f) };
            string name = "����Source     ����ת��";
            Span<float> inventory = new float[] { 1 };
            Dictionary<string, int> dic = new Dictionary<string, int>() { { "���", 1 }, { "����", 2 }, { "!!", 3 } };


            //��Flat�У��Ǳ������ݲ���Ƕ�����л���ÿһ�����뵥�����л����ȡָ�룬��ָ�����鴫�ݸ���Ҫ�洢�ĸ�����
            //���������������ִ�С�˺�ֱ�ӱ����Ƶ����棬�Ǳ�������洢ʱ��Ҫ�����������¼������������ͷ��ƫ�ƣ��ƺ�������ͷ��������Ҫ���¼��㣩


            //��ʼ�����ṹ������
            Monster.StartPosArrayVector(builder, posArray.Count);
            //������ʱ���ֽ���������ӣ�ȡ����ʱ���ֽ�������ȡ������������ȡ����ʱ���������λ����λȡ���������Ȼ�������
            for (int i = 0; i < posArray.Count; i++)
                //ѭ������ÿһ���ṹ��
                Vec2.CreateVec2(builder, posArray[i].x, posArray[i].y);
            //������������ȡ����ָ��
            VectorOffset posArrayOffset = builder.EndVector();

            StringOffset nameOffset = builder.CreateString(name);

            //����������ӻ�������ʱ������ʹ��Put����������Add��������ΪStartVector�����Ѿ������ݶ��룬��ÿ�ε���Add�������᳢�����¶���
            Monster.StartInventoryVector(builder, inventory.Length);
            for (int i = 0; i < inventory.Length; i++)
                builder.PutFloat(inventory[i]);
            VectorOffset inventoryOffset = builder.EndVector();

            //�ַ�����ָ������飬ʹ��ջ�����ڶѿ��ٶ���ռ�
            Span<int> KeyOffsetArray = stackalloc int[dic.Count];
            //int��ֵ���飬Ϊ�˷�ֹ�ֵ�������ηֱ��ȡkey��value����������ͬʱ�������������key��value
            Span<int> ValueArray = stackalloc int[dic.Count];
            int index = 0;
            foreach (var kv in dic)
            {
                //ѭ������ÿһ��string����¼ָ��
                KeyOffsetArray[index] = builder.CreateString(kv.Key).Value;
                ValueArray[index] = kv.Value;
                index++;
            }
            //ʹ����չ��������ָ�����飬֮���Բ�ʹ��StartVector��������ΪFlat�в���Ƕ�����л��Ǳ������޷��ڴ��������ͬʱ����string
            VectorOffset dicOffset_Key = builder.CreateVector_Offset(KeyOffsetArray);
            //��������ʹ�ô˷���
            VectorOffset dicOffset_Value = builder.CreateVector_Data(ValueArray);

            //��ʼ����Monster�������֮ǰ���ܴ����Ǳ���
            Monster.StartMonster(builder);
            Monster.AddPos(builder, Vec2.CreateVec2(builder, 1.2f, -2.4f));
            Monster.AddPosArray(builder, posArrayOffset);
            Monster.AddMana(builder, 10);
            Monster.AddName(builder, nameOffset);
            Monster.AddFriendly(builder, true);
           // Monster.AddInventory(builder, inventoryOffset);
            Monster.AddColor(builder, MyGame.Color.Red);
            Monster.AddDicKey(builder, dicOffset_Key);
            Monster.AddDicValue(builder, dicOffset_Value);
            //���ָ��
            int endOffset = Monster.EndMonster(builder).Value;
            //�����ָ����뻺�棬�����л���Ҫ��ָ��
            builder.Finish(endOffset);

            Debug.Log("���л�һ��ʱ��" + stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();
            using (var fileStream = new FileStream(@"D:\test.txt", FileMode.Create, FileAccess.Write))
            {
                fileStream.Write(builder.DataBuffer.ToSpan(builder.DataBuffer.Position, builder.Offset));
            }

            byte[] buffer = null;
            using (var fileStream = new FileStream(@"D:\test.txt", FileMode.Open, FileAccess.Read))
            {
                buffer = ArrayPool<byte>.Shared.Rent((int)fileStream.Length);
                fileStream.Read(buffer);
                Debug.Log("����" + buffer.Length);
            }

            Monster monster = Monster.GetRootAsMonster(new ByteBuffer(buffer));
            StringBuilder sb = new StringBuilder();
            sb.Append("Position: ").Append(monster.Pos.Value.X).Append(' ').Append(monster.Pos.Value.Y).AppendLine();
            sb.Append("Position Array:").AppendLine();
            for (int i = monster.PosArrayLength - 1; i >= 0; i--)
            {
                var pos = monster.PosArray(i);
                sb.Append("  ").Append(pos.Value.X).Append(' ').Append(pos.Value.Y).AppendLine();
            }
            sb.Append("Mana: ").Append(monster.Mana).AppendLine();
            sb.Append("Name: ").Append(monster.Name).AppendLine();
            sb.Append("Friendly: ").Append(monster.Friendly).AppendLine();

            sb.Append("Inventory:").AppendLine();
            for (int i = monster.InventoryLength - 1; i > 0; i--)
            {
                sb.Append("  ").Append(monster.Inventory(i)).AppendLine();
            }

            sb.Append("Color: ").Append(monster.Color).AppendLine();

            sb.Append("Dictionary:").AppendLine();
            for (int i = monster.DicKeyLength - 1; i >= 0; i--)
            {
                sb.Append("  Key: ").Append(monster.DicKey(i))
                  .Append(", Value: ").Append(monster.DicValue(i)).AppendLine();
            }
            Debug.Log(sb.ToString());
            Debug.Log("�����л�һ��ʱ��" + stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();

        }


    }
}