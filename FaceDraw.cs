using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
// 绘制类，画人脸框，画关键点等
namespace FaceNetCore4
{
    class FaceDraw
    {
        // 画人脸框
        public static int draw_rects(ref Mat img, int face_num, BDFaceBBox[] info)
        {
            if (face_num <= 0)
            {
                return 0;
            }
            Scalar color = new Scalar(0, 255, 0);
            for (int i = 0; i < face_num; i++)
            {
                int x = Convert.ToInt32(info[i].center_x - info[i].width / 2.0);
                int y = Convert.ToInt32(info[i].center_y - info[i].height / 2.0);
                int w = Convert.ToInt32(info[i].width);
                int h = Convert.ToInt32(info[i].height);
                Rect rect = new Rect(x, y, w, h);
                Cv2.Rectangle(img, rect, color);
            }
            return 0;
        }
        // 画人脸框
        public static int draw_rects(ref Mat img, int face_num, BDFaceTrackInfo[] track_info)
        {          
            if (face_num <= 0)
            {
                return 0;
            }
            Scalar color = new Scalar(0, 255, 0);
            for(int i = 0; i < face_num; i++)
            {
                int x = Convert.ToInt32(track_info[i].box.center_x- track_info[i].box.width / 2.0);
                int y = Convert.ToInt32(track_info[i].box.center_y - track_info[i].box.height / 2.0);
                int w = Convert.ToInt32(track_info[i].box.width);
                int h = Convert.ToInt32(track_info[i].box.height);
                Rect rect = new Rect(x,y,w,h);
                Cv2.Rectangle(img, rect, color);
            }
            return 0;
        }
        // 画人脸关键点
        public static int draw_shape(ref Mat img, int face_num, BDFaceTrackInfo[] track_info)
        {
            if (face_num <= 0)
            {
                return 0;
            }
            int face_id = 0;
            Scalar color = new Scalar(0, 255, 255);
            Scalar color2 = new Scalar(0, 0, 255);
            for (int i = 0; i < face_num; ++i)
            {
                int point_size = track_info[i].landmark.size / 2;
                int radius = 2;
                face_id = track_info[i].face_id;
                for (int j = 0; j < point_size; ++j)
                {
                    Cv2.Circle(img, (int)track_info[i].landmark.data[j * 2], (int)track_info[i].landmark.data[j * 2 + 1], radius, color);
                }
                if (point_size == 72)
                {
                    const int components = 9;
                    int[] comp1 = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                    int[] comp2 = { 13, 14, 15, 16, 17, 18, 19, 20, 13, 21 };
                    int[] comp3 = { 22, 23, 24, 25, 26, 27, 28, 29, 22 };
                    int[] comp4 = { 30, 31, 32, 33, 34, 35, 36, 37, 30, 38 };
                    int[] comp5 = { 39, 40, 41, 42, 43, 44, 45, 46, 39 };
                    int[] comp6 = { 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 47 };
                    int[] comp7 = { 51, 57, 52 };
                    int[] comp8 = { 58, 59, 60, 61, 62, 63, 64, 65, 58 };
                    int[] comp9 = { 58, 66, 67, 68, 62, 69, 70, 71, 58 };
                    int[][] idx = { comp1, comp2, comp3, comp4, comp5, comp6, comp7, comp8, comp9 };
                    int[] npoints = { 13, 10, 9, 10, 9, 11, 3, 9, 9 };

                    for (int m = 0; m < components; ++m)
                    {
                        for (int n = 0; n < npoints[m] - 1; ++n)
                        {
                            Point p1 = new Point(track_info[i].landmark.data[idx[m][n] * 2], track_info[i].landmark.data[idx[m][n] * 2 + 1]);
                            Point p2 = new Point(track_info[i].landmark.data[idx[m][n + 1] * 2], track_info[i].landmark.data[idx[m][n + 1] * 2 + 1]);
                            Cv2.Line(img, p1, p2, color2);
                        }
                    }
                }
                string s_face_id = face_id.ToString();
                double font_scale = 2;
                Point pos = new Point(track_info[i].box.center_x, track_info[i].box.center_y);
                Cv2.PutText(img, s_face_id, pos, HersheyFonts.HersheyComplex, font_scale, new Scalar(0, 255, 255));              
            }
           
            return 0;
        }

        // 画人脸关键点
        public static int draw_landmark(ref Mat img, int face_num, BDFaceLandmark[] landmark)
        {
            if (face_num <= 0)
            {
                return 0;
            }
            Scalar color = new Scalar(0, 255, 255);
            Scalar color2 = new Scalar(0, 0, 255);
            for (int i = 0; i < face_num; ++i)
            {
                int point_size = landmark[i].size / 2;
                int radius = 2;
                for (int j = 0; j < point_size; ++j)
                {
                    Cv2.Circle(img, (int)landmark[i].data[j * 2], (int)landmark[i].data[j * 2 + 1], radius, color);
                }
                if (point_size == 72)
                {
                    const int components = 9;
                    int[] comp1 = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                    int[] comp2 = { 13, 14, 15, 16, 17, 18, 19, 20, 13, 21 };
                    int[] comp3 = { 22, 23, 24, 25, 26, 27, 28, 29, 22 };
                    int[] comp4 = { 30, 31, 32, 33, 34, 35, 36, 37, 30, 38 };
                    int[] comp5 = { 39, 40, 41, 42, 43, 44, 45, 46, 39 };
                    int[] comp6 = { 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 47 };
                    int[] comp7 = { 51, 57, 52 };
                    int[] comp8 = { 58, 59, 60, 61, 62, 63, 64, 65, 58 };
                    int[] comp9 = { 58, 66, 67, 68, 62, 69, 70, 71, 58 };
                    int[][] idx = { comp1, comp2, comp3, comp4, comp5, comp6, comp7, comp8, comp9 };
                    int[] npoints = { 13, 10, 9, 10, 9, 11, 3, 9, 9 };

                    for (int m = 0; m < components; ++m)
                    {
                        for (int n = 0; n < npoints[m] - 1; ++n)
                        {
                            Point p1 = new Point(landmark[i].data[idx[m][n] * 2], landmark[i].data[idx[m][n] * 2 + 1]);
                            Point p2 = new Point(landmark[i].data[idx[m][n + 1] * 2], landmark[i].data[idx[m][n + 1] * 2 + 1]);
                            Cv2.Line(img, p1, p2, color2);
                        }
                    }
                }


            }
            return 0;
        }
    }
}
