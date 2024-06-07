﻿using DrawTools.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace DrawTools
{
    /// <summary>
    /// 折线
    /// </summary>
    public sealed class PolylineDrawTool : DrawGeometryBase
    {
        public PolylineDrawTool(DrawingCanvas drawingCanvas) : base(drawingCanvas)
        {
            this.DrawingToolType = DrawToolType.Polyline;

            // 准备要处理的事件
            this.CanTouchDown = true;
        }

        #region 鼠标键盘事件

        public override Boolean OnTouchLeave(Point point)
        {
            if (mousePoint.HasValue)
            {
                points.Add(mousePoint.Value);
                mousePoint = null;
            }

            if (points.Count < 2)
                this.drawingCanvas.DeleteVisual(this);
            else
            {
                geometry = geometry.GetWidenedPathGeometry(pen);
                Draw();
            }

            this.drawingCanvas.DeleteWorkingDrawTool(this);

            this.IsFinish = true;

            this.CanTouchDown = false;
            this.CanTouchMove = false;
            this.CanTouchLeave = false;

            if (this.TouchId == 0 && this.drawingCanvas.IsMouseCaptured)
                this.drawingCanvas.ReleaseMouseCapture();

            return true;
        }

        public override Boolean OnTouchDown(Int32 touchId, Point point)
        {
            this.TouchId = touchId;

            if (points.Count == 0)
            {
                this.drawingCanvas.AddWorkingDrawTool(this);

                this.pen = this.drawingCanvas.Pen;

                points.Add(point);

                geometry = new PathGeometry();

                var figure = new PathFigure { StartPoint = point };
                pathGeometry.Figures.Add(figure);

                this.CanTouchMove = true;

                if (this.TouchId != 0 || !this.drawingCanvas.CaptureMouse())
                    this.CanTouchLeave = true;

                this.drawingCanvas.AddVisual(this);
            }
            else if ((point - points.Last()).Length <= pen.Thickness)
                OnTouchLeave(point);
            else if (mousePoint.HasValue)
            {
                points.Add(mousePoint.Value);
                mousePoint = null;
            }

            return true;
        }

        public override Boolean OnTouchMove(Point point)
        {
            if ((point - points.Last()).Length <= pen.Thickness)
                return true;

            var figure = pathGeometry.Figures[0];
            var line = new LineSegment(point, true) { IsSmoothJoin = true };

            if (mousePoint.HasValue)
                figure.Segments[figure.Segments.Count - 1] = line;
            else
                figure.Segments.Add(line);

            mousePoint = point;

            var dc = this.RenderOpen();
            dc.DrawGeometry(null, pen, geometry);
            dc.Close();

            return true;
        }

        #endregion

        #region 序列化

        public override DrawGeometrySerializerBase ToSerializer()
        {
            var serializer = new DrawPolylineSerializer
            {
                Color = ((SolidColorBrush)pen.Brush).Color,
                StrokeThickness = pen.Thickness,
                Geometry = geometry.ToString()
            };

            if (geometry.Transform != null)
                serializer.Matrix = geometry.Transform.Value;

            return serializer;
        }

        public override void DeserializeFrom(DrawGeometrySerializerBase serializer)
        {
            this.pen = new Pen(new SolidColorBrush(serializer.Color), serializer.StrokeThickness);

            this.geometry = Geometry.Parse(serializer.Geometry).GetFlattenedPathGeometry();
            this.geometry.Transform = new TranslateTransform(serializer.Matrix.OffsetX, serializer.Matrix.OffsetY);

            this.IsFinish = true;

            this.Draw();
        }

        #endregion

        #region 字段

        private List<Point> points = new List<Point>();
        private Point? mousePoint;

        #endregion
    }
}
