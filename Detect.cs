using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using OpenCvSharp;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.IO;

namespace FaceNetCore4
{
    class Detect
    {
        //人脸检测 type 0： 表示rgb 人脸检测 1：表示nir人脸检测
        [DllImport("BaiduFaceApi.dll", EntryPoint = "detect", CharSet = CharSet.Ansi
           , CallingConvention = CallingConvention.Cdecl)]
        public static extern int detect(IntPtr ptr, IntPtr mat, int type);

        // 1:N人脸识别（传人脸特征值和库里的比对, 人脸库可参考FaceManager）
        [DllImport("BaiduFaceApi.dll", EntryPoint = "identify", CharSet = CharSet.Ansi
          , CallingConvention = CallingConvention.Cdecl)]
        // type为0表示提取生活照的特征值，1表示证件照的特征值，2表示nir的特征值
        private static extern IntPtr identify(ref BDFaceFeature f1, string group_id_list,
            string user_id, int type = 0);


        string res = "";
        bool detectedFlag = false;

        public string video_detect_identify()
        {
            // 默认电脑自带摄像头，device可能为0，若外接usb摄像头，则device可能为1.
            //int dev = 1;
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
                    Console.WriteLine("Open Camera Error!");
                    return "";
                }
                // Frame image buffer
                Mat image = new Mat();

                while (true)
                {
                    string user_group = "test_group";

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

                            //Console.WriteLine("in face detect face_id is {0}:", info[index].index);

                            //  Console.WriteLine("in face detect score is:{0:f}", info[index].score);
                            //// 角度
                            //Console.WriteLine("in face detect mAngle is:{0:f}", info[index].angle);
                            //// 人脸宽度
                            //Console.WriteLine("in face detect mWidth is:{0:f}", info[index].width);
                            //// 中心点X,Y坐标
                            //Console.WriteLine("in face detect mCenter_x is:{0:f}", info[index].center_x);
                            //Console.WriteLine("in face detect mCenter_y is:{0:f}", info[index].center_y);
                        }
                        FaceDraw.draw_rects(ref image, faceSize, info);
                        Marshal.FreeHGlobal(ptT);
                        window.ShowImage(image);
                        Cv2.WaitKey(1);


                        if (faceSize > 0)
                        {
                            try
                            {
                                Mat face = new FaceCrop().gen_face_crop(image);
                                string res = identify_by_mat(face);
                                Console.WriteLine();
                                Console.WriteLine(res);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }

                        //if (faceSize > 0)
                        //{
                        //    try
                        //    {
                        //        BDFaceFeature[] features = get_features(image);
                        //        if (features.Length > 0)
                        //        {
                        //            BDFaceFeature feature = features[0];
                        //            foreach(var f in feature.data)
                        //            {
                        //                Console.WriteLine(f);
                        //            }
                        //            string floatData = feature.data.ToString();
                        //            BDFaceFeature f1 = new BDFaceFeature();
                        //            f1.size = 1;
                        //            //floatData
                        //            //string result = identify_by_feature(feature);
                        //            //Console.WriteLine(res);
                        //            //dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(res);
                        //            //if (json.msg == "success")
                        //            //{
                        //            //    string user_id = json.data.result[0].user_id;
                        //            //    Console.WriteLine(user_id);
                        //            //}
                        //        }
                        //    }
                        //    catch (Exception e)
                        //    {
                        //        Console.WriteLine(e);
                        //    }

                        //}


                        //Thread thread = new Thread(() => { res = identify_by_mat(image); });
                        //thread.Start();
                        //thread.Join();

                        //if (faceSize > 0 && !detectedFlag)
                        //{
                        //    //BDFaceFeature feature = get_features(image);
                        //    //res = identify_by_feature(feature);
                        //    //Console.WriteLine(res);

                        //    ThreadPool.QueueUserWorkItem(state => identify_by_mat(image));
                        //    //res = identify_by_mat(image);

                        //    if (res != "")
                        //    {
                        //        Console.WriteLine(res);
                        //        detectedFlag = true;
                        //        //res = "";
                        //        //break;
                        //    }
                        //}



                        //ThreadPool.QueueUserWorkItem(state => identify_by_feature(feature));
                        //res=identify_by_mat(image);
                        //if (res != "")
                        //{
                        //    Console.WriteLine(res);
                        //    res = "";
                        //    break;
                        //}
                    }
                }
            }
            return "";
        }


        public void video_feature()
        {
            // 默认电脑自带摄像头，device可能为0，若外接usb摄像头，则device可能为1.
            //int dev = 1;
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
                    Console.WriteLine("Open Camera Error!");
                    return;
                }
                // Frame image buffer
                Mat image = new Mat();

                while (true)
                {
                    string user_group = "test_group";

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
                        }
                        FaceDraw.draw_rects(ref image, faceSize, info);
                        Marshal.FreeHGlobal(ptT);
                        window.ShowImage(image);
                        Cv2.WaitKey(1);

                        if (faceSize > 0)
                        {
                            try
                            {
                                BDFaceFeature[] features = get_features(image);
                                if (features.Length > 0)
                                {
                                    BDFaceFeature feature = features[0];
                                    string serializedStr = "";
                                    FaceFeature ff = new FaceFeature();
                                    if (ff.serializeObjToStr(feature, out serializedStr))
                                    {
                                        File.WriteAllTextAsync("feature.txt", serializedStr);
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }

                        }
                    }
                }
            }
        }

        public void deserialize_identify()
        {
            BDFaceFeature feature;
            string serializedStr = File.ReadAllText("feature.txt");
            FaceFeature ff = new FaceFeature();
            if (ff.deserializeStrToObj(serializedStr,out feature)){
                string res = identify_by_feature(feature);
                Console.WriteLine(res);

            }
        }

        public BDFaceFeature[] get_features(Mat mat)
        {
            BDFaceFeature[] features = null;
            try
            {
                FaceFeature faceFeature = new FaceFeature();
                features = faceFeature.get_face_feature_by_mat(mat);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return features;
        }

        public string identify_by_feature(BDFaceFeature f1)
        {

            string user_group = "test_group";
            IntPtr ptr = identify(ref f1, user_group, "");
            string buf = Marshal.PtrToStringAnsi(ptr);
            res = buf;
            return buf;
        }

        public string identify_by_mat(Mat mat)
        {
            string buf = "";

            try
            {
                BDFaceFeature[] features = get_features(mat);
                if (features.Length > 0)
                {
                    BDFaceFeature feature = features[0];
                    Console.WriteLine(string.Join(",", feature.data));
                    string user_group = "test_group";
                    IntPtr ptr = identify(ref features[0], user_group, "");
                    buf = Marshal.PtrToStringAnsi(ptr);
                }
                res = buf;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return buf;
        }
    }
}
