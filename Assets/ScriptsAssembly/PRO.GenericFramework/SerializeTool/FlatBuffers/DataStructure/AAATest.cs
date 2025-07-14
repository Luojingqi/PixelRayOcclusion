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
            //生成序列化字节数组缓存，容量不够时大小会翻倍
            FlatBufferBuilder builder = new FlatBufferBuilder(1024);
            //假设这些是我们的程序数据
            List<Vector2> posArray = new List<Vector2>() { new Vector2(1.3f, 4.2f), new Vector2(5.3f, 8.4f), new Vector2(10.11f, 31.24f) };
            string name = "心绪Source     请勿转载";
            Span<float> inventory = new float[] { 1 };
            Dictionary<string, int> dic = new Dictionary<string, int>() { { "你好", 1 }, { "世界", 2 }, { "!!", 3 } };


            //在Flat中，非标量数据不能嵌套序列化，每一个必须单独序列化后获取指针，将指针数组传递给需要存储的父对象
            //标量类型数组区分大小端后直接被复制到缓存，非标量数组存储时需要遍历数组重新计算相对于数组头的偏移（似乎是数组头，反正需要重新计算）


            //开始创建结构体数组
            Monster.StartPosArrayVector(builder, posArray.Count);
            //创建的时候字节流正向添加，取出的时候字节流反向取出，所以我们取出的时候从索引高位到低位取出，最后仍然是正向的
            for (int i = 0; i < posArray.Count; i++)
                //循环创建每一个结构体
                Vec2.CreateVec2(builder, posArray[i].x, posArray[i].y);
            //创建完数组后获取数组指针
            VectorOffset posArrayOffset = builder.EndVector();

            StringOffset nameOffset = builder.CreateString(name);

            //往数组中添加基本数据时，我们使用Put方法而不是Add方法，因为StartVector方法已经将数据对齐，而每次调用Add方法都会尝试重新对齐
            Monster.StartInventoryVector(builder, inventory.Length);
            for (int i = 0; i < inventory.Length; i++)
                builder.PutFloat(inventory[i]);
            VectorOffset inventoryOffset = builder.EndVector();

            //字符串的指针的数组，使用栈数组在堆开辟额外空间
            Span<int> KeyOffsetArray = stackalloc int[dic.Count];
            //int的值数组，为了防止字典遍历两次分别获取key和value，所以我们同时开辟两个数组存key和value
            Span<int> ValueArray = stackalloc int[dic.Count];
            int index = 0;
            foreach (var kv in dic)
            {
                //循环创建每一个string并记录指针
                KeyOffsetArray[index] = builder.CreateString(kv.Key).Value;
                ValueArray[index] = kv.Value;
                index++;
            }
            //使用扩展方法创建指针数组，之所以不使用StartVector方法，因为Flat中不能嵌套序列化非标量，无法在创建数组的同时创建string
            VectorOffset dicOffset_Key = builder.CreateVector_Offset(KeyOffsetArray);
            //基本类型使用此方法
            VectorOffset dicOffset_Value = builder.CreateVector_Data(ValueArray);

            //开始生成Monster对象，完成之前不能创建非标量
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
            //完成指针
            int endOffset = Monster.EndMonster(builder).Value;
            //将完成指针放入缓存，反序列化需要此指针
            builder.Finish(endOffset);

            Debug.Log("序列化一次时间" + stopwatch.ElapsedMilliseconds);
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
                Debug.Log("长度" + buffer.Length);
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
            Debug.Log("反序列化一次时间" + stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();

        }


    }
}