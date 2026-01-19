using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace STARCITIZEN_OpenCV
{
    public class OverlayItem
    {
        public Rectangle Rect { get; set; }
        public string Label { get; set; }
    }
    public partial class OverlayForm : Form
    {
        public Rectangle CurrentDragRect; // 현재 드래그 중인 영역
        public Bitmap AnchorImage;        // 앵커 이미지
        public Point AnchorPos;           // 앵커 위치
        public List<OverlayItem> RectsToDraw = new List<OverlayItem>();        
        public OverlayForm()
        {
            InitializeComponent();
            // 코드로 속성 강제 지정
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;         // 검정색 부분을 모두 투명하게 만듦
            this.WindowState = FormWindowState.Maximized; // 전체 화면으로 확대
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // 1. 드래그 중인 영역 그리기 (점선)
            if (!CurrentDragRect.IsEmpty)
            {
                using (Pen dashedPen = new Pen(Color.White, 1))
                {
                    dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    e.Graphics.DrawRectangle(dashedPen, CurrentDragRect);
                }
            }
            // 앵커 이미지 가이드 (50% 투명도)
            if (AnchorImage != null)
            {
                var matrix = new System.Drawing.Imaging.ColorMatrix { Matrix33 = 0.5f };
                var attr = new System.Drawing.Imaging.ImageAttributes();
                attr.SetColorMatrix(matrix);

                e.Graphics.DrawImage(AnchorImage,
                    new Rectangle(AnchorPos.X, AnchorPos.Y, AnchorImage.Width, AnchorImage.Height),
                    0, 0, AnchorImage.Width, AnchorImage.Height, GraphicsUnit.Pixel, attr);
            }

            // 현재 드래그 중인 노란색 점선 박스
            if (!CurrentDragRect.IsEmpty)
            {
                using (Pen p = new Pen(Color.Yellow, 1) { DashStyle = DashStyle.Dash })
                    e.Graphics.DrawRectangle(p, CurrentDragRect);
            }

            // 3. 기존 버튼 박스들 그리기
            using (Pen pen = new Pen(Color.Lime, 1))
            using (Font font = new Font("Arial", 10, FontStyle.Bold))
            {
                foreach (var item in RectsToDraw)
                {
                    // 1. 보정된 위치에 사각형 그리기
                    e.Graphics.DrawRectangle(pen, item.Rect);

                    // 2. 사각형 바로 위에 버튼 이름 그리기
                    e.Graphics.DrawString(item.Label, font, Brushes.Lime, item.Rect.X, item.Rect.Y - 18);

                    // 3. 중심점에 작은 십자선 표시 (좌표 확인용)
                    int centerX = item.Rect.X + (item.Rect.Width / 1);
                    int centerY = item.Rect.Y + (item.Rect.Height / 1);
                    e.Graphics.DrawLine(pen, centerX - 5, centerY, centerX + 5, centerY);
                    e.Graphics.DrawLine(pen, centerX, centerY - 5, centerX, centerY + 5);
                }
            }
        }
    }



}
