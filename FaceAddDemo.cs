using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Runtime.InteropServices;

namespace FaceNetCore4
{
    class FaceAddDemo
    {

        // 人脸注册(传特征值,特征值可参考FaceFeature.cs提取，亦可参考FaceCompare.cs查看特征值的比对)
        [DllImport("BaiduFaceApi.dll", EntryPoint = "user_add", CharSet = CharSet.Ansi
            , CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr user_add(ref BDFaceFeature f1, string user_id, string group_id,
            string user_info = "");


        [DllImport("BaiduFaceApi.dll", EntryPoint = "user_add_by_mat", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr user_add_by_mat(IntPtr mat, string user_id, string group_id, string user_info = "");
        public static void Main1(string[] args)
        {
            String path = "D:\\Gun\\";
            string group_id = "ujn";
            string user_info = "test";
            FaceFeature feature = new FaceFeature();
            DirectoryInfo files = new DirectoryInfo(path);

            foreach (FileInfo file in files.GetFiles("*.jpg"))
            {
                //Console.WriteLine(file.Name);
                string user = Path.GetFileNameWithoutExtension(file.Name);
                //Console.WriteLine(file.FullName);
                //Mat mat = Cv2.ImRead(file.FullName);
                //IntPtr mptr = user_add_by_mat(mat.CvPtr, user, group_id, user_info);
                //string mbuf = Marshal.PtrToStringAnsi(mptr);
                //Console.WriteLine("user_add_by_mat res is:" + mbuf);

                BDFaceFeature[] feaList1 = feature.get_face_feature_by_path(file.FullName);
                if (feaList1 == null)
                {
                    Console.WriteLine("get feature fail");
                    return;
                }
                // 假设测试的图片是1个人，
                BDFaceFeature f1 = feaList1[0];
                // 人脸注册 (传特征值人脸注册，该方法注册不保存人脸图片入库)
                IntPtr ptr = user_add(ref f1, user, group_id, user_info);
                string buf = Marshal.PtrToStringAnsi(ptr);
                Console.WriteLine("user_add res is:" + buf);

            }
        }
    }
}
