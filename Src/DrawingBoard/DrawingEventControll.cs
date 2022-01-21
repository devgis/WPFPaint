using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace App1.DrawingBoard
{
    internal class DrawingEventControll
    {


        static public void setupDrawingEventControll(InkCanvas inkCanvas, Canvas canvas)
        {
            PropertyConfiguration.setConfiguration(inkCanvas);
            Listener.setupListener(inkCanvas, canvas);
        }

    }

    public class Listener
    {
        static public InkCanvas inkCanvas;
        static private Canvas canvas;
        static public void setupListener(InkCanvas inkCanvas, Canvas canvas)
        {
            configureListenerProperty(inkCanvas, canvas);
            distributeListener();
        }
        static public void configureListenerProperty(InkCanvas inkCanvas, Canvas canvas)
        {
            Listener.inkCanvas = inkCanvas;
            Listener.canvas = canvas;
        }
        static public void distributeListener()
        {
            inkCanvas.InkPresenter.UnprocessedInput.PointerPressed += UnprocessedInput_PointerPressed;
            inkCanvas.InkPresenter.UnprocessedInput.PointerMoved += UnprocessedInput_PointerMoved;
            inkCanvas.InkPresenter.UnprocessedInput.PointerReleased += UnprocessedInput_PointerReleased;
            inkCanvas.InkPresenter.StrokeInput.StrokeStarted += StrokeInput_StrokeStarted;
            inkCanvas.InkPresenter.StrokesErased += InkPresenter_StrokesErased;
            long_press_event();
        }



        static public void long_press_event()
        {
            Lasso.long_press_event();
        }
        static private void UnprocessedInput_PointerPressed(InkUnprocessedInput sender, PointerEventArgs args)
        {
            Lasso.UnprocessedInput_PointerPressed(args, inkCanvas, canvas);
        }

        static private void UnprocessedInput_PointerMoved(InkUnprocessedInput sender, PointerEventArgs args)
        {
            Lasso.UnprocessedInput_PointerMoved(args, inkCanvas, canvas);
        }

        static private void UnprocessedInput_PointerReleased(InkUnprocessedInput sender, PointerEventArgs args)
        {
            Lasso.UnprocessedInput_PointerReleased(args, inkCanvas, canvas);
        }

        static private void StrokeInput_StrokeStarted(InkStrokeInput sender, PointerEventArgs args)
        {
            Lasso.StrokeInput_StrokeStarted(inkCanvas, canvas);
        }

        static private void InkPresenter_StrokesErased(InkPresenter sender, InkStrokesErasedEventArgs args)
        {
            Lasso.InkPresenter_StrokesErased(inkCanvas, canvas);
        }
    }
    public class PropertyConfiguration
    {
        static public void setConfiguration(InkCanvas inkCanvas)
        {
            setAllowDeviceTypes(inkCanvas);
            setPenProperty(inkCanvas);
             setAllowEvent(inkCanvas);
        }

        static public void setAllowDeviceTypes(InkCanvas inkCanvas)
        {
            inkCanvas.InkPresenter.InputDeviceTypes =
                  Windows.UI.Core.CoreInputDeviceTypes.Pen
                | Windows.UI.Core.CoreInputDeviceTypes.Touch
                | Windows.UI.Core.CoreInputDeviceTypes.Mouse;
        }

        static public void setPenProperty(InkCanvas inkCanvas)
        {
            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(new InkDrawingAttributes()
            {
                Color = Windows.UI.Colors.Black,
                IgnorePressure = false,
                IgnoreTilt = false,
                FitToCurve = false
            });
        }

        static public void setAllowEvent(InkCanvas inkCanvas)
        {
            inkCanvas.IsDoubleTapEnabled = true;
        }
    }
    public class Lasso
    {
        static Canvas global_Canvas = null;
        static GestureRecognizer gestureRecognizer = new GestureRecognizer() { HoldStartDelay = new TimeSpan(1000) };
        static private Polyline lasso;
        static private Rect boundingRect;
        static private Point currentPos;
        static private Rect operateRect = new Rect(0, 0, 0, 0);



        static public void long_press_event()
        {
            Lasso.SelectedRectAction.TriggerMenu.configureTriggerMenu();
        }

        static public void UnprocessedInput_PointerPressed(PointerEventArgs args, InkCanvas inkCanvas, Canvas canvas)
        {
            Lasso.BuildingLasso.UnprocessedInput_PointerPressed(args, inkCanvas, canvas);
            Lasso.SelectedRectAction.SelectedRectMove.UnprocessedInput_PointerPressed(args);
            Lasso.SelectedRectAction.TriggerMenu.UnprocessedInput_PointerPressed(args);
        }

        static public void UnprocessedInput_PointerMoved(PointerEventArgs args, InkCanvas inkCanvas, Canvas canvas)
        {
            Lasso.BuildingLasso.UnprocessedInput_PointerMoved(args, inkCanvas, canvas);
            Lasso.SelectedRectAction.SelectedRectMove.UnprocessedInput_PointerMoved(args, inkCanvas, canvas);
            Lasso.SelectedRectAction.TriggerMenu.UnprocessedInput_PointerMoved(args);
        }

        static public void UnprocessedInput_PointerReleased(PointerEventArgs args, InkCanvas inkCanvas, Canvas canvas)
        {
            if (operateRect.Height == 0)
            {
                Lasso.BuildingLasso.UnprocessedInput_PointerReleased(args, inkCanvas, canvas);
            }
            operateRect.Height = 0;
            Lasso.SelectedRectAction.TriggerMenu.UnprocessedInput_PointerReleased(args);
        }

        static public void StrokeInput_StrokeStarted(InkCanvas inkCanvas, Canvas canvas)
        {
            Lasso.RetangleAction.clearSelectedRect(inkCanvas, canvas);
        }

        static public void InkPresenter_StrokesErased(InkCanvas inkCanvas, Canvas canvas)
        {
            Lasso.RetangleAction.clearSelectedRect(inkCanvas, canvas);
        }


        public class BuildingLasso : RetangleAction
        {
            static public void UnprocessedInput_PointerPressed(PointerEventArgs args, InkCanvas inkCanvas, Canvas canvas)
            {
                lasso = new Polyline()
                {
                    Stroke = new SolidColorBrush(Windows.UI.Colors.Blue),
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection() { 5, 2 },
                };

                lasso.Points.Add(args.CurrentPoint.RawPosition);

                canvas.Children.Add(lasso);
            }
            static public void UnprocessedInput_PointerMoved(PointerEventArgs args, InkCanvas inkCanvas, Canvas canvas)
            {
                lasso.Points.Add(args.CurrentPoint.RawPosition);
            }

            static public void UnprocessedInput_PointerReleased(PointerEventArgs args, InkCanvas inkCanvas, Canvas canvas)
            {
                lasso.Points.Add(args.CurrentPoint.RawPosition);
                
                boundingRect =
                  inkCanvas.InkPresenter.StrokeContainer.SelectWithPolyLine(
                    lasso.Points);

                drawBoundingRect(canvas, args);
            }
        }

        public class SelectedRectAction : RetangleAction
        {
            public class SelectedRectMove
            {
                static public void UnprocessedInput_PointerPressed(PointerEventArgs args)
                {
                    currentPos = args.CurrentPoint.RawPosition;
                }
                static public void UnprocessedInput_PointerMoved(PointerEventArgs args, InkCanvas inkCanvas, Canvas canvas)
                {
                    if (
                args.CurrentPoint.RawPosition.X >= boundingRect.Left &&
                args.CurrentPoint.RawPosition.X <= boundingRect.Right &&
                args.CurrentPoint.RawPosition.Y >= boundingRect.Top &&
                args.CurrentPoint.RawPosition.Y <= boundingRect.Bottom
                )
                    {
                        Point delta = new Point(args.CurrentPoint.RawPosition.X - currentPos.X, args.CurrentPoint.RawPosition.Y - currentPos.Y);
                        operateRect = inkCanvas.InkPresenter.StrokeContainer.MoveSelected(delta);
                        currentPos = args.CurrentPoint.Position;

                        boundingRect = new Rect(boundingRect.X + delta.X, boundingRect.Y + delta.Y, boundingRect.Width, boundingRect.Height);
                        
                        drawBoundingRect(canvas, args);
                    }
                }
            }

            public class TriggerMenu
            {
                public static void configureTriggerMenu()
                {
                    gestureRecognizer.GestureSettings = Windows.UI.Input.GestureSettings.HoldWithMouse;
                    gestureRecognizer.Holding += GestureRecognizer_Holding;
                    //gestureRecognizer.
                }
                public static void GestureRecognizer_Holding(GestureRecognizer sender, HoldingEventArgs args)
                {
                    if (args.HoldingState == HoldingState.Started) //长按状态
                    {
                        if (global_Canvas != null && global_Canvas.Children.Count > 0 && global_Canvas.Children[0] is Rectangle) //选中
                        {
                            var rectangle = global_Canvas.Children[0] as Rectangle;
                            if (boundingRect.Contains(args.Position)) //在选中对象上长按
                            {
                                Point point = new Point(rectangle.Width / 2 + rectangle.CenterPoint.X, rectangle.Height + rectangle.CenterPoint.Y);
                                CreateMenuFlyout(global_Canvas, args.Position, "剪切", "复制", "粘贴");
                            }
                            else   //不在选中对象上长按
                            {
                                CreateMenuFlyout(global_Canvas, args.Position, "粘贴");
                            }
                        }
                        else  //未选中
                        {
                            CreateMenuFlyout(global_Canvas, args.Position, "粘贴");
                        }
                    }
                }

                private static void CreateMenuFlyout(Windows.UI.Xaml.UIElement target, Point point, params string[] items)
                {
                    var menuFlyout = new MenuFlyout();
                    foreach (var item in items)
                    {
                        var it = new MenuFlyoutItem() { Text = item };
                        it.Click += (o, s) =>
                        {
                            var mit = o as MenuFlyoutItem;
                            if (mit.Text == "剪切")
                            {
                                Listener.inkCanvas.InkPresenter.StrokeContainer.CopySelectedToClipboard();
                                Listener.inkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
                            }
                            else if (mit.Text == "复制")
                            {
                                Listener.inkCanvas.InkPresenter.StrokeContainer.CopySelectedToClipboard();
                            }
                            else if (mit.Text == "粘贴")
                            {
                                Listener.inkCanvas.InkPresenter.StrokeContainer.PasteFromClipboard(point);
                            }
                            if (global_Canvas != null)
                            {
                                global_Canvas.Children.Clear();
                            }
                        };
                        menuFlyout.Items.Add(it);
                    }
                    menuFlyout.ShowAt(target, point);
                }

                public static void UnprocessedInput_PointerPressed(PointerEventArgs args)
                {
                    var ps = args.GetIntermediatePoints();
                    if (ps != null && ps.Count > 0)
                    {
                        gestureRecognizer.ProcessDownEvent(ps[0]);
                         args.Handled = true;
                    }
                }
                public static void UnprocessedInput_PointerMoved(PointerEventArgs args)
                {
                    gestureRecognizer.ProcessMoveEvents(args.GetIntermediatePoints());
                      args.Handled = true;
                }
                public static void UnprocessedInput_PointerReleased(PointerEventArgs args)
                {
                    var ps = args.GetIntermediatePoints();
                    if (ps != null && ps.Count > 0)
                    {
                        gestureRecognizer.ProcessUpEvent(ps[0]);
                         args.Handled = true;
                        gestureRecognizer.CompleteGesture();
                    }
                }

            }

        }
        public class RetangleAction
        {
            static public void drawBoundingRect(Canvas canvas, PointerEventArgs args)
            {
                canvas.Children.Clear();
                if (!((boundingRect.Width == 0) ||
                  (boundingRect.Height == 0) ||
                  boundingRect.IsEmpty))
                {
                    var rectangle = new Rectangle()
                    {
                        Stroke = new SolidColorBrush(Windows.UI.Colors.Blue),
                        Fill = new SolidColorBrush(Windows.UI.Colors.Transparent),
                        StrokeThickness = 1,
                        StrokeDashArray = new DoubleCollection() { 5, 2 },
                        Width = boundingRect.Width,
                        Height = boundingRect.Height,
                        IsDoubleTapEnabled = true
                    };

                    global_Canvas = canvas;


                    Canvas.SetLeft(rectangle, boundingRect.X);
                    Canvas.SetTop(rectangle, boundingRect.Y);

                    canvas.Children.Add(rectangle);


                }
            }



            static public void clearSelectedRect(InkCanvas inkCanvas, Canvas selectionCanvas)
            {
                var strokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();
               
                foreach (var stroke in strokes)
                {
                    stroke.Selected = false;
                }
                if (selectionCanvas.Children.Any())
                {
                    selectionCanvas.Children.Clear();
                    boundingRect = Rect.Empty;
                }
            }

        }
    }


}


