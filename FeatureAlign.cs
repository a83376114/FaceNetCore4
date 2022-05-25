using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 特征值对齐示例
namespace FaceNetCore4
{
    class FeatureAlign
    {
        // 传入特征值当二进制保存到MemoryStream
        public static void feature2stream(BDFaceFeature f)
        {
            MemoryStream mem_stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(mem_stream);
            foreach (float value in f.data)
            {
                writer.Write(value);
            }
        }
        // 传入特征值当二进制文件保存
        public static void feature2file(BDFaceFeature f)
        {
            // 以下代码为保存成二进制文件中
            using (FileStream file = File.Create("feature.txt"))
            {
                using (BinaryWriter writer = new BinaryWriter(file))
                {
                    foreach (float value in f.data)
                    {
                        writer.Write(value);
                    }
                }
            }
        }
        // 从二进制txt中读取特征值，为如上保存的数据二进制文件
        public void file2feature()
        {
            float[] features; // 128个float数组
            String filename = "feature.txt";
            using (FileStream fs = File.OpenRead(filename))
            {
                int count = (int)fs.Length / sizeof(float);
                Console.WriteLine("count is:{0}", count);
                features = new float[count];
                using (BinaryReader br = new BinaryReader(fs))
                {
                    for (int i = 0; i < count; i++)
                    {
                        features[i] = br.ReadSingle();
                        Console.WriteLine("features-txt is:{0}", features[i]);
                    }
                }

            }
        }
    }
}
