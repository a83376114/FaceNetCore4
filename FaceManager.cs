using System;
using System.Runtime.InteropServices;
using OpenCvSharp;
/**
 *  备注（人脸数据库管理说明）：
 *  人脸数据库为采用sqlite3的数据库，会自动生成一个db目录夹，下面有数据库face.db文件保存数据库
 *  可用sqliteExpert之类的可视化工具打开查看,亦可用命令行，方法请自行百度。
 *  该数据库仅仅可适应于5w人左右的人脸库，且设计表格等属于小型通用化。
 *  若不能满足客户个性化需求，客户可自行设计数据库保存数据。宗旨就是每个人脸图片提取一个特征值保存。
 *  人脸1:1,1:N比对及识别实际就是特征值的比对。1:1只要提取2张不同的图片特征值调用compare_feature比对。
 *  1：N是提取一个特征值和数据库中已保存的N个特征值一一比对(比对速度很快，不用担心效率问题)，
 *  最终取分数高的值为最高相似度。
 *  相似度识别的分数可自行测试根据实验结果拟定，一般推荐相似度大于80分为同一人。
 *  
 */ 
namespace FaceNetCore4
{
    class FaceManager
    {
        // 人脸注册(传特征值,特征值可参考FaceFeature.cs提取，亦可参考FaceCompare.cs查看特征值的比对)
        [DllImport("BaiduFaceApi.dll", EntryPoint = "user_add", CharSet = CharSet.Ansi
            , CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr user_add(ref BDFaceFeature f1, string user_id, string group_id, 
            string user_info="");

        // 人脸注册(传opencv图片帧,特征值可参考FaceFeature.cs提取，亦可参考FaceCompare.cs查看特征值的比对)
        [DllImport("BaiduFaceApi.dll", EntryPoint = "user_add_by_mat", CharSet = CharSet.Ansi
            , CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr user_add_by_mat(IntPtr mat, string user_id, string group_id,
            string user_info = "");

        // 人脸更新(传图片帧)
        [DllImport("BaiduFaceApi.dll", EntryPoint = "user_update", CharSet = CharSet.Ansi
            , CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr user_update(IntPtr mat, string user_id, string group_id,
            string user_info = "");
      
        // 用户删除
        [DllImport("BaiduFaceApi.dll", EntryPoint = "user_delete", CharSet = CharSet.Ansi
           , CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr user_delete(string user_id, string group_id);
        // 组添加
        [DllImport("BaiduFaceApi.dll", EntryPoint = "group_add", CharSet = CharSet.Ansi
           , CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr group_add(string group_id);
        // 组删除
        [DllImport("BaiduFaceApi.dll", EntryPoint = "group_delete", CharSet = CharSet.Ansi
           , CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr group_delete(string group_id);
        // 查询用户信息
        [DllImport("BaiduFaceApi.dll", EntryPoint = "get_user_info", CharSet = CharSet.Ansi
           , CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr get_user_info(string user_id, string group_id);

        // 查询用户图片
        [DllImport("BaiduFaceApi.dll", EntryPoint = "get_user_image", CharSet = CharSet.Ansi
           , CallingConvention = CallingConvention.Cdecl)]
        private static extern int get_user_image(IntPtr out_mat, string user_id, string group_id);

        // 用户组列表查询
        [DllImport("BaiduFaceApi.dll", EntryPoint = "get_user_list", CharSet = CharSet.Ansi
           , CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr get_user_list(string group_id, int start = 0, int length = 100);
        // 组列表查询
        [DllImport("BaiduFaceApi.dll", EntryPoint = "get_group_list", CharSet = CharSet.Ansi
           , CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr get_group_list(int start = 0, int length = 100);

        // 数据库人脸数量查询
        [DllImport("BaiduFaceApi.dll", EntryPoint = "db_face_count", CharSet = CharSet.Ansi
           , CallingConvention = CallingConvention.Cdecl)]
        private static extern int db_face_count(string group_id);


        // 测试人脸注册
        public void user_add()
        {
            // 人脸注册
            string user_id = "test";
            string group_id = "test_group";
            string file_name = @"D:\Gun\test.jpg";
           
            string user_info = "user_info_test";
            // 提取人脸特征值数组（多人会提取多个人的特征值）
/*
            FaceFeature feature = new FaceFeature();
            BDFaceFeature[] feaList1 = feature.get_face_feature_by_path(file_name
            if (feaList1 == null)
            {
                Console.WriteLine("get feature fail");
                return;
            }
            // 假设测试的图片是1个人，
            BDFaceFeature f1 = feaList1[0];
            // 人脸注册 (传特征值人脸注册，该方法注册不保存人脸图片入库)
            IntPtr ptr = user_add(ref f1, user_id, group_id, user_info);
            string buf = Marshal.PtrToStringAnsi(ptr);
            Console.WriteLine("user_add res is:" + buf);
*/
            // 通过图片帧人脸注册（只有该方法进行的人脸注册，人脸库才会保存人脸图片)
            
            Mat mat = Cv2.ImRead(file_name);
            IntPtr mptr = user_add_by_mat(mat.CvPtr, user_id, group_id, user_info);
            string mbuf = Marshal.PtrToStringAnsi(mptr);
            Console.WriteLine("user_add_by_mat res is:" + mbuf);
        }

        public string user_add(string photo_path,string police_id,string group_id,string user_info)
        {
            Mat mat = Cv2.ImRead(photo_path);
            IntPtr mptr = user_add_by_mat(mat.CvPtr, police_id, group_id, user_info);
            string mbuf = Marshal.PtrToStringAnsi(mptr);
            Console.WriteLine("user_add_by_mat res is:" + mbuf);

            return "0";
        }
      
        // 测试人脸更新
        public void user_update()
        {
            string user_id = "test_user";
            string group_id = "test_group";
            string file_name = "../images/1.jpg";
            Mat mat = Cv2.ImRead(file_name);
            string user_info = "user_info";
            // 人脸更新   
            IntPtr ptr = user_update(mat.CvPtr, user_id, group_id, user_info);
            string buf = Marshal.PtrToStringAnsi(ptr);
            Console.WriteLine("user_update res is:" + buf);
        }     

        // 测试用户删除 （用户删除后，人脸数据也被删除）
        public void user_delete()
        {
            string user_id = "test_user";
            string group_id = "test_group";
            IntPtr ptr = user_delete(user_id, group_id);
            string buf = Marshal.PtrToStringAnsi(ptr);
            Console.WriteLine("user_delete res is:" + buf);
        }

        // 组添加
        public void group_add()
        {
            string group_id = "group2";
            IntPtr ptr = group_add(group_id);
            string buf = Marshal.PtrToStringAnsi(ptr);
            Console.WriteLine("group_add res is:" + buf);
        }

        // 组删除
        public void group_delete()
        {
            string group_id = "test_group2";
            IntPtr ptr = group_delete(group_id);
            string buf = Marshal.PtrToStringAnsi(ptr);
            Console.WriteLine("group_delete res is:" + buf);
        }

        // 查询用户信息
        public void get_user_info()
        {
            string user_id = "1";
            string group_id = "test_group";
            IntPtr ptr = get_user_info(user_id , group_id);
            string buf = Marshal.PtrToStringAnsi(ptr);
            Console.WriteLine("get_user_info res is:" + buf);
        }

        // 查询用户图片
        public void get_user_image()
        {
            string user_id = "mz";
            string group_id = "test_group";
            Mat out_mat = new Mat();
            int res = get_user_image(out_mat.CvPtr, user_id, group_id);
            if (res == 0)
            {
                Console.WriteLine("get_user_image success");
                // 图片保存到本地
                out_mat.ImWrite("user.jpg");
            }
            else
            {
                Console.WriteLine("get_user_image error{0}:", res);
            }
        }

        // 用户组列表查询
        public void test_get_user_list()
        {
            string group_id = "test_group";
            IntPtr ptr = get_user_list(group_id);
            string buf = Marshal.PtrToStringAnsi(ptr);
            Console.WriteLine("get_user_list res is:" + buf);
        }   

        // 组列表查询
        public void test_get_group_list()
        {
            IntPtr ptr = get_group_list();
            string buf = Marshal.PtrToStringAnsi(ptr);
            Console.WriteLine("get_group_list res is:" + buf);
        }

        // 人脸库数量查询
        public void db_face_count()
        {
            string group_id = "test_group";
            // 参数传组id表示查该组都人脸数量
            int count = db_face_count(group_id);
            Console.WriteLine("count  is:" + count);
            string group_id2 = null;
            // 参数传null表示查整个库
            int count2 = db_face_count(group_id2);
            Console.WriteLine("all count is:" + count2);
        }
    }
}
