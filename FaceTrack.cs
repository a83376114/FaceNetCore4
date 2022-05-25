using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using OpenCvSharp;

// 人脸跟踪
namespace FaceNetCore4
{  
    // 人脸跟踪配置结构体        
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct BDFaceTrackConf
    {
        public float detect_intv_before_track;     // 未跟踪到人脸前的检测时间间隔
        public float detect_intv_during_track;     // 已跟踪到人脸后的检测时间间隔
    };


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BDFaceBBox
    {
        public int index;  // 人脸索引值
        public float center_x; // 人脸中心点x坐标
        public float center_y; // 人脸中心点y坐标
        public float width; // 人脸宽度
        public float height; // 人脸高度
        public float angle; // 人脸角度            
        public float score;  // 人脸置信度
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
     public struct BDFaceLandmark
    {
         public int index; // 人脸关键点索引值
         public int size; // 人脸关键点数量
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 144)]
         public float[] data;// = new float[144];
         public float score; // 人脸关键点置信度
    }
     [StructLayout(LayoutKind.Sequential, Pack = 1)]
     public struct BDFaceTrackInfo
     {
         public int face_id;
         [MarshalAs(UnmanagedType.Struct)]
         public BDFaceBBox box;
         [MarshalAs(UnmanagedType.Struct)]
         public BDFaceLandmark landmark;
     } 
   
    // 人脸跟踪
    class FaceTrack
    {
        [DllImport("BaiduFaceApi.dll", EntryPoint = "track", CharSet = CharSet.Ansi
           , CallingConvention = CallingConvention.Cdecl)]
        // type 为0时候执行RGB人脸跟踪，1时候执行NIR人脸跟踪
        public static extern int track(IntPtr ptr, IntPtr mat, int type);      

        [DllImport("BaiduFaceApi.dll", EntryPoint = "clear_track_history", CharSet = CharSet.Ansi
         , CallingConvention = CallingConvention.Cdecl)]
        // type 为0时候执行RGB人脸跟踪，1时候执行NIR人脸跟踪
        public static extern void clear_track_history(int type);

        // 测试人脸跟踪
        public void image_track()
        {
            Console.WriteLine("test_track");
            int max_track_num = 50; // 设置最多检测跟踪人数（多人脸检测），默认为1，最多可设为50

            BDFaceTrackInfo[] track_info = new BDFaceTrackInfo[max_track_num];
            for (int i = 0; i < max_track_num; i++)
            {
                track_info[i] = new BDFaceTrackInfo();
                track_info[i].box = new BDFaceBBox();
                track_info[i].box.score = 0;
                track_info[i].box.width = 0;
                track_info[i].landmark.data = new float[144];             
                track_info[i].face_id = 0;                
            }
            int sizeTrack = Marshal.SizeOf(typeof(BDFaceTrackInfo));
            IntPtr ptT = Marshal.AllocHGlobal(sizeTrack * max_track_num);
            Mat mat = Cv2.ImRead("../images/2.jpg");
            // faceNum为返回的检测到的人脸个数
            int type = 0;
            int faceNum = track(ptT, mat.CvPtr, type);
            Console.WriteLine("faceSize is:" + faceNum);
            // 因为需预分配内存，所以返回的人脸数若大于预先分配的内存数，则仅仅显示预分配的人脸数
            if (faceNum > max_track_num)
            {
                faceNum = max_track_num;
            }
            for (int index = 0; index < faceNum; index++) { 

                IntPtr ptr = new IntPtr();
                if (8 == IntPtr.Size)
                {
                    ptr = (IntPtr)(ptT.ToInt64() + sizeTrack * index);
                }
                else if (4 == IntPtr.Size)
                {
                    ptr = (IntPtr)(ptT.ToInt32() + sizeTrack * index);
                }

                track_info[index] = (BDFaceTrackInfo)Marshal.PtrToStructure(ptr, typeof(BDFaceTrackInfo));
                Console.WriteLine("track face_id is {0}:", track_info[index].face_id);
                Console.WriteLine("track landmarks is:");

                for(int i = 0; i < 144; i++)
                {
                    Console.WriteLine("lanmark data is {0}:", track_info[index].landmark.data[i]);
                }
                Console.WriteLine("track landmarks score is:{0}", track_info[index].landmark.score);
                Console.WriteLine("track landmarks index is:{0}", track_info[index].landmark.index);

                // 索引值
                Console.WriteLine("track score is:{0}", track_info[index].box.index);
                // 置信度
                Console.WriteLine("track score is:{0}", track_info[index].box.score);
                // 角度
                Console.WriteLine("track mAngle is:{0}", track_info[index].box.angle);
                // 人脸宽度
                Console.WriteLine("track mWidth is:{0}", track_info[index].box.width);
                // 中心点X,Y坐标
                Console.WriteLine("track mCenter_x is:{0}", track_info[index].box.center_x);
                Console.WriteLine("track mCenter_y is:{0}", track_info[index].box.center_y);             
            }
            // 画人脸框
            FaceDraw.draw_rects(ref mat, faceNum, track_info);
            // 图片画框保存
            mat.ImWrite("track.jpg");
            Marshal.FreeHGlobal(ptT);          
        }
       
        //usb摄像头实时人脸检测示例
        public void usb_video_track()
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
                        BDFaceTrackInfo[] track_info = new BDFaceTrackInfo[ilen];
                        for (int i = 0; i < ilen; i++)
                        {
                            track_info[i].box = new BDFaceBBox();
                            track_info[i].box.score = 0;
                            track_info[i].box.width = 0;
                            track_info[i].landmark.data = new float[144];                          
                            track_info[i].face_id = 0;
                        }
                        int sizeTrack = Marshal.SizeOf(typeof(BDFaceTrackInfo));
                        IntPtr ptT = Marshal.AllocHGlobal(sizeTrack* ilen);                     
                        //Cv2.ImWrite("usb_track_Cv2.jpg", image);
                        /*  trackMat
                         *  传入参数: maxTrackObjNum:检测到的最大人脸数，传入外部分配人脸数，需要分配对应的内存大小。
                         *            传出检测到的最大人脸数
                         *    返回值: 传入的人脸数 和 检测到的人脸数 中的最小值,实际返回的人脸。
                         ****/
                        int faceSize = ilen;//返回人脸数  分配人脸数和检测到人脸数的最小值
                        int curSize = ilen;//当前人脸数 输入分配的人脸数，输出实际检测到的人脸数
                        int type = 0;
                        faceSize = track(ptT, image.CvPtr, type);
                        for (int index = 0; index < faceSize; index++)
                        {
                            IntPtr ptr = new IntPtr();
                            if( 8 == IntPtr.Size)
                            {
                                ptr = (IntPtr)(ptT.ToInt64() + sizeTrack * index);
                            }
                            else if(4 == IntPtr.Size)
                            {
                                ptr = (IntPtr)(ptT.ToInt32() + sizeTrack * index);
                            }
                            
                            track_info[index] = (BDFaceTrackInfo)Marshal.PtrToStructure(ptr, typeof(BDFaceTrackInfo));
                            //face_info[index] = (BDFaceBBox)Marshal.PtrToStructure(info_ptr, typeof(BDFaceBBox));
                            Console.WriteLine("in Liveness::usb_track face_id is {0}:",track_info[index].face_id);
                            Console.WriteLine("in Liveness::usb_track landmarks is:");
                                                                              
                            Console.WriteLine("in Liveness::usb_track score is:{0:f}", track_info[index].box.score);
                            // 角度
                            Console.WriteLine("in Liveness::usb_track mAngle is:{0:f}", track_info[index].box.angle);
                            // 人脸宽度
                            Console.WriteLine("in Liveness::usb_track mWidth is:{0:f}", track_info[index].box.width);
                            // 中心点X,Y坐标
                            Console.WriteLine("in Liveness::usb_track mCenter_x is:{0:f}", track_info[index].box.center_x);
                            Console.WriteLine("in Liveness::usb_track mCenter_y is:{0:f}", track_info[index].box.center_y);
                                                   
                        }
                       
                        FaceDraw.draw_rects(ref image, faceSize, track_info);
                        FaceDraw.draw_shape(ref image, faceSize, track_info);
                        Marshal.FreeHGlobal(ptT);
                        window.ShowImage(image);
                        Cv2.WaitKey(1);
                        Console.WriteLine("mat not empty");
                    }
                    else
                    {
                        Console.WriteLine("mat is empty");
                    }
                }              
            }
        }
       
         // 清除跟踪的人脸信息
         public void test_clear_tracked_faces()
         {
            int type = 0;
            clear_track_history(type);
            Console.WriteLine("after clear tracked faces");
        }

     }
 }
