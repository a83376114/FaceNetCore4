using System;
using System.Runtime.InteropServices;
using System.IO;
using OpenCvSharp;

// 人脸比对（备注：人脸比对，实际上是人脸的特征值比对，提取出人脸特征值，用compare_feature方法比对)
namespace FaceNetCore4
{
    // 人脸比较1:1、1:N、抽取人脸特征值、按特征值比较等
    class FaceCompare
    {
        // 特征值比对（传2个人脸的特征值,所有的1:1比对归根结底都是特征值比对，都可通过提取特征值来比对）
        [DllImport("BaiduFaceApi.dll", EntryPoint = "compare_feature", CharSet = CharSet.Ansi
            , CallingConvention = CallingConvention.Cdecl)]
        // type为0表示提取生活照的特征值，1表示证件照的特征值，2表示nir的特征值
        private static extern float compare_feature(ref BDFaceFeature f1, ref BDFaceFeature f2, int type = 0);

        // 人脸1:1比对（传2个人脸的opencv图片帧进行比对）
        [DllImport("BaiduFaceApi.dll", EntryPoint = "match", CharSet = CharSet.Ansi
            , CallingConvention = CallingConvention.Cdecl)]
        // type为0表示提取生活照的特征值，1表示证件照的特征值，2表示nir的特征值
        private static extern int match(IntPtr mat1, IntPtr mat2, int type = 0);

        // 1:N人脸识别（传人脸特征值和库里的比对, 人脸库可参考FaceManager）
        [DllImport("BaiduFaceApi.dll", EntryPoint = "identify", CharSet = CharSet.Ansi
          , CallingConvention = CallingConvention.Cdecl)]
        // type为0表示提取生活照的特征值，1表示证件照的特征值，2表示nir的特征值
        private static extern IntPtr identify(ref BDFaceFeature f1, string group_id_list,
            string user_id, int type = 0);

        // 1:N人脸识别（传人opencv图片帧和库里的比对, 人脸库可参考FaceManager）
        [DllImport("BaiduFaceApi.dll", EntryPoint = "identify_by_mat", CharSet = CharSet.Ansi
          , CallingConvention = CallingConvention.Cdecl)]
        // type为0表示提取生活照的特征值，1表示证件照的特征值，2表示nir的特征值
        private static extern IntPtr identify_by_mat(IntPtr mat, string group_id_list,
            string user_id, int type = 0);

        // 提前加载库里所有数据到内存中
        [DllImport("BaiduFaceApi.dll", EntryPoint = "load_db_face", CharSet = CharSet.Ansi
          , CallingConvention = CallingConvention.Cdecl)]
        public static extern bool load_db_face();

        // 1:N人脸识别（特征值和内存已加载的整个库数据比对）
        [DllImport("BaiduFaceApi.dll", EntryPoint = "identify_with_all", CharSet = CharSet.Ansi
          , CallingConvention = CallingConvention.Cdecl)]
        // type为0表示提取生活照的特征值，1表示证件照的特征值，2表示nir的特征值
        private static extern IntPtr identify_with_all(ref BDFaceFeature f1, int type = 0);

        // 1:N人脸识别（opencv图片帧和内存已加载的整个库数据比对）
        [DllImport("BaiduFaceApi.dll", EntryPoint = "identify_with_all_by_mat", CharSet = CharSet.Ansi
          , CallingConvention = CallingConvention.Cdecl)]
        // type为0表示提取生活照的特征值，1表示证件照的特征值，2表示nir的特征值
        private static extern IntPtr identify_with_all_by_mat(IntPtr mat, int type = 0);

        //C#测试usb摄像头实时人脸识别
        public void usb_video_identify()
        {
            // 默认电脑自带摄像头，device可能为0，若外接usb摄像头，则device可能为1.
            VideoCapture cap = VideoCapture.FromCamera(1);
            if (!cap.IsOpened())
            {
                cap = VideoCapture.FromCamera(0);
            }
            using (var window = new Window("face"))
            using (cap)
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
                    string user_group = "test_group";


                    cap.Read(image); // same as cvQueryFrame
                    if (!image.Empty())
                    {
                        // string user_id = "test_user";
                        //string user_id = "";

                        //// type为0表示提取生活照的特征值，1表示证件照的特征值，2表示nir的特征值 (type需要和前面特征值提取的一致，否则会出错)
                        //IntPtr ptr = identify_by_mat(image.CvPtr, user_group, user_id);
                        //string buf = Marshal.PtrToStringAnsi(ptr);
                        //Console.WriteLine("identify res is:" + buf);

                        load_db_face();
                        window.ShowImage(image);
                        // type为0表示提取生活照的特征值，1表示证件照的特征值，2表示nir的特征值 (type需要和前面特征值提取的一致，否则会出错)
                        int type = 0;
                        IntPtr ptr1 = identify_with_all_by_mat(image.CvPtr, type);
                        string buf1 = Marshal.PtrToStringAnsi(ptr1);
                        Console.WriteLine("identify with all db res is:" + buf1);
                    }
                    //if (!image.Empty())
                    //{
                    //    int ilen = 10;//传入的人脸数
                    //    BDFaceBBox[] info = new BDFaceBBox[ilen];

                    //    int sizeTrack = Marshal.SizeOf(typeof(BDFaceBBox));
                    //    IntPtr ptT = Marshal.AllocHGlobal(sizeTrack * ilen);

                    //    int faceSize = ilen;//返回人脸数  分配人脸数和检测到人脸数的最小值
                    //    int curSize = ilen;//当前人脸数 输入分配的人脸数，输出实际检测到的人脸数
                    //    int type = 0;
                    //    faceSize = detect(ptT, image.CvPtr, type);
                    //    IntPtr ptr1 = identify_by_mat(image.CvPtr, user_group, "");
                    //    string buf = Marshal.PtrToStringAnsi(ptr1);
                    //    Console.WriteLine("identify res is:" + buf);
                    //    for (int index = 0; index < faceSize; index++)
                    //    {
                    //        IntPtr ptr = new IntPtr();
                    //        if (8 == IntPtr.Size)
                    //        {
                    //            ptr = (IntPtr)(ptT.ToInt64() + sizeTrack * index);
                    //        }
                    //        else if (4 == IntPtr.Size)
                    //        {
                    //            ptr = (IntPtr)(ptT.ToInt32() + sizeTrack * index);
                    //        }

                    //        info[index] = (BDFaceBBox)Marshal.PtrToStructure(ptr, typeof(BDFaceBBox));
                    //        //face_info[index] = (BDFaceBBox)Marshal.PtrToStructure(info_ptr, typeof(BDFaceBBox));
                    //        Console.WriteLine("in face detect face_id is {0}:", info[index].index);

                    //        //Console.WriteLine("in face detect score is:{0:f}", info[index].score);
                    //        //// 角度
                    //        //Console.WriteLine("in face detect mAngle is:{0:f}", info[index].angle);
                    //        //// 人脸宽度
                    //        //Console.WriteLine("in face detect mWidth is:{0:f}", info[index].width);
                    //        //// 中心点X,Y坐标
                    //        //Console.WriteLine("in face detect mCenter_x is:{0:f}", info[index].center_x);
                    //        //Console.WriteLine("in face detect mCenter_y is:{0:f}", info[index].center_y);                           
                    //    }

                    //    FaceDraw.draw_rects(ref image, faceSize, info);
                    //    Marshal.FreeHGlobal(ptT);
                    //    window.ShowImage(image);
                    //    Cv2.WaitKey(1);
                    //}
                    //else
                    //{
                    //    Console.WriteLine("mat is empty");
                    //}
                }
            }
        }

        // 通过传图片比对（1:1）
        public void match_by_mat()
        {
            string file_name1 = "../images/1.jpg";
            string file_name2 = "../images/2.jpg";
            Mat mat1 = Cv2.ImRead(file_name1);
            Mat mat2 = Cv2.ImRead(file_name2);
            // 返回整形分值，若<0，则表示错误码
            int score = match(mat1.CvPtr, mat2.CvPtr);
            Console.WriteLine("compare score result is:{0}", score);
        }

        // 通过特征值比对（1:1）
        public void match()
        {
            // 获取特征值1   共128个字节
            string file_name1 = "../images/1.jpg";
            FaceFeature feature = new FaceFeature();
            // 提取第一个人脸特征值数组（多人会提取多个人的特征值）
            BDFaceFeature[] feaList1 = feature.get_face_feature_by_path(file_name1);
            if (feaList1 == null)
            {
                Console.WriteLine("get feature1 fail");
                return;
            }

            string file_name2 = "../images/2.jpg";
            // 提取第一个人脸特征值（多人会提取多个人的特征值）
            BDFaceFeature[] feaList2 = feature.get_face_feature_by_path(file_name2);
            if (feaList2 == null)
            {
                Console.WriteLine("get feature2 fail");
                return;
            }
            // 假设测试的图片是1个人，
            BDFaceFeature f1 = feaList1[0];
            BDFaceFeature f2 = feaList2[0];
            float score = compare_feature(ref f1, ref f2);
            Console.WriteLine("compare score result is:{0}", score);
        }

        // 测试1:N比较，传入特征值
        public void identify()
        {
            // 获取特征值1   共128个字节
            string file_name1 = "../images/2.jpg";
            FaceFeature feature = new FaceFeature();
            // 提取第一个人脸特征值数组（多人会提取多个人的特征值）
            BDFaceFeature[] feaList1 = feature.get_face_feature_by_path(file_name1);
            if (feaList1 == null)
            {
                Console.WriteLine("get feature fail");
                return;
            }
            string user_group = "test_group";
            string user_id = "test_user";
            BDFaceFeature f1 = feaList1[0];
            // type为0表示提取生活照的特征值，1表示证件照的特征值，2表示nir的特征值 (type需要和前面特征值提取的一致，否则会出错)
            IntPtr ptr = identify(ref f1, user_group, user_id);
            string buf = Marshal.PtrToStringAnsi(ptr);
            Console.WriteLine("identify res is:" + buf);
        }

        public string identify(BDFaceFeature f1)
        {
            string user_group = "test_group";
            IntPtr ptr = identify(ref f1, user_group, "");
            string buf = Marshal.PtrToStringAnsi(ptr);
            return buf;
        }

        // 测试1:N比较，传入opencv图片帧
        public void identify_by_mat()
        {
            // 获取特征值1   共128个字节
            string file_name1 = "../images/2.jpg";
            Mat mat1 = Cv2.ImRead(file_name1);

            string user_group = "test_group";
            // string user_id = "test_user";
            string user_id = "";

            // type为0表示提取生活照的特征值，1表示证件照的特征值，2表示nir的特征值 (type需要和前面特征值提取的一致，否则会出错)
            IntPtr ptr = identify_by_mat(mat1.CvPtr, user_group, user_id);
            string buf = Marshal.PtrToStringAnsi(ptr);
            Console.WriteLine("identify res is:" + buf);
        }

        // 测试1:N比较，传入提取的人脸特征值和已加载的内存中整个库比较
        public void identify_with_all()
        {
            // 和整个库比较，需要先加载整个数据库到内存中
            load_db_face();
            // 获取特征值1   共128个字节
            string file_name1 = "../images/2.jpg";
            FaceFeature feature = new FaceFeature();
            // 提取第一个人脸特征值数组（多人会提取多个人的特征值）
            BDFaceFeature[] feaList1 = feature.get_face_feature_by_path(file_name1);
            if (feaList1 == null)
            {
                Console.WriteLine("get feature fail");
                return;
            }
            BDFaceFeature f1 = feaList1[0];
            // type为0表示提取生活照的特征值，1表示证件照的特征值，2表示nir的特征值 (type需要和前面特征值提取的一致，否则会出错)
            int type = 0;
            IntPtr ptr = identify_with_all(ref f1, type);
            string buf = Marshal.PtrToStringAnsi(ptr);
            Console.WriteLine("identify with all db res is:" + buf);
        }

        public string identify_with_all(BDFaceFeature f1)
        {
            string buf = null;

            return buf;
        }

        // 测试1:N比较，传入opencv图片帧和已加载的内存中整个库比较
        public void identify_with_all_by_mat()
        {
            // 和整个库比较，需要先加载整个数据库到内存中
            load_db_face();
            // 获取特征值1   共128个字节
            string file_name1 = "../images/2.jpg";
            Mat mat1 = Cv2.ImRead(file_name1);
            // type为0表示提取生活照的特征值，1表示证件照的特征值，2表示nir的特征值 (type需要和前面特征值提取的一致，否则会出错)
            int type = 0;
            IntPtr ptr = identify_with_all_by_mat(mat1.CvPtr, type);
            string buf = Marshal.PtrToStringAnsi(ptr);
            Console.WriteLine("identify with all db res is:" + buf);
        }
    }
}
