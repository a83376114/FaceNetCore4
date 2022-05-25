using System;
using System.Runtime.InteropServices;
using OpenCvSharp;

// 人脸扣图
namespace FaceNetCore4
{
    // 扣图配置参数结构体        
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct BDFaceCropFaceConf
    {
        public int is_flat;         // 是否镜像
        public int crop_size;       // 抠图大小
        public float enlarge_ratio; // 抠图的倍数
    };
    //  人脸扣图
    class FaceCrop
    {
        [DllImport("BaiduFaceApi.dll", EntryPoint = "face_crop", CharSet = CharSet.Ansi
            , CallingConvention = CallingConvention.Cdecl)]
        private static extern int face_crop(IntPtr out_mat, IntPtr mat);

        public void test_get_face_crop()
        {
            string img_rgb = @"D:\Gun\test.jpg";          
            Mat mat = Cv2.ImRead(img_rgb);
            get_face_crop(mat);
        }
        // 人脸扣图
        public void get_face_crop(Mat mat)
        {
            Mat out_mat = new Mat();
            int res = face_crop(out_mat.CvPtr, mat.CvPtr);
            if (res == 0)
            {
                // 输出的图片cols
                Console.WriteLine("out_mat cols is {0}:", out_mat.Cols);
                // 输出的图片rows
                Console.WriteLine("out_mat rows is {0}:", out_mat.Rows);
                // 图片保存到本地
                out_mat.ImWrite("crop.jpg");
            }
            else
            {
                Console.WriteLine("dark enhance fail");
            }
        }

        public Mat gen_face_crop(Mat mat)
        {
            Mat out_mat = new Mat();
            int res = face_crop(out_mat.CvPtr, mat.CvPtr);
            
            return out_mat;
        }
    }
}
