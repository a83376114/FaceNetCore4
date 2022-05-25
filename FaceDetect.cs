using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using OpenCvSharp;

namespace FaceNetCore4
{
    // 人脸检测配置
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct DetectConf
    {
        public int max_detect_num;                     // 需要检测的最大人脸数目,推荐10
        public float min_face_size;                    // 需要检测的最小人脸大小，若有小图检测不到人脸，可设为30
        public float not_face_threshold;               // 过滤非人脸的阈值
        public float scale_ratio;                      // 输入图像的缩放系数
                                                       //（检测分值大于该阈值认为是人脸）
    };
    // 人脸检测
    class FaceDetect
    {
        //人脸检测 type 0： 表示rgb 人脸检测 1：表示nir人脸检测
        [DllImport("BaiduFaceApi.dll", EntryPoint = "detect", CharSet = CharSet.Ansi
           , CallingConvention = CallingConvention.Cdecl)]
        public static extern int detect(IntPtr ptr, IntPtr mat, int type);

        // 测试人脸检测
        public void image_detect()
        {
            Console.WriteLine("test_face_detect");
            int max_detect_num = 50; // 设置最多检测跟踪人数（多人脸检测），默认为1，最多可设为50
            // type 0： 表示rgb 人脸检测 1：表示nir人脸检测
            int type = 0;         
            BDFaceBBox[] info = new BDFaceBBox[max_detect_num];
            
            int sizeTrack = Marshal.SizeOf(typeof(BDFaceBBox));
            IntPtr ptT = Marshal.AllocHGlobal(sizeTrack * max_detect_num);

            string img_rgb = "../images/1.jpg";                 
            Mat mat = Cv2.ImRead(img_rgb);
            int faceNum = detect(ptT, mat.CvPtr, type);
            Console.WriteLine("faceNum is:" + faceNum);
            // 因为需预分配内存，所以返回的人脸数若大于预先分配的内存数，则仅仅显示预分配的人脸数
            if (faceNum > max_detect_num)
            {
                faceNum = max_detect_num;
            }
            for (int index = 0; index < faceNum; index++)
            {

                IntPtr ptr = new IntPtr();
                if (8 == IntPtr.Size)
                {
                    ptr = (IntPtr)(ptT.ToInt64() + sizeTrack * index);
                }
                else if (4 == IntPtr.Size)
                {
                    ptr = (IntPtr)(ptT.ToInt32() + sizeTrack * index);
                }

                info[index] = (BDFaceBBox)Marshal.PtrToStructure(ptr, typeof(BDFaceBBox));
          
                // 索引值
                Console.WriteLine("detect index is:{0}", info[index].index);
                // 置信度
                Console.WriteLine("detect score is:{0}", info[index].score);
                // 角度
                Console.WriteLine("detect mAngle is:{0}", info[index].angle);
                // 人脸宽度
                Console.WriteLine("detect mWidth is:{0}", info[index].width);
                // 中心点X,Y坐标
                Console.WriteLine("detect mCenter_x is:{0}", info[index].center_x);
                Console.WriteLine("detect mCenter_y is:{0}", info[index].center_y);
               
               
            }
            // 检测后画框图片可保存至本地
            FaceDraw.draw_rects(ref mat, faceNum, info);
            mat.ImWrite("detect.jpg");
            Marshal.FreeHGlobal(ptT);
        }

        //C#测试usb摄像头实时人脸检测
        public void usb_video_detect()
        {
            // 默认电脑自带摄像头，device可能为0，若外接usb摄像头，则device可能为1.
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
                        int ilen = 10;//传入的人脸数
                        BDFaceBBox[] info = new BDFaceBBox[ilen];
                        
                        int sizeTrack = Marshal.SizeOf(typeof(BDFaceBBox));
                        IntPtr ptT = Marshal.AllocHGlobal(sizeTrack * ilen);
                       
                        int faceSize = ilen;//返回人脸数  分配人脸数和检测到人脸数的最小值
                        int curSize = ilen;//当前人脸数 输入分配的人脸数，输出实际检测到的人脸数
                        int type = 0;
                        faceSize = detect(ptT, image.CvPtr, type);
                        for (int index = 0; index < faceSize; index++)
                        {
                            IntPtr ptr = new IntPtr();
                            if (8 == IntPtr.Size)
                            {
                                ptr = (IntPtr)(ptT.ToInt64() + sizeTrack * index);
                            }
                            else if (4 == IntPtr.Size)
                            {
                                ptr = (IntPtr)(ptT.ToInt32() + sizeTrack * index);
                            }

                            info[index] = (BDFaceBBox)Marshal.PtrToStructure(ptr, typeof(BDFaceBBox));
                            //face_info[index] = (BDFaceBBox)Marshal.PtrToStructure(info_ptr, typeof(BDFaceBBox));
                            Console.WriteLine("in face detect face_id is {0}:", info[index].index);                        

                            Console.WriteLine("in face detect score is:{0:f}", info[index].score);
                            // 角度
                            Console.WriteLine("in face detect mAngle is:{0:f}", info[index].angle);
                            // 人脸宽度
                            Console.WriteLine("in face detect mWidth is:{0:f}", info[index].width);
                            // 中心点X,Y坐标
                            Console.WriteLine("in face detect mCenter_x is:{0:f}", info[index].center_x);
                            Console.WriteLine("in face detect mCenter_y is:{0:f}", info[index].center_y);                           
                        }
                                           
                        FaceDraw.draw_rects(ref image, faceSize, info);
                        Marshal.FreeHGlobal(ptT);
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
