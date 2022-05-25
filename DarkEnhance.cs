using System;
using System.Runtime.InteropServices;
using OpenCvSharp;

// 暗光恢复
namespace FaceNetCore4
{
    //  暗光恢复
    class DarkEnhance
    {      
        [DllImport("BaiduFaceApi.dll", EntryPoint = "dark_enhance", CharSet = CharSet.Ansi
            , CallingConvention = CallingConvention.Cdecl)]
        private static extern int dark_enhance(IntPtr out_mat, IntPtr mat);

        public void test_get_dark_enhance()
        {
            try
            {
                Mat mat = Cv2.ImRead("..\\images\\rgb.png");           
                get_dark_enhance(mat);
                
            }    
            catch(System.IO.FileNotFoundException e)
            {
                Console.WriteLine("file not found exception");
            }  
                             
                                       
        }
        // 暗光恢复
        public void get_dark_enhance(Mat mat)
        {
            Mat out_mat = new Mat();
            int res = dark_enhance(out_mat.CvPtr, mat.CvPtr);
            if (res==0)
            {
                // 输出的图片cols
                Console.WriteLine("out_mat cols is {0}:", out_mat.Cols);
                // 输出的图片rows
                Console.WriteLine("out_mat rows is {0}:", out_mat.Rows);
                // 图片保存到本地
                out_mat.ImWrite("dark_enhance.jpg");
            }
            else
            {
                Console.WriteLine("dark enhance fail");
            }
           
        }
    }
}
