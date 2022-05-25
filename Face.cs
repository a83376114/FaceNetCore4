using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using OpenCvSharp;

// sdk使用注意事项，使用sdk前，请参考文档进行授权激活，否则
// sdk初始化可能失败，可通过打开日志face_conf.json，字段改为true。
// 查找是否通过了授权激活或激活失败的原因

// 人脸c#入口类
namespace FaceNetCore4
{
    class Face
    {      
        // sdk初始化
        [DllImport("BaiduFaceApi.dll", EntryPoint = "sdk_init", CharSet =CharSet.Ansi
            , CallingConvention =CallingConvention.Cdecl)]
        private static extern int sdk_init(string model_path);
        // 是否授权
        [DllImport("BaiduFaceApi.dll", EntryPoint = "is_auth", CharSet = CharSet.Ansi
               , CallingConvention = CallingConvention.Cdecl)]
        private static extern bool is_auth();
        // sdk销毁
        [DllImport("BaiduFaceApi.dll", EntryPoint = "sdk_destroy" , CharSet = CharSet.Ansi
            , CallingConvention = CallingConvention.Cdecl)]
        private static extern void sdk_destroy();

        // 获取设备指纹
        [DllImport("BaiduFaceApi.dll", EntryPoint = "get_device_id", CharSet = CharSet.Ansi
            , CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr get_device_id();

        // 获取sdk版本号
        [DllImport("BaiduFaceApi.dll", EntryPoint = "sdk_version", CharSet = CharSet.Ansi
            , CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr sdk_version();

        // 获取sdk信息如设备指纹，版本号
        static void get_sdk_info()
        {
            // 获取设备指纹
            IntPtr ptr = get_device_id();
            string buf = Marshal.PtrToStringAnsi(ptr);
            Console.WriteLine("device id is:" + buf);
            // 获取sdk版本号
            IntPtr ptr_v = sdk_version();
            string vbuf = Marshal.PtrToStringAnsi(ptr_v);
            Console.WriteLine("sdk version is:" + vbuf);
        }

        // 人脸c#入口方法
       public static void face()
        {
            Console.WriteLine("in main");
            // model_path为模型文件夹路径，即models文件夹（里面存的是人脸识别的模型文件）
            // 传空为采用默认路径，若想定置化路径，请填写全局路径如：d:\\face （models模型文件夹目录放置后为d:\\face\\models）
            // 若模型文件夹采用定置化路径，则激活文件(license.ini, license.key)也可采用定制化路径放置到该目录如d:\\face\\license
            // 亦可在激活文件默认生成的路径
          //  string model_path = null;
            string model_path = @"D:\Projects\myProjects\Gun_UI\test\FaceNetCore4";
           // string model_path="d:\\face";
            int n = sdk_init(model_path);
            //若没通过初始化，则n不为0, 返回的错误码及原因可参考文档
            if(n != 0)
            {
                Console.WriteLine("sdk init fail and errcode is {0:D}", n);
                Console.ReadLine();
                return;
             }
            //// 获取设备指纹
            //get_sdk_info();
            //// 验证是否授权
            //bool authed = is_auth();
            //Console.WriteLine("authed res is:"+authed);
            //long t_begin = TimeUtil.get_time_stamp();
            //// 以下为demo示例,请参考FaceDemo打开相关注释测试验证
            //FaceDemo demo = new FaceDemo();
            
            //demo.face_demo();
            //long t_end = TimeUtil.get_time_stamp();
            //Console.WriteLine("time cost is:"+(t_end-t_begin));
            //// 使用完毕，销毁sdk，释放内存
            //sdk_destroy();
            //Console.WriteLine("end main");
            //Console.ReadLine();
        }
    }
}
