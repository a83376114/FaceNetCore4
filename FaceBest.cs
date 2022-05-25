using System;
using System.Runtime.InteropServices;
using OpenCvSharp;

// 最优人脸检测
namespace FaceNetCore4
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    // 最优人脸
    struct BDFaceBest
    {
        public float score; //置信度分值
    };
    /**
        * @brief   
        */

    // 最优人脸
    class FaceBest
    {
        [DllImport("BaiduFaceApi.dll", EntryPoint = "face_best", CharSet = CharSet.Ansi
            , CallingConvention = CallingConvention.Cdecl)]
        private static extern int face_best(IntPtr ptr, IntPtr mat);

        public void test_get_face_best()
        {
            string img_rgb = "../images/rgb.png";            
            Mat mat = Cv2.ImRead(img_rgb);
            get_face_best(mat);
        }
        // 最优人脸检测
        public void get_face_best(Mat mat)
        {
            int max_face_num = 50; // 设置最多检测跟踪人数（多人脸检测），默认为1，最多可设为50

            BDFaceBest[] info = new BDFaceBest[max_face_num];
            int size = Marshal.SizeOf(typeof(BDFaceBest));
            IntPtr ptT = Marshal.AllocHGlobal(size * max_face_num);

            int faceNum = face_best(ptT, mat.CvPtr);
            Console.WriteLine("faceNum is:" + faceNum);
            for (int index = 0; index < faceNum; index++)
            {
                IntPtr ptr = new IntPtr();
                if (8 == IntPtr.Size)
                {
                    ptr = (IntPtr)(ptT.ToInt64() + size * index);
                }
                else if (4 == IntPtr.Size)
                {
                    ptr = (IntPtr)(ptT.ToInt32() + size * index);
                }

                info[index] = (BDFaceBest)Marshal.PtrToStructure(ptr, typeof(BDFaceBest));
                // 最优人脸检测分值
                Console.WriteLine("face best score is {0}:", info[index].score);
            }
            Marshal.FreeHGlobal(ptT);
        }

        //通过usb摄像头获取最佳人脸图片
        public void usb_get_best_face()
        {
            // 笔记本摄像头id，默认为0，若插入了外置摄像头，dev可能为1
            int dev = 0;
            using (var window = new Window("face"))
            using (VideoCapture cap = VideoCapture.FromCamera(dev))
            {
                if (!cap.IsOpened())
                {
                    Console.WriteLine("open camera error");
                    return;
                }
                // Frame image buffer
                Mat image = new Mat();
                // When the movie playback reaches end, Mat.data becomes NULL.
                while (true)
                {
                    cap.Read(image); // same as cvQueryFrame
                    if (!image.Empty())
                    {
                        get_face_best(image);
                        window.ShowImage(image);
                        Cv2.WaitKey(1);
                    }
                    else
                    {
                        Console.WriteLine("mat is empty");
                    }
                }             
            }
        }
    }
}
